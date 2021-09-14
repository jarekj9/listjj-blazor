using System.ComponentModel.DataAnnotations.Schema;
using System;
using Listjj.Models;
using System.Collections.Generic;


namespace Listjj.ViewModels
{
    public class ListItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Value { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public Guid CategoryId { get; set; }
        public string UserId { get; set; }
        public bool IsEditing;
        public string Tags { get; set; } = string.Empty;
        public CategoryViewModel Category { get; set; }
        public ApplicationUser User { get; set; }
        public List<FileViewModel> Files { get; set;} = new List<FileViewModel>();
    }
}