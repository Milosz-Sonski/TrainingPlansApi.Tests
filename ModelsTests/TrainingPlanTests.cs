using NUnit.Framework;
using TrainingPlansApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class TrainingPlanModelTests
{
    private IList<ValidationResult> ValidateModel(TrainingPlan model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    // Upewnienie się, że walidacja modelu działa i wychwytuje braki danych.
    [Test]
    public void TrainingPlan_Should_FailValidation_WhenRequiredFieldsAreMissing()
    {
        var model = new TrainingPlan(); // wszystkie pola są domyślne/null

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Spodziewamy się, że isValid == false
        Assert.That(isValid, Is.False);
        Assert.That(results.Count, Is.GreaterThan(0));

        var failedFields = results.SelectMany(r => r.MemberNames).ToList();
        Assert.That(failedFields, Does.Contain("Name"));
        Assert.That(failedFields, Does.Contain("TrainingDays"));
        Assert.That(failedFields, Does.Contain("Exercises"));
        Assert.That(failedFields, Does.Contain("CreatedBy"));
        Assert.That(failedFields, Does.Contain("Description"));
    }

    // Upewnienie się, że poprawne dane nie powodują błędów walidacyjnych.
    [Test]
    public void TrainingPlan_Should_PassValidation_WhenAllFieldsAreProvided()
    {
        var model = new TrainingPlan
        {
            Name = "Plan A",
            TrainingDays = "Monday",
            Exercises = "Push-ups",
            CreatedBy = "Admin",
            Description = "Test plan",
            CreatedAt = DateTime.UtcNow
        };

        var results = ValidateModel(model);

        Assert.That(results.Count, Is.EqualTo(0)); // Brak błędów walidacji
    }
}
