using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAccountant.Repositories.Core.Entities;

namespace SmartAccountant.Repositories.Core.EntityConfiguration;

internal sealed class SharedStatementConfiguration : IEntityTypeConfiguration<SharedStatement>
{
    public void Configure(EntityTypeBuilder<SharedStatement> builder)
    {
        builder.HasOne(x => x.DependentAccount)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
