namespace TACHYON.Dto
{
    public class SelectItemDto
    {
        public SelectItemDto(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }
        public SelectItemDto()
        {

        }

        public string Id { get; set; }
        public string DisplayName { get; set; }
    }
}