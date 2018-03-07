#pragma once
_declspec(dllexport) double frand(double a, double b);
_declspec(dllexport) int add_to_42(int value_to_add);
_declspec(dllexport) double* linear_create(double* X, double* Y, int nbPoints);
_declspec(dllexport) int perceptron(double a, double b, double c, double x1, double x2);
_declspec(dllexport) void linear_train_classification(double * ptr);