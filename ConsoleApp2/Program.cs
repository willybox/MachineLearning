using System;
using System.Runtime.InteropServices;

namespace ConsoleApp2
{
    class Program
    {
        [DllImport("C:\\Users\\Jo\\Desktop\\Cours\\5A\\MachineLearning\\Debug\\Algorithms.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr linear_create(double[] X, double[] Y, int nbPoints);
        [DllImport("C:\\Users\\Jo\\Desktop\\Cours\\5A\\MachineLearning\\Debug\\Algorithms.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void linear_train_classification(double[] ptr);
        static void Main(string[] args)
        {
            int nbPoints = 3;
            int totalSize = 4 + 3 * nbPoints;

            IntPtr a = linear_create(new double[] { 1, 2, 5, 6, 7, 8 }, new double[] { 1, -1, 1 }, nbPoints);

            double[] res = new double[totalSize];
            Marshal.Copy(a, res, 0, totalSize);
            Console.WriteLine(res[1]);
            Console.WriteLine(res[2]);
            Console.WriteLine(res[3]);

            Console.WriteLine("Linear classification");
            linear_train_classification(res);

            Console.WriteLine(res[1]);
            Console.WriteLine(res[2]);
            Console.WriteLine(res[3]);
            Console.ReadKey();
        }

    }

}