namespace TACHYON.Dto
{
    public class SelectItemDto : ISelectItemDto
    {

        public SelectItemDto(string id,
            string displayName,
            bool isOther = false)
        {
            Id = id;
            DisplayName = displayName;
            IsOther = DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName);
        }

        public SelectItemDto()
        {
        }

        public string Id { get; set; }

        public bool? IsOther { get; set; }

        public string DisplayName { get; set; }
    }

    public interface ISelectItemDto
    {
        string Id { get; set; }
        string DisplayName { get; set; }
        bool? IsOther { get; set; }
    }
}