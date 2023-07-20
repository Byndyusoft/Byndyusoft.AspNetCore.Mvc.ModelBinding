using AutoFixture;
using Byndyusoft.ExampleProject;
using NUnit.Framework;

namespace Byndyusoft.ExampleTestsProject
{
    [TestFixture]
    public partial class ExampleClassTests
    {
        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();

            _sut = new ExampleClass();
        }

        private Fixture _fixture = null!;
        private ExampleClass _sut = null!;

        [TestCaseSource(typeof(TestSourceClass), nameof(TestSourceClass.TestSources))]
        public void Test_ExampleAddMethod(TestSource data)
        {
            // Act
            var result = _sut.ExampleAddMethod(data.A, data.B);

            // Assert
            Assert.That(result, Is.EqualTo(data.ExpectedResult));
        }

        [Test]
        public void Test_ExampleAddMethod_WithRandomInput()
        {
            // Arrange
            var a = _fixture.Create<int>();
            var b = _fixture.Create<int>();
            var expectedResult = a + b;

            // Act
            var result = _sut.ExampleAddMethod(a, b);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }

    public partial class ExampleClassTests //test source
    {
        internal static class TestSourceClass
        {
            public static TestSource[] TestSources =
            {
                new(1, 1, 2),
                new(5, 6, 11),
                new(-10, 2, -8)
            };
        }

        public record TestSource(int A, int B, int ExpectedResult);
    }
}