using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace Listjj.Models
{
    public class ListItem : BaseEntity
    {
        public double Value { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")] public Category Category { get; set; }
        public virtual ICollection<File> Files { get; set; }

        public ListItem() {
            this.Value = 0;
            this.Name = "";
            this.Description = "";
            this.Active = true;
        }
    }
}