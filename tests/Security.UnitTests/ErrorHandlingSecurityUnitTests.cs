using System;
using System.Collections.Generic;
using ArrayApp.WebAPI.Filters;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;

namespace ArrayApp.Security.UnitTests;

[TestFixture]
public class ErrorHandlingSecurityUnitTests
{
    [Test]
    public void ApiExceptionFilter_WhenUnhandledExceptionOccurs_MustSuppressStackTrace()
    {
        var filter = new ApiExceptionFilterAttribute();

        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "req-trace-security-test-999";

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = new InvalidOperationException("Fatal database connection timeout occurred at Server=10.0.0.1;User=sa;Password=SecretPassword!")
        };

        filter.OnException(exceptionContext);

        exceptionContext.ExceptionHandled.Should().BeTrue("Exception must be marked handled to avoid default middleware leaking stack trace");
        exceptionContext.Result.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)exceptionContext.Result!;
        objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var problemDetails = objectResult.Value.Should().BeAssignableTo<ProblemDetails>().Subject;
        problemDetails.Status.Should().Be(500);

        // Security check: Problem details must NOT disclose database credentials or stack trace
        problemDetails.Detail.Should().NotContain("SecretPassword");
        problemDetails.Detail.Should().NotContain("Server=10.0.0.1");
        problemDetails.Detail.Should().NotContain("at Server=");
        problemDetails.Detail.Should().NotContain("Exception");

        // Must include correlation trace ID for operator diagnostics without exposing internals to client
        problemDetails.Extensions.Should().ContainKey("traceId");
        problemDetails.Extensions["traceId"].Should().Be("req-trace-security-test-999");
    }
}
