using System;
using System.Linq;
using System.Reflection;
using ArrayApp.WebAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace ArrayApp.Security.UnitTests;

[TestFixture]
public class ControllerAuthorizationAuditUnitTests
{
    [Test]
    public void AllApiControllers_MustEnforce_AuthorizeAttribute()
    {
        var assembly = typeof(AccountController).Assembly;
        var controllerTypes = assembly.GetTypes()
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract)
            .ToList();

        controllerTypes.Should().NotBeEmpty();

        var unprotectedControllers = controllerTypes
            .Where(c => !c.IsDefined(typeof(AuthorizeAttribute), inherit: true))
            .Select(c => c.Name)
            .ToList();

        unprotectedControllers.Should().BeEmpty(
            "Every public API controller must be decorated with [Authorize] to enforce authenticated access by default (OWASP A01:2021 Broken Access Control)");
    }
}
