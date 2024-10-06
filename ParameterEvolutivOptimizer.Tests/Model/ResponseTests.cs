using NUnit.Framework;

namespace ParameterEvolutivOptimizer.Model.UnitTests
{
    [TestFixture]
    public class ResponseTests
    {
        [Test]
        public void IsBetterThan_OtherResponseBiggerAndIsToMaximize_ReturnsFalse()
        {
            var baseResponse = new Response("test", 0, true, 1);
            var newResponse = new Response("test", 1, true, 1);

            var result = baseResponse.IsBetterThan(newResponse);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsBetterThan_OtherResponseBiggerAndIsToMaximize_ReturnsTrue()
        {
            var baseResponse = new Response("test", 1, true, 1);
            var newResponse = new Response("test", 0, true, 1);

            var result = baseResponse.IsBetterThan(newResponse);

            Assert.That(result);
        }
    }
}
