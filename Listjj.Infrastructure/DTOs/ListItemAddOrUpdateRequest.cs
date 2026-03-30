using System;
using System.Collections.Generic;

namespace Listjj.Infrastructure.DTOs
{
    public class ListItemAddOrUpdateRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public bool Active { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
