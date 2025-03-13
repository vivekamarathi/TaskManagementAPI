
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;
using Xunit;

public class TaskControllerTests
{
    private TaskController GetControllerWithMockedDependencies(out Mock<RedisCacheService> mockCache, out AppDbContext dbContext)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
            .Options;

        dbContext = new AppDbContext(options);
        mockCache = new Mock<RedisCacheService>();

        return new TaskController(dbContext, mockCache.Object);
    }

    [Fact]
    public async Task GetTasksByUser_ShouldReturnTasks_WhenUserHasTasks()
    {
        // Arrange
        var controller = GetControllerWithMockedDependencies(out var mockCache, out var dbContext);
        dbContext.Tasks.AddRange(
            new TaskTable { Id = 1, AssignedUserId = 101, Title = "Task 1" },
            new TaskTable { Id = 2, AssignedUserId = 101, Title = "Task 2" },
            new TaskTable { Id = 3, AssignedUserId = 102, Title = "Task 3" }
        );
        await dbContext.SaveChangesAsync();

        mockCache.Setup(c => c.GetAsync<List<TaskTable>>("tasks_user_101"))
                 .ReturnsAsync((List<TaskTable>)null); // Simulate no cache hit

        // Act
        var result = await controller.GetTasksByUser(101);

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<TaskTable>>>(result);
        var tasks = Assert.IsType<List<TaskTable>>(actionResult.Value);
        Assert.Equal(2, tasks.Count);
    }

    [Fact]
    public async Task GetTasks_ShouldReturnAllTasks()
    {
        // Arrange
        var controller = GetControllerWithMockedDependencies(out _, out var dbContext);
        dbContext.Tasks.AddRange(
            new TaskTable { Id = 1, Title = "Task 1" },
            new TaskTable { Id = 2, Title = "Task 2" }
        );
        await dbContext.SaveChangesAsync();

        // Act
        var result = await controller.GetTasks();

        // Assert
        var tasks = Assert.IsType<List<TaskTable>>(result.Value);
        Assert.Equal(2, tasks.Count);
    }

    [Fact]
    public async Task GetTask_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        var controller = GetControllerWithMockedDependencies(out _, out var dbContext);
        var task = new TaskTable { Id = 1, Title = "Test Task" };
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await controller.GetTask(1);

        // Assert
        var actionResult = Assert.IsType<ActionResult<TaskTable>>(result);
        var returnedTask = Assert.IsType<TaskTable>(actionResult.Value);
        Assert.Equal(1, returnedTask.Id);
        Assert.Equal("Test Task", returnedTask.Title);
    }

    [Fact]
    public async Task GetTask_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var controller = GetControllerWithMockedDependencies(out _, out _);

        // Act
        var result = await controller.GetTask(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateTask_ShouldReturnCreatedAtAction_WhenTaskIsCreated()
    {
        // Arrange
        var controller = GetControllerWithMockedDependencies(out _, out var dbContext);
        var newTask = new TaskTable { Id = 1, Title = "New Task" };

        // Act
        var result = await controller.CreateTask(newTask);

        // Assert
        var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdTask = Assert.IsType<TaskTable>(actionResult.Value);
        Assert.Equal("New Task", createdTask.Title);
        Assert.Single(dbContext.Tasks); // Ensure it's added to DB
    }

    [Fact]
    public async Task UpdateTask_ShouldReturnNoContent_WhenTaskIsUpdated()
    {
        // Arrange
        var controller = GetControllerWithMockedDependencies(out _, out var dbContext);
        var task = new TaskTable { Id = 1, Title = "Initial Task" };
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync();

        var updatedTask = new TaskTable { Id = 1, Title = "Updated Task" };

        // Act
        var result = await controller.UpdateTask(1, updatedTask);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var dbTask = await dbContext.Tasks.FindAsync(1);
        Assert.Equal("Updated Task", dbTask.Title);
    }

    [Fact]
    public async Task UpdateTask_ShouldReturnBadRequest_WhenIdMismatch()
    {
        // Arrange
        var controller = GetControllerWithMockedDependencies(out _, out _);
        var task = new TaskTable { Id = 1, Title = "Task" };

        // Act
        var result = await controller.UpdateTask(2, task);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnNoContent_WhenTaskIsDeleted()
    {
        // Arrange
        var controller = GetControllerWithMockedDependencies(out _, out var dbContext);
        var task = new TaskTable { Id = 1, Title = "Task to delete" };
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await controller.DeleteTask(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Null(await dbContext.Tasks.FindAsync(1)); // Ensure deletion
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var controller = GetControllerWithMockedDependencies(out _, out _);

        // Act
        var result = await controller.DeleteTask(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
