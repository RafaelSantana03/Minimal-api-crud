using Microsoft.EntityFrameworkCore;
using MinimalApiProject.Data;
using MinimalApiProject.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Esse código define a conexão do Entity Framework Core com o banco de dados SQL Server usando a string de conexão "DefaultConnection".
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

async Task<List<UsuarioModel>> GetUsuarios(AppDbContext context)
{
    return await context.Usuarios.ToListAsync();
}

//endpoint para obter todos os usuários.
app.MapGet("/Usuarios", async (AppDbContext context) =>
{
    return await GetUsuarios(context);
});

//endpoint para obter um usuário específico pelo ID.
app.MapGet("/Usuario/{id}", async (AppDbContext context, int id) =>
{
    var usuario = await context.Usuarios.FindAsync(id);
    if (usuario == null)
    {
        return Results.NotFound("Usuário não localizado");
    }
    return Results.Ok(usuario);
});

//endpoint para criar um novo usuário.
app.MapPost("/Usuario", async (AppDbContext context, UsuarioModel usuario) =>
{
    context.Usuarios.Add(usuario);
    await context.SaveChangesAsync();
    
    return await GetUsuarios(context);
});

//endpoint para atualizar um usuário existente.
app.MapPut("/Usuario", async (AppDbContext context, UsuarioModel usuarioAtualizado) =>
{
    var usuario = await context.Usuarios.FindAsync(usuarioAtualizado.Id);

    if (usuario == null)
    {
        return Results.NotFound("Usuário não localizado");
    }
    usuario.UserName = usuarioAtualizado.UserName;
    usuario.Email = usuarioAtualizado.Email;
    usuario.Nome = usuarioAtualizado.Nome;
    

    context.Update(usuario);
    await context.SaveChangesAsync();

    return Results.Ok(usuario);
});

//endpoint para excluir um usuário pelo ID.
app.MapDelete("/Usuarios/{id}", async (AppDbContext context, int id) =>
{
    var usuario = await context.Usuarios.FindAsync(id);
    if (usuario == null)
    {
        return Results.NotFound("Usuário não localizado");
    }
    
    context.Usuarios.Remove(usuario);
    await context.SaveChangesAsync();
    
    return Results.Ok(await GetUsuarios(context));
});

app.Run();
