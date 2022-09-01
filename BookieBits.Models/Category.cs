using System.ComponentModel.DataAnnotations;

namespace BookieBits.Models
{
    public class Category
    {
        [Key]
        public int ID { get; set; }
        [Required (ErrorMessage ="Name field cannot be empty")]
        public string Name { get; set; }

        [Display(Name ="Display Order")]
        [Range(1,100, ErrorMessage ="Display must be between 1 and 100 !!")]
        public int DisplayOrder { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
