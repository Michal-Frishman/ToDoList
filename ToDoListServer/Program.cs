using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToDoApi;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ToDoDb");

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, 
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"))
);

// builder.Services.AddDbContext<ToDoDbContext>(options =>
//     options.UseMySql("name=ToDoDB", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"))
// );

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("AllowAllOrigins");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
    c.RoutePrefix = string.Empty; 
});

app.MapGet("/items", async (ToDoDbContext db) =>
{
    return await db.Items.ToListAsync();
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

app.MapPut("/items/{id}", async (ToDoDbContext db, int id) =>
{
  var item = await db.Items.FindAsync(id);
        if (item is null) return Results.NotFound();

        item.IsComplete=!item.IsComplete;

        System.Console.WriteLine(item.IsComplete);
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

