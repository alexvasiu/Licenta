using System;
using System.Collections.Generic;

namespace ANN
{
    [Serializable]
    public class Neuron
    {
        public List<double> Weights { get; set;}
        public double Output { get; set; }
        public double Delta { get; set; } = 0.0;
    }
}