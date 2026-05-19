using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Infrastructure;
using Airbnb.UserService.Domain;

var services = new ServiceCollection();
services.AddDbContext<UserDbContext>(options => 
    options.UseNpgsql("Host=localhost;Database=airbnb_user_dev;Username=postgres;Password=postgres"));
// We need mocks for IIntegrationEventBridge and IDomainEventPolicyExecutor
services.AddSingleton<Airbnb.SharedKernel.Domain.IIntegrationEventBridge, DummyBridge>();
services.AddSingleton<Airbnb.SharedKernel.Domain.IDomainEventPolicyExecutor, DummyExecutor>();

var sp = services.BuildServiceProvider();
var db = sp.GetRequiredService<UserDbContext>();

var user = new User("test@test.com", UserRole.User, "Test", AuthProvider.Local, "local");
db.Users.Add(user);
user.AddRefreshToken("token123", DateTime.UtcNow.AddDays(1));

try {
    db.SaveChanges();
    Console.WriteLine("Save successful!");
} catch (Exception ex) {
    Console.WriteLine("Exception: " + ex.ToString());
}

class DummyBridge : Airbnb.SharedKernel.Domain.IIntegrationEventBridge {
    public System.Threading.Tasks.Task StageAsync(System.Collections.Generic.IEnumerable<Airbnb.SharedKernel.Domain.IDomainEvent> events, System.Threading.CancellationToken ct) => System.Threading.Tasks.Task.CompletedTask;
}
class DummyExecutor : Airbnb.SharedKernel.Domain.IDomainEventPolicyExecutor {
    public System.Threading.Tasks.Task ExecuteAsync(System.Collections.Generic.IEnumerable<Airbnb.SharedKernel.Domain.IDomainEvent> events, System.Threading.CancellationToken ct) => System.Threading.Tasks.Task.CompletedTask;
}
