using Microsoft.EntityFrameworkCore;
using MinimalShoppingListApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApiDbContext>(opt => opt.UseInMemoryDatabase("ShoppingListApi"));

var app = builder.Build();


app.MapGet("/shoppinglist", async(ApiDbContext db) => await db.Groceries.ToListAsync());

app.MapGet("/shoppinglist/{id}", async (int id, ApiDbContext db) =>
{
    var grocery = await db.Groceries.FindAsync(id);

    return grocery != null ? Results.Ok(grocery) : Results.NotFound();

});

app.MapPost("/shoppinglist", async (Grocery grocery, ApiDbContext db) =>
{
    db.Groceries.Add(grocery);

    await db.SaveChangesAsync();

    return Results.Created($"/Shoppinglist/{grocery.Id}", grocery);
});


app.MapDelete("/shoppinglist/{id}", async (int id, ApiDbContext db) =>
{

    var grocery = await db.Groceries.FindAsync(id);

    if (grocery != null)
    {
        db.Groceries.Remove(grocery);
       await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
