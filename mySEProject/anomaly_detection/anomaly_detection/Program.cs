using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NeoCortexApi;
using NeoCortexApi.Encoders;
using static NeoCortexApiSample.MultiSequenceLearning;
using NeoCortexApiSample;
using System.Reflection.Metadata.Ecma335;
using System.Formats.Asn1;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NeoCortexApiSample
{
    class CSVFileReader
    {
        private string _filePathToCSV;
        private int _columnIndex;


        //data from https://github.com/numenta/NAB/blob/master/data/realTraffic/speed_7578.csv

        public CSVFileReader(string filePathToCSV = "", int columnIndex = 0)
        {
            _filePathToCSV = filePathToCSV;
            _columnIndex = columnIndex;
        }

        public List<double> ReadFile()
        {
            // method implementation
            List<double> inputnumbers = new List<double>();
            string[] csvLines = File.ReadAllLines(_filePathToCSV);
            for (int i = 1; i < csvLines.Length; i++)
            {
                string[] columns = csvLines[i].Split(new char[] { ',', '"' });
                inputnumbers.Add((double.Parse(columns[_columnIndex])/10));
            }
            return inputnumbers;
        }

        public void SequenceConsoleOutput() {

           foreach (double k in ReadFile())
            {
                Console.WriteLine(k);
            }

        }
    }
    
    class Program
    {
        /// <summary>
        /// This sample shows a typical experiment code for SP and TM.
        /// You must start this code in debugger to follow the trace.
        /// and TM.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            //
            // Starts experiment that demonstrates how to learn spatial patterns.
            //SpatialPatternLearning experiment = new SpatialPatternLearning();
            //experiment.Run();

            //
            // Starts experiment that demonstrates how to learn spatial patterns.
            //SequenceLearning experiment = new SequenceLearning();
            //experiment.Run();

            RunMultiSimpleSequenceLearningExperiment();
            //CSVFileReader cv = new CSVFileReader(@"D:\general\test_file2.csv", 2);
            //cv.SequenceConsoleOutput();


        }
        
        private static void RunMultiSimpleSequenceLearningExperiment()
        {
            Dictionary<string, List<double>> sequences = new Dictionary<string, List<double>>();

            List<double> testsequence = new List<double>();
            
            CSVFileReader cv = new CSVFileReader(@"D:\general\test_file2.csv", 2);

            testsequence.AddRange(cv.ReadFile());

            List<double> finaltestsequence1 = testsequence.GetRange(0,6);
            List<double> finaltestsequence2 = testsequence.GetRange(14,20);

            //sequences.Add("S1", new List<double>(new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, }));
            sequences.Add("S1", new List<double>(finaltestsequence1));
            //sequences.Add("S2", new List<double>(new double[] { 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0 }));
            sequences.Add("S2", new List<double>(finaltestsequence2));
            
            //
            // Prototype for building the prediction engine.
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);

        }
        private static void RunMultiSequenceLearningExperiment()
        {
            Dictionary<string, List<double>> sequences = new Dictionary<string, List<double>>();

            //sequences.Add("S1", new List<double>(new double[] { 0.0, 1.0, 0.0, 2.0, 3.0, 4.0, 5.0, 6.0, 5.0, 4.0, 3.0, 7.0, 1.0, 9.0, 12.0, 11.0, 12.0, 13.0, 14.0, 11.0, 12.0, 14.0, 5.0, 7.0, 6.0, 9.0, 3.0, 4.0, 3.0, 4.0, 3.0, 4.0 }));
            //sequences.Add("S2", new List<double>(new double[] { 0.8, 2.0, 0.0, 3.0, 3.0, 4.0, 5.0, 6.0, 5.0, 7.0, 2.0, 7.0, 1.0, 9.0, 11.0, 11.0, 10.0, 13.0, 14.0, 11.0, 7.0, 6.0, 5.0, 7.0, 6.0, 5.0, 3.0, 2.0, 3.0, 4.0, 3.0, 4.0 }));

            sequences.Add("S1", new List<double>(new double[] { 0.0, 1.0, 2.0, 3.0, 4.0, 2.0, 5.0, }));
            sequences.Add("S2", new List<double>(new double[] { 8.0, 1.0, 2.0, 9.0, 10.0, 7.0, 11.00 }));

            //
            // Prototype for building the prediction engine.
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);

            //
            // These list are used to see how the prediction works.
            // Predictor is traversing the list element by element. 
            // By providing more elements to the prediction, the predictor delivers more precise result.
            var list1 = new double[] { 1.0, 2.0, 3.0, 4.0, 2.0, 5.0 };
            var list2 = new double[] { 2.0, 3.0, 4.0 };
            var list3 = new double[] { 8.0, 1.0, 2.0 };

            predictor.Reset();
            PredictNextElement(predictor, list1);

            predictor.Reset();
            PredictNextElement(predictor, list2);

            predictor.Reset();
            PredictNextElement(predictor, list3);
        }
        private static void PredictNextElement(Predictor predictor, double[] list)
        {
            Debug.WriteLine("------------------------------");

            foreach (var item in list)
            {
                var res = predictor.Predict(item);

                if (res.Count > 0)
                {
                    foreach (var pred in res)
                    {
                        Debug.WriteLine($"{pred.PredictedInput} - {pred.Similarity}");
                    }

                    var tokens = res.First().PredictedInput.Split('_');
                    var tokens2 = res.First().PredictedInput.Split('-');
                    Debug.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens2.Last()}");
                }
                else
                    Debug.WriteLine("Nothing predicted :(");
            }

            Debug.WriteLine("------------------------------");
        }
    }
}