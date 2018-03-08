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


    private List<Transform> addedSpheres = new List<Transform>();

    private string inputX1 = "";
    private string inputX2 = "";

    void OnGUI()
    {
        GUI.Label(new Rect(10, 0, 150, 20), "X1 (ou X) :");
        inputX1 = GUI.TextField(new Rect(20, 20, 150, 20), inputX1);

        GUI.Label(new Rect(10, 40, 150, 20), "X2 (ou Y) :");
        inputX2 = GUI.TextField(new Rect(20, 60, 150, 20), inputX2);

        GUI.enabled = true;

        if (GUI.Button(new Rect(60, 90, 60, 20), "Calculer"))
        {
            if (!string.IsNullOrEmpty(inputX1) && !string.IsNullOrEmpty(inputX2))
            {
                useResult(Convert.ToDouble(inputX1), Convert.ToDouble(inputX2));
            }

            else
            {
                Debug.Log("Les valeurs doivent etre des nombres !");
            }
        }

        if (GUI.Button(new Rect(200, 20, 240, 20), "Ajouter sphere aleatoire"))
        {
            addRandomSphere();
        }

        if (GUI.Button(new Rect(200, 45, 240, 20), "Supprimer derniere sphere ajoutee"))
        {
            removeLastAddedSphere();
        }

        if (GUI.Button(new Rect(200, 65, 240, 20), "Supprimer toutes les spheres ajoutees"))
        {
            removeAllAddedSphere();
        }
    }

    private void useResult(double x1, double x2)
    {
        double value = calculateValue(x1, x2, result[0], result[1], result[2]);

        Vector3 point = new Vector3();
        point.x = (float)x1;
        point.z = (float)x2;

        Debug.Log("x = " + point.x);
        Debug.Log("z = " + point.z);

        Transform newSphere;

        switch (linearAlgorithm)
        {
            case Algorithm.Classification:

                if (value > 0)
                {
                    point.y = redSpheres[0].position.y;

                    newSphere = Instantiate(redSpheres[0], point, new Quaternion());
                    newSphere.name = "Sphere ajoutee " + (addedSpheres.Count + 1) + " (rouge)";

                    addedSpheres.Add(newSphere);

                    Debug.Log("color = red");
                }

                else if (value < 0)
                {
                    point.y = blueSpheres[0].position.y;

                    newSphere = Instantiate(blueSpheres[0], point, new Quaternion());
                    newSphere.name = "Sphere ajoutee " + (addedSpheres.Count + 1) + " (bleue)";

                    addedSpheres.Add(newSphere);

                    Debug.Log("color = blue");
                }

                else
                {
                    point.y = blueSpheres[0].position.y;

                    newSphere = Instantiate(whiteSpheres[0], point, new Quaternion());
                    newSphere.GetComponent<MeshRenderer>().material = Resources.Load("Colors/Jaune", typeof(Material)) as Material;
                    newSphere.name = "Sphere ajoutee " + (addedSpheres.Count + 1) + " (milieu)";

                    addedSpheres.Add(newSphere);

                    Debug.Log("color = yellow");
                }

                break;

            case Algorithm.Regression:
            case Algorithm.NoMatrixLibraryRegression:

                point.y = (float)value;

                newSphere = Instantiate(whiteSpheres[0], point, new Quaternion());
                newSphere.GetComponent<MeshRenderer>().material = Resources.Load("Colors/Jaune", typeof(Material)) as Material;
                newSphere.name = "Sphere ajoutee " + (addedSpheres.Count + 1);

                addedSpheres.Add(newSphere);

                Debug.Log("y = " + point.y);

                break;
        }
    }

    private void addRandomSphere()
    {
        System.Random randomer = new System.Random();

        double randomX1 = randomer.NextDouble() + randomer.Next(-6, 11); // le plateau n'est pas pile au milieu
        double randomX2 = randomer.NextDouble() + randomer.Next(-9, 8); // le plateau n'est pas pile au milieu

        useResult(randomX1, randomX2);
    }

    private void removeLastAddedSphere()
    {
        if (addedSpheres.Count > 0)
        {
            Destroy(addedSpheres[addedSpheres.Count - 1].gameObject);
            addedSpheres.RemoveAt(addedSpheres.Count - 1);
        }
    }

    private void removeAllAddedSphere()
    {
        for (int i = (addedSpheres.Count - 1); i >= 0 ; i--)
        {
            Destroy(addedSpheres[i].gameObject);
            addedSpheres.RemoveAt(i);
        }
    }

    private double calculateValue(double x1, double x2, double a, double b, double c)
    {
        return ((a * x1) + (b * x2) + c);
    }

}
