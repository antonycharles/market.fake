using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messaging.Abstractions;

namespace Messaging.Contracts.Events;

public sealed record User_Updated_Event(
    Guid UserId,
    string Name,
    string Email,
    string Status
    ) : IntegrationEvent;