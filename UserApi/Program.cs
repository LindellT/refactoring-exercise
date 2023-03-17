using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserContext>(opt => opt.UseSqlite($"Data Source=user.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var Scope = app.Services.CreateScope();
    var context = Scope.ServiceProvider.GetRequiredService<UserContext>();
    await context.Database.MigrateAsync();
}

app.UseHttpsRedirection();

app.MapGet("/user/create/{email}/{password}", (UserContext userContext, string email, string password) =>
{
    var validPassword = !string.IsNullOrEmpty(password) && password.Length > 7;

    var validationProblems = new List<(string field, string description)>();

    if (!validPassword)
    {
        validationProblems.Add(("password", "Invalid password. Password must be minimum of 8 characters."));
    }

    var validEmail = !string.IsNullOrEmpty(password) && email.Length > 2 && email.Contains('@');

    if (!validEmail)
    {
        validationProblems.Add(("email", "Invalid email. Email must have a recipient and domain and contain @ sign."));
    }

    if (validationProblems.Any())
    {
        return Results.ValidationProblem(validationProblems.ToDictionary(p => p.field, p => validationProblems.Where(vp => vp.field == p.field).Select(vp => vp.description).ToArray()));
    }

    var user = new User { Email = email, Password = password };
    
    if (userContext.Users.Any(u => u.Email == user.Email && !user.IsDeleted))
    {
        return Results.BadRequest("Email reserved.");
    }

    userContext.Add(user);
    var createdRows = userContext.SaveChanges();

    if (createdRows == 0)
    {
        return Results.BadRequest("User creation failed.");
    }

    return Results.CreatedAtRoute("GetUserById", new { user.Id });
})
.WithName("CreateUser")
.WithOpenApi();

app.MapGet("/user/update/{id:int}/{email?}/{password?}", (UserContext userContext, int id, string? email, string? password) =>
{
    var validPassword = !string.IsNullOrEmpty(password) && password.Length > 7;
    var validEmail = !string.IsNullOrEmpty(email) && email.Length > 2 && email.Contains('@');

    if (!validPassword && !validEmail)
    {
        return Results.BadRequest("Invalid email and password");
    }

    var user = userContext.Users.Find(id);

    if (user is null || user.IsDeleted)
    {
        return Results.NotFound();
    }

    if (userContext.Users.Any(u => u.Email == email && !user.IsDeleted && u.Id != id))
    {
        return Results.BadRequest("Email reserved.");
    }

    user.Email = validEmail ? email! : user.Email;
    user.Password = validPassword ? password! : user.Password;

    var success = userContext.SaveChanges() == 1;

    if (!success)
    {
        return Results.BadRequest("Updating user failed or no changes");
    }

    return Results.Ok();
})
.WithName("UpdateUser")
.WithOpenApi();

app.MapGet("/user/delete/{id:int}", (UserContext userContext, int id) =>
{
    var user = userContext.Users.Find(id);

    if (user is null)
    {
        return Results.NotFound();
    }

    user.IsDeleted = true;

    var success = userContext.SaveChanges() == 1;

    if (!success)
    {
        return Results.BadRequest("Deleting user failed");
    }

    return Results.Ok();
})
.WithName("DeleteUser")
.WithOpenApi();

app.MapGet("/user", (UserContext userContext) =>
{
    return userContext.Users.Where(u => !u.IsDeleted).ToList();
})
.WithName("ListUsers")
.WithOpenApi();

app.MapGet("/user/{id:int}", (UserContext userContext, int id) =>
{
    var user = userContext.Users.Find(id);

    if (user is null || user.IsDeleted)
    {
        return Results.NotFound();
    }

    return Results.Ok(user);
})
.WithName("GetUserById")
.WithOpenApi();

app.Run();

internal class UserContext : DbContext
{
    public UserContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
}

internal class User
{ 
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }
}
