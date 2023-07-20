using Byndyusoft.ExampleProject.Interfaces;

namespace Byndyusoft.NotPackableProject
{
    public class NotPackableExampleClass : IExampleClass
    {
        public int ExampleAddMethod(int a, int b)
        {
            return a * b;
        }
    }
}
