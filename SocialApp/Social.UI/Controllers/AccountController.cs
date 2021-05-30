using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.Core.Entity.Enums;
using Social.Core.Service;
using Social.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Social.UI.Controllers
{
    //AccountController da Login ve Logout işlemleri oluşturuluyor.
    public class AccountController : Controller
    {
        private readonly ICoreService<User> _userservice;
        public AccountController(ICoreService<User> UserService)
        {
            _userservice = UserService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(User model)
        {   /*  Login Kısmı
            - Username ve Passworde göre kontrol
            - Userın active olmasına bakılması
            - İstediğimiz kullanıcılara UserName göre rol ataması ve admin ataması
            - Claim oluşturulması 
            - SignIn Oluşturulması
            - Home Index'e yönlendirme
            - Uygun olmayanlar için hata bildirimi
            */
            if (_userservice.Any(x => x.UserName == model.UserName && x.Password == model.Password))
            {
                var usermodel = _userservice.GetByDefault(x => x.UserName == model.UserName && x.Password == model.Password);

                if (usermodel.Status == Status.Active)
                {
                    string role = "Kullanıcı";

                    if (usermodel.UserName == "nmneren")
                    {
                        role = "Admin";
                    }

                    var claims = new List<Claim>()
                    {
                    new Claim(ClaimTypes.Role,role),
                    new Claim("ID",usermodel.ID.ToString()),
                    new Claim("UserName",usermodel.UserName),
                    new Claim(ClaimTypes.Name,usermodel.Name),
                    new Claim(ClaimTypes.Email,usermodel.Email),
                    new Claim("Image",usermodel.ProfileImagePath)
                    };

                    var useridentity = new ClaimsIdentity(claims, "Login");
                    ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);
                    await HttpContext.SignInAsync(principal);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Message"] = "Hesabınız Askıya Alınmıştır";
                }
            }
            else
            {
                TempData["Error"] = "Kullanıcı Adı veya Şifre Hatalı. Kayıtlı Değilseniz Lütfen Hesap Oluşturun!";
            }
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            /*  Logout Kısmı
            - SignOut yapılması
            - Login sayfasına yönlendirme
            */
            TempData["Success"] = "Çıkış Başarılı";
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

    }
}
