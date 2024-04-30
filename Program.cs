
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
            /////////////     Hämta alla personer ///////////////////////////////////

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
            ////////////////////    Hämta alla intressen för en specifik person ////////////////////

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
            /////////////// Hämta alla länkar för en specifik person    ////////////////////

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

                // Kontrollera om personens hobbies-lista är null och skapa en ny lista om det behövs
                if (person.Hobbies == null)
                {
                    person.Hobbies = new List<Hobby>();
                }

                // Kontrollera om intresset redan existerar i databasen
                var existingHobby = await context.Hobbys.FirstOrDefaultAsync(h => h.Title == hobby.Title);

                if (existingHobby == null)
                {
                    // Om intresset inte finns, lägg till det i databasen
                    existingHobby = new Hobby { Title = hobby.Title, Description = hobby.Description };
                    await context.Hobbys.AddAsync(existingHobby);
                }

                // Lägg till det befintliga intresset i personens hobbies-lista
                person.Hobbies.Add(existingHobby);

                await context.SaveChangesAsync();
                return Results.Created($"/persons/{personId}/hobbies", person.Hobbies);
            });



            ///////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////   Endpoint för att lägga till en länk för en hobby    //////////////////////

            app.MapPost("/hobbies/{hobbyId}/links", async (int hobbyId, Link link, ApplicationDbContext context) =>
            {
                // Hitta hobbyn i databasen
                var hobby = await context.Hobbys.FindAsync(hobbyId);

                if (hobby == null)
                {
                    return Results.NotFound($"Hobbyn med ID {hobbyId} hittades inte.");
                }
                // Kontrollera om hobbyn redan har länkar
                if (hobby.Links == null)
                {
                    hobby.Links = new List<Link>();
                }

                // Skapa en ny länk med den angivna informationen
                var newLink = new Link { Url = link.Url };

                // Lägg till den nya länken i hobbyns lista över länkar
                hobby.Links.Add(newLink);

                // Spara ändringarna i databasen
                await context.SaveChangesAsync();

                return Results.Created($"/hobbies/{hobbyId}/links", newLink);

            });
            app.Run();
        }
    }
}
