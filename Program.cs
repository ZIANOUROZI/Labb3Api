
using Labb3Api.Data;
using Labb3Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Labb3Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add services to the container.
            builder.Services.AddAuthorization();

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



            /////////////////////////////////////////////////////////////////////////
            /////////////      Skapa en ny person       ////////////////////////////

            app.MapPost("/persons", async (Person person, ApplicationDbContext context) =>
            {
                context.Persons.Add(person);
                await context.SaveChangesAsync();
                return Results.Created($"/persons/{person.PersonId}", person);
            });
            ////////////////////////////////////////////////////////////////////////////
            /////////////     H�mta alla personer ///////////////////////////////////

            app.MapGet("/persons", async (ApplicationDbContext context) =>
            {
                var persons = await context.Persons
                 .Include(h => h.Hobbies)
                 .ThenInclude(l => l.Links).ToListAsync();
                if (persons == null || !persons.Any())
                {
                    return Results.NotFound("Hittade ingen person");
                }
                return Results.Ok(persons);
            });

            ///////////////////////////////////////////////////////////////////////////////////////
            ////////////////////    H�mta alla intressen f�r en specifik person ////////////////////

            app.MapGet("/persons/{personId}/hobbies", async (int personId, ApplicationDbContext context) =>
            {
                var person = await context.Persons
                .Include(h => h.Hobbies)
                .FirstOrDefaultAsync(p => p.PersonId == personId);
                if (person == null)
                {
                    return Results.NotFound("Personen hittades inte");
                }
                var hobbeis = person.Hobbies;
                return Results.Ok(hobbeis);
            });


            /////////////////////////////////////////////////////////////////////////////////
            /////////////// H�mta alla l�nkar f�r en specifik person    ////////////////////

            app.MapGet("/person/{personId}/links", async (int personId, ApplicationDbContext context) =>
            {
                var person = await context.Persons
                .Include(h => h.Hobbies)
                .ThenInclude(l => l.Links)
                .FirstOrDefaultAsync(p => p.PersonId == personId);
                if(person==null)
                {
                    return Results.NotFound("Personen hittades inte");
                }
                var links = person.Hobbies.SelectMany(l => l.Links);
                return Results.Ok(links);
            });

            /////////////////////////////////////////////////////////////////////////////////////
            ///////    Koppla en person till ett nytt intresse         //////////////////////////
            app.MapPost("/persons/{personId}/hobbies", async (int personId, Hobby hobby, ApplicationDbContext context) =>
            {
                var person = await context.Persons.FindAsync(personId);

                if (person == null)
                {
                    return Results.NotFound($"Personen med ID {personId} hittades inte.");
                }

                // Kontrollera om personens hobbies-lista �r null och skapa en ny lista om det beh�vs
                if (person.Hobbies == null)
                {
                    person.Hobbies = new List<Hobby>();
                }

                // Kontrollera om intresset redan existerar i databasen
                var existingHobby = await context.Hobbys.FirstOrDefaultAsync(h => h.Title == hobby.Title);

                if (existingHobby == null)
                {
                    // Om intresset inte finns, l�gg till det i databasen
                    existingHobby = new Hobby { Title = hobby.Title, Description = hobby.Description };
                    await context.Hobbys.AddAsync(existingHobby);
                }

                // L�gg till det befintliga intresset i personens hobbies-lista
                person.Hobbies.Add(existingHobby);

                await context.SaveChangesAsync();
                return Results.Created($"/persons/{personId}/hobbies", person.Hobbies);
            });



            ///////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////   Endpoint f�r att l�gga till en l�nk f�r en hobby    //////////////////////

            app.MapPost("/hobbies/{hobbyId}/links", async (int hobbyId, Link link, ApplicationDbContext context) =>
            {
                // Hitta hobbyn i databasen
                var hobby = await context.Hobbys.FindAsync(hobbyId);

                if (hobby == null)
                {
                    return Results.NotFound($"Hobbyn med ID {hobbyId} hittades inte.");
                }
                // Kontrollera om hobbyn redan har l�nkar
                if (hobby.Links == null)
                {
                    hobby.Links = new List<Link>();
                }

                // Skapa en ny l�nk med den angivna informationen
                var newLink = new Link { Url = link.Url };

                // L�gg till den nya l�nken i hobbyns lista �ver l�nkar
                hobby.Links.Add(newLink);

                // Spara �ndringarna i databasen
                await context.SaveChangesAsync();

                return Results.Created($"/hobbies/{hobbyId}/links", newLink);

            });
            app.Run();
        }
    }
}
