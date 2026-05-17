using Microsoft.EntityFrameworkCore;
using MyBills.Data.Contexts;
using MyBills.Data.Repositories;

namespace MyBills.Data.IntegrationTests;

public class UserRepositoryRegistrationIntegrationTests
{
    [Fact]
    public async Task RegisterNewUserAsync_CreatesUserAndUserDetailsRows()
    {
        var dbName = $"MyBills_Registration_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<MyBillsContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        await using var context = new MyBillsContext(options);
        var repository = new UserRepository(context);

        var email = "integration.user@test.local";
        var password = "Passw0rd!";
        var friendlyName = "Integration User";

        var result = await repository.RegisterNewUserAsync(email, password, friendlyName);

        Assert.True(result);

        var user = await context.Users.SingleOrDefaultAsync(x => x.Email == email);
        Assert.NotNull(user);

        var userDetail = await context.UserDetails.SingleOrDefaultAsync(x => x.UserId == user!.Id);
        Assert.NotNull(userDetail);
        Assert.Equal(friendlyName, userDetail!.FirstName);
    }
}
