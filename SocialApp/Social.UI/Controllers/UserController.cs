using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social.Core.Entity.Enums;
using Social.Core.Service;
using Social.Model.Entities;
using Social.UI.Models;
using Social.UI.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.UI.Controllers
{
    /*  UserController
    - User Profili (Index)
    - User Ekleme/Kayıt Etme (Add)
    - User Güncelleme (Edit)
    - User Password Değişimi (ChangePassword)
    - User Hesabımı Sil (ConfirmDelete - UserDelete)
    - User Arama (UserSearch)
    --------------------------------
    Not: UserAdd Bölümü Login Olmadan Gidileceği İçin O Action Hariç Gerisi [Authorize] vardır.
    */
    public class UserController : Controller
    {
        private readonly ICoreService<User> _userservice;
        private readonly ICoreService<Following> _followingservice;
        private readonly ICoreService<Image> _imgservice;
        private IHostingEnvironment _hostingEnvironment;
        public UserController(ICoreService<User> UserService, ICoreService<Following> FollowingService, ICoreService<Image> ImgService, IHostingEnvironment environment)
        {
            _userservice = UserService;
            _followingservice = FollowingService;
            _imgservice = ImgService;
            _hostingEnvironment = environment;
        }


        [Authorize]
        public IActionResult Index(int? id)
        {
            /*  Index
            - Login User id sini alma
            - Gelen Id ye göre User bulma (userModel)
            - O Usera ait resimlerini User Modelin icerisinde olan İmage listesine bulup doldurma
            - Userın Takip ettiği ve takipçilerin listelerini çekip onlar arasında Active Userın sayısını değişkenlere atma
            (followedList/followedCount - followList/followCount)
            - Bu sayfaya gelen User Login olan Userın Takip ettiği kişiler arasında ise ona göre sayfada button değişikliği
            - Tuple ile sayfaya User Model ve takip sayıları(Count) sayfaya yollama
             */
            int followCount = 0, followedCount = 0;

            var loginId = User.Claims.FirstOrDefault(x => x.Type.EndsWith("ID")).Value;
            var userModel = _userservice.GetById(id);
            userModel.Images = _imgservice.GetDefaultList(x => x.UserId == userModel.ID);
            var followedList = _followingservice.GetDefaultList(x => x.FollowedId == userModel.ID);
            foreach (var item in followedList)
            {
                if (_userservice.GetById(item.UserId).Status == Status.Active)
                {
                    followedCount++;
                }
            }
            var followList = _followingservice.GetDefaultList(x => x.UserId == userModel.ID);
            foreach (var item in followList)
            {
                if (_userservice.GetById(item.FollowedId).Status == Status.Active)
                {
                    followCount++;
                }
            }

            if (_followingservice.Any(x => x.UserId == int.Parse(loginId) && x.FollowedId == userModel.ID))
            {
                TempData["Kontrol"] = true;
            }

            return View(Tuple.Create<User, int, int>(userModel, followedCount, followCount));
        }


        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(User model)
        {
            /*  Add
            - Sayfadan gelen modelin UserName ve Emaili daha önceden var olmasını kontrol
            - İlk oluşan Usera Default Profil resmi ekleme
            - Kayıt başarılı ise Login sayfasına yönlendirme
            - olumsuzlukların hata mesajları doldurma
             */

            if (!_userservice.Any(x => x.UserName == model.UserName))
            {
                if (!_userservice.Any(x => x.Email == model.Email))
                {
                    model.ProfileImagePath = "\\uploads\\DefaultProfileImage.jpeg";

                    if (_userservice.Add(model))
                    {
                        TempData["Success"] = "Kayıt işlemi başarılı";
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        TempData["Error"] = "Kayıt işlemi esnasında bir hata oluştu";
                    }

                }
                else
                {
                    TempData["Error"] = "Aynı E-Posta Mevcuttur.Lütfen Başka E-Posta Deneyiniz!";
                }
            }
            else
            {
                TempData["Error"] = "Aynı Kullanıcı Adı Mevcuttur.Lütfen Başka Kullanıcı Adı Deneyiniz!";
            }

            return View(model);
        }

        [HttpGet, Authorize]
        public IActionResult Edit(int? id)
        {
            return View(_userservice.GetById(id));
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Edit(User model, List<IFormFile> files)
        {
            /*  Edit
            - Sayfadan gelen modelin Id sine göre Db den modeli alma
            - Sayfadan gelen modelin içerisini Db den alınan modelin içerisine atma
            - Sayfadan gelen resim için resim için oluşturulan Class kullanımı (Upload)
            - Resim yükleme başarılşı ise modelin içerisine yolunu atma (ProfileImagePath)
            - User Service Upload Metodu ile db de güncelleme yapma
            - Güncelleme başarılı ise User Edit sayfasına yönlendirme
             */

            if (ModelState.IsValid)
            {
                var newModel = _userservice.GetById(model.ID);
                newModel.Name = model.Name;
                newModel.Email = model.Email;
                newModel.Url = model.Url;
                newModel.Biography = model.Biography;
                newModel.Gender = model.Gender;

                bool imgControl;

                string imagePath = Upload.ImageUpload(files, _hostingEnvironment, out imgControl);

                if (imgControl)
                {
                    newModel.ProfileImagePath = imagePath;
                }
                else
                {
                    newModel.ProfileImagePath = newModel.ProfileImagePath;
                }

                bool result = _userservice.Update(newModel);

                if (result)
                {
                    TempData["Success"] = "Güncelleme işlemi başarılı";
                    return Redirect($"/User/Edit/{newModel.ID}");
                }
                else
                {
                    TempData["Error"] = "Güncelleme işlemi esnasında bir hata oluştu";
                }

            }
            return View(model);
        }



        [HttpGet, Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> ChangePassword(PasswordVM model)
        {
            /*  ChangePassword
            - Şifre Değişikliği için ViewModel Kullanma
            - Sayfadan gelen modelin Id sine göre modeli Dbden alma (userModel)
            - Sayfadan Gelen model ile Db den gelen bilgi kontrolu
            - Şifre tekrarının kontrolü
            - Db den alınan User Modelin içerisine sayfadan gelen yeni şifreyi atma
            - User Service Upload Metodu ile db de güncelleme yapma
            - Güncelleme başarılı ise User Edit sayfasına yönlendirme
             */
            var userModel = _userservice.GetById(model.UserId);

            if (model.OldPassword == userModel.Password)
            {
                if (model.NewPassword == model.ConfirmPassword)
                {
                    userModel.Password = model.NewPassword;

                    bool result = _userservice.Update(userModel);

                    if (result)
                    {
                        TempData["Success"] = "Şifre Değişimi Başarılı";

                        return Redirect($"/User/Edit/{userModel.ID}");
                    }
                    else
                    {
                        TempData["Error"] = "Şifre değişiminde Hata Oluştu!";
                    }
                }
                else
                {
                    TempData["Error"] = "Şifre Tekrarı Uyuşmuyor!";
                }
            }
            else
            {
                TempData["Error"] = "Eski Şifre Doğru Değil!";
            }

            return View(model);
        }


        //Admin Kendi Hesabını Silemez
        [HttpGet, Authorize(Roles = "Kullanıcı")]
        public async Task<IActionResult> ConfirmDelete()
        {
            return View();
        }

        //Admin Kendi Hesabını Silemez
        [HttpPost, Authorize(Roles = "Kullanıcı")]
        public async Task<IActionResult> UserDelete(User model)
        {
            /*  ConfirmDelete/UserDelete
            - Sayfadan gelen modelin UserName göre Dbden modeli alma (userModel)
            - Sayfadan gelen modelin şifresi ile Db den alınan modelin şifre kontrolü
            - Doğru ise User Service Delete Motodu ile Userı silme
            - Silme başarılı ise Account Login ne yönlendirme
             */

            var userModel = _userservice.GetByDefault(x => x.UserName == model.UserName);

            if (userModel.Password == model.Password)
            {
                var result = _userservice.Delete(userModel);

                if (!result)
                {
                    TempData["Error"] = "Hesabınız Silinemedi";
                }
                else
                {
                    TempData["Success"] = "Hesabınız Silindi!";
                    return RedirectToAction("Login", "Account");
                }
            }
            return Redirect($"/User/Edit/{userModel.ID}");
        }


        [Authorize]
        public async Task<IActionResult> UserSearch()
        {
            /*  UserSearch
            - Login Userın Idsini alma
            - Dbden Active olan bütün Userları çekme (userList)
            - Login olan kişi haricindekileri Active Userları(userList) yeni listeye ekleme (newList)
            - Yeni listeyi sayfaya gönderme (newList)
             */
            var loginId = User.Claims.FirstOrDefault(x => x.Type.EndsWith("ID")).Value;
            var userList = _userservice.GetActive();

            List<User> newList = new List<User>();
            
            foreach (var item in userList)
            {
                if (item.ID != int.Parse(loginId))
                {
                    newList.Add(item);
                }
            }

            return View(newList);
        }
    }
}
