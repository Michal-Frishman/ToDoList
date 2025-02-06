using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ToDoApi;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("totodb");

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, 
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"))
);

// builder.Services.AddDbContext<ToDoDbContext>(options =>
//     options.UseMySql("name=ToDoDB", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"))
// );

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAllOrigins",
//         builder => builder.AllowAnyOrigin()
//                           .AllowAnyMethod()
//                           .AllowAnyHeader());
// });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod() 
              .AllowAnyHeader(); 
    });
});
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ToDo API",
        Version = "v1",
        Description = "A simple ToDo API to manage tasks."
    });
});

var app = builder.Build();
app.UseCors("AllowAllOrigins");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
    c.RoutePrefix = string.Empty; 
});

// app.MapGet("/items", async (ToDoDbContext db) =>
// {
//     return await db.Items.ToListAsync();
// });
app.MapGet("/tasks", async (ToDoDbContext context) =>
{
    var tasks = await context.Items.ToListAsync();
    return tasks.Any() ? Results.Ok(tasks) : Results.NoContent();
});
app.MapGet("/", ()=>
{
    return "to do api is running";
});
app.MapPost("/items", async (ToDoDbContext db, Item newItem) =>
{
    db.Items.Add(newItem);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{newItem.Id}", newItem);
});

app.MapPut("/items/{id}",async (int id,Item isComplete, ToDoDbContext db) =>{
    var item = await db.Items.FindAsync(id);
    if(item is null) return Results.NotFound();
    item.IsComplete=isComplete.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

