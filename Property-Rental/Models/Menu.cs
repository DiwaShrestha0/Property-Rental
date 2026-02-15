using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyRental.Models
{
    public class Menu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string ControllerName { get; set; }

        [StringLength(100)]
        public string ActionName { get; set; }

        public virtual ICollection<UserMenu> UserMenus { get; set; }
    }
}
