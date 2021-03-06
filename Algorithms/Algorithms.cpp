// Algorithms.cpp : définit les fonctions exportées pour l'application DLL.
//

#include "stdafx.h"
#include "Algorithms.h"
#include <cstdlib>
#include <iostream>
#include <Eigen/Dense>
using Eigen::MatrixXd;


extern "C"
{
	__declspec(dllexport) const double LE_PAS = 0.5;
	__declspec(dllexport) const int NB_ITER = 1000000;


	__declspec(dllexport) double frand(double a, double b)
	{
		return a + ((double)rand() / (double)RAND_MAX) * (b - a);
	}

	__declspec(dllexport) double* linear_create(unsigned int nbCoefficients)
	{
		double* ret = new double[nbCoefficients];

		for (unsigned int i = 0; i < nbCoefficients; i++)
		{
			ret[i] = frand(-1, 1);
		}

		return ret;
	}

	// Pas utilisé, utilisation inconnue
	__declspec(dllexport) double* linear_remove(double* valueToRemove)
	{
		// ?
		for (unsigned int i = 0; valueToRemove[i] != NULL; i++)
		{
			valueToRemove[i] = 0;
		}

		return valueToRemove;
	}

	__declspec(dllexport) int perceptron(double* coefficients, double* coordinatesValues, unsigned int nbInputs)
	{
		double result = 0;

		for (unsigned int i = 0; i < nbInputs; i++)
		{
			result += coefficients[i] * coordinatesValues[i];
		}

		if (result > 0)
		{
			return 1;
		}

		return -1;
	}

	__declspec(dllexport) int multi_layer_perceptron(double* coefficients, double** coordinates)
	{
		// Do Something

		return -1;
	}

	__declspec(dllexport) void linear_train_classification(double* coefficients, double* coordinates, double* values,
	                                                       unsigned int nbPoints, unsigned int nbInputs)
	{
		const unsigned int nb_input_with_expedient = nbInputs + 1; // Nombre de valeurs + 1, pour compter le biais

		for (unsigned int i = 0; i < NB_ITER; i++)
		{
			for (unsigned int j = 0; j < nbPoints; j++)
			{
				double* x = new double[nb_input_with_expedient];

				for (unsigned int k = 0; k < nbInputs; k++)
				{
					x[k] = coordinates[j * nbInputs + k]; // Pour récupérer par tranche de valeurs (les valeurs sont successives)
				}

				x[nbInputs] = 1; // Biais

				double y = values[j];


				int g = perceptron(coefficients, x, nb_input_with_expedient);


				// Règle de Rosenblatt
				for (unsigned int l = 0; l < nb_input_with_expedient; l++)
				{
					coefficients[l] = coefficients[l] + LE_PAS * (y - g) * x[l];
				}
			}
		}
	}

	__declspec(dllexport) void linear_train_regression(double* coefficients, double* coordinates, double* values,
	                                                   unsigned int nbPoints, unsigned int nbInputs)
	{
		const unsigned int nb_input_with_expedient = nbInputs + 1; // Nombre de valeurs + 1, pour compter le biais

		MatrixXd X(nbPoints, nb_input_with_expedient);
		MatrixXd Y(nbPoints, 1);

		for (unsigned int i = 0; i < nbPoints; i++)
		{
			for (unsigned int j = 0; j < nbInputs; j++)
			{
				X(i, j) = coordinates[i * nbInputs + j]; // Pour récupérer par tranche de valeurs (les valeurs sont successives)
			}

			X(i, nbInputs) = 1; // Biais

			Y(i, 0) = values[i];
		}

		MatrixXd X1 = X.transpose();
		MatrixXd X2 = X1 * X;
		MatrixXd inverse = X2.inverse();

		MatrixXd w = inverse * X1 * Y;

		for (unsigned int k = 0; k < nb_input_with_expedient; k++)
		{
			coefficients[k] = w(k);
		}
	}

	// Est limité car non générique
	__declspec(dllexport) void linear_train_no_matrix_library_regression(double* coefficients, double* coordinates,
	                                                                     double* values, unsigned int nbPoints)
	{
		double sumxi = 0;
		double sumxi2 = 0;

		double sumyi = 0;
		double sumyi2 = 0;

		double sumzi = 0;

		double sumxiyi = 0;
		double sumxizi = 0;
		double sumyizi = 0;

		/* Regression multiple selon la methode des moindres carrés */
		for (unsigned int i = 0; i < nbPoints; i++)
		{
			double x = coordinates[i * 2];
			double y = coordinates[i * 2 + 1];
			double z = values[i];

			sumxi += x;
			sumxi2 += x * x;

			sumyi += y;
			sumyi2 += y * y;

			sumzi += z;

			sumxiyi += x * y;
			sumxizi += x * z;
			sumyizi += y * z;
		}

		/* Equation du plan : ax + by + c = z */
		coefficients[2] =
			(
				(sumxi * sumxizi - sumxi2 * sumzi) * (-sumxiyi * sumyi + sumxi * sumyi2)
				- (-sumxi2 * sumyi + sumxi * sumxiyi) * (sumxi * sumyizi - sumxiyi * sumzi)
			)
			/
			(
				- (-sumxi2 * sumyi + sumxi * sumxiyi) * (-sumxiyi * nbPoints + sumxi * sumyi)
				+ (-sumxiyi * sumyi + sumxi * sumyi2) * (-sumxi2 * nbPoints + sumxi * sumxi)
			);

		coefficients[1] =
			(
				(sumxi * sumyizi - sumxiyi * sumzi)
				- (-sumxiyi * nbPoints + sumxi * sumyi) * coefficients[2])
			/
			(-sumxiyi * sumyi + sumxi * sumyi2);

		coefficients[0] =
			(sumzi - sumyi * coefficients[1] - nbPoints * coefficients[2])
			/ sumxi;
	}
}
