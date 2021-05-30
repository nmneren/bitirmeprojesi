using Social.Core.Map;
using System;
using System.Collections.Generic;
using System.Text;
using Social.Model.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Social.Model.Maps
{
    public class ImageMap : CoreMap<Image>
    {
        public override void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.Property(x => x.Content).HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.ImagePath).IsRequired(true);

            base.Configure(builder);
        }
    }
}
