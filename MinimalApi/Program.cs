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

#region Car Races endpoints
//get car races
app.MapGet("api/carraces",
    (RaceDb db) =>
    {
        var carRaces = db.CarRaces.Include(x => x.Cars).ToList();
        return Results.Ok(carRaces);
    })
    .WithName("GetCarRaces")
    .WithTags("Car races");



//get car race
app.MapGet("api/carraces/{id}",
    (int id, RaceDb db) =>
    {
            var carRace = db
                .CarRaces
                .Include(x => x.Cars)
                .FirstOrDefault(x => x.Id == id);

            if (carRace == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(carRace);
    })
    .WithName("GetCarRace")
    .WithTags("Car races");


//create car race
app.MapPost("api/carraces/cars",
    (CarRaceCreateModel carRaceModel, RaceDb db) =>
    {
        var newCarRace = new CarRace
        {
            Name = carRaceModel.Name,
            Location = carRaceModel.Location,
            Distance = carRaceModel.Distance,
            TimeLimit = carRaceModel.TimeLimit,
            Status = "Created"
        };
        db.CarRaces.Add(newCarRace);
        db.SaveChanges();
        return Results.Ok(newCarRace);
    }).WithName("CreateCarRace")
    .WithTags("Car races");


//update car race
app.MapPut("api/carraces/{id}",
    ([FromQuery] int id, [FromBody] CarRaceCreateModel carRaceModel, RaceDb db) =>
    {
        var dbCarRace = db
                .CarRaces
                .Include(x => x.Cars)
                .FirstOrDefault(x => x.Id == id);

        if (dbCarRace == null)
        {
            return Results.NotFound($"CarRace with id: {id} isn't found.");
        }

        dbCarRace.Location = carRaceModel.Location;
        dbCarRace.Name = carRaceModel.Name;
        dbCarRace.TimeLimit = carRaceModel.TimeLimit;
        dbCarRace.Distance = carRaceModel.Distance;
        db.SaveChanges();

        return Results.Ok(dbCarRace);
    })
    .WithName("UpdateCarRace")
    .WithTags("Car races");

//delete car
app.MapDelete("api/carraces/{id}",
    (int id, RaceDb db) =>
    {
        var dbCarRace = db
                 .CarRaces
                 .Include(x => x.Cars)
                 .FirstOrDefault(dbCarRace => dbCarRace.Id == id);

        if (dbCarRace == null)
        {
            return Results.NotFound($"CarRace with id: {id} isn't found.");
        }

        db.Remove(dbCarRace);
        db.SaveChanges();

        return Results.Ok($"CarRace with id: {id} was successfuly deleted");
    })
    .WithName("DeleteCarRace")
    .WithTags("Car races");

//add car to car race
app.MapPut("{carRaceId}/addcar/{carId}",
    (int carRaceId, int carId, RaceDb db) =>
    {
        var dbCarRace = db
                    .CarRaces
                    .Include(x => x.Cars)
                    .SingleOrDefault(x => x.Id == carRaceId);

        if (dbCarRace == null)
        {
            return Results.NotFound($"Car Race with id: {carRaceId} not found");
        }

        var dbCar = db.Cars.SingleOrDefault(x => x.Id == carId);

        if (dbCar == null)
        {
            return Results.NotFound($"Car with id: {carId} not found");
        }

        dbCarRace.Cars.Add(dbCar);
        db.SaveChanges();
        return Results.Ok(dbCarRace);

    })
    .WithName("AddCarToCarRace")
    .WithTags("Car races");

//Start car race
app.MapPut("{id}/start",
    (int id, RaceDb db) =>
    {
        var dbcarRace = db
                .CarRaces
                .Include(x => x.Cars)
                .FirstOrDefault(carRace => carRace.Id == id);

        if (dbcarRace == null)
        {
            return Results.NotFound($"Car Race with id: {id} not found");
        }

        dbcarRace.Status = "Started";
        //var finishedRace = CarRaceService.RunRace(carRace);
        db.SaveChanges();

        return Results.Ok(dbcarRace);
    })
    .WithName("StartCarRace")
    .WithTags("Car races");

//delete car from car race



#endregion

//Motor bike endpoints
#region Motorbike endpoints

//get motorbikes
app.MapGet("api/motorbikes", 
    (RaceDb db) =>
{
    var motorbikes = db.Motorbikes.ToList();
    return Results.Ok(motorbikes);
})
.WithName("GetMotorbikes")
.WithTags("Motorbikes");

//get motorbike
app.MapGet("api/motorbikes/{id}",
    (int id, RaceDb db) =>
    {
        /*
        var motorbike1 = new Motorbike
        {
            TeamName = "Team A"
        };
        return motorbike1;
        */
        var dbMotorbike= db.Motorbikes.FirstOrDefault(x => x.Id == id);
        if (dbMotorbike == null)
        {
            return Results.NotFound($" Motorbike with {id} not found");
        }

        return Results.Ok(dbMotorbike);

    })
    .WithName("GetMotorbike")
    .WithTags("Motorbikes");

//create motorbike
app.MapPost("api/motorbikes",
    (MotorbikeCreateModel motorbikeModel, RaceDb db) =>
    {
        var newMotorBike = new Motorbike
        {
            TeamName = motorbikeModel.TeamName,
            Speed = motorbikeModel.Speed,
            MelfunctionChance = motorbikeModel.MelfunctionChance
        };
        db.Motorbikes.Add(newMotorBike);
        db.SaveChanges();

        return Results.Ok(newMotorBike);
    })
    .WithName("CreateMotorbike")
    .WithTags("Motorbikes");

//update motorbike

app.MapPut("api/motorbikes/{id}",
    ([FromQuery] int id, [FromBody]MotorbikeCreateModel motorbikeModel, RaceDb db) =>
    {
        var dbMotorcycle = db.Cars.FirstOrDefault(x => x.Id == id);
        if (dbMotorcycle == null)
        {
            return Results.NotFound($" Motorcycle with {id} not found");
        }

        dbMotorcycle.TeamName = dbMotorcycle.TeamName;
        dbMotorcycle.Speed = dbMotorcycle.Speed;
        dbMotorcycle.MelfunctionsOccured = (int)dbMotorcycle.MelfunctionChance;
        db.SaveChanges();

        return Results.Ok(dbMotorcycle);
    })
    .WithName("UpdateMotorbike")
    .WithTags("Motorbikes");

//delete motorbike
app.MapDelete("api/motorbikes/{id}",
    (int id, RaceDb db) =>
    {
        var dbMotorbike = db.Motorbikes.FirstOrDefault(x => x.Id == id);
        if (dbMotorbike == null)
        {
            return Results.NotFound($" Car with {id} not found");
        }
        db.Remove(dbMotorbike);
        db.SaveChanges();
        return Results.Ok($"Motorbike with {id} has been deleted");
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

public record CarRace
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public int Distance { get; set; }
    public int TimeLimit { get; set; }
    public string Status { get; set; }
    public List<Car> Cars { get; set; } = new List<Car>();
}

public record CarRaceCreateModel
{
    public string Name { get; set; }
    public string Location { get; set; }
    public int Distance { get; set; }
    public int TimeLimit { get; set; }
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

public record MotorbikeCreateModel
{
    public string TeamName { get; set; }
    public int Speed { get; set; }
    public double MelfunctionChance { get; set; }
}



#endregion


//Persistance
public class RaceDb : DbContext
{
    public RaceDb(DbContextOptions<RaceDb> options) : base(options)
    {
    }

    public DbSet<Car> Cars { get; set; }

    public DbSet<CarRace> CarRaces { get; set; }

    public DbSet<Motorbike> Motorbikes { get; set; }
    
}
