namespace PropertyRental.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProperties { get; set; }
        public int TotalUsers { get; set; }
        public int TotalRoles { get; set; }
        public int TotalMenus { get; set; }
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
    }
}
