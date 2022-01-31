using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
//Add the Entity Framework Core DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<RaceDb>(options =>
options.UseSqlServer(connectionString));


// Add services to the container.
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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};



#region Cars endpoints
//get cars
app.MapGet("api/cars", 
    (RaceDb db) =>
{
    var cars = db.Cars.ToList();
    return Results.Ok(cars);
})
    .WithName("GetCars")
    .WithTags("Cars");

//get car
app.MapGet("api/cars/{id}",
    (int id, RaceDb db) =>
    {
        var dbCar = db.Cars.FirstOrDefault(x => x.Id == id);
        if (dbCar == null)
        {
            return Results.NotFound($" Car with {id} not found");
        }

        return Results.Ok(dbCar);

    })
    .WithName("GetCar")
    .WithTags("Cars");

//create car
app.MapPost("api/cars",
    (CarCreateModel carModel,RaceDb db) =>
    {
        var newCar = new Car
        {
            TeamName = carModel.TeamName,
            Speed = carModel.Speed,
            MelfunctionChance = carModel.MelfunctionChance
        };

        db.Cars.Add(newCar);
        db.SaveChanges();

        return Results.Ok(newCar);
    }).WithName("CreateCar")
    .WithTags("Cars");

//update car
app.MapPut("api/cars/{id}",
    ([FromQuery] int id, [FromBody]CarCreateModel carModel, RaceDb db) =>
    {
        var dbCar = db.Cars.FirstOrDefault(x => x.Id == id);
        if (dbCar == null)
        {
            return Results.NotFound($" Car with {id} not found");
        }

        dbCar.TeamName = carModel.TeamName;
        dbCar.Speed = carModel.Speed;
        dbCar.MelfunctionsOccured = (int) carModel.MelfunctionChance;
        db.SaveChanges();

        return Results.Ok(dbCar);
    })
    .WithName("UpdateCar")
    .WithTags("Cars");

//delete car
app.MapDelete("api/cars/{id}",
    (int id, RaceDb db) =>
    {
        var dbCar = db.Cars.FirstOrDefault(x => x.Id == id);
        if (dbCar == null)
        {
            return Results.NotFound($" Car with {id} not found");
        }
        db.Remove(dbCar);
        db.SaveChanges();
        return Results.Ok($"Car with {id} has been deleted");
    })
    .WithName("DeleteCar")
    .WithTags("Cars");
#endregion

//Motor bike endpoints
#region Motorbike endpoints
app.MapGet("api/motorbikes", () =>
{
    var motorbike1 = new Motorbike
    {
        TeamName = "Team A"
    };

    var motorbike2 = new Motorbike
    {
        TeamName = "Team B"
    };

    var motorbikes = new List<Motorbike>
    {
        motorbike1, motorbike2
    };
    return motorbikes;
})
.WithName("GetMotorbikes")
.WithTags("Motorbikes");

app.MapGet("api/motorbikes/{id}",
    (int id) =>
    {
        var motorbike1 = new Motorbike
        {
            TeamName = "Team A"
        };
        return motorbike1;

    })
    .WithName("GetMotorbike")
    .WithTags("Motorbikes");

app.MapPost("api/motorbikes",
    (Motorbike motorbike) =>
    {
        return motorbike;
    })
    .WithName("CreateMotorbike")
    .WithTags("Motorbikes");

app.MapPut("api/motorbikes/{id}",
    (Motorbike motorbike) =>
    {
        return motorbike;
    })
    .WithName("UpdateMotorbike")
    .WithTags("Motorbikes");

app.MapDelete("api/motorbikes/{id}",
    (int id) =>
    {
        return $"Motorbike with id: {id} was deleted";
    })
    .WithName("DeleteMotorbike")
    .WithTags("Motorbikes");
#endregion

#region defaultendpoints
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithTags("default");
#endregion
app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


#region Models
public record Car
{
    public int Id { get; set; }
    public string TeamName { get; set; }
    public int Speed { get; set; }
    public double MelfunctionChance { get; set; }
    public int MelfunctionsOccured { get; set; }
    public int DistanceCoverdInMiles { get; set; }
    public bool FinishedRace { get; set; }
    public int RacedForHours { get; set; }
}


public record CarCreateModel
{
    public string TeamName { get; set; }
    public int Speed { get; set; }
    public double MelfunctionChance { get; set; }
}
public record Motorbike
{
    public int Id { get; set; }
    public string TeamName { get; set; }
    public int Speed { get; set; }
    public double MelfunctionChance { get; set; }
    public int MelfunctionsOccured { get; set; }
    public int DistanceCoverdInMiles { get; set; }
    public bool FinishedRace { get; set; }
    public int RacedForHours { get; set; }
}
#endregion


//Persistance
public class RaceDb : DbContext
{
    public RaceDb(DbContextOptions<RaceDb> options) : base(options)
    {
    }

    public DbSet<Car> Cars { get; set; }

    public DbSet<Motorbike> Motorbikes { get; set; }
    
}
