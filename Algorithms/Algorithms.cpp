// Algorithms.cpp : définit les fonctions exportées pour l'application DLL.
//

#include "stdafx.h"
#include <cstdlib>


extern "C"
{

	double frand(double a, double b)
	{
		return (rand() / (double)RAND_MAX) * (b - a) + a;
	}

	_declspec(dllexport) int add_to_42(int value_to_add)
	{
		return value_to_add + 42;
	}


	_declspec(dllexport) int* linear_create(double* X, double* Y)
	{
		double a = frand(1, -1);
		double b = frand(1, -1);
		double c = frand(1, -1);


	}

	_declspec(dllexport) int* linear_remove(int* value_to_remove)
	{

	}

}
