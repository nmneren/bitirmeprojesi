using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Social.Core.Service;
using Social.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.UI.Areas.Admin.Controllers
{
    /*  ImageController
    - Bütün Resimleri Görme (Index)
    - Tek Resim Sayfası (OneImg)
    - Resim Silme (Delete)
    */
    // Area İsmini Belitme ve Login Olan Userin Rolunün "Admin" Olması Kontrolü
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class ImageController : Controller
    {

        private readonly ICoreService<User> _userservice;
        private readonly ICoreService<Following> _followingservice;
        private readonly ICoreService<Image> _imgservice;

        public ImageController(ICoreService<User> UserService, ICoreService<Following> FollowingService, ICoreService<Image> ImgService)
        {
            _userservice = UserService;
            _followingservice = FollowingService;
            _imgservice = ImgService;
        }


        public IActionResult Index()
        {
            // Index Bütün resimleri Dbden alıp sayfaya gönderme
            return View(_imgservice.GetAll());
        }

        [HttpGet]
        public async Task<IActionResult> OneImg(int? id)
        {
            /*  OneImg
            - Gelen Id ye göre resmi dbden çekme
            - Img modelin içerisinde tuttuğumuz User modeli doldurma
            - Modeli sayfaya yollama
           */
            var img = _imgservice.GetById(id);
            img.Users = _userservice.GetById(img.UserId);

            return View(img);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            /*  Delete
            - Login Userın id sini alma
            - Image Servicenin Delete motoduna gelen resmin idsini yazma
            - Silme işlemi başarılı ise User Index sayfasına yönlendirme
            */
            var img = _imgservice.GetById(id);
            bool result = _imgservice.Delete(id);

            if (result)
            {
                return Redirect($"/Admin/User/UserPage/{img.UserId}");
            }
            else
            {
                TempData["Error"] = "Güncelleme işlemi esnasında bir hata oluştu";
            }

            return Redirect($"/Admin/Image/OneImg/{id}");
        }
    }
}
