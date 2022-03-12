using System;
using System.ComponentModel.DataAnnotations;

namespace Listjj.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role;

        public bool IsEditing;
    }
}