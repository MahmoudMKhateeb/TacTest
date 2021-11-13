using System.ComponentModel;

namespace TACHYON.PriceOffers
{
    public enum PriceOfferCommissionType : byte
    {
        //todo reference settings in xml comments  

        /// <summary>
        /// Market Place Request Commission Percentage
        /// Percentage/Number.
        /// Default Value = 30%
        /// </summary>
        [Description("CommissionPercentage")]
        CommissionPercentage = 1,

        /// <summary>
        /// Market Place Request Commission Value
        /// Floating number, up to two decimal places.
        ///  Default Value = 100
        /// </summary>
        [Description("CommissionValue")]
        CommissionValue = 2,

        /// <summary>
        /// Market Place Request Commission Min Value
        /// Floating number, up to two decimal places.
        /// The minimum value of a commission for a request in the market place.
        /// If the applied percentage of the commission got a value less than this min value, the system will take the min value.
        /// Ex: Commission percentage is 10%
        /// Request price is 100$.
        /// Min value is 20$.
        /// 10% out of the 100$ = 10$
        /// 10$ is less than min value, so the commission value in this case will be 20$.
        /// </summary>
        [Description("CommissionMinimumValue")]
        CommissionMinimumValue = 3
    }
}
