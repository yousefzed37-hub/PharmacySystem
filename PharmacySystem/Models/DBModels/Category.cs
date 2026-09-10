using PharmacySystem.Models.DBModels;
using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.Models
{
    public class Category
    {
         public int Id { get; set; }
         public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
         
        public string? Description { get; set; }
         public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
    }
}

