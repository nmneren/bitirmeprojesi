using Social.Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Model.Entities
{
    public class Following : CoreEntity
    {
        public int? FollowedId { get; set; }
        public int? UserId { get; set; }
    }
}
