using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airbnb.BookingService.Infrastructure.Saga;

public class BookingSagaDbContext : SagaDbContext
{
    public BookingSagaDbContext(DbContextOptions<BookingSagaDbContext> options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new BookingStateMap(); }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}

public class BookingStateMap : SagaClassMap<BookingState>
{
    protected override void Configure(EntityTypeBuilder<BookingState> entity, ModelBuilder modelBuilder)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.CurrencyCode).HasMaxLength(3);
        
        // Ensure index for performance
        entity.HasIndex(x => x.BookingId);
    }
}
