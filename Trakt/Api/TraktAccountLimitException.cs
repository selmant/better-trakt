using System;
using System.Net;
using System.Net.Http;

namespace Trakt.Api;

/// <summary>
/// trakt.tv answered 420: the account has reached a limit of its plan, such as the number of collected items.
/// Retrying can't succeed until the user upgrades the account or removes items.
/// </summary>
public class TraktAccountLimitException : HttpRequestException
{
    /// <summary>
    /// The status code trakt.tv uses for an exceeded account limit.
    /// </summary>
    public const HttpStatusCode AccountLimitExceeded = (HttpStatusCode)420;

    /// <summary>
    /// Initializes a new instance of the <see cref="TraktAccountLimitException"/> class.
    /// </summary>
    public TraktAccountLimitException()
        : this("The trakt.tv account limit is exceeded")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TraktAccountLimitException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public TraktAccountLimitException(string message)
        : base(message, null, AccountLimitExceeded)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TraktAccountLimitException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public TraktAccountLimitException(string message, Exception innerException)
        : base(message, innerException, AccountLimitExceeded)
    {
    }
}
