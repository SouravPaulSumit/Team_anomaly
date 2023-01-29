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
                inputnumbers.Add(double.Parse(columns[_columnIndex]));
            }
            return inputnumbers;
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

            // RunMultiSimpleSequenceLearningExperiment();

            CSVFileReader cv = new CSVFileReader(@"D:\general\test_file.csv", 2);
            
            foreach (double k in cv.ReadFile())
            {
                Console.WriteLine(k);
            }
        }

        private static void RunMultiSimpleSequenceLearningExperiment()
        {
            Dictionary<string, List<double>> sequences = new Dictionary<string, List<double>>();
            List<double> testsequence = new List<double>();
            CSVFileReader cv = new CSVFileReader(@"D:\general\test_file.csv", 2);
            testsequence.AddRange(cv.ReadFile());

            sequences.Add("S1", new List<double>(new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, }));
            sequences.Add("S2", new List<double>(new double[] { 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0 }));

            //
            // Prototype for building the prediction engine.
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);

        }
    }

}
    

    
    