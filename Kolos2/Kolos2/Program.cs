using Kolos2.Models;
using Kolos2.Repos;
using Kolos2.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();
        builder.Services.AddScoped<IChampionshipRepo, ChampionshipRepo>();
        builder.Services.AddScoped<IChampionShipService, ChampionShipService>();


        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();
        
        app.Run();
    }
}