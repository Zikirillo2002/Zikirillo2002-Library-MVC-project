using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    [Table(nameof(Book))]
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(550)]
        public string Description { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        public ICollection<Publication> Publications { get; set; }
    }
}
