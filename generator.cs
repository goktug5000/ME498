using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class generator : MonoBehaviour
{

    private const double Sqrt3 = 1.73205080757; //√3 sabiti

    // E0 hesaplama fonksiyonu
    private static double E0(double K, double Phi, double f, double En, double Tn, double w, double n, double pn)
    {
        double IFL = K * 1000 / (Math.Sqrt(3) * En); // full-load current in A
        double IL = IFL * 0.05; // no-load current in A
        double R = En / IFL; // internal resistance in ohms

        double I0 = IL + (Tn / (w * n * En)); // no-load current as a function of torque

        double pi = Math.PI;

        double E0 = (En - (I0 * R)) + (2 * pi * f * Phi * n) / (pn * w);
        return E0/Sqrt3;
    }

    // Vn hesaplama fonksiyonu
    private static double Vn(double En, double E0)
    {
        return (2 * En) / (Sqrt3 * E0);
    }

    // If hesaplama fonksiyonu
    private static double If(double H, double Phi)
    {
        return H / (Math.Sqrt(2) * Phi);
    }

    // teta n hesaplama fonksiyonu
    private static double ThetaN(double w, double Tn, double pn)
    {
        return w * Tn / pn;
    }

    // an hesaplama fonksiyonu
    private static double AlphaN(double w, double Tn, double pn, double n)
    {
        return -Math.Atan((n * w * Tn) / (2 * pn));
    }


    [SerializeField] static double E0value;
    [SerializeField] static double Vnvalue;
    [SerializeField] static double Ifvalue;
    [SerializeField] static double ThetaNvalue;

    // Karakteristik denklem fonksiyonu
    public static double CharacteristicEquation(double K, double Phi, double f, double En, double H, double w, double n, double pn, double Tn, double t)
    {
        E0value = E0(K, Phi, f, En, Tn, w, n, pn);
        Vnvalue = Vn(En, E0value);
        Ifvalue = If(H, Phi);
        ThetaNvalue = ThetaN(w, Tn, pn);
        /*
            K: Jeneratörün kendine özgü sabiti.
            Φ: Jeneratörün manyetik alan akısıdır.
            f: Jeneratörün nominal frekansıdır.

         */
        /*
            E, E0, and Vn: voltage, measured in volts (V).
            If: current, measured in amperes (A).
            θn and αn: angle, measured in radians (rad).
            p: dimensionless.
            ω: angular frequency, measured in radians per second (rad/s).
            t: time, measured in seconds (s).
        */
        double sum = 0;
        for (int i = 1; i <= n; i++)
        {
            double AlphaNvalue = AlphaN(w, Tn, pn, i);
            sum += ((2 * Vnvalue * Ifvalue * Math.Cos(i * ThetaNvalue) / (pn * Math.PI)) * Math.Sin(i * w * t + AlphaNvalue));
        }
        Debug.Log("Sum: " + sum);
        return E0value + sum;
    }
    public static double CalculatePout(double voltage, double current, double powerFactor)
    {
        // Convert power factor from degrees to radians
        double powerFactorRadians = Math.PI * powerFactor / 180.0;

        // Calculate Pout using the formula Pout = V x I x cos(θ)
        double Pout = voltage * current * Math.Cos(powerFactorRadians);

        return Pout;
    }


}
