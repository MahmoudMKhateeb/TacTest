import { AfterViewInit, Component, ElementRef, Inject, Injector, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import KTWizard from '@metronic/common/js/components/wizard';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { DxValidationGroupComponent } from '@node_modules/devextreme-angular/ui/validation-group';
import { DxReportDesignerComponent } from '@node_modules/devexpress-reporting-angular';
import * as ko from '@node_modules/knockout';
import { ajaxSetup } from '@node_modules/@devexpress/analytics-core/core/internal/ajaxSetup';
import {
  API_BASE_URL,
  CompanyType,
  EditionListDto,
  EditionServiceProxy,
  PricePackageServiceProxy,
  SelectItemDto,
} from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'app-create-report-type',
  templateUrl: './create-report-type.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrls: [
    './create-report-type.component.scss',
    '../../../../../../../node_modules/jquery-ui/themes/base/all.css',
    '../../../../../../../node_modules/devexpress-richedit/dist/dx.richedit.css',
    '../../../../../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css',
    '../../../../../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css',
    '../../../../../../../node_modules/@devexpress/analytics-core/dist/css/dx-querybuilder.css',
    '../../../../../../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css',
    '../../../../../../../node_modules/devexpress-reporting/dist/css/dx-reportdesigner.css',
  ],
  animations: [appModuleAnimation()],
})
export class CreateReportTypeComponent extends AppComponentBase implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('wizard', { static: true }) el: ElementRef;
  @ViewChild('step1FormGroup', { static: false }) step1FormGroup: DxValidationGroupComponent;

  @ViewChild('designer', { static: false }) designer: DxReportDesignerComponent;

  title = 'DXReportViewerSample';
  getDesignerModelAction = `/DXXRD/GetDesignerModel`;
  hostUrl: string;
  koReportUrl = ko.observable('');
  get reportUrl() {
    return this.koReportUrl();
  }

  set reportUrl(newUrl) {
    this.koReportUrl(newUrl);
  }

  // start of form groups and form models
  step1Form = this.fb.group({
    // reportType: [null, Validators.required],
    // reportName: [null, Validators.required],
  });
  step1Model: any = {
    type: null,
    reportName: null,
  };
  step2Form = this.fb.group({
    // reportType: [null, Validators.required],
    // reportName: [null, Validators.required],
  });
  step2Model: any = {
    editionType: null,
    excludingCompanies: null,
  };
  // end of form groups and form models

  private wizard: KTWizard;
  stepToCompleteFrom: number = this._activatedRoute.snapshot.queryParams['completedSteps'];
  activeStep: number;
  allTypes: any[] = [];

  selectAttributesDataSource: any = {};
  allCompanies: SelectItemDto[] = [];
  allEditionTypes: EditionListDto[] = [];

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _router: Router,
    private fb: FormBuilder,
    @Inject(API_BASE_URL) hostUrl: string,
    private _editionService: EditionServiceProxy,
    private _pricePackagesServiceProxy: PricePackageServiceProxy,
    private enumService: EnumToArrayPipe
  ) {
    super(injector);
    ajaxSetup.ajaxSettings = {
      headers: {
        Authorization: 'Bearer ' + abp.auth.getToken(),
      },
    };
    this.hostUrl = hostUrl;
  }

  ngOnInit(): void {
    this.reportUrl = 'TripDetailsReport';
  }

  ngAfterViewInit() {
    // Initialize form wizard
    this.wizard = new KTWizard(this.el.nativeElement, {
      startStep: this.stepToCompleteFrom || 1,
    });
    this.activeStep = this.wizard.getStep();
    console.log('activeStep', this.activeStep);
    // Validation before going to next page
    this.watchForWizardNextBtnOnClick();
  }

  /**
   * handles wizard next btn on click event
   * @private
   */
  private watchForWizardNextBtnOnClick() {
    this.wizard.on('beforeNext', (wizardObj) => {
      switch (this.wizard.getStep()) {
        case 1: {
          console.log('this.step1Form', this.step1Form);
          // document.getElementById('step1FormGroupButton').click();
          // this.step1FormGroup.instance.validate();
          if (this.step1Form.invalid) {
            wizardObj.stop();
            this.step1Form.markAllAsTouched();
            this.notify.error(this.l('PleaseCompleteMissingFields'));
          } else {
            this.createOrEditStep1();
            this.getEditionTypes();
            wizardObj.goNext();
          }
          break;
        }
        case 2: {
          if (this.step2Form.invalid) {
            wizardObj.stop();
            this.step2Form.markAllAsTouched();
            this.notify.error(this.l('PleaseCompleteMissingFields'));
          } else {
            this.createOrEditStep2();
            wizardObj.goNext();
          }
          break;
        }
        case 3: {
          break;
        }
        default: {
          break;
        }
      }
    });
  }

  ngOnDestroy() {
    this.wizard = undefined;
  }

  /**
   * send api call to save step1
   * @private
   */
  private createOrEditStep1() {
    this.updateRoutingQueries(1);
  }

  /**
   * send api call to save step2
   * @private
   */
  private createOrEditStep2() {
    this.updateRoutingQueries(2);
  }

  /**
   * Updates Router Query Parameters
   * @param step
   */
  updateRoutingQueries(step?) {
    this._router.navigate([], {
      queryParams: {
        completedSteps: step || 1,
      },
      queryParamsHandling: 'merge',
    });
    this.activeStep = step;
  }

  open() {
    this.designer.bindingSender.open(this.reportUrl);
  }

  /**
   * get all edition types
   * @private
   */
  private getEditionTypes() {
    // this._editionService.getEditions().subscribe((result) => {
    //     this.allEditionTypes = result.items;
    // });
    this.allEditionTypes = this.enumService.transform(CompanyType).map((item) => {
      return {
        id: Number(item.key),
        displayName: item.value,
      };
    });
  }

  /**
   * loads All Companies base on selected edition
   */
  loadAllCompanies(): void {
    // there is a difference between company type in front end  & backend
    this._pricePackagesServiceProxy.getCompanies(this.step2Model.editionType).subscribe((res) => {
      this.allCompanies = res.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
  }
}
