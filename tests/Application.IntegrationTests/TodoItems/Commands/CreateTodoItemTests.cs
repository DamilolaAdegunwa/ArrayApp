﻿using ArrayApp.Application.Common.Exceptions;
using ArrayApp.Application.TodoItems.Commands.CreateTodoItem;
using ArrayApp.Application.TodoLists.Commands.CreateTodoList;
using ArrayApp.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace ArrayApp.Application.IntegrationTests.TodoItems.Commands;

using static Testing;

public class CreateTodoItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateTodoItemCommand();

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldCreateTodoItem()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var command = new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Tasks"
        };

        var itemId = await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().NotBeNull();
        item!.Id.Should().Be(command.ListId);
        item.Title.Should().Be(command.Title);
        item.CreatorUserId.Should().Be(userId);
        item.CreationTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        item.LastModifierUserId.Should().Be(userId);
        item.LastModificationTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
