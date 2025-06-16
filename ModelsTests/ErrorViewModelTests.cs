using NUnit.Framework;
using TrainingPlansApi.Models;

namespace TrainingPlansApi.Tests.ModelsTests
{
    [TestFixture]
    public class ErrorViewModelTests
    {
        // Poprawne działanie logiki !string.IsNullOrEmpty(...) (zwracanie true, jeśli coś jest wpisane.)
        [Test]
        public void ShowRequestId_ShouldBeTrue_WhenRequestIdIsNotNullOrEmpty()
        {
            var model = new ErrorViewModel { RequestId = "abc123" };
            Assert.That(model.ShowRequestId, Is.True);
        }

        // Ochrona przed błędami w logice warunkowej, np. gdyby null został błędnie uznany za poprawny identyfikator.
        [Test]
        public void ShowRequestId_ShouldBeFalse_WhenRequestIdIsNull()
        {
            var model = new ErrorViewModel { RequestId = null };
            Assert.That(model.ShowRequestId, Is.False);
        }

        // Zachowanie spójności – pusty string nie powinien być traktowany jako poprawny identyfikator.
        [Test]
        public void ShowRequestId_ShouldBeFalse_WhenRequestIdIsEmpty()
        {
            var model = new ErrorViewModel { RequestId = "" };
            Assert.That(model.ShowRequestId, Is.False);
        }
    }
}
