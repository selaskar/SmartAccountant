using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using SmartAccountant.Abstractions.Exceptions;

namespace SmartAccountant.Identity.Tests.AuthorizationServiceTests;

[TestClass]
public sealed class UserId
{
    private ClaimsPrincipal? user;
    private Mock<HttpContext> contextMock = null!;
    private Mock<IHttpContextAccessor> contextAccessorMock = null!;

    private AuthorizationService sut = null!;

    [TestInitialize]
    public void Initialize()
    {
        contextMock = new();

        contextAccessorMock = new Mock<IHttpContextAccessor>();
        contextAccessorMock.SetupGet(x => x.HttpContext).Returns(contextMock.Object);

        sut = new AuthorizationService(contextAccessorMock.Object);
    }

    [TestMethod]
    public void ThrowAuthenticationExceptionForMissingIdentity()
    {
        // Arrange
        user = new();

        contextMock.SetupGet(x => x.User).Returns(user);

        // Act, Assert
        Assert.ThrowsExactly<AuthenticationException>(() => sut.UserId.ToString());
    }

    [TestMethod]
    public void ThrowAuthenticationExceptionForObjectId()
    {
        // Arrange
        user = new ClaimsPrincipal(new ClaimsIdentity("AuthenticationTypes.Federation"));

        contextMock.SetupGet(x => x.User).Returns(user);

        // Act, Assert
        Assert.ThrowsExactly<AuthenticationException>(() => sut.UserId.ToString());
    }

    [TestMethod]
    public void ReturnCorrectObjectId()
    {
        // Arrange
        string id = "2E145299-7858-449C-8E6B-267CD0E91C2B";
        Claim oid = new("oid", id);
        user = new ClaimsPrincipal(new ClaimsIdentity([oid], "AuthenticationTypes.Federation"));

        contextMock.SetupGet(x => x.User).Returns(user);

        // Act
        Guid result = sut.UserId;

        // Assert
        Assert.AreEqual(id, result.ToString("D"), true, CultureInfo.InvariantCulture);
    }
}
