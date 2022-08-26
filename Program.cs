using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using MinimalAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

async Task<List<SuperHero>> GetAllHeroes(DataContext context) => await context.SuperHeroes.ToListAsync();
app.MapGet("/superhero", async (DataContext context) => await context.SuperHeroes.ToListAsync());
app.MapGet("/superhero/{id}", async (DataContext context, int id) =>
    await context.SuperHeroes.FindAsync(id) is SuperHero hero ? Results.Ok(hero) :
    Results.NotFound("Sorry, Not found"));
app.MapPost("/superhero", async (DataContext context, SuperHero hero) =>
{
    context.SuperHeroes.Add(hero);
    await context.SaveChangesAsync();
    return Results.Ok(await GetAllHeroes(context));
}

);

app.MapPut("/superhero/{id}", async (DataContext context, SuperHero hero, int id) =>
{
    var dbHero = await context.SuperHeroes.FindAsync(id);
    if (dbHero == null) return Results.NotFound("No Hero Found");


    dbHero.Name = hero.Name;
    dbHero.FirstName = hero.FirstName; 
    dbHero.LastName = hero.LastName;
    dbHero.Place = hero.Place;

    await context.SaveChangesAsync();
    return Results.Ok(await GetAllHeroes(context));
}
);

app.MapDelete("/superhero/{id}", async (DataContext context, int id) =>
{
    var dbHero = await context.SuperHeroes.FindAsync(id);
    if (dbHero == null) return Results.NotFound("No Hero Found");

    context.SuperHeroes.Remove(dbHero);
    await context.SaveChangesAsync();
    return Results.Ok(await GetAllHeroes(context));

}
);

app.Run();
