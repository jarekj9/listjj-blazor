using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;
using Listjj.Models;

namespace Listjj.ViewModels
{
    public class CategoryViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsEditing;
    }


}