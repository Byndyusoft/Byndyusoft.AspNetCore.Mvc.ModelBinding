using Byndyusoft.ExampleProject.Interfaces;

namespace Byndyusoft.ExampleProject.SecondPackage
{
    public class SecondExampleClass : IExampleClass
    {
        public int ExampleAddMethod(int a, int b)
        {
            return a - b;
        }
    }
}
