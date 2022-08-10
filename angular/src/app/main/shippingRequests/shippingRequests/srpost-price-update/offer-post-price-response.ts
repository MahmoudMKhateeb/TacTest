export class OfferPostPriceResponse {
  public isOfferRejected: boolean;
  public rejectionReason: string;

  constructor(isOfferRejected: boolean, rejectionReason: string) {
    this.isOfferRejected = isOfferRejected;
    this.rejectionReason = rejectionReason;
  }
}
