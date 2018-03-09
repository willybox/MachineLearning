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
    private static int spheresAddedCounter;
    private const uint NbInputs = 2;
    private const uint NbInputWithExpedient = NbInputs + 1; // nbInputs + le biais

    // Variables des données
    private static double[] _coordinates;
    private static double[] _values;

    // Variable des coefficients)
    private double[] _resultedCoefficients;


    // Variables du menu
    private List<Transform> _addedSpheres = new List<Transform>();

    private string _inputX1 = "";
    private string _inputX2 = "";
    private string _inputX3 = "";


    // Lance le script
    private void Start()
    {
        Calculate();

        GC.Collect();
    }


    private void Calculate()
    {
        _nbPoints = (uint)(redSpheres.Length + blueSpheres.Length);

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

        GUI.Label(new Rect(10, 40, 150, 20), "X2 (ou Z) :");
        _inputX2 = GUI.TextField(new Rect(20, 60, 150, 20), _inputX2);

        GUI.Label(new Rect(10, 80, 150, 20), "X3 (ou Y) :");
        _inputX3 = GUI.TextField(new Rect(20, 100, 150, 20), _inputX3);

        GUI.enabled = true;

        if (GUI.Button(new Rect(10, 130, 80, 20), "Ajouter Y"))
        {
            if (!string.IsNullOrEmpty(_inputX1) && !string.IsNullOrEmpty(_inputX2) && !string.IsNullOrEmpty(_inputX3))
            {
                AddSphere(Convert.ToDouble(_inputX1), Convert.ToDouble(_inputX2), Convert.ToDouble(_inputX3), false);
            }

            else
            {
                Debug.Log("Les valeurs doivent etre des nombres !");
                Debug.Log("\n");
            }
        }

        if (GUI.Button(new Rect(100, 130, 80, 20), "Calculer Y"))
        {
            if (!string.IsNullOrEmpty(_inputX1) && !string.IsNullOrEmpty(_inputX2))
            {
                AddSphere(Convert.ToDouble(_inputX1), Convert.ToDouble(_inputX2), 0, true);
            }

            else
            {
                Debug.Log("Les valeurs doivent etre des nombres !");
                Debug.Log("\n");
            }
        }

        if (GUI.Button(new Rect(200, 20, 240, 20), "Ajouter sphere aleatoire")) CreateRandomSphere(false);

        if (GUI.Button(new Rect(200, 40, 240, 20), "Ajouter sphere calculée")) CreateRandomSphere(true);

        if (GUI.Button(new Rect(200, 65, 240, 20), "Supprimer derniere sphere ajoutee")) RemoveLastAddedSphere();

        if (GUI.Button(new Rect(200, 85, 240, 20), "Supprimer toutes les spheres ajoutees")) RemoveAllAddedSphere();

        if (GUI.Button(new Rect(270, 130, 80, 20), "Recalculer"))
        {
            RecalculateWithAddedSpheres();
        }
    }

    private void RecalculateWithAddedSpheres()
    {
        switch (LinearAlgorithm)
        {
            case Algorithm.Classification:

                // Récupère les nouvelles sphères et les ajoute au tableau d'entrées
                List<Transform> newRedSpheres = new List<Transform>();
                List<Transform> newBlueSpheres = new List<Transform>();

                for (int i = 0; i < _addedSpheres.Count; i++)
                {
                    if (_addedSpheres[i].position.y > 0)
                    {
                        newRedSpheres.Add(_addedSpheres[i]);
                    }

                    else
                    {
                        newBlueSpheres.Add(_addedSpheres[i]);
                    }
                }

                Transform[] newRedSpheresArray = newRedSpheres.ToArray();
                Transform[] newBlueSpheresArray = newBlueSpheres.ToArray();

                Transform[] tempRedSpheres = new Transform[redSpheres.Length + newRedSpheresArray.Length];
                Transform[] tempBlueSpheres = new Transform[blueSpheres.Length + newBlueSpheresArray.Length];

                Array.ConstrainedCopy(redSpheres, 0, tempRedSpheres, 0, redSpheres.Length);
                Array.ConstrainedCopy(newRedSpheresArray, 0, tempRedSpheres, redSpheres.Length, newRedSpheresArray.Length);

                Array.ConstrainedCopy(blueSpheres, 0, tempBlueSpheres, 0, blueSpheres.Length);
                Array.ConstrainedCopy(newBlueSpheresArray, 0, tempBlueSpheres, blueSpheres.Length, newBlueSpheresArray.Length);

                redSpheres = tempRedSpheres;
                blueSpheres = tempBlueSpheres;

                // Réinitialise les hauteurs des sphères rouges et bleues
                for (int i = 0; i < redSpheres.Length; i++)
                {
                    Vector3 positions = redSpheres[i].position;
                    positions.y = 0;
                    redSpheres[i].position = positions;
                }

                for (int i = 0; i < blueSpheres.Length; i++)
                {
                    Vector3 positions = blueSpheres[i].position;
                    positions.y = 0;
                    blueSpheres[i].position = positions;
                }

                break;

            case Algorithm.Regression:
            case Algorithm.NoMatrixLibraryRegression:

                List<Transform> newSpheres = new List<Transform>();

                for (int i = 0; i < _addedSpheres.Count; i++)
                {
                    newSpheres.Add(_addedSpheres[i]);
                }

                Transform[] newSpheresArray = newSpheres.ToArray();

                Transform[] tempSpheres = new Transform[redSpheres.Length + newSpheresArray.Length];

                Array.ConstrainedCopy(redSpheres, 0, tempSpheres, 0, redSpheres.Length);
                Array.ConstrainedCopy(newSpheresArray, 0, tempSpheres, redSpheres.Length, newSpheresArray.Length);

                redSpheres = tempSpheres;

                break;
        }

        // Réinitialise les hauteurs des sphères blanches
        for (int i = 0; i < whiteSpheres.Length; i++)
        {
            Vector3 positions = whiteSpheres[i].position;
            positions.y = -1;
            whiteSpheres[i].position = positions;
        }

        // Réinitialise la liste des sphères ajoutées
        _addedSpheres = new List<Transform>();

        _nbPoints = 0;
        spheresAddedCounter = 0;

        Calculate();

        GC.Collect();
    }

    // Utilise les coefficients pour calculer le résultat de la fonction sur les coordonnées d'entrée selon l'algorithme utilisé
    private void AddSphere(double x1, double x2, double x3, bool calculate)
    {
        Vector3 point = new Vector3
        {
            x = (float) x1,
            z = (float) x2,
            y = (float) x3
        };

        Debug.Log("x = " + point.x);
        Debug.Log("z = " + point.z);

        Transform newSphere;

        switch (LinearAlgorithm)
        {
            case Algorithm.Classification:

                if (calculate)
                {
                    double value = CalculateValue(x1, x2, _resultedCoefficients);

                    if (value > 0)
                    {
                        point.y = redSpheres[0].position.y;

                        newSphere = Instantiate(redSpheres[0], point, new Quaternion());
                        newSphere.name = "Sphere calculee ajoutee " + (_addedSpheres.Count + 1) + " (rouge)";

                        _addedSpheres.Add(newSphere);

                        Debug.Log("color = Rouge");
                    }

                    else if (value < 0)
                    {
                        point.y = blueSpheres[0].position.y;

                        newSphere = Instantiate(blueSpheres[0], point, new Quaternion());
                        newSphere.name = "Sphere calculee ajoutee " + (_addedSpheres.Count + 1) + " (bleu)";

                        _addedSpheres.Add(newSphere);

                        Debug.Log("color = Bleu");
                    }

                    // Peu de chances que ça arrive
                    else if (value == 0)
                    {
                        point.y = 0;

                        newSphere = Instantiate(whiteSpheres[0], point, new Quaternion());
                        newSphere.GetComponent<MeshRenderer>().material =
                            Resources.Load("Colors/Jaune", typeof(Material)) as Material;
                        newSphere.name = "Sphere calculee ajoutee " + (_addedSpheres.Count + 1) + " (milieu)";

                        _addedSpheres.Add(newSphere);

                        Debug.Log("color = Jaune");
                    }
                }

                // Sphère totalement aléatoire (pas de calcul de y) donc couleur aléatoire
                else
                {
                    point.y = 0;

                    string color;
                    Random randomer = new Random();
                    int randomNumber = randomer.Next(0, 2);

                    if (randomNumber == 1)
                    {
                        color = "Rouge";
                    }

                    else
                    {
                        color = "Bleu";
                    }

                    newSphere = Instantiate(whiteSpheres[0], point, new Quaternion());
                    newSphere.GetComponent<MeshRenderer>().material =
                        Resources.Load("Colors/" + color, typeof(Material)) as Material;
                    newSphere.name = "Sphere non calculee ajoutee " + (_addedSpheres.Count + 1) + " (" + color.ToLower() + ")";

                    _addedSpheres.Add(newSphere);

                    Debug.Log("color = " + color);
                }

                break;

            case Algorithm.Regression:
            case Algorithm.NoMatrixLibraryRegression:

                string sphereDefinition = "non calculee";

                if (calculate)
                {
                    point.y = (float) CalculateValue(x1, x2, _resultedCoefficients);
                    sphereDefinition = "calculee";
                }

                newSphere = Instantiate(whiteSpheres[0], point, new Quaternion());
                newSphere.GetComponent<MeshRenderer>().material =
                    Resources.Load("Colors/Jaune", typeof(Material)) as Material;
                newSphere.name = "Sphere " + sphereDefinition + " ajoutee " + (_addedSpheres.Count + 1);

                _addedSpheres.Add(newSphere);

                Debug.Log("y = " + point.y);

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        Debug.Log("\n");
    }

    // Ajoute une boule avec une position aléaoire et calcule sa valeur si demandé (la coordonnée y)
    private void CreateRandomSphere(bool calculate)
    {
        Random randomer = new Random();

        double randomX1 = randomer.NextDouble() + randomer.Next(-10, 10);
        double randomX2 = randomer.NextDouble() + randomer.Next(-10, 10);
        double randomX3 = randomer.NextDouble() + randomer.Next(-10, 10);

        AddSphere(randomX1, randomX2, randomX3, calculate);
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