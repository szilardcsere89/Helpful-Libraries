﻿using Lombiq.HelpfulLibraries.Libraries.Mvc;
using Lombiq.HelpfulLibraries.Tests.Controllers;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace Lombiq.HelpfulLibraries.Tests.UnitTests.Models
{
    public class TypedRouteTests
    {
        [Theory]
        [MemberData(nameof(TypedRouteShouldWorkCorrectlyData))]
        public void TypedRouteShouldWorkCorrectly(
            string expected,
            Expression<Action<RouteTestController>> actionExpression,
            (string Name, object Value)[] additional,
            string tenantName)
        {
            const string id = "Lombiq.HelpfulLibraries.Tests";

            var typeFeatureProvider = new TypeFeatureProvider();
            typeFeatureProvider.TryAdd(typeof(RouteTestController), new FeatureInfo(id, new ExtensionInfo(id)));

            var model = TypedRoute.CreateFromExpression(actionExpression, additional, typeFeatureProvider);
            model.ToString(tenantName).ShouldBe(expected);
        }

        public static IEnumerable<object[]> TypedRouteShouldWorkCorrectlyData()
        {
            static Expression<Action<RouteTestController>> AsExpression(
                Expression<Action<RouteTestController>> expression) =>
                expression;

            var noMoreArguments = Array.Empty<(string Name, object Value)>();
            var noTenant = string.Empty;
            var someTenant = "SomeTenant";

            return new[]
            {
                new object[]
                {
                    "/Lombiq.HelpfulLibraries.Tests/RouteTest/Foo",
                    AsExpression(controller => controller.Foo()),
                    noMoreArguments,
                    noTenant,
                },
                new object[]
                {
                    "/Lombiq.HelpfulLibraries.Tests/RouteTest/Bar",
                    AsExpression(controller => controller.Bar()),
                    noMoreArguments,
                    noTenant,
                },
                new object[]
                {
                    "/Admin/Lombiq.HelpfulLibraries.Tests/RouteTest/Baz",
                    AsExpression(controller => controller.Baz()),
                    noMoreArguments,
                    noTenant,
                },
                new object[]
                {
                    "/SomeTenant/Lombiq.HelpfulLibraries.Tests/RouteTest/Foo",
                    AsExpression(controller => controller.Foo()),
                    noMoreArguments,
                    someTenant,
                },
                new object[]
                {
                    "/SomeTenant/Lombiq.HelpfulLibraries.Tests/RouteTest/Bar",
                    AsExpression(controller => controller.Bar()),
                    noMoreArguments,
                    someTenant,
                },
                new object[]
                {
                    "/SomeTenant/Admin/Lombiq.HelpfulLibraries.Tests/RouteTest/Baz",
                    AsExpression(controller => controller.Baz()),
                    noMoreArguments,
                    someTenant,
                },
                new object[]
                {
                    "/I/Am/Routed",
                    AsExpression(controller => controller.Route()),
                    noMoreArguments,
                    noTenant,
                },
                new object[]
                {
                    "/I/Am/Routed?wat=is+this",
                    AsExpression(controller => controller.Route()),
                    new (string Name, object Value)[] { ("wat", "is this") },
                    noTenant,
                },
                new object[]
                {
                    "/content/10",
                    AsExpression(controller => controller.RouteSubstitution(10)),
                    noMoreArguments,
                    noTenant,
                },
                new object[]
                {
                    "/Lombiq.HelpfulLibraries.Tests/RouteTest/Arguments?number=9001&fraction=2.71&dateTime=1997-08-29T02%3A14%3A00&text=done",
                    AsExpression(controller => controller.Arguments(9001, 2.71, new DateTime(1997, 8, 29, 2, 14, 0), "done")),
                    noMoreArguments,
                    noTenant,
                },
            };
        }
    }
}
