import { AfterViewInit, Component, ElementRef, Injector, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import KTWizard from '@metronic/common/js/components/wizard';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { DxValidationGroupComponent } from '@node_modules/devextreme-angular/ui/validation-group';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { DxReportDesignerComponent } from '@node_modules/devexpress-reporting-angular';
import * as ko from '@node_modules/knockout';
import { ajaxSetup } from '@node_modules/@devexpress/analytics-core/core/internal/ajaxSetup';

@Component({
  selector: 'app-create-report-type',
  templateUrl: './create-report-type.component.html',
  styleUrls: [
    './create-report-type.component.scss',
    '../../../../../node_modules/jquery-ui/themes/base/all.css',
    '../../../../../node_modules/devexpress-richedit/dist/dx.richedit.css',
    '../../../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css',
    '../../../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css',
    '../../../../../node_modules/@devexpress/analytics-core/dist/css/dx-querybuilder.css',
    '../../../../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css',
    '../../../../../node_modules/devexpress-reporting/dist/css/dx-reportdesigner.css',
  ],
  animations: [appModuleAnimation()],
})
export class CreateReportTypeComponent extends AppComponentBase implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('wizard', { static: true }) el: ElementRef;
  @ViewChild('step1FormGroup', { static: false }) step1FormGroup: DxValidationGroupComponent;

  @ViewChild('designer', { static: false }) designer: DxReportDesignerComponent;

  title = 'DXReportViewerSample';
  getDesignerModelAction = `/DXXRD/GetDesignerModel`;
  hostUrl = 'http://localhost:44301/';
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
  step2Model: any[] = [];
  step3Form = this.fb.group({
    // reportType: [null, Validators.required],
    // reportName: [null, Validators.required],
  });
  step3Model: any = {
    editionType: null,
    excludingCompanies: null,
  };
  // end of form groups and form models

  private wizard: KTWizard;
  stepToCompleteFrom: number = this._activatedRoute.snapshot.queryParams['completedSteps'];
  activeStep: number;
  allTypes: any[] = [];

  selectAttributesDataSource: any = {};
  allCompanies: any[] = [];
  allEditionTypes: any[] = [];

  constructor(injector: Injector, private _activatedRoute: ActivatedRoute, private _router: Router, private fb: FormBuilder) {
    super(injector);
    ajaxSetup.ajaxSettings = {
      headers: {
        Authorization: 'Bearer ' + abp.auth.getToken(),
      },
    };
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
            this.getAttributes();
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
          if (this.step3Form.invalid) {
            wizardObj.stop();
            this.step3Form.markAllAsTouched();
            this.notify.error(this.l('PleaseCompleteMissingFields'));
          } else {
            this.createOrEditStep3();
            wizardObj.goNext();
          }
          break;
        }
        case 4: {
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
   * send api call to save step3
   * @private
   */
  private createOrEditStep3() {
    this.updateRoutingQueries(3);
  }

  /**
   * send api to get all report type attributes
   */
  getAttributes() {
    let self = this;
    this.selectAttributesDataSource = {};
    this.selectAttributesDataSource.store = new CustomStore({
      key: 'id',
      load(loadOptions: LoadOptions) {
        return new Promise((resolve) => {
          resolve([]);
        }).then((res) => {
          return {
            data: res,
            totalCount: 0,
          };
        });
        /*self._unitOfMeasuresServiceProxy
                    .getAll(JSON.stringify(loadOptions))
                    .toPromise()
                    .then((response) => {
                        return {
                            data: response.data,
                            totalCount: response.totalCount,
                        };
                    })
                    .catch((error) => {
                        console.log(error);
                        throw new Error('Data Loading Error');
                    });*/
      },
    });
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
}
