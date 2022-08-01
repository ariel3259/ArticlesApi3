using EntityFrameworkCore.UseRowNumberForPaging;
using Microsoft.EntityFrameworkCore;
using ArticlesApi.Entities;
using ArticlesApi.Interfaces;
using ArticlesApi.Adapters;
using ArticlesApi.Dto;
using FluentValidation.Results;
using ArticlesApi.Validators;

var builder = WebApplication.CreateBuilder(args);

string con = builder.Configuration.GetConnectionString("Dev");

//Inyect Context
builder.Services.AddDbContext<AppDbContext>((options) => {
    options.UseSqlServer(con, (builder) => builder.UseRowNumberForPaging());
}, contextLifetime: ServiceLifetime.Transient);

//Inyect Services
builder.Services.AddTransient<IAdapter<ArticlesRequest, ArticlesEntity>, ArticlesEntityAdapter>();
builder.Services.AddTransient<IAdapter<ArticlesEntity, ArticlesResponse>, ArticlesResponseAdapter>();

//Inyect Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Initialize validators
ArticlesRequestValidator articlesRequestValidator = new();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/articles", (int offset, int limit, IAdapter<ArticlesEntity, ArticlesResponse> adapter, HttpContext ctx, AppDbContext context) =>
{
    try{
        Pages<ArticlesEntity> articlesPage = context.GetAll<ArticlesEntity>(offset, limit);
        List<ArticlesResponse> articles = new();
        foreach (ArticlesEntity article in articlesPage.Items)
        {
            articles.Add(adapter.AdaptTo(article));
        }
        ctx.Response.Headers.Add("X-Total-Count", articlesPage.Total.ToString());
        return Results.Ok(articles);
    }catch(Exception ex)
    {
        return Results.Json(data: ex.Message, statusCode: 500);
    }
});

app.MapPost("/api/articles", (ArticlesRequest articlesRequest, IAdapter<ArticlesRequest, ArticlesEntity> adapter, AppDbContext context) =>
{
    try
    {
        ValidationResult result = articlesRequestValidator.Validate(articlesRequest);
        if (!result.IsValid) return Results.BadRequest(new ArticlesEntity());
        var entity = context.Articles.Add(adapter.AdaptTo(articlesRequest));
        context.SaveChanges();
        return Results.Ok(entity.Entity);
    }catch(Exception ex)
    {
        return Results.Json(data: ex.Message, statusCode: 500);
    }
});

app.MapPut("/api/articles/{id}", (int id, ArticlesRequest articlesRequest, AppDbContext context) =>
{
    try
    {
        ValidationResult result = articlesRequestValidator.Validate(articlesRequest);
        if (!result.IsValid) return Results.BadRequest(new ArticlesEntity());
        ArticlesEntity? articleToModify = context.Articles.FirstOrDefault(x => x.Active && x.Id == id);
        if (articleToModify == null) return Results.NotFound(new ArticlesEntity());
        articleToModify.Name = articlesRequest.Name;
        articleToModify.Description = articlesRequest.Description;
        articleToModify.Price = articlesRequest.Price;
        articleToModify.Stock = articlesRequest.Stock;
        context.SaveChanges();
        return Results.Ok(articleToModify);
    }catch(Exception ex)
    {
        return Results.Json(data: ex.Message, statusCode: 500);
    }
});

app.MapMethods("/api/articles/{id}/price", new[] { "PATCH" }, (int id, double price, AppDbContext context) =>
{
    try
    {
        ArticlesEntity? articleToModify = context.Articles.FirstOrDefault(x => x.Active && x.Id == id);
        if (articleToModify == null) return Results.NotFound(new ArticlesEntity());
        articleToModify.Price = price;
        context.SaveChanges();
        return Results.Ok(articleToModify);
    }
    catch (Exception ex)
    {
        return Results.Json(data: ex.Message, statusCode: 500);
    }
});

app.MapMethods("/api/articles/{id}/stock", new[] { "PATCH" }, (int id, int stock, AppDbContext context) =>
{
    try
    {
        ArticlesEntity? articleToModify = context.Articles.FirstOrDefault(x => x.Active && x.Id == id);
        if (articleToModify == null) return Results.NotFound(new ArticlesEntity());
        articleToModify.Stock = stock;
        context.SaveChanges();
        return Results.Ok(articleToModify);
    }catch(Exception ex)
    {
        return Results.Json(data: ex.Message, statusCode: 500);
    }
} );

app.MapDelete("/api/articles/{id}", (int id, AppDbContext context) => {
    try
    {
        ArticlesEntity? articleToDelete = context.Articles.FirstOrDefault(x => x.Active && x.Id == id);
        if (articleToDelete == null) return Results.Conflict();
        articleToDelete.Active = false;
        context.SaveChanges();
        return Results.NoContent();
    }catch(Exception ex)
    {
        return Results.Json(data: ex.Message, statusCode: 500);
    }
});

app.Run();
