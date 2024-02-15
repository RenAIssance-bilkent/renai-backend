var builder = WebApplication.CreateBuilder(args);

//configs
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB")); // settings written in appsettings.json
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JsonWebToken"));


builder.Services.AddSingleton<MongoDBService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
