using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SphereScript : MonoBehaviour {

    [SerializeField]
    private Transform[] whiteSpheres;
    [SerializeField]
    private Transform[] redSpheres;
    [SerializeField]
    private Transform[] blueSpheres;

    private static int nbPoints;
    private static double[] coordinates;
    private static double[] values;


    // Use this for initialization
    void Start()
    {
        nbPoints = 0;
        int nbSpheres = redSpheres.Length + blueSpheres.Length;

        coordinates = new double[nbSpheres * 2];
        values = new double[nbSpheres];

        getCoordinatesAndValues(redSpheres, "red");
        getCoordinatesAndValues(blueSpheres, "blue");

        int totalSize = 4 + 3 * nbPoints;

        IntPtr resultPtr = CPPTOUnityLibWrapper.linear_create(coordinates, values, nbSpheres);

        double[] result = new double[totalSize];
        Marshal.Copy(resultPtr, result, 0, totalSize);
        Debug.Log(result[0]);
        Debug.Log(result[1]);
        Debug.Log(result[2]);
        Debug.Log(result[3]);
        Debug.Log(result[4]);

        CPPTOUnityLibWrapper.linear_train_classification(result);

        displayFunction(whiteSpheres, result[1], result[2], result[3]);
    }

    private void getCoordinatesAndValues(Transform[] spheres, string color)
    {
        for (int i = 0; i < spheres.Length; i++)
        {
            coordinates[nbPoints] = spheres[i].position.x;
            coordinates[nbPoints + 1] = spheres[i].position.z;

            if (color == "blue")
            {
                values[i] = -1;
            }

            else
            {
                values[i] = 1;
            }
            
            nbPoints += 2;
        }
    }

    private void displayFunction(Transform[] spheres, double a, double b, double c)
    {
        for (int i = 0; i < spheres.Length; i++)
        {
            if ((a * spheres[i].position.x) + (b * spheres[i].position.z) + c > 0)
            {
                spheres[i].position.Set(spheres[i].position.x, spheres[i].position.y + 2, spheres[i].position.z);
            }
        }
    }

}
