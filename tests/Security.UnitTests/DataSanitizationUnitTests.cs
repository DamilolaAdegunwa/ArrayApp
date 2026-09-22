using System.Reflection;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace ArrayApp.Security.UnitTests;

[TestFixture]
public class DataSanitizationUnitTests
{
    [Test]
    public void UserDto_MustNotContain_SensitiveSecurityProperties()
    {
        var properties = typeof(UserDto).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name.ToLowerInvariant())
            .ToList();

        var forbiddenPropertySubstrings = new[]
        {
            "password",
            "hash",
            "salt",
            "securitystamp",
            "concurrencystamp",
            "token",
            "secret"
        };

        foreach (var prop in properties)
        {
            foreach (var forbidden in forbiddenPropertySubstrings)
            {
                prop.Should().NotContain(forbidden, 
                    $"UserDto must never expose credential or security tokens to clients (violates OWASP API3:2023 Excessive Data Exposure)");
            }
        }
    }

    [Test]
    public void ApplicationUser_ContainsSensitiveFields_ThatMustBeExcludedFromApiResponses()
    {
        var userProps = typeof(ApplicationUser).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToList();

        userProps.Should().Contain("PasswordHash");
        userProps.Should().Contain("SecurityStamp");

        // Confirms UserDto does not contain them
        var dtoProps = typeof(UserDto).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToList();

        dtoProps.Should().NotContain("PasswordHash");
        dtoProps.Should().NotContain("SecurityStamp");
    }
}
