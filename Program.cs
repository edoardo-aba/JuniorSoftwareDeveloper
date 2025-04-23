using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using TaskManagerApi.Models; //! importing my model for the task

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.WriteIndented = true; //! for pretty indentation
});

builder.Services.AddHttpClient("docuware", client =>
{
    client.BaseAddress = new Uri("https://services.paloalto.swiss:10443/api2/");
});


var app = builder.Build();

string dataFolder = "data";
Directory.CreateDirectory(dataFolder);
string dataFilePath = Path.Combine(dataFolder, "tasks.json");

//! for loading all the tasks from the file
List<TaskItem> LoadTasks()
{
    if (!File.Exists(dataFilePath))
    {
        return new List<TaskItem>();
    }

    var json = File.ReadAllText(dataFilePath);

    if (string.IsNullOrWhiteSpace(json))
    {
        return new List<TaskItem>();
    }

    //! extra control otherwise if the json is empty, when adding a task it tries to parse it but fails
    try
    {
        return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
    }
    catch (JsonException)
    {
        // Handle invalid JSON format gracefully
        return new List<TaskItem>();
    }
}

//! for saving the tasks back to the json
void SaveTasks(List<TaskItem> tasks)
{
    var json = JsonSerializer.Serialize(tasks);
    File.WriteAllText(dataFilePath, json);
}

app.MapGet("/", () => "Task Manager API - Test Project");

// Post API
app.MapPost("/tasks", async (TaskItem task, IHttpClientFactory httpFactory) =>
{
    //! for saving in local json file
    var tasks = LoadTasks();
    task.Id        = Guid.NewGuid();
    task.CreatedAt = DateTime.UtcNow;
    tasks.Add(task);
    SaveTasks(tasks);

    //! for saving in Docuware
    var client = httpFactory.CreateClient("docuware");

    // here i am using the same payload as in the docuware example of the route add-record
    var payload = new
    {
        userId     = 23,
        passwordWS = "1234",
        cabinetId  = "804dfcb0-cf00-49c7-bb23-ec68bc3a6097",
        indexFields = new[]
        {
            new { fieldName = "TASK_ID",          fieldValue = task.Id.ToString() },
            new { fieldName = "TASK_DESCRIPTION", fieldValue = task.Title         },
            new { fieldName = "CREATION_DATE",    fieldValue = task.CreatedAt.ToString("o") }
        }
    };

    // sending the payload to docuware and waiting for the response
    var resp = await client.PostAsJsonAsync("Docuware/add-record", payload);
    resp.EnsureSuccessStatusCode();

    // return the created task
    return Results.Created($"/tasks/{task.Id}", task);
});

// Get API
app.MapGet("/tasks", (HttpRequest request) =>
{
    // check if the tenant ID is provided in the request headers
    if (!request.Headers.TryGetValue("X-Tenant-ID", out var tenantId))
    {
        return Results.BadRequest("Missing X-Tenant-ID header");
    }

    // filter tasks based on the tenant ID
    var tasks = LoadTasks().Where(t => t.TenantId == tenantId);
    return Results.Ok(tasks);
});

// Put API
app.MapPut("/tasks/{id}", (Guid id, TaskItem updatedTask) =>
{
    // load all the task from the json and check if the task exists
    var tasks = LoadTasks();
    var taskFound = tasks.FirstOrDefault(t => t.Id == id);

    if(taskFound is null){
        return Results.NotFound("Task does not exists");
    }

    if (updatedTask.Id != id)
    {
        return Results.BadRequest("Id in the body does not match Id in URL");
    }

    taskFound.Title = updatedTask.Title;
    taskFound.IsCompleted = updatedTask.IsCompleted;
    taskFound.TenantId = updatedTask.TenantId; //! i choose to modify the tenant Id in case it was assigned to another tenant

    SaveTasks(tasks);

    return Results.Ok(taskFound);
});

// Get API for the list of tasks pushed to Docuware, so i can see the task that are there since from the swagger i get unauthorized (401)
app.MapGet("/tasks/records", async (IHttpClientFactory httpFactory) =>
{
    var client = httpFactory.CreateClient("docuware");

    // here i create the payload for the query-documents route
    var query = new
    {
        userId       = 23,
        passwordWS   = "1234",
        cabinetId    = "804dfcb0-cf00-49c7-bb23-ec68bc3a6097",
        filterFields = Array.Empty<object>()   // no filter fields 
    };

    // sending the request with the needed payload to docuware and waiting for the response
    var resp = await client.PostAsJsonAsync("Docuware/query-documents", query);
    var body = await resp.Content.ReadAsStringAsync();

    return Results.Content(body, "application/json");
});

app.Run();