using FluentValidation;
using UserManagementAPI;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
//AddUSerValidator from Services
var users = new List<User>(); //  Create a global list of users
builder.Services.AddSingleton(users); //   Register the list of users as a singleton
builder.Services.AddScoped<IValidator<User>>(provider => new UserValidator(provider.GetRequiredService<List<User>>())); //  Passing users to the validator

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRequestLogging();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();
