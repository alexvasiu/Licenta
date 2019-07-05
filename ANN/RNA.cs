using System;
using System.Collections.Generic;
using System.Linq;

namespace ANN
{
    [Serializable]
    public class RNA
    {
        /*
         * Data = points * genre list
         */

        private readonly List<(List<double>, string)> _traningData;
        public List<(List<double>, string)> TestData;
        private readonly Func<double, double> _activationFunction;
        private readonly Func<double, double> _activationFunctionInverse;

        private readonly List<string> _genres = new List<string> { "blues", "classical", "country", "disco", "hiphop", "jazz", "metal", "pop", "reggae", "rock" };

        private readonly Random _random = new Random();
        private AnnMode _mode;
        public RNA(List<(List<double>, string)> data, AnnMode mode, Func<double, double> activationFunction)
        {
            _mode = mode;
            _activationFunction = activationFunction;

            if (_activationFunction == ActivationFunctions.Linear)
                _activationFunctionInverse = ActivationFunctions.InverseLinear;
            else if (_activationFunction == ActivationFunctions.Sigmoid)
                _activationFunctionInverse = ActivationFunctions.InverseSigmoid;
            else if (_activationFunction == ActivationFunctions.Relu)
                _activationFunctionInverse = ActivationFunctions.InverseRelu;
            else if (_activationFunction == ActivationFunctions.Tahn)
                _activationFunctionInverse = ActivationFunctions.InverseTahn;

            var random = new Random();

            if (mode == AnnMode.Training)
                _traningData = data?.Where(x => random.NextDouble() < 0.8).ToList();
            else
            {
                data.Shuffle();
                var stop = (int) (data.Count * 0.8);
                _traningData = data; // data.Take(stop).ToList();
                TestData = data.Skip(stop).ToList();
            }

            _traningData?.Shuffle();
        }

        private List<double> GenerateList(int length)
        {
            var list = new List<double>();
            for (var i = 0; i < length; i++)
                list.Add(_random.NextDouble());
            return list;
        }

        public List<List<Neuron>> NetInit(int noOutputs, int noHidden)
        {
            var hiddenLayer = new List<Neuron>();
            var outputLayer = new List<Neuron>();
            
            for (var i = 0; i < noHidden; i++)
                hiddenLayer.Add(new Neuron
                {
                    Output = 0,
                    Delta = 0,
                    Weights = GenerateList(_traningData[0].Item1.Count + 1)
                });

            for (var i = 0; i < noOutputs; i++)
                outputLayer.Add(new Neuron
                {
                    Output = 0,
                    Delta = 0,
                    Weights = GenerateList(noHidden + 1)
                });

            return new List<List<Neuron>> {hiddenLayer, outputLayer};
        }

        private static double Activate(IEnumerable<double> inputs, IReadOnlyList<double> weights)
        {
            return inputs.Select((t, i) => t * weights[i]).Sum() + weights.Last();
        }

        public double Transfer(double value) => _activationFunction(value);

        private List<double> ForwardPropagation(List<double> inputs, ref List<List<Neuron>> net)
        {
            foreach (var layer in net)
            {
                var newInputs = new List<double>();
                foreach (var neuron in layer)
                {
                    var activation = Activate(inputs, neuron.Weights);
                    neuron.Output = Transfer(activation);
                    newInputs.Add(neuron.Output);
                }

                inputs = newInputs;
            }

            return inputs;
        }

        private double TransferInverse(double value) => _activationFunctionInverse(value);

        private void BackPropagation(IReadOnlyList<int> expected, ref List<List<Neuron>> net)
        {
            for (var i = net.Count - 1; i >= 0; i--)
            {
                var currentLayer = net[i];
                var errors = new List<double>();
                if (i == net.Count - 1)
                    errors.AddRange(currentLayer.Select((currentNeuron, j) => expected[j] - currentNeuron.Output));
                else
                {
                    for (var j = 0; j < currentLayer.Count; j++)
                    {
                        var currentErr = 0.0;
                        var nextLayer = net[i + 1];
                        foreach (var neuron in nextLayer)
                            currentErr += neuron.Weights[j] * neuron.Delta;
                        errors.Add(currentErr);
                    }
                }

                for (var j = 0; j < currentLayer.Count; j++)
                    currentLayer[j].Delta = errors[j] * TransferInverse(currentLayer[j].Output);
            }
        }

