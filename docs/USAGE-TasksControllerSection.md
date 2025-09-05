# TasksControllerSection Client - Tasks Management API

**Created:** 202501051800  
**Description:** Client implementation for managing scheduled tasks and background worker operations in the Sufficit system.

## Overview

The `TasksControllerSection` provides a client-side interface for interacting with the Tasks API, allowing management of scheduled tasks that run as background jobs using CRON expressions.

## Purpose

- **Task Management**: Create, update, delete and list scheduled tasks
- **Job Execution**: Execute tasks synchronously or asynchronously on demand
- **Background Processing**: Manage tasks that are processed by the background worker service
- **CRON Scheduling**: Support for Linux CRON-like scheduling expressions

## Architecture

### Dependencies
- **Base Class**: `AuthenticatedControllerSection` (requires manager role authorization)
- **Controller Route**: `/tasks`
- **Models**: `ScheduleTask`, `EndPointResponse` from `Sufficit.Tasks` and `Sufficit.EndPoints`
- **Authorization**: Requires `ManagerRole.NormalizedName` for all operations

### Integration
The client integrates with:
- **TasksController** in `sufficit-endpoints` project
- **TasksProvider** for data persistence
- **BackgroundTasksWorker** for automated execution
- **IJobExecution** interface for job implementations

## Methods

### Execute Tasks

#### `ExecuteTask(Guid id, CancellationToken cancellationToken)`
- **Purpose**: Executes a task synchronously, waiting for completion
- **HTTP**: GET /tasks/execute?id={guid}
- **Returns**: `EndPointResponse` with execution results
- **Authorization**: Manager role required

#### `ExecuteTaskBackground(Guid id, CancellationToken cancellationToken)`
- **Purpose**: Starts task execution in background, returns immediately
- **HTTP**: POST /tasks/execute?id={guid}
- **Returns**: `EndPointResponse` confirming execution started
- **Authorization**: Manager role required

### Task Management

#### `CreateOrUpdateTask(ScheduleTask task, CancellationToken cancellationToken)`
- **Purpose**: Creates new task or updates existing one
- **HTTP**: POST /tasks
- **Body**: `ScheduleTask` JSON object
- **Returns**: `Guid?` of created/updated task
- **Authorization**: Manager role required

#### `DeleteTask(Guid id, CancellationToken cancellationToken)`
- **Purpose**: Removes a scheduled task permanently
- **HTTP**: DELETE /tasks?id={guid}
- **Returns**: `Guid?` of deleted task
- **Authorization**: Manager role required

#### `GetAllTasks(CancellationToken cancellationToken)`
- **Purpose**: Retrieves all scheduled tasks in the system
- **HTTP**: GET /tasks
- **Returns**: `IEnumerable<ScheduleTask>`
- **Authorization**: Manager role required

## ScheduleTask Model

```csharp
public class ScheduleTask : CronSchedule
{
    public Guid Id { get; set; }                    // Unique identifier
    public string Method { get; set; }              // Full type name for job class
    public object? Args { get; set; }               // JSON arguments for execution
    public string? Servers { get; set; }            // Server filtering
    public bool Active { get; set; } = true;        // Enable/disable task
    public bool Locked { get; set; } = false;       // Concurrency control
    public DateTime Timestamp { get; set; }         // Last execution time
    
    // CRON Schedule properties (inherited)
    public string? Minutes { get; set; }            // 00-59
    public string? Hours { get; set; }              // 00-23  
    public string? MonthDays { get; set; }          // 01-31
    public string? Months { get; set; }             // 01-12
    public string? WeekDays { get; set; }           // 00-06 (Sunday=0)
}
```

## Usage Examples

### Basic Task Creation

