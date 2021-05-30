using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.Core.Service;
using Social.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.UI.Areas.Admin.Controllers
{
    //FollowingController Takip Listelerini Görme
    // Area İsmini Belitme ve Login Olan Userin Rolunün "Admin" Olması Kontrolü
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class FollowingController : Controller
    {

        private readonly ICoreService<User> _userservice;
        private readonly ICoreService<Following> _followingservice;
        private readonly ICoreService<Image> _imgservice;

        public FollowingController(ICoreService<User> UserService, ICoreService<Following> FollowingService, ICoreService<Image> ImgService)
        {
            _userservice = UserService;
            _followingservice = FollowingService;
            _imgservice = ImgService;
        }

        public async Task<IActionResult> Followed(int? id)
        {
            /*  Followed
            - Id ye göre following listesi çektik
            - Listedeki Userların Id sine göre Db den alıp yeni listeye ekleme (listModel)
            - Eğer liste boş değil ise sayfaya modeli yollamak
             */
            var listId = _followingservice.GetDefaultList(x => x.FollowedId == id);

            List<User> listModel = new List<User>();

            foreach (var item in listId)
            {
                listModel.Add(_userservice.GetById(item.UserId));
            }

            if (listModel != null)
            {
                return View(listModel);
            }

            return View();

        }

        public async Task<IActionResult> Follower(int? id)
        {
            /*  Follower
            - Id ye göre following listesi çektik
            - Listedeki Userların Id sine göre Db den alıp yeni listeye ekleme (listModel)
            - Eğer liste boş değil ise sayfaya modeli yollamak
             */
            var listId = _followingservice.GetDefaultList(x => x.UserId == id);

            List<User> listModel = new List<User>();

            foreach (var item in listId)
            {
                listModel.Add(_userservice.GetById(item.FollowedId));
            }

            if (listModel != null)
            {
                return View(listModel);
            }

            return View();
        }
    }
}
