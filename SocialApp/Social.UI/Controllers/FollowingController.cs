using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.Core.Entity.Enums;
using Social.Core.Service;
using Social.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.UI.Controllers
{
    //FollowingController takip listeleri ve takip etme takipten çıkma işlemleri
    //Login kontrolü
    [Authorize]
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
            - O liste içerisinde dönerek aktif kullanıncı idlerini bulup yeni listeye ekleme işlemi
            - Eğer liste boş değil ise sayfaya modeli yollamak
             */
            var listId = _followingservice.GetDefaultList(x => x.FollowedId == id);

            List<User> listModel = new List<User>();

            foreach (var item in listId)
            {
                if (_userservice.GetById(item.UserId).Status == Status.Active)
                {
                    listModel.Add(_userservice.GetById(item.UserId));
                }
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
            - O liste içerisinde dönerek aktif kullanıncı idlerini bulup yeni listeye ekleme işlemi
            - Eğer liste boş değil ise sayfaya modeli yollamak
             */
            var listId = _followingservice.GetDefaultList(x => x.UserId == id);

            List<User> listModel = new List<User>();

            foreach (var item in listId)
            {
                if (_userservice.GetById(item.FollowedId).Status == Status.Active)
                {
                    listModel.Add(_userservice.GetById(item.FollowedId));
                }
            }

            if (listModel != null)
            {
                return View(listModel);
            }

            return View();
        }

        public async Task<IActionResult> Follow(int? id)
        {
            /*  Follower
            - Login olan userın idsini bulmak
            - Followingservice add metotu ile içerisinnde yeni following modeli oluşturma 
            - UserId si login followedId side metota gelen id yi atmak
            - User Index Safyasına yönlendirme
             */
            var loginId = User.Claims.FirstOrDefault(x => x.Type.EndsWith("ID")).Value;

            _followingservice.Add(new Following { UserId = int.Parse(loginId),FollowedId = id });


            return Redirect($"/User/Index/{id}");
        }

        public async Task<IActionResult> UnFollow(int? id)
        {

            /*  Follower
            - Login olan userın idsini bulmak
            - Followingservicenin Delete motoduna UserIdsi LoginId ye FollowedIdside gelen idye eşit olan modeli yazmak
            - User Index Safyasına yönlendirme
             */
            var loginId = User.Claims.FirstOrDefault(x => x.Type.EndsWith("ID")).Value;

            _followingservice.Delete(_followingservice.GetByDefault(x => x.UserId == int.Parse(loginId) && x.FollowedId == id));

            return Redirect($"/User/Index/{id}");
        }

    }
}
