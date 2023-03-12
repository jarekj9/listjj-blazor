using System;
using System.ComponentModel.DataAnnotations;

namespace Listjj.Infrastructure.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        [Required]
        [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-‌​]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"(?=[A-Za-z0-9@#$%^&+!=*-]+$)^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[@#$%^&+!=*-])(?=.{6,}).*$",
            ErrorMessage = "Password missing required chars. 6-32 chars and there must be lower case, upper case, digit and one of @#$%^&+!=*-")]
        public string Password { get; set; } = "";
        public string Role { get; set; }

        public bool IsEditing { get; set; }
    }
}