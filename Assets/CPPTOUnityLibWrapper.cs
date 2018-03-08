
using System;
using System.Runtime.InteropServices;


public static class CPPTOUnityLibWrapper
{

    [DllImport("Algorithms")]
    public static extern IntPtr linear_create(int nbInputs);

    [DllImport("Algorithms")]
    public static extern IntPtr linear_remove(IntPtr valueToRemove);


    [DllImport("Algorithms")]
    public static extern void linear_train_classification(double[] coefficients, double[] coordinates, double[] values, int nbPoints);
    
    [DllImport("Algorithms")]
    public static extern void linear_train_regression(double[] coefficients, double[] coordinates, double[] values, int nbPoints);

    [DllImport("Algorithms")]
    public static extern void linear_train_no_matrix_library_regression(double[] coefficients, double[] coordinates, double[] values, int nbPoints);


    /*
    [DllImport("Algorithms")]
    public static extern ??? linear_classify(System.IntPtr, ???);

    [DllImport("Algorithms")]
    public static extern ??? linear_predict(System.IntPtr, ???);
    */

}
