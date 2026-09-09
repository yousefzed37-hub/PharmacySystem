using PharmacySystem.Models.DBModels;
using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }
             
        public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
    }
}

