using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAccountant.Repositories.Core.Entities;

namespace SmartAccountant.Repositories.Core.EntityConfiguration;

internal sealed class VirtualCardConfiguration : IEntityTypeConfiguration<VirtualCard>
{
    public void Configure(EntityTypeBuilder<VirtualCard> builder)
    {
        builder.HasOne(x => x.Parent)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
