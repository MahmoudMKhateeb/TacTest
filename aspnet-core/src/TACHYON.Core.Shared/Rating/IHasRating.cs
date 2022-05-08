namespace TACHYON.Rating
{
    public interface IHasRating
    {
        decimal Rate { get; set; }
        int RateNumber { get; set; }
    }
}