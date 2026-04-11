using Microsoft.AspNetCore.Identity;

namespace JegyMester.DataContext.Entities
{
    public class Role : IdentityRole
    {
        public override string Id { get; set; } = string.Empty;
        public override string? Name
        {
            get => base.Name;
            set => base.Name = value;
        }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
