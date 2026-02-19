// DTO for updating inventory record 
using System.ComponentModel.DataAnnotations;
namespace ManuBackend.DTOs
{
   
        public class UpdateInventoryMaterialDto
        {
            [Range(0, int.MaxValue)]
            public int? AvailableQty { get; set; }

            [Range(0, int.MaxValue)]
            public int? ThresholdQty { get; set; }
        }
    }