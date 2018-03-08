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
    private static int nbValues;
    private static double[] coordinates;
    private static double[] values;


    // Use this for initialization
    void Start()
    {
        nbPoints = 0;
        nbValues = 0;

        int nbSpheres = redSpheres.Length + blueSpheres.Length;

        coordinates = new double[nbSpheres * 2];
        values = new double[nbSpheres];

        

        getCoordinatesAndValues(redSpheres, "red");
        getCoordinatesAndValues(blueSpheres, "blue");
        
        IntPtr resultPtr = CPPTOUnityLibWrapper.linear_create(2);

        double[] result = new double[3];
        Marshal.Copy(resultPtr, result, 0, 3);

        
        CPPTOUnityLibWrapper.linear_regression(result, coordinates, values, nbSpheres);
        displayFunction2(whiteSpheres, result[0], result[1], result[2]);
        //displayFunction2(redSpheres, result[0], result[1], result[2]);
        //displayFunction2(blueSpheres, result[0], result[1], result[2]);

        /*CPPTOUnityLibWrapper.linear_train_classification(result, coordinates, values, nbSpheres);
        displayFunction(whiteSpheres, result[0], result[1], result[2]);
        displayFunction(redSpheres, result[0], result[1], result[2]);
        displayFunction(blueSpheres, result[0], result[1], result[2]);*/
    }

    private void getCoordinatesAndValues(Transform[] spheres, string color)
    {
        for (int i = 0; i < spheres.Length; i++)
        {
            coordinates[nbPoints] = spheres[i].position.x;
            coordinates[nbPoints + 1] = spheres[i].position.z;
            values[nbValues] = spheres[i].position.y;

            /*if (color == "blue")
            {
                values[nbValues] = -1;
            }
            else
            {
                values[nbValues] = 1;
            }*/
            
            nbPoints += 2;
            nbValues += 1;
        }
    }

    private void displayFunction(Transform[] spheres, double a, double b, double c)
    {
        for (int i = 0; i < spheres.Length; i++)
        {
            if ((b * spheres[i].position.x) + (c * spheres[i].position.z) + a > 0)
            {
                spheres[i].position += Vector3.up * 1f;
            }
            else
            {
                spheres[i].position += Vector3.down * 1f;
            }
        }
    }

    private void displayFunction2(Transform[] spheres, double a, double b, double c)
    {
        for (int i = 0; i < spheres.Length; i++)
        {
            float x = spheres[i].position.x;
            float z = spheres[i].position.z;
            float y = (float) ((b * spheres[i].position.x) + (c * spheres[i].position.z) + a);
            spheres[i].position = new Vector3(x,y,z);
        }
    }

}
