import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import {
  GetAllGoodsCategoriesForDropDownOutput,
  GoodsDetailsServiceProxy,
  SaasPricePackageServiceProxy,
  SelectItemDto,
  ShippingTypeEnum,
} from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { Workbook } from '@node_modules/exceljs';
import { exportDataGrid } from '@node_modules/devextreme/excel_exporter';
import { saveAs } from 'file-saver';

@Component({
  selector: 'app-saas-price-package',
  templateUrl: './saas-price-package.component.html',
})
export class SaasPricePackageComponent extends AppComponentBase implements OnInit {
  dataSource: any;
  AllTruckTypes: SelectItemDto[];
  AllTransType: SelectItemDto[];
  Allcities: SelectItemDto[];
  AllActores: SelectItemDto[];
  allGoodCategorys: GetAllGoodsCategoriesForDropDownOutput[];
  allShippingTypes: SelectItemDto[];

  constructor(
    private injector: Injector,
    private _saasPP: SaasPricePackageServiceProxy,
    private _enumToArrayPipe: EnumToArrayPipe,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAllPricePackages();
    this.getAllDropDowns();
    this.allShippingTypes = this._enumToArrayPipe.transform(ShippingTypeEnum).map((item) => {
      const selectItem = new SelectItemDto();
      (selectItem.id as any) = Number(item.key);
      selectItem.displayName = item.value;
      return selectItem;
    });
  }

  getAllPricePackages() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        return self._saasPP
          .getAll(JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            return {
              data: response.data,
              totalCount: response.totalCount,
              summary: response.summary,
              groupCount: response.groupCount,
            };
          })
          .catch((error) => {
            throw new Error('Data Loading Error');
          });
      },
    });
  }

  onExporting(e) {
    const workbook = new Workbook();
    const worksheet = workbook.addWorksheet('PricePackages');

    exportDataGrid({
      component: e.component,
      worksheet,
      autoFilterEnabled: true,
      customizeCell: ({ gridCell, excelCell }) => {
        if (gridCell.rowType === 'data') {
          if (gridCell.column.dataField === 'status') {
            excelCell.value = this.l(gridCell.value);
          }
        }
        if (gridCell.rowType === 'group') {
          excelCell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'BEDFE6' } };
        }
      },
    }).then(() => {
      workbook.xlsx.writeBuffer().then((buffer) => {
        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'PricePackages.xlsx');
      });
    });
    e.cancel = true;
  }

  delete(id) {
    this._saasPP.delete(id).subscribe(() => {
      this.getAllPricePackages();
      this.notify.info(this.l('SuccessfullyDeleted'));
    });
  }
  getAllDropDowns() {
    this._saasPP.getAllTruckTypeForDropdown(undefined).subscribe((res) => {
      this.AllTruckTypes = res;
    });
    this._saasPP.getAllTransportTypeForDropdown().subscribe((res) => {
      this.AllTransType = res;
    });
    this._saasPP.getAllCitiesForDropdown(undefined).subscribe((res) => {
      this.Allcities = res;
    });
    this._saasPP.getAllActorShippersForDropdown().subscribe((res) => {
      this.AllActores = res;
    });
    this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown(undefined).subscribe((result) => {
      this.allGoodCategorys = result;
    });
  }
}
