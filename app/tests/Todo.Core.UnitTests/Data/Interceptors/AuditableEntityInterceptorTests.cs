using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Time.Testing;
using Moq;
using System.Globalization;
using System.Security.Claims;
using Todo.Core.Data;
using Todo.Core.Data.Entities;
using Todo.Core.Data.Interceptors;
using Todo.Core.UnitTests.AutoFixture.Attributes;
using Xunit;

namespace Todo.Core.UnitTests.Data.Interceptors;

public class AuditableEntityInterceptorTests
{
    [Theory]
    [InlineAutoMoqData("test-user-id-1", "test-user-id-2", "2026-01-15T10:00:00Z", "2026-02-15T10:00:00Z")]
    public async Task AddedThenModifiedEntity_TracksCreatedAndLastModifiedCorrectly(
        string nowUserId, string laterUserId, string nowDateTimeString, string laterDateTimeString,
        Mock<ClaimsIdentity> claimsIdentity, Mock<HttpContext> httpContext, Mock<IHttpContextAccessor> httpContextAccessor,
        FakeTimeProvider timeProvider, TodoListEntity entity)
    {
        // Arrange
        claimsIdentity
            .Setup(i => i.FindFirst(ClaimTypes.NameIdentifier))
            .Returns(new Claim(ClaimTypes.NameIdentifier, nowUserId));
        httpContext
            .Setup(h => h.User)
            .Returns(new ClaimsPrincipal(claimsIdentity.Object));
        httpContextAccessor
            .Setup(a => a.HttpContext)
            .Returns(httpContext.Object);

        var interceptor = new AuditableEntityInterceptor(httpContextAccessor.Object, timeProvider);
        using var context = (ApplicationDbContext)TestUtils.GetInMemoryContext([interceptor]);

        var nowDateTime = DateTimeOffset.Parse(nowDateTimeString, CultureInfo.InvariantCulture);
        var laterDateTime = DateTimeOffset.Parse(laterDateTimeString, CultureInfo.InvariantCulture);
        timeProvider.SetUtcNow(nowDateTime);

        // Act
        context.TodoLists.Add(entity);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(nowDateTime, entity.Created);
        Assert.Equal(nowUserId, entity.CreatedBy);
        Assert.Equal(nowDateTime, entity.LastModified);
        Assert.Equal(nowUserId, entity.LastModifiedBy);

        // Arrange
        claimsIdentity
            .Setup(i => i.FindFirst(ClaimTypes.NameIdentifier))
            .Returns(new Claim(ClaimTypes.NameIdentifier, laterUserId));

        timeProvider.SetUtcNow(laterDateTime);
        entity.Title = Guid.NewGuid().ToString();

        // Act
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(nowDateTime, entity.Created);
        Assert.Equal(nowUserId, entity.CreatedBy);
        Assert.Equal(laterDateTime, entity.LastModified);
        Assert.Equal(laterUserId, entity.LastModifiedBy);
    }
}
