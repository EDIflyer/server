﻿using Bit.Core.SecretsManager.Entities;

namespace Bit.Core.SecretsManager.Models.Data;

public class PeopleAccessPolicies
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public IEnumerable<UserProjectAccessPolicy> UserAccessPolicies { get; set; }
    public IEnumerable<GroupProjectAccessPolicy> GroupAccessPolicies { get; set; }

    public IEnumerable<BaseAccessPolicy> ToBaseAccessPolicies()
    {
        var policies = new List<BaseAccessPolicy>();
        if (UserAccessPolicies != null)
        {
            policies.AddRange(UserAccessPolicies);
        }

        if (GroupAccessPolicies != null)
        {
            policies.AddRange(GroupAccessPolicies);
        }

        return policies;
    }
}