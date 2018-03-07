
using System;
using System.Runtime.InteropServices;

public static class CPPTOUnityLibWrapper
{

    [DllImport("C:\\Users\\Samuel BIJOU\\Desktop\\ESGI\\Matières\\Machine Learning\\MachineLearning\\x64\\Release\\Algorithms.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr linear_create(double[] X, double[] Y, int nbPoints);

    [DllImport("C:\\Users\\Samuel BIJOU\\Desktop\\ESGI\\Matières\\Machine Learning\\MachineLearning\\x64\\Release\\Algorithms.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void linear_train_classification(double[] datas);

    /*
    [DllImport("../x64/Release/Algorithms")]
    public static extern System.IntPtr linear_remove(System.IntPtr valueToRemove);
    */


    /*
    [DllImport("../x64/Release/Algorithms")]
    public static extern void linear_train_classification(System.IntPtr, ???);

    [DllImport("../x64/Release/Algorithms")]
    public static extern void linear_train_regression(System.IntPtr, ???);
    */


    /*
    [DllImport("../x64/Release/Algorithms")]
    public static extern ??? linear_classify(System.IntPtr, ???);

    [DllImport("../x64/Release/Algorithms")]
    public static extern ??? linear_predict(System.IntPtr, ???);
    */

}
