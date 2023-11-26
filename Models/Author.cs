using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models
{
    [Table(nameof(Author))]
    public class Author
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string FullName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public ICollection<Publication> Publications { get; set; }
    }
}
