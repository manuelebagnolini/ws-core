﻿using Carter;
using Ws.Core.Extensions.Data;

namespace xCore.Modules;

public class Todo : ICarterModule, IEntity<string>
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Title { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public bool IsComplete { get; set; } = false;

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/todo", GetAll);
        app.MapGet("/api/todo/{id}", GetById);
        app.MapPost("/api/todo", Create);
        app.MapPut("/api/todo/{id}/mark-complete", MarkComplete);
        app.MapDelete("/api/todo/{id}", Delete);
    }

    private IEnumerable<Todo> GetAll(IRepository<Todo, string> _repo) => _repo.List;
    private IResult GetById(string id, IRepository<Todo, string> _repo) =>
        _repo.Find(id) is Todo todo
                ? Results.Ok(todo)
                : Results.NotFound();
    private IResult Create(Todo todo, IRepository<Todo, string> _repo)
    {
        todo.IsComplete = false;
        _repo.Add(todo);
        return Results.Created($"/todo/{todo.Id}", todo);
    }
    private IResult Delete(string id, IRepository<Todo, string> _repo) {
        if (_repo.Find(id) is Todo todo) {
            _repo.Delete(todo);
            return Results.NoContent();
        } 
        else
            return Results.NotFound(); 
    }
    private IResult MarkComplete(string id, IRepository<Todo, string> _repo)
    {
        if (_repo.Find(id) is Todo todo)
        {
            todo.IsComplete = true;
            _repo.Update(todo);
            return Results.NoContent();
        }
        else
            return Results.NotFound();
    }
}
