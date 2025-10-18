namespace ProductService.DTOs.OtherDTOs
{
    public class FilterOptionsDTO
    {
        public List<ColorDTO> Colors { get; set; } = new();
        public List<SizeDTO> Sizes { get; set; } = new();
    }
}
