using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Domain.Common.Interfaces;

namespace ArrayApp.Domain.Entities.ChatAggregate;
public class ChatMessage : BaseAuditableEntity, IAggregateRoot
{
    public string Name { get; set; }
}
