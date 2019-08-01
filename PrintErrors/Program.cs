using System;
using System.Threading;

namespace PrintErrors
{
    internal class Program
    {
        public static int Main()
        {
            Console.Error.WriteLine("One");
            Console.Error.WriteLine("Two");
            Thread.Sleep(1000);
            Console.Error.WriteLine("Three");
            return 7;
        }
    }
}