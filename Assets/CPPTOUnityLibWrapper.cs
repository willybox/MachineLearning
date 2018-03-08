
using System;
using System.Runtime.InteropServices;


public static class CPPTOUnityLibWrapper
{

    [DllImport("Algorithms")]
    public static extern IntPtr linear_create(int nbInputs);

    [DllImport("Algorithms")]
    public static extern void linear_train_classification(double[] coefficients, double[] coordinates, double[] values, int nbPoints);

    [DllImport("Algorithms")]
    public static extern void linear_regression(double[] coefficients, double[] coordinates, double[] values, int nbPoints);

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
