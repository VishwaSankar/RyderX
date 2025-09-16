namespace RyderX_Server.DTO.ExtraServiceDTOs
{
    public class ExtraServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}