```csharp
// Create API client
var apiClient = serviceProvider.GetRequiredService<APIClientService>();

// Create a new scheduled task
var task = new ScheduleTask
{
    Method = "Sufficit.Background.Jobs.TableCleanUpJob, Sufficit.Background", 
    Minutes = "0",        // Run at minute 0
    Hours = "2",          // At 2 AM
    MonthDays = "*",      // Every day
    Months = "*",         // Every month
    WeekDays = "*",       // Every weekday
    Active = true
};

// Create the task
var taskId = await apiClient.Tasks.CreateOrUpdateTask(task, cancellationToken);
Console.WriteLine($"Created task with ID: {taskId}");
```

### Execute Task On Demand

```csharp
// Execute immediately and wait for result
var response = await apiClient.Tasks.ExecuteTask(taskId.Value, cancellationToken);
Console.WriteLine($"Execution result: {response.Message}");

// Or execute in background
var backgroundResponse = await apiClient.Tasks.ExecuteTaskBackground(taskId.Value, cancellationToken);
Console.WriteLine($"Background execution started: {backgroundResponse.Message}");
```

### List and Manage Tasks

```csharp
// Get all tasks
var allTasks = await apiClient.Tasks.GetAllTasks(cancellationToken);
foreach (var existingTask in allTasks)
{
    Console.WriteLine($"Task: {existingTask.Id} - {existingTask.Method} - Active: {existingTask.Active}");
}

// Delete a task
var deletedId = await apiClient.Tasks.DeleteTask(taskId.Value, cancellationToken);
Console.WriteLine($"Deleted task: {deletedId}");
```

### Advanced CRON Examples

```csharp
// Run every 15 minutes
var frequentTask = new ScheduleTask
{
    Method = "MyNamespace.FrequentJob, MyAssembly",
    Minutes = "0,15,30,45"  // At 0, 15, 30, 45 minutes
};

// Run every weekday at 9 AM
var businessTask = new ScheduleTask  
{
    Method = "MyNamespace.BusinessJob, MyAssembly",
    Minutes = "0",
    Hours = "9", 
    WeekDays = "1,2,3,4,5"  // Monday to Friday
};

// Run first day of every month
var monthlyTask = new ScheduleTask
{
    Method = "MyNamespace.MonthlyJob, MyAssembly", 
    Minutes = "0",
    Hours = "1",
    MonthDays = "1"  // First day of month
};
```

## Error Handling

```csharp
try
{
    var taskId = await apiClient.Tasks.CreateOrUpdateTask(task, cancellationToken);
    if (taskId.HasValue)
    {
        Console.WriteLine($"Task created successfully: {taskId}");
    }
    else
    {
        Console.WriteLine("Failed to create task - no ID returned");
    }
}
catch (UnauthenticatedExpection ex)
{
    Console.WriteLine("Authentication required for task management");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"HTTP error: {ex.Message}");
}
```

## Job Implementation

To create a custom job that can be scheduled:

```csharp
using Sufficit;
using System.Threading;
using System.Threading.Tasks;

public class MyCustomJob : IJobExecution
{
    public async ValueTask<object?> Run(CancellationToken cancellationToken, params object?[]? args)
    {
        // Your job logic here
        Console.WriteLine("Custom job executing...");
        
        // Access arguments if provided
        if (args?.Length > 0)
        {
            var firstArg = args[0]?.ToString();
            Console.WriteLine($"First argument: {firstArg}");
        }
        
        // Perform work
        await SomeAsyncWork(cancellationToken);
        
        // Return result (optional)
        return new { Status = "Completed", ProcessedItems = 42 };
    }
    
    private async Task SomeAsyncWork(CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        // Your actual work here
    }
}
```

## Security Notes

- All operations require Manager role authorization
- Tasks execute with system-level privileges
- Method types must be valid and accessible
- Arguments are serialized as JSON
- Server filtering allows restricting execution to specific machines

## Related Components

- **TasksController**: Server-side API controller
- **BackgroundTasksWorker**: Automated task execution service  
- **TasksProvider**: Data access layer for task persistence
- **IJobExecution**: Interface for job implementations
- **ScheduleTask**: Core task model with CRON scheduling
