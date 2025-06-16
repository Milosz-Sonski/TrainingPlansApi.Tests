using Moq;
using NUnit.Framework;
using TrainingPlansApi.Controllers;
using TrainingPlansApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingPlansApi.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TrainingPlansApi.Tests.ControllersTests
{
    [TestFixture]
    public class PlansControllerTests
    {
        private TrainingPlansContext _context;
        private PlansController _controller;

        [SetUp]
        public void Setup()
        {
            // Przygotowanie danych do symulacji w prawdziwej bazie danych w pami�ci
            var options = new DbContextOptionsBuilder<TrainingPlansContext>()
                .UseInMemoryDatabase("TestDatabase") // Baza danych w pami�ci
                .Options;

            _context = new TrainingPlansContext(options);

            // Inicjalizacja kontrolera z kontekstem bazy danych
            _controller = new PlansController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            // Zwalnianie zasob�w po ka�dym te�cie
            _controller?.Dispose(); // Zwalniamy zasoby kontrolera
            _context?.Dispose();// Zwalniamy zasoby kontekstu bazy danych
        }

        // Zapewnianie, �e strona do tworzenia planu treningowego jest dost�pna i prawid�owo skonfigurowana w kontrolerze.
        [Test]
        public async Task Create_ReturnsViewResult()
        {
            // Act
            var result = await _controller.Create();  // U�ywamy await, bo metoda jest asynchroniczna

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        // Testowanie, czy dane s� poprawnie pobierane z bazy i przekazywane do widoku.
        [Test]
        public async Task Community_ReturnsListOfPlans()
        {
            // Czyszczenie bazy przed dodaniem danych
            _context.TrainingPlans.RemoveRange(_context.TrainingPlans);
            await _context.SaveChangesAsync();

            // Przygotowanie danych w metodzie testowej z unikalnymi Id
            var data = new List<TrainingPlan>
            {
                new TrainingPlan { Id = 1001, Name = "Plan 1", Description = "Opis 1", CreatedBy = "Milosz", Exercises = "Exercise 1, Exercise 2", TrainingDays = "Day 1, Day 2" },
                new TrainingPlan { Id = 1002, Name = "Plan 2", Description = "Opis 2", CreatedBy = "Milosz", Exercises = "Exercise 3, Exercise 4", TrainingDays = "Day 3, Day 4" },
                new TrainingPlan { Id = 1003, Name = "Plan 3", Description = "Opis 3", CreatedBy = "Milosz", Exercises = "Exercise 5, Exercise 6", TrainingDays = "Day 5, Day 6" }
            };
            _context.TrainingPlans.AddRange(data);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Community();

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as List<TrainingPlan>;
            Assert.IsNotNull(model);
            Assert.AreEqual(3, model.Count);  // Sprawdzamy, czy s� 3 plany
        }

        // Pokazywanie, �e aplikacja poprawnie reaguje na pr�b� edycji nieistniej�cego planu � to test odporno�ci.
        [Test]
        public async Task Edit_ReturnsNotFound_WhenPlanDoesNotExist()
        {
            int invalidId = 999;  // U�ywamy ID, kt�rego nie ma w bazie
            var result = await _controller.Edit(invalidId);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        // Zapewnianie, �e edycja danych dzia�a poprawnie i nast�puje przekierowanie po sukcesie.
        [Test]
        public async Task UpdatePlan_UpdatesExistingPlan()
        {
            var planToUpdate = new TrainingPlan
            {
                Id = 1001,  // Unikalne ID
                Name = "Updated Plan",
                Description = "Updated Description",
                CreatedBy = "Milosz",
                Exercises = "Updated Exercise",
                TrainingDays = "Updated Day"
            };

            var result = await _controller.UpdatePlan(planToUpdate);

            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Community", redirectToActionResult.ActionName);
        }

        /*
        // Symulacja walidacji modelu � u�ytkownik przesy�a b��dne dane, a aplikacja powinna zwr�ci� widok z b��dami, nie zapisuj�c danych.
        [Test]
        public async Task AddTrainingPlan_ReturnsViewResult_WhenModelInvalid()
        {
            // Przygotowanie planu z brakuj�cymi obowi�zkowymi danymi
            var plan = new TrainingPlan
            {
                // Brakuj�ce obowi�zkowe dane, np. brak "Exercises" lub "TrainingDays"
                Id = 0,  // Auto-increment w bazie danych w pami�ci nada unikalne ID
                Name = "Invalid Plan",  // Plan ma nazw�, ale brak danych w innych polach
                CreatedBy = "Milosz",
                Description = "Test Plan"
            };

            // Dodajemy plan, ale nie zapisujemy go od razu, poniewa� model jest nieprawid�owy
            _context.TrainingPlans.Add(plan);

            // Warto�ci w modelu s� nieprawid�owe, wi�c nie powinni�my zapisa� go w bazie
            var result = await _controller.AddTrainingPlan(plan);

            // Assert
            var viewResult = result as ViewResult;  // Sprawdzamy, czy zwr�cono ViewResult
            Assert.IsNotNull(viewResult); // Upewniamy si�, �e wynik to ViewResult
            Assert.AreEqual("Create", viewResult?.ViewName);  // Sprawdzamy, czy metoda zwr�ci�a odpowiedni widok "Create"

            // Sprawdzamy, czy model nie zosta� zapisany w bazie (nie powinno by� go w bazie)
            var planInDb = await _context.TrainingPlans.FindAsync(plan.Id);
            Assert.IsNull(planInDb);  // Powinno zwr�ci� null, poniewa� model jest nieprawid�owy i nie zosta� zapisany
        }
     */
    }
}
