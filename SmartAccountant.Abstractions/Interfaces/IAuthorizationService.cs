﻿using SmartAccountant.Abstractions.Exceptions;

namespace SmartAccountant.Abstractions.Interfaces;

public interface IAuthorizationService
{
    /// <exception cref="AuthenticationException"/>
    Guid UserId { get; }
}
