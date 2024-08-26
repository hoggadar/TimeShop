using System.ComponentModel.DataAnnotations;

namespace TimeShop.Domain.Entities
{
    public class RoleEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<UserEntity>? Users { get; set; }
    }
}
