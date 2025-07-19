using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Core.Enums
{
    public enum SubscriptionStatus
    {
        Active = 1,
        Inactive = 2,
        Cancelled = 3,
        PastDue = 4,
        Unpaid = 5,
        Trialing = 6,
        Paused = 7
    }
}
