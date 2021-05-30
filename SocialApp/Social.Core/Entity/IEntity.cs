using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Core.Entity
{
    public interface IEntity<T>
    {
        T ID { get; set; }
    }
}
