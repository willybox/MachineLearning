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
        public static extern IntPtr linear_create(double[] X, double[] Y, int nbPoints);
        [DllImport("D:\\Projets\\MachineLearning\\Algorithms\\Debug\\Algorithms.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void linear_train_classification(double[] ptr);
        static void Main(string[] args)
        {
            int nbPoints = 3;
            int totalSize = 4 + 3 * nbPoints;

            //IntPtr pntX = Marshal.AllocHGlobal(nbPoints * 2);
            //Marshal.Copy(new double[] { 1, 2, 5, 6, 7, 8 }, 0, pntX, nbPoints * 2);
            //Console.WriteLine("pnt X: " + pntX);

            //IntPtr pntY = Marshal.AllocHGlobal(nbPoints * 2);
            //Marshal.Copy(new double[] { 1, -1, 1 }, 0, pntY, nbPoints);
            //Console.WriteLine("pnt Y: " + pntY);
            IntPtr a = linear_create(new double[] { 1, 2, 5, 6, 7, 8 }, new double[] { 1, -1, 1 }, nbPoints);
            //double[] pntRes = new double[4 + nbPoints * 3];
            //pntRes = linear_create(new double[] { 1, 2, 5, 6, 7, 8 }, new double[] { 1, -1, 1 }, nbPoints);
            //Console.WriteLine(pntRes);

            //double[] res = new double[4 + 3 * nbPoints];
            //Marshal.Copy(pntRes, res, 0, 4 + 3 * nbPoints);
            //Console.Write(res);
            /*
            Marshal.FreeHGlobal(pntX);
            Marshal.FreeHGlobal(pntY);
            Marshal.FreeHGlobal(pntRes);
            */
            Console.WriteLine(a);
            Console.WriteLine("Freedom");

            double[] res = new double[totalSize];
            Marshal.Copy(a, res, 0, totalSize);
            Console.WriteLine(res[1]);
            Console.WriteLine(res[2]);
            Console.WriteLine(res[3]);

            //IntPtr ptnRes = new IntPtr(0);
            //Marshal.Copy(res, 0, ptnRes, totalSize);

            linear_train_classification(res);
            Console.WriteLine(res[1]);
            Console.WriteLine(res[2]);
            Console.WriteLine(res[3]);
            Console.ReadKey();
        }
    }
}
