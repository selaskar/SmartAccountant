using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Abstractions.Exceptions;

public class AuthenticationException(AuthenticationErrors error)
    : EnumException<AuthenticationErrors>(error)
{
}

