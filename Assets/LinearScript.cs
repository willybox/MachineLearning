using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class LinearScript : MonoBehaviour {

    [SerializeField]
    private Transform[] whiteSpheres;
    [SerializeField]
    private Transform[] redSpheres;
    [SerializeField]
    private Transform[] blueSpheres;

    private static uint nbPoints;
    private static uint nbValues;
    private static double[] coordinates;
    private static double[] values;

    int nbInputs = 3;

    private double[] result;


    public enum Algorithm { Classification, Regression, NoMatrixLibraryRegression };

    public Algorithm linearAlgorithm = Algorithm.Classification;


    // Use this for initialization
    void Start()
    {
        nbPoints = 0;
        nbValues = 0;

        int nbSpheres = redSpheres.Length + blueSpheres.Length;

        coordinates = new double[nbSpheres * 2];
        values = new double[nbSpheres];

        result = new double[3];

        switch (linearAlgorithm)
        {
            case Algorithm.Classification:
                result = initializeLinearVariables(Algorithm.Classification);

                CPPTOUnityLibWrapper.linear_train_classification(result, coordinates, values, nbSpheres);

                displayLinearClassification(whiteSpheres, result[0], result[1], result[2]);
                displayLinearClassification(redSpheres, result[0], result[1], result[2]);
                displayLinearClassification(blueSpheres, result[0], result[1], result[2]);

                break;

            case Algorithm.Regression:
                result = initializeLinearVariables(Algorithm.Regression);

                CPPTOUnityLibWrapper.linear_train_regression(result, coordinates, values, nbSpheres);

                displayLinearRegression(whiteSpheres, result[0], result[1], result[2]);

                break;

            case Algorithm.NoMatrixLibraryRegression:
                result = initializeLinearVariables(Algorithm.NoMatrixLibraryRegression);

                CPPTOUnityLibWrapper.linear_train_no_matrix_library_regression(result, coordinates, values, nbSpheres);

                displayLinearRegression(whiteSpheres, result[0], result[1], result[2]);

                break;
        }

        GC.Collect();
    }

    private string inputX1 = "";
    private string inputX2 = "";

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 150, 20), "X1 (ou X) :");
        inputX1 = GUI.TextField(new Rect(10, 20, 150, 20), inputX1);

        GUI.Label(new Rect(0, 40, 150, 20), "X2 (ou Y) :");
        inputX2 = GUI.TextField(new Rect(10, 60, 150, 20), inputX2);

        GUI.enabled = true;

        if (GUI.Button(new Rect(0, 100, 150, 20), "Calculer"))
        {
            useResult(Convert.ToDouble(inputX1), Convert.ToDouble(inputX2));
        }
    }

    private double[] initializeLinearVariables(Algorithm linearAlgorithm)
    {
        getCoordinatesAndValues(redSpheres, "red", linearAlgorithm);
        getCoordinatesAndValues(blueSpheres, "blue", linearAlgorithm);

        IntPtr resultPtr = CPPTOUnityLibWrapper.linear_create(nbInputs);

        double[] result = new double[nbInputs];
        Marshal.Copy(resultPtr, result, 0, nbInputs);

        return result;
    }

    private void getCoordinatesAndValues(Transform[] spheres, string color, Algorithm linearAlgorithm)
    {
        for (uint i = 0; i < spheres.Length; i++)
        {
            coordinates[nbPoints] = spheres[i].position.x;
            coordinates[nbPoints + 1] = spheres[i].position.z;

            if (linearAlgorithm == Algorithm.Classification)
            {
                if (color == "blue")
                {
                    values[nbValues] = -1;
                }

                else
                {
                    values[nbValues] = 1;
                }
            }

            else
            {
                values[nbValues] = spheres[i].position.y;
            }
            
            nbPoints += 2;
            nbValues += 1;
        }
    }

    private void displayLinearClassification(Transform[] spheres, double a, double b, double c)
    {
        for (uint i = 0; i < spheres.Length; i++)
        {
            if ((a * spheres[i].position.x) + (b * spheres[i].position.z) + c > 0)
            {
                spheres[i].position += Vector3.up * 1f;
            }

            else
            {
                spheres[i].position += Vector3.down * 1f;
            }
        }
    }

    private void displayLinearRegression(Transform[] spheres, double a, double b, double c)
    {
        for (uint i = 0; i < spheres.Length; i++)
        {
            float x = spheres[i].position.x;
            float z = spheres[i].position.z;
            float y = (float)((a * spheres[i].position.x) + (b * spheres[i].position.z) + c);

            spheres[i].position = new Vector3(x, y, z);
        }
    }
    private void useResult(double x1, double x2)
    {
        double value = calculateValue(x1, x2, result[0], result[1], result[2]);

        Vector3 point = new Vector3();
        point.x = (float) x1;
        point.z = (float) x2;

        switch (linearAlgorithm)
        {
            case Algorithm.Classification:

                if (value > 0)
                {
                    point.y = redSpheres[0].position.y;
                    Instantiate(redSpheres[0], point, new Quaternion());
                }

                else if (value < 0)
                {
                    point.y = blueSpheres[0].position.y;
                    Instantiate(blueSpheres[0], point, new Quaternion());
                }

                else
                {
                    point.y = blueSpheres[0].position.y;
                    Instantiate(whiteSpheres[0], point, new Quaternion())
                        .GetComponent<MeshRenderer>().material = Resources.Load("Colors/Jaune", typeof(Material)) as Material;
                }

                break;

            case Algorithm.Regression:
            case Algorithm.NoMatrixLibraryRegression:

                point.y = (float) value;
                Instantiate(whiteSpheres[0], point, new Quaternion())
                    .GetComponent<MeshRenderer>().material = Resources.Load("Colors/Jaune", typeof(Material)) as Material;

                break;
        }
    }

    private double calculateValue(double x1, double x2, double a, double b, double c)
    {
        return ((a * x1) + (b * x2) + c);
    }

}
