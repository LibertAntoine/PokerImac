using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using MathNet.Numerics.Integration;
using System.Runtime.InteropServices;

public class AleaVar : MonoBehaviour
{
    System.Random _rand = new System.Random();

    void Start() {}

    public double LoiBeta(double a, double b) // Par rejet
    {
        double u;
        double v;
        double w;
        double x = (a - 1) / (a + b - 2);
        do
        {
            u = _rand.NextDouble();
            v = Math.Pow(x, a - 1) * Math.Pow(1 - x, b - 1) * _rand.NextDouble();
            w = Math.Pow(u, a - 1) * Math.Pow(1 - u, b - 1);
        } while (v > w);






        /*
        //print("u" + u);
        double result =
            (SimpsonRule.IntegrateComposite(x => Math.Pow(x, a - 1) * Math.Pow(1 - x, b - 1) , 0.0, u, 4)
            / SimpsonRule.IntegrateComposite(x => Math.Pow(x, a - 1) * Math.Pow(1 - x, b - 1), 0.0, 1, 4));
        //print("intégrale1" + SimpsonRule.IntegrateThreePoint(x => Math.Pow(x, a - 1) * Math.Pow(1 - x, b - 1), 0.0, u));
        //print("intégrale2" + SimpsonRule.IntegrateThreePoint(x => Math.Pow(x, a - 1) * Math.Pow(1 - x, b - 1), 0.0, 1));

        //print(result);*/
        return u;
    }
    public double LoiNormale(double moyenne, double variance) // Alogrithme Polaire.
    {
        double X = 0;
        double Y = 0;
        double R2 = 0;
        do
        {
            X = _rand.NextDouble() * 2 - 1;
            Y = _rand.NextDouble() * 2 - 1;
            R2 = X * X + Y * Y;
        } while (R2 > 1);

        double r = Math.Sqrt(R2);
        double R = 2 * Math.Sqrt(-Math.Log(r)) / r;
        double result = moyenne + Math.Sqrt(variance) * R * X;

        //print("X: " + result);
        return result;
    }
    
    public double LoiGamma(double a) // Par reget.
    {
        double b = Math.Exp(1) / (a + Math.Exp(1));
        double x, y, z, v;
        do
        {
            x = _rand.NextDouble();
            y = _rand.NextDouble();
            z = (x < b) ? Math.Pow(x / b, 1 / a) : -Math.Log((1 - x) / a / b);
            v = (z < 1) ? Math.Exp(-z) : Math.Pow(z, a - 1);
        } while (y > v);

        return z;
    }
}
