using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMaster.Modules.Exercises.Entities;

namespace TaskMaster.Modules.Exercises.DAL.Configurations;

internal sealed class OpenFormConfiguration : IEntityTypeConfiguration<OpenForm>
{
    public void Configure(EntityTypeBuilder<OpenForm> builder)
    {
        throw new NotImplementedException();
    }
}