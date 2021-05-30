using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.Core.Service;
using Social.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Social.UI.Areas.Admin.Controllers
{

    // Area İsmini Belitme ve Login Olan Userin Rolunün "Admin" Olması Kontrolü
    [Area("Admin"),Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ICoreService<User> _userservice;
        private readonly ICoreService<Image> _imgservice;

        public HomeController(ICoreService<User> UserService, ICoreService<Image> ImgService)
        {
            _userservice = UserService;
            _imgservice = ImgService;
        }

        public IActionResult Index()
        {
            /*  Area Home Index
            - Dbdeki bütün Userların ve Resimlerin sayısını turple ile sayfaya yollama
             */
            var userCount = _userservice.GetAll().Count;
            var imageCount = _imgservice.GetAll().Count;

            return View(Tuple.Create<int, int>(userCount, imageCount));
        }

    }
}
