namespace UserManagementAPI.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Generate a unique ID at creation
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}
