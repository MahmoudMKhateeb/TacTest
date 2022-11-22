import * as moment from 'moment';
export class InvoiceSearchInputDto {
  paymentDate!: moment.Moment | undefined;
  waybillOrSubWaybillNumber!: number;
  containerNumber!: string;
  accountNumber!: string;
}
