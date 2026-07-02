using Microsoft.AspNetCore.Mvc.Testing;

namespace Backend.Tests.IntegrationTests;

public class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        var connection = Environment.GetEnvironmentVariable("TEST_DB_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=gameservices_test;Username=postgres;Password=postgres";
        builder.UseSetting("ConnectionStrings:DefaultConnection", connection);
    }
}
