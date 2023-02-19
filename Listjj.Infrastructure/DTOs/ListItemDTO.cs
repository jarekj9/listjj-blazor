using System;

namespace Listjj.Infrastructure.DTOs
{
    public class ListItemDTO : BaseDTO
    {
        public double SequenceNumber { get; set; }
        public double Value { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public Guid CategoryId { get; set; }
        public string Tags { get; set; }
    }
}
