using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Experimental.UIElements;


public class CSVLinearScript : MonoBehaviour
{

    [SerializeField]
    private Transform[] whiteSpheres;


    public string _filePath;
    private Transform[] spheres;


    public enum Algorithm
    {
        Classification,
        Regression,
        NoMatrixLibraryRegression
    }

    public Algorithm LinearAlgorithm = Algorithm.Classification;


    // Variables des tailles
    private static uint _nbPoints;
    private static uint _nbAddedSpheres;
    private static int spheresAddedCounter;
    private const uint NbInputs = 2;
    private const uint NbInputWithExpedient = NbInputs + 1; // nbInputs + le biais

    // Variables des données
    private static double[] _coordinates;
    private static double[] _values;

    // Variables des coefficients)
    private double[] _resultedCoefficients;


    // Lance le script
    void Start()
    {
        Calculate();

        GC.Collect();
    }


    private void Calculate()
    {
        parseCSV(Algorithm.Classification);

        _nbPoints = (uint)spheres.Length;

        _coordinates = new double[_nbPoints * NbInputs];
        _values = new double[_nbPoints];

        _resultedCoefficients = InitializeLinearVariables();

        switch (LinearAlgorithm)
        {
            case Algorithm.Classification:
                GetCoordinatesAndValues(spheres, Algorithm.Classification);

                CPPTOUnityLibWrapper.linear_train_classification(_resultedCoefficients, _coordinates, _values,
                    _nbPoints, NbInputs);

                DisplayLinearClassification(whiteSpheres, _resultedCoefficients);

                break;

            case Algorithm.Regression:
                GetCoordinatesAndValues(spheres, Algorithm.Regression);

                CPPTOUnityLibWrapper.linear_train_regression(_resultedCoefficients, _coordinates, _values, _nbPoints,
                    NbInputs);

                DisplayLinearRegression(whiteSpheres, _resultedCoefficients);

                break;

            case Algorithm.NoMatrixLibraryRegression:
                GetCoordinatesAndValues(spheres, Algorithm.NoMatrixLibraryRegression);

                CPPTOUnityLibWrapper.linear_train_no_matrix_library_regression(_resultedCoefficients, _coordinates,
                    _values,
                    _nbPoints);

                DisplayLinearRegression(whiteSpheres, _resultedCoefficients);

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    // Récupère les données et initialise les coefficients
    private double[] InitializeLinearVariables()
    {
        IntPtr resultPtr = CPPTOUnityLibWrapper.linear_create(NbInputWithExpedient);

        double[] result = new double[NbInputWithExpedient];
        Marshal.Copy(resultPtr, result, 0, (int)NbInputWithExpedient);

        return result;
    }
    
    private void parseCSV(Algorithm linearAlgorithm)
    {
        if (!string.IsNullOrEmpty(_filePath))
        {
            List<string> lines = new List<string>(System.IO.File.ReadAllLines(_filePath));

            string[] header = lines[0].Split(';');

            lines.RemoveAt(0);

            if (lines.Count > 0)
            {
                while (lines.Count > 80)
                {
                    System.Random randomer = new System.Random();
                    lines.RemoveAt(randomer.Next(0, lines.Count));
                }

                spheres = new Transform[lines.Count];

                for (int i = 0; i < lines.Count; i++)
                {
                    string[] fields = lines[i].Split(';');

                    switch (LinearAlgorithm)
                    {
                        case Algorithm.Classification:

                            Vector3 classificationPositions = new Vector3();
                            classificationPositions.x = convertOrGiveZero(fields[0]);
                            classificationPositions.z = convertOrGiveZero(fields[1]);
                            classificationPositions.y = convertOrGiveZero(fields[2]);

                            _nbAddedSpheres++;

                            string color = "Jaune";

                            if (convertOrGiveZero(fields[2]) == 1)
                            {
                                color = "Bleu";
                            }

                            else if (convertOrGiveZero(fields[2]) == 0)
                            {
                                color = "Rouge";
                            }

                            Transform classificationSphere = Instantiate(whiteSpheres[0], classificationPositions, new Quaternion());
                            classificationSphere.GetComponent<MeshRenderer>().material =
                                Resources.Load("Colors/" + color, typeof(Material)) as Material;
                            classificationSphere.name = "Sphere ajoutee " + _nbAddedSpheres;

                            spheres[i] = classificationSphere;

                            break;

                        case Algorithm.Regression:
                        case Algorithm.NoMatrixLibraryRegression:

                            Vector3 regressionPositions = new Vector3();
                            regressionPositions.x = convertOrGiveZero(fields[0]);
                            regressionPositions.z = convertOrGiveZero(fields[2]);
                            regressionPositions.y = convertOrGiveZero(fields[1]);

                            _nbAddedSpheres++;

                            Transform regressionSphere = Instantiate(whiteSpheres[0], regressionPositions, new Quaternion());
                            regressionSphere.GetComponent<MeshRenderer>().material =
                                Resources.Load("Colors/Jaune", typeof(Material)) as Material;
                            regressionSphere.name = "Sphere ajoutee " + _nbAddedSpheres;

                            spheres[i] = regressionSphere;

                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                switch (linearAlgorithm)
                {
                    case Algorithm.Classification:

                        Debug.Log("x = " + header[0]);
                        Debug.Log("y = " + header[1]);
                        Debug.Log("z = " + header[2]);

                        break;

                    case Algorithm.Regression:
                    case Algorithm.NoMatrixLibraryRegression:

                        Debug.Log("x = " + header[0]);
                        Debug.Log("y = " + header[2]);
                        Debug.Log("z = " + header[1]);

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        else
        {
            Debug.Log("Valeur incorrecte");
        }
    }

    public float convertOrGiveZero(string stringToConvert)
    {
        float result = 0;

        if (!string.IsNullOrEmpty(stringToConvert))
        {
            result = Convert.ToSingle(stringToConvert);
        }

        return result;
    }

    // Récupère les coordonnées des sphères
    private static void GetCoordinatesAndValues(Transform[] spheres, Algorithm linearAlgorithm)
    {
        for (int i = 0; i < spheres.Length; i++)
        {
            _coordinates[spheresAddedCounter * 2] = spheres[i].position.x;
            _coordinates[spheresAddedCounter * 2 + 1] = spheres[i].position.z;

            if (linearAlgorithm == Algorithm.Classification)
            {
                if (Convert.ToInt32(spheres[i].position.y) == 1)
                    _values[spheresAddedCounter] = 1;

                else
                    _values[spheresAddedCounter] = -1;
            }

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
            float y = (float)(coefficients[0] * s.position.x + coefficients[1] * s.position.z + coefficients[2]);

            s.position = new Vector3(s.position.x, y, s.position.z);
        }
    }
}
