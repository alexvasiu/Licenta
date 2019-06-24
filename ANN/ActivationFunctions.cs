using System;

namespace ANN
{
    [Serializable]
    public static class ActivationFunctions
    {
        public static Func<double, double> Linear = x => x;
        public static Func<double, double> Sigmoid = x => 1.0 / (1.0 + Math.Exp(-x));
        public static Func<double, double> Relu = x => Math.Max(0.0, x);
        public static Func<double, double> Tahn = Math.Tanh;

        public static Func<double, double> InverseLinear = x => x;
        public static Func<double, double> InverseSigmoid = x => x * (1 - x);
        public static Func<double, double> InverseRelu = x => Math.Max(0.0, x);
        public static Func<double, double> InverseTahn = x => 1 - Math.Pow(Math.Tanh(x), 2);
    }

    public enum AnnMode
    {
        Training = 0,
        Testing = 1
    }
}