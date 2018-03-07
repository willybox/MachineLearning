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
		return a + ((double)rand() / RAND_MAX) * (b - a);
	}

	__declspec(dllexport) double* linear_create(int nbInput)
	{
		double* ret = new double[nbInput];

		for(int i = 0 ; i < nbInput ; i++)
		{
			ret[i] = frand(1, -1);
		}

		return ret;
	}

	__declspec(dllexport) double* linear_remove(double * valueToRemove)
	{
		// Do something

		return new double[3];
	}

	__declspec(dllexport) int perceptron(double * coefficients, double * coordinates) {
		if (coefficients[0] * coordinates[0] + coefficients[1] * coordinates[1] + coefficients[2] > 0) {
			return 1;
		}

		return -1;
	}

	__declspec(dllexport) void linear_train_classification(double * coefficients, double * coordinates, double * values, int nbPoints)
	{
		for (int i = 0; i < NB_ITER; i++) {
			int randPts = rand() % (nbPoints - 1);

			while (randPts % 2 != 0)
			{
				randPts = rand() % (nbPoints - 1);
			}

			double* x = new double[2];
			x[0] = coordinates[randPts];
			x[1] = coordinates[1 + randPts];

			double y = values[randPts];

			int g = perceptron(coefficients, x);

			coefficients[0] = coefficients[0] + LE_PAS * (y - g) * x[0];
			coefficients[1] = coefficients[1] + LE_PAS * (y - g) * x[1];
			coefficients[2] = coefficients[2] + LE_PAS * (y - g);
		}
	}

	__declspec(dllexport) void linear_train_regression(double * coefficients, double * coordinates, double * values, int nbPoints)
	{
		
	}

}
