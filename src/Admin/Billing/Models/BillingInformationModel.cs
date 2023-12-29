﻿using Bit.Core.Billing.Models;

namespace Bit.Admin.Billing.Models;

public class BillingInformationModel
{
    public BillingInfo BillingInfo { get; set; }
    public Guid? UserId { get; set; }
    public Guid? OrganizationId { get; set; }
    public string Entity { get; set; }
}
