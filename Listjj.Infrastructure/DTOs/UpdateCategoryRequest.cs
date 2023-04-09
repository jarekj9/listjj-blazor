using System;

namespace Listjj.Infrastructure.DTOs
{
    public class UpdateCategoryRequest
    {
        public Guid UserId { get; set; }
        public Guid RecentCategoryId { get; set; }
    }
}
