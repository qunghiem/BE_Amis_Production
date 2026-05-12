using MISA.PRODUCTION.BL.Base;
using MISA.PRODUCTION.BL.Interfaces;
using MISA.PRODUCTION.BL.Services;
using MISA.PRODUCTION.DL.Base;
using MISA.PRODUCTION.DL.Interfaces;
using MISA.PRODUCTION.DL.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// DI ??ng ký
builder.Services.AddScoped(typeof(IBaseDL<>), typeof(BaseDL<>));
builder.Services.AddScoped(typeof(IBaseBL<>), typeof(BaseBL<>));
builder.Services.AddScoped<IShiftDL, ShiftDL>();
builder.Services.AddScoped<IShiftBL, ShiftBL>();

// Thêm CORS cho phép FE g?i
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// ??t UseCors TR??C MapControllers
app.UseCors("AllowAll");

app.UseHttpsRedirection();



app.UseAuthorization();



app.MapControllers();

app.Run();
