using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingPlansApi.Controllers;
using TrainingPlansApi.Models;
using TrainingPlansApi.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TrainingPlansApi.Tests.ControllersTests
{
    public class TrainingPlansControllerTests
    {
        private async Task<TrainingPlansContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<TrainingPlansContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // oddzielna baza dla każdego testu
                .Options;

            var context = new TrainingPlansContext(options);

            context.TrainingPlans.AddRange(
                new TrainingPlan
                {
                    Id = 1,
                    Name = "Plan A",
                    Description = "Test A",
                    TrainingDays = "Monday, Wednesday, Friday",
                    Exercises = "Push-ups, Squats",
                    CreatedBy = "Admin",
                    CreatedAt = DateTime.UtcNow
                },
                new TrainingPlan
                {
                    Id = 2,
                    Name = "Plan B",
                    Description = "Test B",
                    TrainingDays = "Tuesday, Thursday",
                    Exercises = "Running, Pull-ups",
                    CreatedBy = "Admin",
                    CreatedAt = DateTime.UtcNow
                });

            await context.SaveChangesAsync();
            return context;
        }

        // Zapewnianie, że GetDbContext() działa poprawnie i umożliwia dalsze testowanie z danymi startowymi.
        [Test]
        public async Task GetDbContext_ShouldAddTrainingPlans_WhenDatabaseIsEmpty()
        {
            var context = await GetDbContext();
            var plans = await context.TrainingPlans.ToListAsync();

            Assert.That(plans.Count, Is.EqualTo(2));
            Assert.That(plans.Any(p => p.Name == "Plan A"));
            Assert.That(plans.Any(p => p.Name == "Plan B"));
        }

        // Walidacja podstawowych działań metody GET /api/trainingplans/{id}.
        [Test]
        public async Task GetTrainingPlan_ShouldReturnTrainingPlan_WhenIdExists()
        {
            var context = await GetDbContext();
            var controller = new TrainingPlansController(context);

            var result = await controller.GetTrainingPlan(1);

            // result is ActionResult<TrainingPlan>, so we use .Value
            var value = result.Value;

            Assert.That(value, Is.Not.Null);
            Assert.That(value.Id, Is.EqualTo(1));
            Assert.That(value.Name, Is.EqualTo("Plan A"));
        }

        // Test reakcji kontrolera na błędny id
        [Test]
        public async Task GetTrainingPlan_ReturnsNotFound_WhenIdDoesNotExist()
        {
            var context = await GetDbContext();
            var controller = new TrainingPlansController(context);

            var result = await controller.GetTrainingPlan(999); // ID nie istnieje

            // W przypadku ActionResult<T> używamy .Result
            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null, "Expected NotFoundObjectResult, but was null.");

            var value = notFoundResult.Value?.GetType()
                .GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(notFoundResult.Value));

            Assert.That(value, Is.Not.Null);
            Assert.That(value!.ContainsKey("Code"));
            Assert.That(value["Code"], Is.EqualTo(404));
            Assert.That(value["Message"], Is.EqualTo("Training plan not found"));
        }

        // Upewnianie się, że kasowanie działa i wpływa na stan bazy.
        [Test]
        public async Task DeleteTrainingPlan_ShouldRemovePlan_WhenIdExists()
        {
            var context = await GetDbContext();
            var controller = new TrainingPlansController(context);

            var deleteResult = await controller.DeleteTrainingPlan(1);
            Assert.That(deleteResult, Is.InstanceOf<NoContentResult>());

            var result = await controller.GetTrainingPlan(1);
            var notFound = result.Result as NotFoundObjectResult;
            Assert.That(notFound, Is.Not.Null);
        }

        // Obsługa błędów przy DELETE
        [Test]
        public async Task DeleteTrainingPlan_ReturnsNotFound_WhenIdMissing()
        {
            var context = await GetDbContext();
            var controller = new TrainingPlansController(context);

            var result = await controller.DeleteTrainingPlan(999); // Nieistniejący ID

            Assert.That(result, Is.Not.Null);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404).Or.EqualTo(null)); // czasem null == 404

            var value = notFoundResult.Value?.GetType()
                .GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(notFoundResult.Value));

            Assert.That(value, Is.Not.Null);
            Assert.That(value!.ContainsKey("Code"));
            Assert.That(value["Code"], Is.EqualTo(404));
            Assert.That(value["Message"], Is.EqualTo("Training plan not found"));
        }

        // Zapobieganie niejednoznaczności danych – standardowa kontrola spójności.
        [Test]
        public async Task PutTrainingPlan_ReturnsBadRequest_WhenIdMismatch()
        {
            var context = await GetDbContext();
            var controller = new TrainingPlansController(context);

            var trainingPlan = new TrainingPlan
            {
                Id = 2, // mismatch
                Name = "Mismatch Plan",
                Description = "Bad update",
                TrainingDays = "Monday",
                Exercises = "Push-ups",
                CreatedBy = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            var result = await controller.PutTrainingPlan(1, trainingPlan); // ID != plan.Id

            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);

            // konwersja obiektu anonimowego do słownika
            var value = badRequest.Value?.GetType()
                .GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(badRequest.Value));

            Assert.That(value, Is.Not.Null);
            Assert.That(value!["Code"], Is.EqualTo(400));
            Assert.That(value["Message"], Is.EqualTo("ID mismatch"));
        }

        // Walidacja danych wejściowych.
        [Test]
        public async Task PostTrainingPlan_ReturnsBadRequest_WhenInvalidModel()
        {
            var context = await GetDbContext();
            var controller = new TrainingPlansController(context);
            controller.ModelState.AddModelError("Name", "Required");

            var result = await controller.PostTrainingPlan(new TrainingPlan());

            var badRequest = result.Result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);

            var value = badRequest.Value?.GetType()
                .GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(badRequest.Value));

            Assert.That(value, Is.Not.Null);
            Assert.That(value!["Code"], Is.EqualTo(400));
            Assert.That(value["Message"], Is.EqualTo("Invalid model"));
        }
    }
}
