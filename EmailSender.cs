using FluentEmail.Core;
using Microsoft.AspNetCore.Identity.UI.Services;

public class EmailSender : IEmailSender
{
    private readonly IFluentEmailFactory _fluentEmail;

    public EmailSender(IFluentEmailFactory fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        await _fluentEmail.Create()
            .To(email)
            .Subject(subject)
            .Body(htmlMessage, true)
            .SendAsync();
    }
}