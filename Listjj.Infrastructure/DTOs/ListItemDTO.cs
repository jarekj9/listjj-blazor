using System;

namespace Listjj.Infrastructure.DTOs
{
    public class ListItemDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public double SequenceNumber { get; set; }
        public double Value { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public Guid CategoryId { get; set; }
        public string Tags { get; set; }
    }
}
