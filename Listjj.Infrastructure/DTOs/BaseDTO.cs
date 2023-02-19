using System;

namespace Listjj.Infrastructure.DTOs
{
    public class BaseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
    }
}
