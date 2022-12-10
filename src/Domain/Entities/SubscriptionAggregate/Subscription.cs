using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Entities.AdvertAggregate;

namespace ArrayApp.Domain.Entities.SubscriptionAggregate;
public class Subscription : BaseAuditableEntity, IAggregateRoot
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public SubscriptionStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public ApplicationUser User { get; set; }
    public Product Product { get; set; }
    public decimal Price { get; set; }
    public int RenewalInterval { get; set; }
    public bool AutoRenew { get; set; }
}

public enum SubscriptionStatus
{
    Active,
    Expired,
    Cancelled
}

public enum PaymentMethod
{
    CreditCard,
    BankTransfer,
    PayPal
}

public class Product
{
    public int Id { get; set; }
}