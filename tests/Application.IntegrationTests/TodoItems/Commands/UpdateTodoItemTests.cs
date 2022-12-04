using ArrayApp.Application.Common.Exceptions;
using ArrayApp.Application.TodoItems.Commands.CreateTodoItem;
using ArrayApp.Application.TodoItems.Commands.UpdateTodoItem;
using ArrayApp.Application.TodoLists.Commands.CreateTodoList;
using ArrayApp.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace ArrayApp.Application.IntegrationTests.TodoItems.Commands;

using static Testing;

public class UpdateTodoItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new UpdateTodoItemCommand { Id = 99, Title = "New Title" };
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldUpdateTodoItem()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "New Item"
        });

        var command = new UpdateTodoItemCommand
        {
            Id = itemId,
            Title = "Updated Item Title"
        };

        await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().NotBeNull();
        item!.Title.Should().Be(command.Title);
        item.LastModifierUserId.Should().NotBeNull();
        item.LastModifierUserId.Should().Be(userId);
        item.LastModificationTime.Should().NotBeNull();
        item.LastModificationTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
