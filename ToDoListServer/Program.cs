// using Microsoft.EntityFrameworkCore;
// using Microsoft.OpenApi.Models;
// using ToDoApi;


// var builder = WebApplication.CreateBuilder(args);

// var connectionString = builder.Configuration.GetConnectionString("totodb");

// builder.Services.AddDbContext<ToDoDbContext>(options =>
//     options.UseMySql(connectionString, 
//     Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"))
// );

// // builder.Services.AddDbContext<ToDoDbContext>(options =>
// //     options.UseMySql("name=ToDoDB", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"))
// // );
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAllOrigins",
//         builder =>
//         {
//             builder.AllowAnyOrigin()
//                    .AllowAnyMethod()
//                    .AllowAnyHeader();
//         });
// });
// // builder.Services.AddCors(options =>
// // {
// //     options.AddPolicy("AllowSpecificOrigins", policy =>
// //     {
// //         policy.AllowAnyOrigin()
// //               .AllowAnyMethod() 
// //               .AllowAnyHeader(); 
// //     });
// // });
// builder.Services.AddEndpointsApiExplorer();
// // builder.Services.AddSwaggerGen();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.SwaggerDoc("v1", new OpenApiInfo
//     {
//         Title = "ToDo API",
//         Version = "v1",
//         Description = "A simple ToDo API to manage tasks."
//     });
// });

// var app = builder.Build();
// app.UseCors("AllowAllOrigins");

// app.UseSwagger();
// app.UseSwaggerUI(c =>
// {
//     c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
//     c.RoutePrefix = string.Empty; 
// });

// // app.MapGet("/items", async (ToDoDbContext db) =>
// // {
// //     return await db.Items.ToListAsync();
// // });
// app.MapGet("/items", async (ToDoDbContext context) =>
// {
//     var items = await context.Items.ToListAsync();
//     return items.Any() ? Results.Ok(items) : Results.NoContent();
// });
// app.MapGet("/", ()=>
// {
//     return "to do api is running";
// });
// app.MapPost("/items", async (ToDoDbContext db, Item newItem) =>
// {
//     db.Items.Add(newItem);
//     await db.SaveChangesAsync();
//     return Results.Created($"/items/{newItem.Id}", newItem);
// });

// app.MapPut("/items/{id}",async (int id,Item isComplete, ToDoDbContext db) =>{
//     var item = await db.Items.FindAsync(id);
//     if(item is null) return Results.NotFound();
//     item.IsComplete=isComplete.IsComplete;
//     await db.SaveChangesAsync();
//     return Results.NoContent();
// });

// app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
// {
//     var item = await db.Items.FindAsync(id);
//     if (item is null) return Results.NotFound();

//     db.Items.Remove(item);
//     await db.SaveChangesAsync();
//     return Results.NoContent();
// });

// app.Run();
// using Microsoft.EntityFrameworkCore;
// using Microsoft.OpenApi.Models;
// using ToDoApi;

// var builder = WebApplication.CreateBuilder(args);

// // Get the connection string for the database
// var connectionString = builder.Configuration.GetConnectionString("tododb");

// // Configure the DbContext with MySQL
// builder.Services.AddDbContext<ToDoDbContext>(options =>
//     options.UseMySql(connectionString, 
//     Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"))
// );

// // Configure CORS to allow all origins
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAllOrigins",
//         builder =>
//         {
//             builder.AllowAnyOrigin()
//                    .AllowAnyMethod()
//                    .AllowAnyHeader();
//         });
// });

// // Add services for Swagger
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.SwaggerDoc("v1", new OpenApiInfo
//     {
//         Title = "ToDo API",
//         Version = "v1",
//         Description = "A simple ToDo API to manage tasks."
//     });
// });

// var app = builder.Build();

// // Use CORS policy before any other middleware that handles requests
// app.UseCors("AllowAllOrigins");

// // Enable Swagger UI
// app.UseSwagger();
// app.UseSwaggerUI(c =>
// {
//     c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
//     c.RoutePrefix = string.Empty; 
// });

// // Define endpoint to get all items
// app.MapGet("/items", async (ToDoDbContext context) =>
// {
//     var items = await context.Items.ToListAsync();
//     return items.Any() ? Results.Ok(items) : Results.NoContent();
// });

// // Define a simple health check endpoint
// app.MapGet("/", () => "ToDo API is running");

// // Define endpoint to create a new item
// app.MapPost("/items", async (ToDoDbContext db, Item newItem) =>
// {
//     db.Items.Add(newItem);
//     await db.SaveChangesAsync();
//     return Results.Created($"/items/{newItem.Id}", newItem);
// });

// // Define endpoint to update an existing item
// app.MapPut("/items/{id}", async (int id, Item updatedItem, ToDoDbContext db) =>
// {
//     var item = await db.Items.FindAsync(id);
//     if (item is null) return Results.NotFound();

//     item.IsComplete = updatedItem.IsComplete; // Update the IsComplete property
//     await db.SaveChangesAsync();
//     return Results.NoContent();
// });

// // Define endpoint to delete an item
// app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
// {
//     var item = await db.Items.FindAsync(id);
//     if (item is null) return Results.NotFound();

//     db.Items.Remove(item);
//     await db.SaveChangesAsync();
//     return Results.NoContent();
// });

// // Run the application
// app.Run();
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ToDoApi;

var builder = WebApplication.CreateBuilder(args);

// Get the connection string for the database
var connectionString = builder.Configuration.GetConnectionString("ToDoDb");

// Configure the DbContext with MySQL
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, 
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"))
);

// Configure CORS to allow all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


// Add services for Swagger
builder.Services.AddEndpointsApiExplorer();
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

// Use CORS policy before any other middleware that handles requests
app.UseCors("AllowAllOrigins");

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
    c.RoutePrefix = string.Empty; 
});

// Define endpoint to get all items
app.MapGet("/items", async (ToDoDbContext context) =>
{
    var items = await context.Items.ToListAsync();
    return items.Any() ? Results.Ok(items) : Results.NoContent();
});

// Define a simple health check endpoint
app.MapGet("/", () => "ToDo API is running");

// Define endpoint to create a new item
app.MapPost("/items", async (ToDoDbContext db, Item newItem) =>
{
    db.Items.Add(newItem);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{newItem.Id}", newItem);
});

// Define endpoint to update an existing item
app.MapPut("/items/{id}", async (int id, Item updatedItem, ToDoDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.IsComplete = updatedItem.IsComplete; // Update the IsComplete property
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Define endpoint to delete an item
app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Run the application
app.Run();
