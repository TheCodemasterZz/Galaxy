﻿using Galaxy.Auditing;
using Galaxy.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Galaxy.Identity.Domain
{
    public abstract class FullyAuditIdentityUserEntity : FullyAuditIdentityUserEntity<int>, IAggregateRoot, IEntity, IFullyAudit
    {
    }
}
