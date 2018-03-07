
using System.Runtime.InteropServices;

public static class CPPTOUnityLibWrapper
{

    [DllImport("../x64/Release/Algorithms")]
    public static extern System.IntPtr linear_create(System.IntPtr X, System.IntPtr Y, int nbPoints);

    [DllImport("../x64/Release/Algorithms")]
    public static extern void linear_train_classification(System.IntPtr datas);

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
