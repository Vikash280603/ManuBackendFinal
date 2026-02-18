using System.ComponentModel.DataAnnotations;
namespace ManuBackend.DTOs
{
    public class CreateInventoryDto
    {
        [Required]
        public int ProductId {  get; set; }
        [Required]
        public string Location { get; set; } = string.Empty;
        public List<CreateInventoryMaterialDto>? Materials {  get; set; }
    }
}
