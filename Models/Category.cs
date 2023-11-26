using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models
{
    [Table(nameof(Category))]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
