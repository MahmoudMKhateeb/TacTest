import { AfterViewInit, Component, ElementRef, EventEmitter, Inject, Injector, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import KTWizard from '@metronic/common/js/components/wizard';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { DxValidationGroupComponent } from '@node_modules/devextreme-angular/ui/validation-group';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import {
  API_BASE_URL,
  CreateOrEditReportDto,
  ReportFormat,
  ReportParameterDto,
  ReportParameterType,
  ReportServiceProxy,
  SelectItemDto,
} from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { AutomationSetupModalComponent } from '@app/main/reporting/tenant-reports/generate-report-by-company/automation-setup-modal/automation-setup-modal.component';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { ajaxSetup } from '@node_modules/@devexpress/analytics-core/core/internal/ajaxSetup';
import * as ko from '@node_modules/knockout';
import localAnalyticMessages from '../../../../../dx-analytics-core.ar.json';
import localReportingMessages from '../../../../../dx-reporting.ar.json';

@Component({
  selector: 'app-generate-report-by-company',
  templateUrl: './generate-report-by-company.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrls: [
    './generate-report-by-company.component.scss',
    '../../../../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css',
    '../../../../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css',
    '../../../../../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css',
  ],
  animations: [appModuleAnimation()],
})
export class GenerateReportByCompanyComponent extends AppComponentBase implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('wizard', { static: true }) el: ElementRef;
  @ViewChild('automationSetupModal', { static: false }) automationSetupModal: AutomationSetupModalComponent;
  @ViewChild('step1FormGroup', { static: false }) step1FormGroup: DxValidationGroupComponent;

  // start of form groups and form models
  step1Form = this.fb.group({
    reportType: [null, Validators.required],
    reportName: [null, Validators.required],
    grantedRoles: [null, Validators.required],
    reportFormat: [null, Validators.required],
  });

  step2Form = this.fb.group({});

  step3Form = this.fb.group({});

  // end of form groups and form models
  koReportUrl = ko.observable('');
  get reportUrl() {
    return this.koReportUrl();
  }

  set reportUrl(newUrl) {
    this.koReportUrl(newUrl);
  }

  private wizard: KTWizard;
  stepToCompleteFrom: number = this._activatedRoute.snapshot.queryParams['completedSteps'];
  activeStep: number;
  reportDefinitions: SelectItemDto[] = [];
  selectAttributesDataSource: any = {};
  allRoles: SelectItemDto[] = [];
  allUsers: SelectItemDto[] = [];
  reportDto: CreateOrEditReportDto;
  allFormats;
  reportParameterTypeEnum = ReportParameterType;
  parameters: Map<string, any>;
  invokeAction = '/DXXRDV';
  hostUrl: string;

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _router: Router,
    private fb: FormBuilder,
    @Inject(API_BASE_URL) hostUrl: string,
    private _reportService: ReportServiceProxy,
    private _enumToArray: EnumToArrayPipe
  ) {
    super(injector);
    this.hostUrl = hostUrl;
  }

  ngOnInit(): void {
    this.parameters = new Map<string, string>();
    this.reportDto = new CreateOrEditReportDto();
    ajaxSetup.ajaxSettings = {
      headers: {
        Authorization: `Bearer ${abp.auth.getToken()}`,
      },
    };
    this.allFormats = this._enumToArray.transform(ReportFormat).map((item) => {
      item.key = Number(item.key);
      return item;
    });
    this.loadReportDefinitions();
    this.loadRoles();
  }

  private loadReportDefinitions() {
    this._reportService.getReportDefinitionsForDropdown().subscribe((result) => {
      this.reportDefinitions = result;
    });
  }
  ngAfterViewInit() {
    // Initialize form wizard
    this.wizard = new KTWizard(this.el.nativeElement, {
      startStep: this.stepToCompleteFrom || 1,
    });
    this.activeStep = this.wizard.getStep();
    this.watchForWizardNextBtnOnClick();
  }

  /**
   * handles wizard next btn on click event
   * @private
   */
  private watchForWizardNextBtnOnClick() {
    this.wizard.on('afterPrev', (wizardObj) => {
      console.log('afterPrev', wizardObj);
      this.updateRoutingQueries(this.wizard.getStep());
    });
    this.wizard.on('beforeNext', (wizardObj) => {
      switch (this.wizard.getStep()) {
        case 1: {
          console.log('this.step1Form', this.step1Form);
          document.getElementById('step1FormGroupButton').click();
          this.step1FormGroup.instance.validate();
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
    this.loadReportUrl();
  }

  /**
   * send api call to save step2
   * @private
   */
  private createOrEditStep2() {
    this.updateRoutingQueries(2);
    if (!this.reportDto.id) {
      this.createOrEdit();
    }
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
      key: 'parameterName',
      load() {
        return self._reportService.getReportFilters(self.reportDto.reportDefinitionId).toPromise();
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

  openGeneratedAutomaticallyModal() {
    // if (this.step4Model.isGeneratedAutomatically) {
    //   this.automationSetupModal.show();
    // }
  }

  isArray(item: any): boolean {
    return Array.isArray(item);
  }

  private loadRoles() {
    this._reportService.getTenantRoles().subscribe((result) => {
      this.allRoles = result;
    });
  }

  private loadUsers() {
    if (isNotNullOrUndefined(this.reportDto.grantedRoles)) {
      this._reportService.getTenantUsers(this.reportDto.grantedRoles).subscribe((result) => {
        this.allUsers = result;
      });
    }
  }

  selectedRoleChanged() {
    this.loadUsers();
    this.reportDto.excludedUsers = undefined;
  }

  validateGroup(params) {
    console.log('params', params);
    params.validationGroup.validate();
  }

  setParameter(name: string, value) {
    this.parameters.set(name, value);
    console.log('parameters Value');
    console.log(this.parameters);
  }
  getParameter(name: string): any {
    return this.parameters.get(name);
  }

  createOrEdit() {
    const parameterKeys = Array.from(this.parameters.keys());
    const selectedParameters = parameterKeys
      .filter((paraKey) => {
        return isNotNullOrUndefined(this.parameters.get(paraKey));
      })
      .map((paraKey) => {
        const item = new ReportParameterDto();
        item.name = paraKey;
        item.value = this.parameters.get(paraKey);
        return item;
      });
    this.reportDto.parameters = selectedParameters;
    this._reportService.createOrEdit(this.reportDto).subscribe((reportId) => {
      this.reportDto.id = reportId;
      this.reportUrl = `${this.reportUrl}?reportId=${reportId}`;
    });
  }

  private loadReportUrl() {
    this._reportService.getReportUrl(this.reportDto.reportDefinitionId).subscribe((result) => {
      this.reportUrl = result;
    });
  }

  publish() {
    this._reportService.publish(this.reportDto.id).subscribe(() => {
      this.notify.success(this.l('CreatedSuccessfully'));
      this._router.navigate(['app/main/reporting/all-reports']);
    });
  }

  customizeLocalization($event) {
    $event.args.LoadMessages(localAnalyticMessages);
    $event.args.LoadMessages(localReportingMessages);
  }
}
