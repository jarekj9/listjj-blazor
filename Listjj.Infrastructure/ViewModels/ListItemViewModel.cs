using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Listjj.Infrastructure.ViewModels
{
    public class ListItemViewModel : BaseViewModel
    {
        public double SequenceNumber { get; set; }
        public double Value { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public Guid CategoryId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public bool IsEditing { get; set; }
        public CategoryViewModel Category { get; set; }
        public List<FileViewModel> Files { get; set; }
    }
}
