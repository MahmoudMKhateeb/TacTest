import { AfterViewInit, Component, ElementRef, Inject, Injector, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import KTWizard from '@metronic/common/js/components/wizard';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { DxValidationGroupComponent } from '@node_modules/devextreme-angular/ui/validation-group';
import { DxReportDesignerComponent } from '@node_modules/devexpress-reporting-angular';
import * as ko from '@node_modules/knockout';
import { ajaxSetup } from '@node_modules/@devexpress/analytics-core/core/internal/ajaxSetup';
import {
  API_BASE_URL,
  CompanyType,
  CreateOrEditReportDefinitionDto,
  CreateReportTemplateByNameInput,
  EditionListDto,
  EditionServiceProxy,
  ReportDefinitionServiceProxy,
  ReportType,
  SelectItemDto,
} from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { ActionId } from '@node_modules/devexpress-reporting/designer/actions/actionId';
import localAnalyticMessages from '../../../../../../dx-analytics-core.ar.json';
import localReportingMessages from '../../../../../../dx-reporting.ar.json';

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
  @ViewChild('step2FormGroup', { static: false }) step2FormGroup: DxValidationGroupComponent;
  @ViewChild('step3FormGroup', { static: false }) step3FormGroup: DxValidationGroupComponent;

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
    reportType: [null, Validators.required],
    reportName: [null, Validators.required],
  });

  step2Form = this.fb.group({
    filters: [null, Validators.required],
  });

  step3Form = this.fb.group({
    editionType: [null, Validators.required],
  });
  // end of form groups and form models

  private wizard: KTWizard;
  stepToCompleteFrom: number = this._activatedRoute.snapshot.queryParams['completedSteps'];
  activeStep: number;
  allTypes = this.enumService.transform(ReportType).map((item) => {
    item.key = Number(item.key);
    return item;
  });
  allCompanies: SelectItemDto[] = [];
  allEditionTypes: EditionListDto[] = [];
  reportDefinitionDto: CreateOrEditReportDefinitionDto;

  isEditionTypeInvalid: boolean;
  editionTypeCallbacks: any[] = [];
  editionTypeAdapterConfig = {
    getValue: () => {
      return this.selectedGrantedEditionIds?.length > 0;
    },
    applyValidationResults: (e) => {
      this.isEditionTypeInvalid = !e.isValid;
    },
    validationRequestsCallbacks: this.editionTypeCallbacks,
  };

  isExcludingCompaniesInvalid: boolean;

  isFiltersInvalid: boolean;
  filtersCallbacks: any[] = [];
  filtersAdapterConfig = {
    getValue: () => {
      return this.reportDefinitionDto.parameterDefinitions?.length > 0;
    },
    applyValidationResults: (e) => {
      this.isFiltersInvalid = !e.isValid;
    },
    validationRequestsCallbacks: this.filtersCallbacks,
  };
  selectAttributesDataSource: any = {};
  selectedGrantedEditionIds: number[];
  selectedExcludedTenantIds: number[];
  isReportDefinitionNameDuplicated: boolean;

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _router: Router,
    private fb: FormBuilder,
    @Inject(API_BASE_URL) hostUrl: string,
    private _editionService: EditionServiceProxy,
    private enumService: EnumToArrayPipe,
    private _reportDefinitionService: ReportDefinitionServiceProxy
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
    this.reportDefinitionDto = new CreateOrEditReportDefinitionDto();
    this.isReportDefinitionNameDuplicated = false;
    this.reportUrl = '';
    const clonedReportDefinitionId = this._activatedRoute.snapshot.queryParams['clonedReportDefinitionId'];

    if (isNotNullOrUndefined(clonedReportDefinitionId)) {
      // do a request to server-side to get cloned report definition
      this._reportDefinitionService.getReportDefinitionForClone(clonedReportDefinitionId).subscribe((result) => {
        this.reportDefinitionDto.type = result.type;
        this.reportDefinitionDto.displayName = result.displayName;
        this.reportDefinitionDto.parameterDefinitions = result.parameterDefinitions;
        this.selectedGrantedEditionIds = result.grantedEditionIds;
        this.selectedExcludedTenantIds = result.excludedTenantIds;
      });
    }
  }

  ngAfterViewInit() {
    // Initialize form wizard
    this.wizard = new KTWizard(this.el.nativeElement, {
      startStep: this.stepToCompleteFrom || 1,
    });
    this.activeStep = this.wizard.getStep();
    // Validation before going to next page
    this.watchForWizardNextBtnOnClick();
  }

  /**
   * handles wizard next btn on click event
   * @private
   */
  private watchForWizardNextBtnOnClick() {
    this.wizard.on('afterPrev', (wizardObj) => {
      this.updateRoutingQueries(this.wizard.getStep());
    });
    this.wizard.on('beforeNext', (wizardObj) => {
      switch (this.wizard.getStep()) {
        case 1: {
          document.getElementById('step1FormGroupButton').click();
          this.step1FormGroup.instance.validate();
          if (this.step1Form.invalid) {
            wizardObj.stop();
            this.step1Form.markAllAsTouched();
            this.notify.error(this.l('PleaseCompleteMissingFields'));
          } else {
            this.createOrEditStep1();
            this.getFilters();
            this.getEditionTypes();
            wizardObj.goNext();
          }
          break;
        }
        case 2: {
          document.getElementById('step2FormGroupButton').click();
          this.step2FormGroup.instance.validate();
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
          document.getElementById('step3FormGroupButton').click();
          this.step3FormGroup.instance.validate();
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
          this.createOrEdit();
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
    const reportTemplateInput = new CreateReportTemplateByNameInput();
    reportTemplateInput.reportDefinitionType = this.reportDefinitionDto.type;
    reportTemplateInput.reportDefinitionName = this.reportDefinitionDto.displayName;
    if (isNotNullOrUndefined(this.reportDefinitionDto.reportTemplateUrl) && this.reportDefinitionDto.reportTemplateUrl !== '') {
      return;
    }
    this._reportDefinitionService.createTemplateByName(reportTemplateInput).subscribe((result) => {
      this.reportDefinitionDto.reportTemplateUrl = result.url;
      this.reportUrl = result.url;
    });
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

  createOrEdit(): void {
    this.reportDefinitionDto.grantedEditionIds = (this.selectedGrantedEditionIds as any[]).map((x) => x.id);
    if (isNotNullOrUndefined(this.reportDefinitionDto.excludedTenantIds)) {
      this.reportDefinitionDto.excludedTenantIds = (this.selectedExcludedTenantIds as any[]).map((x) => x.id);
    }
    this._reportDefinitionService.createOrEdit(this.reportDefinitionDto).subscribe(() => {
      this.notify.success('SavedSuccessfully');
      this._router.navigate(['app/main/reporting/report-types']);
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
    if (isNotNullOrUndefined(this.selectedGrantedEditionIds)) {
      (this.selectedGrantedEditionIds as any) = this.allEditionTypes.filter((item) => this.selectedGrantedEditionIds.includes(item.id as any));
      this.loadAllCompanies();
    }
  }

  /**
   * loads All Companies base on selected edition
   */
  loadAllCompanies(): void {
    // there is a difference between company type in front end  & backend
    this.revalidateEditionType();
    if (isNotNullOrUndefined(this.selectedGrantedEditionIds)) {
      const selectedEditionIds = (this.selectedGrantedEditionIds as any[]).map((x) => x.id);

      this._reportDefinitionService.getCompanies(selectedEditionIds).subscribe((res) => {
        this.allCompanies = res.map((item) => {
          (item.id as any) = Number(item.id);
          return item;
        });
        if (isNotNullOrUndefined(this.selectedExcludedTenantIds)) {
          (this.selectedExcludedTenantIds as any) = this.allCompanies.filter((item) => this.selectedExcludedTenantIds.includes(item.id as any));
        }
      });
    }
  }

  validateGroup(params) {
    console.log('params', params);
    params.validationGroup.validate();
  }

  revalidateEditionType() {
    this.editionTypeCallbacks.forEach((func) => {
      func();
    });
  }

  revalidateFilters() {
    this.filtersCallbacks.forEach((func) => {
      func();
    });
    this.step2Form.get('filters').setValue(this.reportDefinitionDto.parameterDefinitions);
  }

  /**
   * send api to get all report type attributes
   */
  getFilters() {
    let self = this;
    this.selectAttributesDataSource = {};
    this.selectAttributesDataSource.store = new CustomStore({
      key: 'name',
      load() {
        return self._reportDefinitionService.getReportFilters(self.reportDefinitionDto.type).toPromise();
      },
    });
  }

  customizeMenuActions(args) {
    args.GetById(ActionId.Scripts).visible = false;
    args.GetById(ActionId.NewReport).visible = false;
    args.GetById(ActionId.NewReportViaWizard).visible = false;
    args.GetById(ActionId.OpenReport).visible = false;
    args.GetById(ActionId.SaveAs).visible = false;
    args.GetById(ActionId.Exit).visible = false;
    args.GetById(ActionId.Preview).visible = false;
  }

  // check definition name duplication

  checkNameDuplication(name: string) {
    if (this.reportDefinitionDto.id) {
      return;
    }
    this._reportDefinitionService.isReportDefinitionNameUsedBefore(name).subscribe((isNameDuplicated) => {
      this.isReportDefinitionNameDuplicated = isNameDuplicated;
      if (isNameDuplicated) {
        this.step1Form.controls['reportName'].setErrors({ invalid: true });
        this.step1Form.controls['reportName'].markAsTouched();
      }
    });
  }

  customizeLocalization($event) {
    $event.args.LoadMessages(localAnalyticMessages);
    $event.args.LoadMessages(localReportingMessages);
  }
}
