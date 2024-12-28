var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication("token")
    .AddJwtBearer("token", options =>
    {
        // Configure the Authority to the expected value for
        // the authentication provider. This ensures the token
        // is appropriately validated.
        options.Authority = "https://localhost:5001";

        options.TokenValidationParameters.ValidateAudience = false;
    });
builder.Services.AddAuthorization();

// Add policy for CORS
const string allow_all = "allow_all";
builder.Services.AddCors(options =>
        options.AddPolicy(name: allow_all,
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
        )
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(allow_all);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization();

app.Run();