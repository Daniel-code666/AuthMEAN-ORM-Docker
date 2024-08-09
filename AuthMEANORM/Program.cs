using AuthMEANORM.Context;
using AuthMEANORM.Models.UsersModel;
using AuthMEANORM.Repository.ImplementClass;
using AuthMEANORM.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connStr = builder.Configuration.GetConnectionString("MongoDb");

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseMongoDB(connStr!, "Users");
});

builder.Services.AddCors(p => p.AddPolicy("cors", build => {
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUsersRepo, UsersRepo>();

builder.Services.AddControllersWithViews();

builder.Services.AddControllers().AddNewtonsoftJson().ConfigureApiBehaviorOptions(o => {
    o.SuppressInferBindingSourcesForParameters = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseCors("cors");

app.UseAuthorization();

app.MapControllers();

app.Run();
