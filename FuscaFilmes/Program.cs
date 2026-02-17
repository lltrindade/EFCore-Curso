using System.Text.Json.Serialization;
using FuscaFilmes.DbContexts;
using FuscaFilmes.Entities;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Context>(
    options => options.UseSqlite(builder.Configuration["ConnectionStrings:FuscaFilmesStr"])
);

//-»» Garantir que o banco de dados seja criado
/*
using (var context = new Context())
{
    context.Database.EnsureCreated();
}
*/

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JsonOptions>(options =>
{
    //-»» Configurações para o JSON permitir vírgulas finais
    options.SerializerOptions.AllowTrailingCommas = true;
    //-»» Configurações para o JSON evitar ciclos de referência
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//-»» Para trabalhar com HTTPs
//app.UseHttpsRedirection();

app.MapGet("/diretores", (Context context) =>
{
    //using var context = new Context();
    return context.Diretores
        .Include(diretor => diretor.Filmes)
        .ToList();
})
.WithOpenApi();

app.MapPost("/diretores", (Context context, Diretor diretor) =>
{
    //using var context = new Context();
    context.Diretores.Add(diretor);
    context.SaveChanges();
})
.WithOpenApi();

app.MapPut("/diretores/{diretorId}", (Context context, int diretorId, Diretor diretorNew) =>
{
    //using var context = new Context();
    var diretor = context.Diretores.Find(diretorId);
    if (diretor != null)
    {
        diretor.Nome = diretorNew.Nome;
        if (diretorNew.Filmes.Count > 0)
        {
            diretor.Filmes.Clear();
            foreach (var filme in diretorNew.Filmes)
            {
                diretor.Filmes.Add(filme);
            }
        }
        context.SaveChanges();
    }
})
.WithOpenApi();

app.MapDelete("/diretores/{diretorId}", (Context context, int diretorId) =>
{
    //using var context = new Context();
    var diretor = context.Diretores.Find(diretorId);
    if (diretor != null)
    {
        context.Diretores.Remove(diretor);
        context.SaveChanges();
    }
})
.WithOpenApi();


app.Run();
