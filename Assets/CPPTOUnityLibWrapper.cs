
using System.Runtime.InteropServices;

public static class CPPTOUnityLibWrapper
{

    [DllImport("../x64/Release/Algorithms")]
    public static extern int add_to_42(int valueToAdd);


    [DllImport("../x64/Release/Algorithms")]
    public static extern System.IntPtr linear_create();

    [DllImport("../x64/Release/Algorithms")]
    public static extern System.IntPtr linear_remove(System.IntPtr valueToRemove);


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
