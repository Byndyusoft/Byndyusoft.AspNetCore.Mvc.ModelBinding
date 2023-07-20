using Byndyusoft.ExampleProject.Interfaces;

namespace Byndyusoft.ExampleProject
{
    public class ExampleClass : IExampleClass
    {
        public int ExampleAddMethod(int a, int b)
        {
            return a + b;
        }
    }
}