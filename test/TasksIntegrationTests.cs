using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sufficit.Client;
using Sufficit.EndPoints.Configuration;
using Sufficit.Identity;
using Sufficit.Tasks;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sufficit.Client.IntegrationTests
{
    /// <summary>
    /// Integration tests for Tasks Controller endpoints
    /// Tests the endpoints:
    /// - GET /tasks - List all scheduled tasks
    /// - POST /tasks - Create or update scheduled task
    /// - DELETE /tasks?id={id} - Delete scheduled task
    /// - GET /tasks/execute?id={id} - Execute task synchronously
    /// - POST /tasks/execute?id={id} - Execute task asynchronously
    /// 
    /// Configuration in appsettings.json:
    /// - Sufficit:EndPoints:BaseAddress - API base URL
    /// - Sufficit:Authentication:Tokens:Manager - Manager token for authentication
    /// </summary>
    [Trait("Category", "Integration")]
    public class TasksIntegrationTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly APIClientService _apiClient;
        private readonly Guid _testTaskId = Guid.NewGuid();

        public TasksIntegrationTests(ITestOutputHelper output)
        {
            _output = output;

            // Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var endpointsUrl = configuration["Sufficit:EndPoints:BaseAddress"]
                ?? throw new InvalidOperationException("EndPoints BaseAddress not configured in appsettings.json");

            // Use Manager token for integration tests (TasksController requires Manager role)
            var token = configuration["Sufficit:Authentication:Tokens:Manager"]
                ?? throw new InvalidOperationException("Manager token not configured in appsettings.json");

            var timeout = uint.TryParse(configuration["Sufficit:EndPoints:TimeOut"], out var t) ? t : 30u;

            // Configure EndPoints options
            var options = new EndPointsAPIOptions
            {
                BaseAddress = endpointsUrl,
                TimeOut = timeout
            };

            // Create API client
            _apiClient = new APIClientService(
                Options.Create(options),
                new StaticTokenProvider(token),
                NullLogger<APIClientService>.Instance);
        }

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Database", "ReadOnly")]
        public async Task GetAllTasks_ReturnsTaskList()
        {
            // Arrange
            _output.WriteLine("Testing GET /tasks - List all scheduled tasks");

            // Act
            var tasks = await _apiClient.Tasks.GetAllTasks(CancellationToken.None);

            // Assert
            Assert.NotNull(tasks);
            var taskList = tasks.ToList();

            _output.WriteLine($"Found {taskList.Count} scheduled tasks");

            foreach (var task in taskList)
            {
                Assert.NotEqual(Guid.Empty, task.Id);
                Assert.NotNull(task.Method);
                Assert.NotEmpty(task.Method);

                _output.WriteLine($"Task: {task.Id} -> Method: '{task.Method}', Active: {task.Active}");
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Database", "Write")]
        public async Task CreateTask_WithValidData_ReturnsTaskId()
        {
            // Arrange
            var task = new ScheduleTask
            {
                Id = _testTaskId,
                Method = "Sufficit.Tests.TestJob, Sufficit.Tests",
                Active = false, // Disabled to prevent actual execution
                Minutes = "00",
                Hours = "00"
            };

            _output.WriteLine($"Testing POST /tasks - Create task with ID: {task.Id}");

            // Act
            var createdTaskId = await _apiClient.Tasks.CreateOrUpdateTask(task, CancellationToken.None);

            // Assert
            Assert.NotNull(createdTaskId);
            Assert.Equal(_testTaskId, createdTaskId.Value);

            _output.WriteLine($"Task created successfully: {createdTaskId}");
        }

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Database", "Write")]
        public async Task UpdateTask_WithExistingId_ReturnsTaskId()
        {
            // Arrange - First create a task
            var task = new ScheduleTask
            {
                Id = _testTaskId,
                Method = "Sufficit.Tests.TestJob, Sufficit.Tests",
                Active = false,
                Minutes = "00",
                Hours = "00"
            };

            await _apiClient.Tasks.CreateOrUpdateTask(task, CancellationToken.None);

            // Modify task
            task.Hours = "02,04,06"; // Every 2 hours (hours 2, 4, 6)
            _output.WriteLine($"Testing POST /tasks - Update task with ID: {task.Id}");

            // Act
            var updatedTaskId = await _apiClient.Tasks.CreateOrUpdateTask(task, CancellationToken.None);

            // Assert
            Assert.NotNull(updatedTaskId);
            Assert.Equal(_testTaskId, updatedTaskId.Value);

            _output.WriteLine($"Task updated successfully: {updatedTaskId}");
        }

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Database", "Write")]
        public async Task DeleteTask_WithValidId_ReturnsTaskId()
        {
            // Arrange - First create a task
            var task = new ScheduleTask
            {
                Id = _testTaskId,
                Method = "Sufficit.Tests.TestJob, Sufficit.Tests",
                Active = false,
                Minutes = "00",
                Hours = "00"
            };

            await _apiClient.Tasks.CreateOrUpdateTask(task, CancellationToken.None);
            _output.WriteLine($"Testing DELETE /tasks?id={_testTaskId}");

            // Act
            var deletedTaskId = await _apiClient.Tasks.DeleteTask(_testTaskId, CancellationToken.None);

            // Assert
            Assert.NotNull(deletedTaskId);
            Assert.Equal(_testTaskId, deletedTaskId.Value);

            _output.WriteLine($"Task deleted successfully: {deletedTaskId}");
        }

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Database", "Write")]
        public async Task ExecuteTaskSync_WithValidId_ReturnsResponse()
        {
            // Arrange - Create a test task
            var task = new ScheduleTask
            {
                Id = _testTaskId,
                Method = "Sufficit.Tests.TestJob, Sufficit.Tests",
                Active = false,
                Minutes = "00",
                Hours = "00"
            };

            await _apiClient.Tasks.CreateOrUpdateTask(task, CancellationToken.None);
            _output.WriteLine($"Testing GET /tasks/execute?id={_testTaskId} - Synchronous execution");

            // Act
            var response = await _apiClient.Tasks.ExecuteTask(_testTaskId, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            // Note: Response may vary based on whether the job type exists and implements IJobExecution
            _output.WriteLine($"Execution response: Success={response.Success}, Message='{response.Message}'");
        }

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Database", "Write")]
        public async Task ExecuteTaskAsync_WithValidId_ReturnsResponse()
        {
            // Arrange - Create a test task
            var task = new ScheduleTask
            {
                Id = _testTaskId,
                Method = "Sufficit.Tests.TestJob, Sufficit.Tests",
                Active = false,
                Minutes = "00",
                Hours = "00"
            };

            await _apiClient.Tasks.CreateOrUpdateTask(task, CancellationToken.None);
            _output.WriteLine($"Testing POST /tasks/execute?id={_testTaskId} - Asynchronous execution");

            // Act
            var response = await _apiClient.Tasks.ExecuteTaskBackground(_testTaskId, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            // Background execution should return immediately with "job execution started" message
            _output.WriteLine($"Background execution response: Success={response.Success}, Message='{response.Message}'");
        }

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Database", "Write")]
        public async Task TaskLifecycle_CreateUpdateDelete_WorksCorrectly()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new ScheduleTask
            {
                Id = taskId,
                Method = "Sufficit.Tests.TestJob, Sufficit.Tests",
                Active = false,
                Minutes = "00",
                Hours = "00"
            };

            _output.WriteLine($"Testing full task lifecycle with ID: {taskId}");

            try
            {
                // Act & Assert - Create
                _output.WriteLine("Step 1: Creating task");
                var createdId = await _apiClient.Tasks.CreateOrUpdateTask(task, CancellationToken.None);
                Assert.NotNull(createdId);
                Assert.Equal(taskId, createdId.Value);

                // Act & Assert - Update
                _output.WriteLine("Step 2: Updating task");
                task.Hours = "03,06,09"; // Every 3 hours
                var updatedId = await _apiClient.Tasks.CreateOrUpdateTask(task, CancellationToken.None);
                Assert.NotNull(updatedId);
                Assert.Equal(taskId, updatedId.Value);

                // Act & Assert - Verify in list
                _output.WriteLine("Step 3: Verifying task in list");
                var allTasks = await _apiClient.Tasks.GetAllTasks(CancellationToken.None);
                Assert.Contains(allTasks, t => t.Id == taskId);

                // Act & Assert - Delete
                _output.WriteLine("Step 4: Deleting task");
                var deletedId = await _apiClient.Tasks.DeleteTask(taskId, CancellationToken.None);
                Assert.NotNull(deletedId);
                Assert.Equal(taskId, deletedId.Value);

                _output.WriteLine("Full lifecycle test completed successfully");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Lifecycle test failed: {ex.Message}");
                
                // Cleanup attempt
                try
                {
                    await _apiClient.Tasks.DeleteTask(taskId, CancellationToken.None);
                }
                catch { }
                
                throw;
            }
        }

        public void Dispose()
        {
            // Cleanup: Ensure test task is deleted
            try
            {
                _apiClient.Tasks.DeleteTask(_testTaskId, CancellationToken.None).Wait(TimeSpan.FromSeconds(5));
                _output.WriteLine($"Cleanup: Test task {_testTaskId} deleted");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Cleanup warning: Failed to delete test task - {ex.Message}");
            }
        }
    }
}
