using System;

namespace Listjj.Infrastructure.ViewModels
{
    public class BaseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
    }
}
