using Social.Core.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Core.Entity
{
    public class CoreEntity : IEntity<int>
    {
        public int ID { get; set; }
        public Status Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
