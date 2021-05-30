using Social.Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Model.Entities
{
    public class Image : CoreEntity
    {
        public string Content { get; set; }
        public string ImagePath { get; set; }
        public int? UserId { get; set; }
        public virtual User Users { get; set; }
    }
}
