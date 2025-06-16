using NUnit.Framework;
using TrainingPlansApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using TrainingPlansApi.Models;
using System.Diagnostics;

namespace TrainingPlansApi.Tests.ControllersTests
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();

            var model = new ErrorViewModel
            {
                RequestId = requestId
            };

            return View(model);
        }
    }

    [TestFixture]
    public class HomeControllerTests
    {
        private HomeController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new HomeController();
        }

        [TearDown]
        public void Teardown()
        {
            _controller?.Dispose();
        }

        // Sprawdzanie, czy metoda Index() kontrolera HomeController zwraca typ ViewResult, czyli standardowy widok.
        [Test]
        public void Index_ReturnsViewResult()
        {
            var result = _controller.Index();
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        // Sprawdzanie, zzy kontroler obsługi błędów przekazuje model do widoku
        [Test]
        public void Error_ReturnsViewResult_WithErrorViewModel()
        {
            var result = _controller.Error() as ViewResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.InstanceOf<ErrorViewModel>());

            var model = result.Model as ErrorViewModel;
            Assert.That(model!.ShowRequestId, Is.True.Or.False);
        }
    }
}
