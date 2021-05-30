using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social.Core.Map;
using Social.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Social.Model.Maps
{
    public class UserMap : CoreMap<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.UserName).IsRequired(true);
            builder.Property(x => x.Password).HasMaxLength(20).IsRequired(true);
            builder.Property(x => x.Name).HasMaxLength(150).IsRequired(true);
            builder.Property(x => x.Email).HasMaxLength(80).IsRequired(true);
            builder.Property(x => x.Biography).HasMaxLength(300).IsRequired(false);
            builder.Property(x => x.Url).HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.Gender).IsRequired(false);
            builder.Property(x => x.ProfileImagePath).IsRequired(false);

            base.Configure(builder);
        }
    }
}
