// Algorithms.cpp : définit les fonctions exportées pour l'application DLL.
//

#include "stdafx.h"
#include "Algorithms.h"
#include <cstdlib>
#include <iostream>


extern "C"
{
	__declspec(dllexport) const double LE_PAS = 0.5;
	__declspec(dllexport) const int NB_ITER = 1000000;


	__declspec(dllexport) double frand(double a, double b)
	{
		return a + ((double)rand() / (double)RAND_MAX) * (b - a);
	}

	__declspec(dllexport) double* linear_create(int nbInput)
	{
		double* ret = new double[nbInput];

		for(unsigned int i = 0 ; i < nbInput ; i++)
		{
			ret[i] = frand(-1, 1);
		}

		return ret;
	}

	__declspec(dllexport) double* linear_remove(double* valueToRemove)
	{
		// ?
		for(unsigned int i = 0; valueToRemove[i] != NULL; i++)
		{
			valueToRemove[i] = 0;
		}

		return valueToRemove;
	}

	__declspec(dllexport) int perceptron(double* coefficients, double* coordinates) {
		if (coefficients[0] * coordinates[0] + coefficients[1] * coordinates[1] + coefficients[2] > 0) {
			return 1;
		}

		return -1;
	}

	__declspec(dllexport) void linear_train_classification(double* coefficients, double* coordinates, double* values, int nbPoints)
	{
		for (unsigned int i = 0; i < NB_ITER; i++) {
			for(unsigned int j = 0; j < nbPoints; j++)
			{
				double* x = new double[2];
				x[0] = coordinates[2 * j];
				x[1] = coordinates[1 + (2 * j)];

				double y = values[j];

				int g = perceptron(coefficients, x);

				coefficients[0] = coefficients[0] + LE_PAS * (y - g) * x[0];
				coefficients[1] = coefficients[1] + LE_PAS * (y - g) * x[1];
				coefficients[2] = coefficients[2] + LE_PAS * (y - g);
			}
		}
	}

	__declspec(dllexport) void linear_train_regression(double* coefficients, double* coordinates, double* values, int nbPoints)
	{
		
	}

	__declspec(dllexport) void linear_train_multiple_regression(double* coefficients, double* coordinates, double* values, int nbPoints)
	{
		float sumxi2 = 0;
		float sumxiyi = 0;
		float sumxi = 0;
		float sumyi2 = 0;
		float sumyi = 0;
		float sumxizi = 0;
		float sumyizi = 0;
		float sumzi = 0;

		/* Regression multiple selon la methode des moindres carrés */
		for (unsigned int i = 0; i < nbPoints; ++i)
		{
			float x = coordinates[2 * i];
			float y = coordinates[(2 * i) + 1];
			float z = values[i];

			sumxi2 += x * x;
			sumxiyi += x * y;
			sumxi += x;
			sumyi2 += y * y;
			sumyi += y;
			sumxizi += x * z;
			sumyizi += y * z;
			sumzi += z;
		}

		/* Equation du plan : ax + by + c = z */
		coefficients[2] = ((sumxi * sumxizi - sumxi2 * sumzi) * (-sumxiyi * sumyi + sumxi * sumyi2) -
			((-sumxi2 * sumyi + sumxi * sumxiyi) * (sumxi * sumyizi - sumxiyi * sumzi))) /
			(-((-sumxi2 * sumyi + sumxi * sumxiyi) * (-sumxiyi * nbPoints + sumxi * sumyi)) +
			(-sumxiyi * sumyi + sumxi * sumyi2) * (-sumxi2 * nbPoints + sumxi * sumxi));
		coefficients[1] = ((sumxi * sumyizi - sumxiyi * sumzi) - (-sumxiyi * nbPoints + sumxi * sumyi) * coefficients[2]) /
			(-sumxiyi * sumyi + sumxi * sumyi2);
		coefficients[0] = (sumzi - sumyi * coefficients[1] - nbPoints * coefficients[2]) / sumxi;
	}

}
