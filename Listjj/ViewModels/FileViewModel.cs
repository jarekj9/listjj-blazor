using System;

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