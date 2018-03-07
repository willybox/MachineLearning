using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Allow managed code to call unmanaged functions that are implemented in a DLL
using System.Runtime.InteropServices;

namespace CSharpConsoleTester
{
    class Program
    {
        [DllImport("D:\\Projets\\MachineLearning\\Algorithms\\Debug\\Algorithms.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ref double linear_create(ref double X, ref double Y, int nbPoints);
        static void Main(string[] args)
        {
            Console.Write("Hi");
            double[] res = linear_create(new double[] { 1, 2, 5, 6, 7, 8 }, new double[] { 1, -1, 1 }, 3);
            Console.Write(res);
            Console.ReadKey();
        }
    }
}
