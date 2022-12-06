using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ArrayApp.Application.Common.Models;
public class SmtpConfig
{
    public bool EnableSSl { get; set; }
    public int Port { get; set; }
    public string Server { get; set; }
    public string Password { get; set; }
    public string UserName { get; set; }
    public string From { get; set; }
    public string SenderDisplayName { get; set; }
    public bool UseDefaultCredentials { get; set; }
    //
    public string Host { get; set; }
    public X509CertificateCollection ClientCertificates { get; set; }
    public ICredentialsByHost Credentials { get; set; }
    public SmtpDeliveryFormat DeliveryFormat { get; set; }
    public SmtpDeliveryMethod DeliveryMethod { get; set; }
    public string PickupDirectoryLocation { get; set; }
    public ServicePoint ServicePoint { get; }
    public string TargetName { get; set; }
    public int Timeout { get; set; }
}
