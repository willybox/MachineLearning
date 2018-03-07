// Algorithms.cpp : définit les fonctions exportées pour l'application DLL.
//

#include "stdafx.h"
#include <cstdlib>

extern "C"
{

	const double LE_PAS = 0.1;
	const int NB_ITER = 10;

	__declspec(dllexport) double frand(double a, double b)
	{
		return (rand() / (double)RAND_MAX) * (b - a) + a;
	}

	_declspec(dllexport) int add_to_42(int value_to_add)
	{
		return value_to_add + 42;
	}

	// X Liste coordonnées
	// Y Liste résultats attendu
	// Liste retourné:
	// [0] Nombre points
	// [1-3] Coefficients
	// [4-...] Coordonnées
	// [...-Dernier] Résultats attendus
	_declspec(dllexport) double* linear_create(double* X, double* Y, int nbPoints)
	{
		// Premier coefficient
		double a = frand(1, -1);
		// Second coefficient
		double b = frand(1, -1);
		// Troisième coefficient
		double c = frand(1, -1);

		double* ret = (double*) malloc(nbPoints*4);

		ret[0] = nbPoints;
		ret[1] = a;
		ret[2] = b;
		ret[3] = c;

		int index = 4;
		for (int i = 0; i < 2 * nbPoints; i++) {
			ret[index] = X[i];
			index++;
		}
		for (int i = 0; i < nbPoints; i++) {
			ret[index] = Y[i];
			index++;
		}

		return ret;
	}

	_declspec(dllexport) int perceptron(double a, double b, double c, double x1, double x2) {
		if (a * x1 + b * x2 + c > 0) {
			return 1;
		}
		return -1;
	}

	_declspec(dllexport) void linear_train_classification(double * ptr)
	{
		double nbPoints = ptr[0];
		double a = ptr[1];
		double b = ptr[2];
		double c = ptr[3];

		int index = 4;
		double* X = (double*)malloc(nbPoints * 2);
		double* Y = (double*)malloc(nbPoints);

		for (int i = 0; i < 2 * nbPoints; i++) {
			X[i] = ptr[index];
			index++;
		}
		for (int i = 0; i < nbPoints; i++) {
			Y[i] = ptr[index];
			index++;
		}

		for (int i = 0; i < NB_ITER; i++) {
			double randPts = frand(0, nbPoints - 1);

			double x1 = X[(int)(4 + (2 * randPts))];
			double x2 = X[(int)(5 + (2 * randPts))];

			double y = X[(int)(4 + (2 * nbPoints) + randPts)];

			int g = perceptron(a, b, c, x1, x2);

			a = a + LE_PAS * (y - g) * x1;
			b = b + LE_PAS * (y - g) * x2;
			c = c + LE_PAS * (y - g);
		}

		free(X);
		free(Y);
	}

}
