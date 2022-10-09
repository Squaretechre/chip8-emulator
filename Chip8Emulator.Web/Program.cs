using Chip8Emulator;

var CorsPolicy = "cors-policy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsPolicy,
        policy  =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyMethod();
            policy.AllowAnyHeader();           
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDebugger, Debugger>();

builder.Services.AddSingleton(s => new Chip8(
    new int[16],
    512,
    () => new Random().Next(0, 100),
    s.GetService<IDebugger>() ?? new Debugger(),
    new Chip8.Thread()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors(CorsPolicy);

app.MapControllers();

app.Run();