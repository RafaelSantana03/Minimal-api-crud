using Microsoft.EntityFrameworkCore;
using MinimalApiProject.Data;
using MinimalApiProject.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Esse c�digo define a conex�o do Entity Framework Core com o banco de dados SQL Server usando a string de conex�o "DefaultConnection".
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

//endpoint para obter todos os usu�rios.
app.MapGet("/Usuarios", async (AppDbContext context) =>
{
    return await GetUsuarios(context);
});

//endpoint para obter um usu�rio espec�fico pelo ID.
app.MapGet("/Usuario/{id}", async (AppDbContext context, int id) =>
{
    var usuario = await context.Usuarios.FindAsync(id);
    if (usuario == null)
    {
        return Results.NotFound("Usu�rio n�o localizado");
    }
    return Results.Ok(usuario);
});

//endpoint para criar um novo usu�rio.
app.MapPost("/Usuario", async (AppDbContext context, UsuarioModel usuario) =>
{
    context.Usuarios.Add(usuario);
    await context.SaveChangesAsync();
    
    return await GetUsuarios(context);
});

//endpoint para atualizar um usu�rio existente.
app.MapPut("/Usuario", async (AppDbContext context, UsuarioModel usuarioAtualizado) =>
{
    var usuario = await context.Usuarios.FindAsync(usuarioAtualizado.Id);

    if (usuario == null)
    {
        return Results.NotFound("Usu�rio n�o localizado");
    }
    usuario.UserName = usuarioAtualizado.UserName;
    usuario.Email = usuarioAtualizado.Email;
    usuario.Nome = usuarioAtualizado.Nome;
    

    context.Update(usuario);
    await context.SaveChangesAsync();

    return Results.Ok(usuario);
});

//endpoint para excluir um usu�rio pelo ID.
app.MapDelete("/Usuarios/{id}", async (AppDbContext context, int id) =>
{
    var usuario = await context.Usuarios.FindAsync(id);
    if (usuario == null)
    {
        return Results.NotFound("Usu�rio n�o localizado");
    }
    
    context.Usuarios.Remove(usuario);
    await context.SaveChangesAsync();
    
    return Results.Ok(await GetUsuarios(context));
});

app.Run();
