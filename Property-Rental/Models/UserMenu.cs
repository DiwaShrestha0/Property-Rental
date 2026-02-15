using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyRental.Models
{
    // Note: Composite Primary Key (UserId, MenuId) typically requires Fluent API configuration in DbContext
    // or [PrimaryKey(nameof(UserId), nameof(MenuId))] in EF Core 7+.
    public class UserMenu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public int MenuId { get; set; }

        [ForeignKey("MenuId")]
        public virtual Menu Menu { get; set; }
    }
}
