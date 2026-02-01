using Mensageria.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mensageria.InfraData.Mappings
{
    public class ArchiveMap : IEntityTypeConfiguration<Archive>
    {
        public void Configure(EntityTypeBuilder<Archive> builder)
        {
            builder.ToTable("tb_archive");
            builder.HasKey(f => f.Id);

            builder.Property(f=> f.Id).HasColumnName("id").IsRequired();
            builder.Property(f=> f.Name).HasColumnName("name").HasColumnType("varchar(50)").IsRequired();
            builder.Property(f=> f.Type).HasColumnName("type").HasColumnType("varchar(50)").IsRequired();
            builder.Property(f=> f.Content).HasColumnName("content").IsRequired();
        }
    }
}
