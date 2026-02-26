using System.Collections.Generic;
using PropertyRental.Models;

namespace PropertyRental
{
    public class Property
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Address { get; set; }

        public decimal Price { get; set; }

        public string? ImagePath { get; set; }

        public bool IsAvailable { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
