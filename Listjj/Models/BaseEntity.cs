using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Listjj.Models
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        [NotMapped] public bool IsEditing;
        [ForeignKey("UserId")] public ApplicationUser User { get; set; }

    }
}