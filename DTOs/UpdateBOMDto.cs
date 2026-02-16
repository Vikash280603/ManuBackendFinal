// DTO for updating an existing BOM entry  

using System.ComponentModel.DataAnnotations;

namespace ManuBackend.DTOs
{
    public class UpdateBOMDto
    {
        public string? MaterialName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int? Quantity { get; set; }
    }
}