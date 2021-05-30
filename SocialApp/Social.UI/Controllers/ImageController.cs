using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social.Core.Entity.Enums;
using Social.Core.Service;
using Social.Model.Entities;
using Social.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.UI.Controllers
{
    /*  ImageController
    - Resim Ekleme (MyPost)
    - Resim Düzenleme (Edit)
    - Resim Silme (Delete)
    - Tek Resim Sayfası (OneImg)
    - Resimi Active Etme (GetActiveImg) Arşivden Çıkarma
    - Resimi Passive Etme (GetPassiveImg) Arşive ekleme
    -----------------------------------------------------
    Not:
    - Arşive Eklenen Resimleri Sadece Kendisi Görür
    - Arşivde Olmayanı Herkes
    */
    //Login Kontrolü
    [Authorize]
    public class ImageController : Controller
    {
        private readonly ICoreService<User> _userservice;
        private readonly ICoreService<Following> _followingservice;
        private readonly ICoreService<Image> _imgservice;
        private IHostingEnvironment _hostingEnvironment;

        public ImageController(ICoreService<User> UserService, ICoreService<Following> FollowingService, ICoreService<Image> ImgService, IHostingEnvironment environment)
        {
            _userservice = UserService;
            _followingservice = FollowingService;
            _imgservice = ImgService;
            _hostingEnvironment = environment;
        }

        [HttpGet]
        public IActionResult MyPost()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MyPost(Image model, List<IFormFile> files)
        {
            /*  MyPost
            - Resim yüklemek sonuncunda resmin yolunu bize donecek olan Class kullanma (Upload)
            - Başarılı ise sayfadan gelen Image modelin resim yollunu ekleme (ImagePath)
            - Image Servicenina Add motodu ile DB ye kayıt etme ekleme
            - Ekleme başarılı ise User Indexe değilse aynın sayfaya yönlendirme
             */
            bool imgcontrol;

            string imagePath = Upload.ImageUpload(files, _hostingEnvironment, out imgcontrol);

            if (imgcontrol)
            {
                model.ImagePath = imagePath;
            }

            bool result = _imgservice.Add(model);

            if (result)
            {
                return Redirect($"/User/Index/{model.UserId}");
            }
            else
            {
                TempData["Error"] = "Ekleme işlemi esnasında bir hata oluştu";
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Edit(int? id)
        {
            // Gelen resim id sine göre resmi bulup sayfaya yollama
            return View(_imgservice.GetById(id));
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Image model)
        {
            /*  Edit
            - Sayfadan gelen modelin id sine göre db den resmi çekme (img)
            - DBden gelen resimin açıklama/içerik (content) kısmına sayfadan gelen modelin açıklama/içerik (content) kısmını atma
            - Image service update motodu ile güncelleme yapma
            - Başarılı ise OneImg sayfasına yönlendirme
             */
            var img = _imgservice.GetById(model.ID);
            img.Content = model.Content;

            bool result = _imgservice.Update(img);

            if (result)
            {
                TempData["Success"] = "Güncelleme işlemi başarılı";
                return Redirect($"/Image/OneImg/{img.ID}");
            }
            else
            {
                TempData["Error"] = "Güncelleme işlemi esnasında bir hata oluştu";
            }

            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            /*  Delete
            - Login Userın id sini alma
            - Image Servicenin Delete motoduna gelen resmin idsini yazma
            - Silme işlemi başarılı ise User Index sayfasına yönlendirme
            */
            var loginId = User.Claims.FirstOrDefault(x => x.Type.EndsWith("ID")).Value;
            bool result = _imgservice.Delete(id);

            if (result)
            {
                return Redirect($"/User/Index/{loginId}");
            }
            else
            {
                TempData["Error"] = "Güncelleme işlemi esnasında bir hata oluştu";
            }

            return Redirect($"/Image/OneImg/{id}");
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

        //GetActiveImg Userların Resimlerini Arşiveden Çıkarma
        [HttpGet]
        public async Task<IActionResult> GetActiveImg(int? id)
        {
            /*  GetActiveImg
            - Gelen Id ye göre resmi dbden çekme
            - Modelin Statusunu Active yapma
            - Image Service Update motodu ile güncelleme yapma 
            - OneImg sayfasına yönlendirme
           */
            var img = _imgservice.GetById(id);
            img.Status = Status.Active;
            _imgservice.Update(img);

            return Redirect($"/Image/OneImg/{id}");
        }

        //GetPassiveImg Userların Resimlerini Arşive Ekleme
        [HttpGet]
        public async Task<IActionResult> GetPassiveImg(int? id)
        {
            /*  GetPassiveImg
            - Gelen Id ye göre resmi dbden çekme
            - Modelin Statusunu Passive yapma
            - Image Service Update motodu ile güncelleme yapma 
            - OneImg sayfasına yönlendirme
           */
            var img = _imgservice.GetById(id);
            img.Status = Status.Passive;
            _imgservice.Update(img);

            return Redirect($"/Image/OneImg/{id}");
        }
    }
}
