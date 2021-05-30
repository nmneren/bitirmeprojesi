using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.Core.Entity.Enums;
using Social.Core.Service;
using Social.Model.Entities;
using Social.UI.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.UI.Controllers
{
    //HomeController Anasayfa ve Keşfet sayfasının yapımı
    //Login Kontrolü
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ICoreService<User> _userservice;
        private readonly ICoreService<Following> _followingservice;
        private readonly ICoreService<Image> _imgservice;

        public HomeController(ICoreService<User> UserService, ICoreService<Following> FollowingService, ICoreService<Image> ImgService)
        {
            _userservice = UserService;
            _followingservice = FollowingService;
            _imgservice = ImgService;
        }

        public async Task<IActionResult> Index()
        {
            /*  HomeIndex
            - Login olan userın Idsini almak
            - Userın takip ettiği Userları bulma (Followed Listesi)
            - Bütün Active resimleri çekme (All Image list)
            - Followed listesi içerisinde dönerken bütün resimlere bakma
            - Resimi yükleyen Userın idsinin FollowedId ye eşitliğine bakma (Takip ettiği kişilerin resimleri)
            - Resim modeli içerisinde tuttuğumuz Userı, FollowedId ye uygun User ile doldurma
            - User Active ise yeni oluşan (Image model) listeye ekleme
            - Yeni listeye (Image model) Login Userında resimlerini ekleme
            - Modeli sayfaya yollama
             */

            var loginId = User.Claims.FirstOrDefault(x => x.Type.EndsWith("ID")).Value;
            var followedList = _followingservice.GetDefaultList(x => x.UserId == int.Parse(loginId));
            var allimglist = _imgservice.GetActive();

            List<Image> imgmodellist = new List<Image>();

            foreach (var followed in followedList)
            {
                foreach (var img in allimglist)
                {
                    if (img.UserId == followed.FollowedId)
                    {
                        img.Users = _userservice.GetById(followed.FollowedId);

                        if (img.Users.Status == Status.Active)
                        {
                            imgmodellist.Add(img);
                        }
                    }
                }
            }

            foreach (var img in allimglist)
            {
                if (img.UserId == int.Parse(loginId))
                {
                    img.Users = _userservice.GetById(int.Parse(loginId));
                    imgmodellist.Add(img);
                }
            }

            return View(imgmodellist);
        }

        //Login Userın takip etmediği Userların resimlerini yükleme
        public async Task<IActionResult> Discover()
        {
            /*  Discover(Keşfet) 
            - Login olan Userın Id sini almak
            - Login Userın takip ettiklerini bulma (followList)
            - Login Userın resimleri hariç Active bütün resimleri çekme (imgModelList)
            - Çektiğimiz resimler içerisinde dönerken resmi yükleyen UserIdnin takip ettiğimiz Userların Id sine Eşit olmaması kontrölü
              (Takip etmediğimiz Userların resimlerini bulma)
            - Resimlerini almak istediğimiz Userların Active olması kontrolü
            - Active ise resimleri yeni listeye ekleme (imgList)
            - Yeni listeyi sayfaya yollama
            - Hiç kimseyi takip etmiyorsa (followList.Count != 0 else bölümü) resimlerin hepsini sayfaya yollama (imgModelList)
             */

            var loginId = User.Claims.FirstOrDefault(x => x.Type.EndsWith("ID")).Value;
            var followList = _followingservice.GetDefaultList(x => x.UserId == int.Parse(loginId));
            var imgModelList = _imgservice.GetDefaultList(x => x.Status == Status.Active && x.UserId != int.Parse(loginId));

            List<Image> imgList = new List<Image>();

            if (followList.Count != 0)
            {
                foreach (var imgitem in imgModelList)
                {
                    bool result = false;

                    foreach (var followitem in followList)
                    {
                        if (imgitem.UserId != followitem.FollowedId)
                        {
                            if (_userservice.GetById(imgitem.UserId).Status == Status.Active)
                            {
                                result = true;
                            }
                        }
                        else
                        {
                            result = false;
                            break;
                        }
                    }

                    if (result)
                    {
                        imgList.Add(imgitem);
                    }
                }

                return View(imgList);
            }

            return View(imgModelList);

        }
    }
}
