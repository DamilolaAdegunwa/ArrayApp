using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using ArrayApp.Application.Common.Models;

namespace ArrayApp.Infrastructure.Services;
public interface IServiceHelper
{
    string GetCurrentUserEmail();
    int? GetCurrentUserId();
    Task<ClaimsPrincipal> GetCurrentUserAsync();
    Uri GetAbsoluteUri();
    public void SendEMail(string to, string message, string subject);
}
public class ServiceHelper : IServiceHelper
{

    readonly IHttpContextAccessor _httpContext;
    private readonly AppConfig _appConfig;
    private readonly SmtpConfig _smtpsettings;
    public ServiceHelper(
        IHttpContextAccessor httpContext,
        IOptions<AppConfig> appConfig,
        IOptions<SmtpConfig> smtpsettings
        )
    {
        _httpContext = httpContext;
        _appConfig = appConfig.Value;
        _smtpsettings = smtpsettings.Value;
    }

    public string GetCurrentUserEmail()
    {
        var email = _httpContext.HttpContext?.User?.FindFirst("name")?.Value;
        return !string.IsNullOrEmpty(email) ? email : "Anonymous";
    }

    public Uri GetAbsoluteUri()
    {
        var request = _httpContext.HttpContext.Request;

        var uriBuilder = new UriBuilder
        {
            Scheme = request.Scheme,
            Host = request.Host.Host,
            Path = request.Path.ToString(),
            Query = request.QueryString.ToString()
        };

        return uriBuilder.Uri;
    }

    public int? GetCurrentUserId()
    {
        var id = _httpContext.HttpContext?.User?.FindFirst("id")?.Value;
        return id is null ? (int?)null : int.Parse(id);
    }

    public async Task<ClaimsPrincipal> GetCurrentUserAsync()
    {
        var user = _httpContext.HttpContext?.User;
        return user;
    }
    public void SendEMail(string to, string message, string subject)
    {
        // create email message
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_smtpsettings.From));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = message };
        using var smtp = new SmtpClient();
        smtp.Connect(_smtpsettings.Host, _smtpsettings.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_smtpsettings.UserName, _smtpsettings.Password);
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}
