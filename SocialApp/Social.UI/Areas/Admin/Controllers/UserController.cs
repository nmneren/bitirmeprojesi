using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.Core.Entity.Enums;
using Social.Core.Service;
using Social.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.UI.Areas.Admin.Controllers
{
    /*  UserController
    - Userların Listesi (Index)
    - Tek User Profil Sayfası (UserPage)
    - User Active Etme (GetActiveUser)
    - User Passive Etme (GetPassiveUser)
    - User Silme (UserDelete)
    --------------------------------------
    Not: Passive olan Userlar askıya alınmış hesap demektir. Passive Userlar giriş yapamaz.
    */

    // Area İsmini Belitme ve Login Olan Userin Rolunün "Admin" Olması Kontrolü
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ICoreService<User> _userservice;
        private readonly ICoreService<Following> _followingservice;
        private readonly ICoreService<Image> _imgservice;

        public UserController(ICoreService<User> UserService, ICoreService<Following> FollowingService, ICoreService<Image> ImgService)
        {
            _userservice = UserService;
            _followingservice = FollowingService;
            _imgservice = ImgService;
        }

        public IActionResult Index()
        {
            /*  Index
            - Dbden bütün kullanıcılar alınır (userlist)
            - Userlar arasında dönerken modelin içerisinde olan İmage listelerini doldurma
            - Listeyi sayfaya gönderme
            */

            var userlist = _userservice.GetAll();

            foreach (var item in userlist)
            {
                item.Images = _imgservice.GetDefaultList(x => x.UserId == item.ID);
            }

            return View(userlist);
        }

        public IActionResult UserPage(int? id)
        {
            /*  UserPage
            - Login olan Userın profil sayfasının açılmama kontrolü (Admin User Profili)
            - Gelen Id ye göre User bulma (userModel)
            - O Usera ait resimlerini User Modelin icerisinde olan İmage listesine bulup doldurma
            - Userın Takip ettiği ve takipçilerin listelerini çekip sayılarını(Count) alma
            - Tuple ile sayfaya User Model ve takip sayıları(Count) sayfaya yollama
            */
            if (id != int.Parse(User.Claims.FirstOrDefault(x => x.Type.EndsWith("ID")).Value))
            {
                var usermodel = _userservice.GetById(id);
                usermodel.Images = _imgservice.GetDefaultList(x => x.UserId == usermodel.ID);
                var followcount = _followingservice.GetDefaultList(x => x.FollowedId == usermodel.ID).Count();
                var followedcount = _followingservice.GetDefaultList(x => x.UserId == usermodel.ID).Count();

                return View(Tuple.Create<User, int, int>(usermodel, followcount, followedcount));
            }
            return RedirectToAction("Index","User");
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveUser(int? id)
        {
            /*  GetActiveUser
            - Login olan Userın profil Active edilmemesi için kotrol (Admin Kendisini Active Yapamaz)
            - Gelen Id ye göre User bulma (userModel)
            - Modelin Statusunu Active yapma
            - User Sevice Update metodu ile güncelleme yapma
            - User Index sayfasına yönlendirme
            */
            if (id != int.Parse(User.Claims.FirstOrDefault(x => x.Type.EndsWith("ID")).Value))
            {
                var usermodel = _userservice.GetById(id);
                usermodel.Status = Status.Active;
                _userservice.Update(usermodel);
            }
            return RedirectToAction("Index", "User");
        }

        [HttpGet]
        public async Task<IActionResult> GetPassiveUser(int? id)
        {
            /*  GetPassiveUser
            - Login olan Userın profil Passive edilmemesi için kotrol (Admin Kendisini Passive Yapamaz)
            - Gelen Id ye göre User bulma (userModel)
            - Modelin Statusunu Passive yapma
            - User Sevice Update metodu ile güncelleme yapma
            - User Index sayfasına yönlendirme
            */
            if (id != int.Parse(User.Claims.FirstOrDefault(x => x.Type.EndsWith("ID")).Value))
            {
                var usermodel = _userservice.GetById(id);
                usermodel.Status = Status.Passive;
                _userservice.Update(usermodel);
            }

            return RedirectToAction("Index", "User");
        }

        [HttpGet]
        public async Task<IActionResult> UserDelete(int? id)
        {
            /*  UserDelete
            - Login olan Userın kendi hesabını silememesi için kontrol (Admin Kendi Hesabını Silemez)
            - Gelen Id ye göre User bulma (userModel)
            - User Sevice Delete metodu ile Useri silme 
            - Silme başarılı ise User Index sayfasına yönlendirme
            */
            if (id != int.Parse(User.Claims.FirstOrDefault(x => x.Type.EndsWith("ID")).Value))
            {
                var usermodel = _userservice.GetById(id);
                var result = _userservice.Delete(usermodel);

                if (result)
                {
                    TempData["Message"] = "Silindi";
                }
                else
                {
                    TempData["Message"] = "Silinemedi!";
                }
            }

            return Redirect($"/Admin/User/Index");
        }
    }
}