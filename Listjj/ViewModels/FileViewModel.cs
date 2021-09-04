using System.ComponentModel.DataAnnotations.Schema;
using System;
using Listjj.Models;
using System.Collections.Generic;


namespace Listjj.ViewModels
{
    public class FileViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public Guid ListItemId { get; set; }
    }
}