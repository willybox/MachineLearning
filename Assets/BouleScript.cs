using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CPPTOUnityLibWrapper;

public class BouleScript : MonoBehaviour {

    [SerializeField]
    private Transform[] whiteSpheres;
    [SerializeField]
    private Transform[] redSpheres;
    [SerializeField]
    private Transform[] blueSpheres;

    private static int nbPoints;
    private static double[] coordinates;
    private static int[] values;


    // Use this for initialization
    void Start()
    {
        Debug.Log("test");

        nbPoints = 0;
        getCoordinatesAndValues(redSpheres, "red");
        getCoordinatesAndValues(blueSpheres, "blue");

        /*
        double[] result = linear_create(coordinates, values, nbPoints);

        linear_train_classification(result);
        */
    }

    private void getCoordinatesAndValues(Transform[] spheres, string color)
    {
        for (int i = 0; i < spheres.Length; i+=2)
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
            
            nbPoints+=2;
        }
    }

    private void displayFunction(Transform[] spheres, double a, double b, double c)
    {
        for (int i = 0; i < spheres.Length; i++)
        {
            if (a * spheres[i].position.x + b * spheres[i].position.z + c > 0)
            {
                spheres[i].position.Set(spheres[i].position.x, spheres[i].position.y + 2, spheres[i].position.z);
            }
        }
    }

}