        private int GetValueFromLabel(string label) => _genres.IndexOf(label);

        private string GetValue(int label) => _genres[label];

        private void UpdateWeights(ref List<List<Neuron>> net, List<double> example, double learningRate)
        {
            for (var i = 0; i < net.Count; i++)
            {
                var inputs = example;
                if (i > 0)
                    inputs = net[i - 1].Select(x => x.Output).ToList();
                foreach (var neuron in net[i])
                {
                    for (var j = 0; j < inputs.Count; j++)
                        neuron.Weights[j] += learningRate * neuron.Delta * inputs[j];
                    neuron.Weights[neuron.Weights.Count - 1] += learningRate * neuron.Delta;
                }
            }
        }

        public void Training(ref List<List<Neuron>> net, int noOutputs, double learningRate, int noEpochs)
        {
            for (var i = 0; i < noEpochs; i++)
            {
                Console.WriteLine(i);
               // var sumError = 0.0;
                foreach (var data in _traningData)
                {
                    var inputs = data.Item1;
                    var output = data.Item2;

                    var computedOutputs = ForwardPropagation(inputs, ref net);
                    var expected = Enumerable.Repeat(0, noOutputs).ToList();
                    expected[GetValueFromLabel(output)] = 1;
                    var computedLabels = Enumerable.Repeat(0.0, noOutputs).ToList();
                    computedLabels[computedOutputs.IndexOf(computedOutputs.Max())] = 1;
                    /*computedOutputs = computedLabels;

                    crt_err = sum([(expected[i] - computed_outputs[i]) * *2 for i in range(0, len(expected))])
                    sum_error += crt_err*/
                    BackPropagation(expected, ref net);
                    UpdateWeights(ref net, inputs, learningRate);
                }
            }
        }

        public List<List<double>> Evaluate(ref List<List<Neuron>> net, int noOutputs)
        {
            var computedOutputs = new List<List<double>>();
            foreach (var data in TestData)
            {
                var computedOutput = ForwardPropagation(data.Item1, ref net);
                var computedLabels = Enumerable.Repeat(0.0, noOutputs).ToList();
                computedLabels[computedOutput.IndexOf(computedOutput.Max())] = 1;
                computedOutput = computedLabels;
                computedOutputs.Add(computedOutput);
            }

            return computedOutputs;
        }

        public List<string> Evaluate(ref List<List<Neuron>> net, int noOutputs, List<List<double>> testData)
        {
            var computedOutputs = new List<List<double>>();
            foreach (var data in testData)
            {
                var computedOutput = ForwardPropagation(data, ref net);
                var computedLabels = Enumerable.Repeat(0.0, noOutputs).ToList();
                computedLabels[computedOutput.IndexOf(computedOutput.Max())] = 1;
                computedOutput = computedLabels;
                computedOutputs.Add(computedOutput);
            }

            return computedOutputs.Select(x => GetValueFromLabelVector(x)).ToList();
        }

        public Dictionary<string, int> GetGeneres() => _genres.ToDictionary(x => x, x => 0);

        public string GetValueFromLabelVector(List<double> value)
        {
            for (var i = 0; i < value.Count; i++)
                if ((int) value[i] == 1)
                    return _genres[i];
            return null;
        }

        public double ComputePerformance(List<List<double>> computedOutputs, List<string> realOutputs)
        {
            var noMatches = computedOutputs.Select((x, i) => GetValueFromLabelVector(x) == realOutputs[i] ? 1 : 0)
                .Sum();
            return noMatches * 100.0 / realOutputs.Count * 1.0;
        }
    } 
}