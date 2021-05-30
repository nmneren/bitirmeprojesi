using Social.Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Model.Entities
{
    public class User : CoreEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Biography { get; set; }
        public string Url { get; set; }
        public string Gender { get; set; }
        public string ProfileImagePath { get; set; }

        public virtual List<Image> Images { get; set; }
        public virtual List<Following> Followings { get; set; }
    }
}
