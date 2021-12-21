namespace TACHYON.Dto
{
    public class SelectItemDto : ISelectItemDto
    {
        private string _displayName;

        public SelectItemDto(string id, string displayName, bool isOther = false)
        {
            Id = id;
            DisplayName = displayName;
            IsOther = isOther;
        }
        public SelectItemDto()
        {

        }

        public string Id { get; set; }

        public bool IsOther { get; set; }

        public string DisplayName
        {
            get
            {
                return TranslatedDisplayName ?? _displayName;
            }
            set
            {
                _displayName = value;

            }

        }


        public string TranslatedDisplayName { get; set; }


    }

    public interface ISelectItemDto
    {
        string Id { get; set; }
        string DisplayName { get; set; }
        bool IsOther { get; set; }


    }
}