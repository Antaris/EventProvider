namespace EventProviderConsoleSample
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Framework.DependencyInjection;
    using Antaris.EventProvider;

    public class Program
    {
        public async Task Main(string[] args)
        {
            // Create our services collection and register our components.
            var services = new ServiceCollection();
            services.AddScoped<IEventProvider>(sp => new EventProvider(() => sp));
            services.AddTransientEventSubscriber<UserCreatedEvent, User, SendWelcomeEmailEventSubscriber>();

            var serviceProvider = services.BuildServiceProvider();

            // Create the event provider.
            var eventProvider = serviceProvider.GetService<IEventProvider>();
            // Get the event.
            var userCreatedEvent = eventProvider.GetEvent<UserCreatedEvent>();

            // Test the event - both direct and external subscribers.
            // We're using async/await here even though we are not actually doing that - just to demonstrate it *can* run asynchrnously
            using (userCreatedEvent.Subscribe(async (user, ct) => await Log(user)))
            {
                var adama = new User { Forename = "William", Surname = "Adama" };
                await userCreatedEvent.PublishAsync(adama);

                var lee = new User { Forename = "Lee", Surname = "Adama", SendWelcomeEmail = true };
                await userCreatedEvent.PublishAsync(lee);
            }
        }

        public async Task Log(User user)
        {
            Console.WriteLine($"User created: {user.Forename} {user.Surname}");
        }
    }

    public class User
    {
        public string Forename { get; set; }
        public string Surname { get; set; }

        public bool SendWelcomeEmail { get; set; }
    }

    public class UserCreatedEvent : Event<User> { }

    public class SendWelcomeEmailEventSubscriber : EventSubscriber<UserCreatedEvent, User>
    {
        public override async Task<bool> FilterAsync(User user, CancellationToken cancelationToken = default(CancellationToken))
        {
            return user.SendWelcomeEmail;
        }

        public override async Task NotifyAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            Console.WriteLine($"Send the welcome email for {user.Forename} {user.Surname}");
        }
    }
}