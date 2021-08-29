using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Listjj.Models
{
    public class File
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[] Bytes {get; set;}
        public Guid ListItemId { get; set; }
        [ForeignKey("ListItemId")] public ListItem ListItem { get; set; } 
    }
}