// Algorithms.cpp : définit les fonctions exportées pour l'application DLL.
//

#include "stdafx.h"
#include "Algorithms.h"
#include <cstdlib>
#include <iostream>


extern "C"
{
	__declspec(dllexport) const double LE_PAS = 0.1;
	__declspec(dllexport) const int NB_ITER = 1000000;

	__declspec(dllexport) double frand(double a, double b)
	{
		return (rand() / (double)RAND_MAX) * (b - a) + a;
	}

	// X Liste coordonnées
	// Y Liste résultats attendu
	// Liste retourné:
	// [0] Nombre points
	// [1-3] Coefficients
	// [4-...] Coordonnées
	// [...-Dernier] Résultats attendus
	__declspec(dllexport) double* linear_create(int nbInput)
	{
		// Premier coefficient
		double a = frand(1, -1);
		// Second coefficient
		double b = frand(1, -1);
		// Troisième coefficient
		double c = frand(1, -1);

		double* ret = (double*)malloc(sizeof(double) * nbInput);

		ret[0] = a;
		ret[1] = b;
		ret[2] = c;

		return ret;
	}

	__declspec(dllexport) int perceptron(double * coefficients, double x1, double x2) {
		if (coefficients[1] * x1 + coefficients[2] * x2 + coefficients[0] > 0) {
			return 1;
		}

		return -1;
	}

	__declspec(dllexport) void linear_train_classification(double * coefficients, double * coordinates, double * values, int nbPoints)
	{
		for (int i = 0; i < NB_ITER; i++) {
			for (int j = 0; j < nbPoints; j++) {
				double x1 = coordinates[2 * j];
				double x2 = coordinates[1 + (2 * j)];
				double y = values[j];

				int g = perceptron(coefficients, x1, x2);

				coefficients[0] = coefficients[0] + LE_PAS * (y - g);
				coefficients[1] = coefficients[1] + LE_PAS * (y - g) * x1;
				coefficients[2] = coefficients[2] + LE_PAS * (y - g) * x2;
			}

			/*
			double randPts = frand(0, nbPoints - 1);

			double x1 = coordinates[(int)(2 * randPts)];
			double x2 = coordinates[(int)(1 + (2 * randPts))];

			double y = values[(int)(randPts)];

			int g = perceptron(coefficients, coordinates);

			coefficients[0] = coefficients[0] + LE_PAS * (y - g) * x1;
			coefficients[1] = coefficients[1] + LE_PAS * (y - g) * x2;
			coefficients[2] = coefficients[2] + LE_PAS * (y - g);
			*/
		}
	}

}
