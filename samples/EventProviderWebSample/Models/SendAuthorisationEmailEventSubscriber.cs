using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Antaris.EventProvider;
using EventProviderWebSample.Services;

namespace EventProviderWebSample.Models
{
    public class SendAuthorisationEmailEventSubscriber : EventSubscriber<ApplicationUserCreatedEvent, ApplicationUser>
    {
        private readonly IEmailSender _emailSender;

        public SendAuthorisationEmailEventSubscriber(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public override Task<bool> FilterAsync(ApplicationUser payload, CancellationToken cancelationToken = default(CancellationToken))
        {
            return Task.FromResult(payload.Email != null);
        }

        public override async Task NotifyAsync(ApplicationUser payload, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _emailSender.SendEmailAsync(payload.Email, $"Welcome to the site", $"Hi {payload.UserName}, this is your welcome email.");
        }
    }
}
