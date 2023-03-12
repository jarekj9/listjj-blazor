using System;

namespace Listjj.Infrastructure.ViewModels
{
    public class FileSimpleViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public Guid ListItemId { get; set; }
    }
}