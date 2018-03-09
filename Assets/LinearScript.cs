using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = System.Random;


public class LinearScript : MonoBehaviour
{
    [SerializeField] private Transform[] whiteSpheres;

    [SerializeField] private Transform[] redSpheres;

    [SerializeField] private Transform[] blueSpheres;


    public enum Algorithm
    {
        Classification,
        Regression,
        NoMatrixLibraryRegression
    }

    public Algorithm LinearAlgorithm = Algorithm.Classification;


    // Variables des tailles
    private static uint _nbPoints;
    private static int spheresAddedCounter = 0;
    private const uint NbInputs = 2;
    private const uint NbInputWithExpedient = NbInputs + 1; // nbInputs + le biais

    // Variables des données
    private static double[] _coordinates;
    private static double[] _values;

    // Variables des coefficients)
    private double[] _resultedCoefficients;


    // Variables du menu
    private readonly List<Transform> _addedSpheres = new List<Transform>();

    private string _inputX1 = "";
    private string _inputX2 = "";


    // Lance le script
    private void Start()
    {
        _nbPoints = (uint) (redSpheres.Length + blueSpheres.Length);

        _coordinates = new double[_nbPoints * NbInputs];
        _values = new double[_nbPoints];

        _resultedCoefficients = InitializeLinearVariables();

        switch (LinearAlgorithm)
        {
            case Algorithm.Classification:
                GetCoordinatesAndValues(redSpheres, "red", Algorithm.Classification);
                GetCoordinatesAndValues(blueSpheres, "blue", Algorithm.Classification);

                CPPTOUnityLibWrapper.linear_train_classification(_resultedCoefficients, _coordinates, _values, _nbPoints, NbInputs);

                DisplayLinearClassification(whiteSpheres, _resultedCoefficients);
                DisplayLinearClassification(redSpheres, _resultedCoefficients);
                DisplayLinearClassification(blueSpheres, _resultedCoefficients);

                break;

            case Algorithm.Regression:
                GetCoordinatesAndValues(redSpheres, "red", Algorithm.Regression);
                GetCoordinatesAndValues(blueSpheres, "blue", Algorithm.Regression);

                CPPTOUnityLibWrapper.linear_train_regression(_resultedCoefficients, _coordinates, _values, _nbPoints, NbInputs);

                DisplayLinearRegression(whiteSpheres, _resultedCoefficients);

                break;

            case Algorithm.NoMatrixLibraryRegression:
                GetCoordinatesAndValues(redSpheres, "red", Algorithm.NoMatrixLibraryRegression);
                GetCoordinatesAndValues(blueSpheres, "blue", Algorithm.NoMatrixLibraryRegression);

                CPPTOUnityLibWrapper.linear_train_no_matrix_library_regression(_resultedCoefficients, _coordinates, _values,
                    _nbPoints);

                DisplayLinearRegression(whiteSpheres, _resultedCoefficients);

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        GC.Collect();
    }

    // Récupère les données et initialise les coefficients
    private double[] InitializeLinearVariables()
    {
        IntPtr resultPtr = CPPTOUnityLibWrapper.linear_create(NbInputWithExpedient);

        double[] result = new double[NbInputWithExpedient];
        Marshal.Copy(resultPtr, result, 0, (int)NbInputWithExpedient);

        return result;
    }

    // Récupère les coordonnées des sphères
    private static void GetCoordinatesAndValues(Transform[] spheres, string color, Algorithm linearAlgorithm)
    {
        for (int i = 0; i < spheres.Length; i++)
        {
            _coordinates[spheresAddedCounter * 2] = spheres[i].position.x;
            _coordinates[spheresAddedCounter * 2 + 1] = spheres[i].position.z;

            if (linearAlgorithm == Algorithm.Classification)
                if (color == "blue")
                    _values[spheresAddedCounter] = -1;

                else
                    _values[spheresAddedCounter] = 1;

            else if (linearAlgorithm == Algorithm.Regression || linearAlgorithm == Algorithm.NoMatrixLibraryRegression)
                _values[spheresAddedCounter] = spheres[i].position.y;

            else
                Debug.Log("Algorithme non reconnu");

            spheresAddedCounter++;
        }
    }

    // Affiche la fonction de classification
    private static void DisplayLinearClassification(Transform[] spheres, double[] coefficients)
    {
        foreach (Transform s in spheres)
        {
            if (coefficients[0] * s.position.x + coefficients[1] * s.position.z + coefficients[2] > 0)
                s.position += Vector3.up * 1f;

            else
                s.position += Vector3.down * 1f;
        }
    }

    // Affiche la fonction de régression
    private static void DisplayLinearRegression(Transform[] spheres, double[] coefficients)
    {
        foreach (Transform s in spheres)
        {
            float y = (float) (coefficients[0] * s.position.x + coefficients[1] * s.position.z + coefficients[2]);

            s.position = new Vector3(s.position.x, y, s.position.z);
        }
    }

    // Affiche le menu de jeu
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 0, 150, 20), "X1 (ou X) :");
        _inputX1 = GUI.TextField(new Rect(20, 20, 150, 20), _inputX1);

        GUI.Label(new Rect(10, 40, 150, 20), "X2 (ou Y) :");
        _inputX2 = GUI.TextField(new Rect(20, 60, 150, 20), _inputX2);

        GUI.enabled = true;

        if (GUI.Button(new Rect(60, 90, 60, 20), "Calculer"))
            if (!string.IsNullOrEmpty(_inputX1) && !string.IsNullOrEmpty(_inputX2))
            {
                UseResult(Convert.ToDouble(_inputX1), Convert.ToDouble(_inputX2));
            }

            else
            {
                Debug.Log("Les valeurs doivent etre des nombres !");
                Debug.Log("\n");
            }

        if (GUI.Button(new Rect(200, 20, 240, 20), "Ajouter sphere aleatoire")) AddRandomSphere();

        if (GUI.Button(new Rect(200, 45, 240, 20), "Supprimer derniere sphere ajoutee")) RemoveLastAddedSphere();

        if (GUI.Button(new Rect(200, 65, 240, 20), "Supprimer toutes les spheres ajoutees")) RemoveAllAddedSphere();
    }

    // Utilise les coefficients pour calculer le résultat de la fonction sur les coordonnées d'entrée selon l'algorithme utilisé
    private void UseResult(double x1, double x2)
    {
        double value = CalculateValue(x1, x2, _resultedCoefficients);

        Vector3 point = new Vector3
        {
            x = (float) x1,
            z = (float) x2
        };

        Debug.Log("x = " + point.x);
        Debug.Log("z = " + point.z);

        Transform newSphere;

        switch (LinearAlgorithm)
        {
            case Algorithm.Classification:

                if (value > 0)
                {
                    point.y = redSpheres[0].position.y;

                    newSphere = Instantiate(redSpheres[0], point, new Quaternion());
                    newSphere.name = "Sphere ajoutee " + (_addedSpheres.Count + 1) + " (rouge)";

                    _addedSpheres.Add(newSphere);

                    Debug.Log("color = red");
                }

                else if (value < 0)
                {
                    point.y = blueSpheres[0].position.y;

                    newSphere = Instantiate(blueSpheres[0], point, new Quaternion());
                    newSphere.name = "Sphere ajoutee " + (_addedSpheres.Count + 1) + " (bleue)";

                    _addedSpheres.Add(newSphere);

                    Debug.Log("color = blue");
                }

                else
                {
                    point.y = 0;

                    newSphere = Instantiate(whiteSpheres[0], point, new Quaternion());
                    newSphere.GetComponent<MeshRenderer>().material =
                        Resources.Load("Colors/Jaune", typeof(Material)) as Material;
                    newSphere.name = "Sphere ajoutee " + (_addedSpheres.Count + 1) + " (milieu)";

                    _addedSpheres.Add(newSphere);

                    Debug.Log("color = yellow");
                }

                break;

            case Algorithm.Regression:
            case Algorithm.NoMatrixLibraryRegression:

                point.y = (float) value;

                newSphere = Instantiate(whiteSpheres[0], point, new Quaternion());
                newSphere.GetComponent<MeshRenderer>().material =
                    Resources.Load("Colors/Jaune", typeof(Material)) as Material;
                newSphere.name = "Sphere ajoutee " + (_addedSpheres.Count + 1);

                _addedSpheres.Add(newSphere);

                Debug.Log("y = " + point.y);

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        Debug.Log("\n");
    }

    // Ajoute une sphère aléatoirement
    private void AddRandomSphere()
    {
        Random randomer = new Random();

        double randomX1 = randomer.NextDouble() + randomer.Next(-10, 10);
        double randomX2 = randomer.NextDouble() + randomer.Next(-10, 10);

        UseResult(randomX1, randomX2);
    }

    // Supprime la dernière sphère ajoutée
    private void RemoveLastAddedSphere()
    {
        if (_addedSpheres.Count <= 0) return;

        Destroy(_addedSpheres[_addedSpheres.Count - 1].gameObject);
        _addedSpheres.RemoveAt(_addedSpheres.Count - 1);
    }

    // Supprime toutes les sphères ajoutées
    private void RemoveAllAddedSphere()
    {
        for (int i = _addedSpheres.Count - 1; i >= 0; i--)
        {
            Destroy(_addedSpheres[i].gameObject);
            _addedSpheres.RemoveAt(i);
        }
    }

    // Calcule la valeur des coordonnées avec les coefficients trouvés
    private static double CalculateValue(double x1, double x2, double[] coefficients)
    {
        return coefficients[0] * x1 + coefficients[1] * x2 + coefficients[2];
    }
}