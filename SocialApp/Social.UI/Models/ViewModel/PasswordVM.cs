using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Social.UI.Models.ViewModel
{
    public class PasswordVM
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Şifre boş geçilemez.")]
        public string OldPassword { get; set; }

        //RegularExpression(@"^(?=.*\d).{4,8}$", ErrorMessage = "Şifre 4-8 karakter arasında olmalı ve en az bir sayı içermelidir.")
        [Required(ErrorMessage = "Şifre boş geçilemez.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Lütfen şifrenizi tekrar giriniz."), Compare("NewPassword", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; }
    }
}
