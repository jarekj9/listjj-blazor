using System;

namespace Listjj.Infrastructure.ViewModels
{
    public class BaseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
