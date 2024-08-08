using DapperCRUDSample.Entities;
using DapperCRUDSample.Persistence;
using DapperCRUDSample.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IStudentRepository, StudentRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region EFCORE API
var studentGroup = app.MapGroup("/api/students");

studentGroup.MapPost("", async (Student student, IStudentRepository studentRepository) =>
{
    await studentRepository.EFCreateAsync(student);

    return TypedResults.Ok();
});

studentGroup.MapPut("", async Task<Results<Ok, BadRequest<string>>> (Student student, IStudentRepository studentRepository) =>
{
    try
    {
        await studentRepository.EFUpdateAsync(student);

        return TypedResults.Ok();
    }
    catch (Exception ex)
    {
        return TypedResults.BadRequest(ex.Message);
    }
});

studentGroup.MapDelete("/{studentId}", async Task<Results<Ok, BadRequest<string>>> (int studentId, IStudentRepository studentRepository) =>
{
    try
    {
        await studentRepository.EFDeleteAsync(studentId);

        return TypedResults.Ok();
    }
    catch (Exception ex)
    {
        return TypedResults.BadRequest(ex.Message);
    }
});

studentGroup.MapGet("/{studentId}", async Task<Results<Ok<Student>, BadRequest<string>>> (int studentId, IStudentRepository studentRepository) =>
{
    try
    {
        var result = await studentRepository.EFGetAsync(studentId);

        return TypedResults.Ok(result);
    }
    catch (Exception ex)
    {
        return TypedResults.BadRequest(ex.Message);
    }
});

studentGroup.MapGet("", async (IStudentRepository studentRepository) =>
{
    var result = await studentRepository.EFListAsync();
    return TypedResults.Ok(result);
});
#endregion

#region Dapper API

var studentDapperGroup = app.MapGroup("/dapper/students");

studentDapperGroup.MapPost("", async (Student student, IStudentRepository studentRepository) =>
{
    await studentRepository.DapperCreateAsync(student);

    return TypedResults.Ok();
});

studentDapperGroup.MapGet("/{studentId}", async Task<Results<Ok<Student>, BadRequest<string>>> (int studentId, IStudentRepository studentRepository) =>
{
    try
    {
        var result = await studentRepository.DapperGetAsync(studentId);

        return TypedResults.Ok(result);
    }
    catch (Exception ex)
    {
        return TypedResults.BadRequest(ex.Message);
    }
});

studentDapperGroup.MapPut("", async Task<Results<Ok, BadRequest<string>>> (Student student, IStudentRepository studentRepository) =>
{
    try
    {
        await studentRepository.DapperUpdateAsync(student);

        return TypedResults.Ok();
    }
    catch (Exception ex)
    {
        return TypedResults.BadRequest(ex.Message);
    }
});

studentDapperGroup.MapDelete("/{studentId}", async Task<Results<Ok, BadRequest<string>>> (int studentId, IStudentRepository studentRepository) =>
{
    try
    {
        await studentRepository.DapperDeleteAsync(studentId);

        return TypedResults.Ok();
    }
    catch (Exception ex)
    {
        return TypedResults.BadRequest(ex.Message);
    }
});

studentDapperGroup.MapGet("/", async (IStudentRepository studentRepository) =>
{
    var result = await studentRepository.DapperListAsync();
    return TypedResults.Ok(result);
});
#endregion


app.UseHttpsRedirection();

app.Run();
