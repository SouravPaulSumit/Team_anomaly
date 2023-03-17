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
using Microsoft.VisualBasic;
using System.Collections;

namespace NeoCortexApiSample
{
      
    public class CSVFileReader
    {
        private string _filePathToCSV;

        public CSVFileReader(string filePathToCSV)
        {
            _filePathToCSV = filePathToCSV;
        }

        public List<List<double>> ReadFile()
        {
            List<List<double>> sequences = new List<List<double>>();
            string[] csvLines = File.ReadAllLines(_filePathToCSV);
            for (int i = 0; i < csvLines.Length; i++)
            {
                string[] columns = csvLines[i].Split(new char[] { ',' });
                List<double> sequence = new List<double>();
                for (int j = 0; j < columns.Length; j++)
                {
                    sequence.Add(double.Parse(columns[j]));
                }
                sequences.Add(sequence);
            }
            return sequences;
        }

        public void CSVSequencesConsoleOutput()
        {
            List<List<double>> sequences = ReadFile();
            for (int i = 0; i < sequences.Count; i++)
            {
                Console.Write("Sequence " + (i + 1) + ": ");
                foreach (double number in sequences[i])
                {
                    Console.Write(number + " ");
                }
                Console.WriteLine("");
            }
        }
    }

    public class CSVFolderReader
    {
        private string _folderPathToCSV;

        public CSVFolderReader(string folderPathToCSV)
        {
            _folderPathToCSV = folderPathToCSV;
        }

        public List<List<double>> ReadFolder()
        {
            List<List<double>> folderSequences = new List<List<double>>();
            string[] fileEntries = Directory.GetFiles(_folderPathToCSV, "*.csv");
            foreach (string fileName in fileEntries)
            {
                string[] csvLines = File.ReadAllLines(fileName);
                List<List<double>> sequencesInFile = new List<List<double>>();
                for (int i = 0; i < csvLines.Length; i++)
                {
                    string[] columns = csvLines[i].Split(new char[] { ',' });
                    List<double> sequence = new List<double>();
                    for (int j = 0; j < columns.Length; j++)
                    {
                        sequence.Add(double.Parse(columns[j]));
                    }
                    sequencesInFile.Add(sequence);
                }
                folderSequences.AddRange(sequencesInFile);
            }
            return folderSequences;
        }

        public void CSVSequencesConsoleOutput()
        {
            List<List<double>> sequences = ReadFolder();
            for (int i = 0; i < sequences.Count; i++)
            {
                Console.Write("Sequence " + (i + 1) + ": ");
                foreach (double number in sequences[i])
                {
                    Console.Write(number + " ");
                }
                Console.WriteLine("");
            }
        }
    }
    
    public class CSVToHTMInput
    {
        public Dictionary<string, List<double>> BuildHTMInput(List<List<double>> sequences)
        {
            Dictionary<string, List<double>> dictionary = new Dictionary<string, List<double>>();
            for (int i = 0; i < sequences.Count; i++)
            {
                string key = "S" + (i + 1);
                List<double> value = sequences[i];
                dictionary.Add(key, value);
            }
            return dictionary;
        }
    }

    public class AnomalyThreshold
    {
        public double CalculateThreshold(List<List<double>> sequences)
        {
            List<double> allNumbers = new List<double>();
            foreach (List<double> sequence in sequences)
            {
                allNumbers.AddRange(sequence);
            }
            double mean = allNumbers.Average();
            double stdDev = Math.Sqrt(allNumbers.Average(v => Math.Pow(v - mean, 2)));

            double threshold = Math.Abs(mean + (3 * stdDev));
            double roundedthreshold = Math.Round(threshold);
            return roundedthreshold;
        }

        public void ShowThreshold(List<List<double>> sequences)
        {
            double threshold = CalculateThreshold(sequences);
            Console.WriteLine("The threshold value is: " + threshold);
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

            RunTestingMultiSequenceLearningExperiment();

            //RunModifiedMultiSimpleSequenceLearningExperiment();
            //RunMultiSimpleSequenceLearningExperiment();
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //RunMultiSequenceLearningExperiment();
            //TestLogMultisequenceExperiment(10);
            

            //CSVFileReader cv = new CSVFileReader(@"D:\general\test_file2.csv", 2);
            //cv.SequenceConsoleOutput();
            //CSVFileReader cv = new CSVFileReader(@"D:\general\test_file2.csv", 2);
            //cv.OutSeq();

            // stopwatch.Stop();
            // Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);

        }

        private static void RunTestingMultiSequenceLearningExperiment()
        {
            Dictionary<string, List<double>> sequences = new Dictionary<string, List<double>>();

            List<double> testsequence = new List<double>();

            CSVFileReader cv = new CSVFileReader(@"D:\general\test_file2.csv", 2);

            testsequence.AddRange(cv.ReadFile());

            List<double> finaltestsequence1 = testsequence.GetRange(0, 30);
            List<double> finaltestsequence2 = testsequence.GetRange(300, 30);
            List<double> finaltestsequence3 = testsequence.GetRange(600, 30);

            sequences.Add("S1", finaltestsequence1);
            sequences.Add("S2", finaltestsequence2);
            sequences.Add("S3", finaltestsequence3);


            //
            // Prototype for building the prediction engine.

            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);

            //
            // These list are used to see how the prediction works.
            // Predictor is traversing the list element by element. 
            // By providing more elements to the prediction, the predictor delivers more precise result.

            var list1 = TestAnomaly(310, 10);

            predictor.Reset();
            ExperimentPredict(predictor, list1);

        }

        private static void ExperimentPredict(Predictor predictor, double[] list)
        {
            
            foreach (var item in list)
            {
                var res = predictor.Predict(item);
                Console.WriteLine("Current element: " + item);
                if (res.Count > 0)
                {
                    var tokens = res.First().PredictedInput.Split('_');
                    var tokens2 = res.First().PredictedInput.Split('-');
                    Console.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens2.Last()}");
                }
                else
                    Console.WriteLine("Nothing predicted. Anomaly can't be detected");
            }
            
        }
        
        private static void TestPredictNextElement(Predictor predictor, double[] list)
        {
            double tolerance = 1;
            for (int i = 0; i < list.Length; i++)
            {
                var item = list[i];
                var res = predictor.Predict(item);
                Console.WriteLine("Current element: " + item);
                if (res.Count > 0)
                {
                    var tokens = res.First().PredictedInput.Split('_');
                    var tokens2 = res.First().PredictedInput.Split('-');
                    if (i < list.Length - 1) // exclude the last element of the list
                    {
                        int nextIndex = i + 1;
                        double nextItem = list[nextIndex];
                        double predictedNextItem = double.Parse(tokens2.Last());
                        Console.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {predictedNextItem}");
                        if (Math.Abs(predictedNextItem - nextItem) <= tolerance)
                        {
                            Console.WriteLine("Equal");
                        }
                        else
                        {
                            Console.WriteLine($"Not equal. Anomaly detected: predicted next element is {predictedNextItem}, actual next element is {nextItem}");
                        }
                    }
                    else if (i == list.Length - 1 && tokens2.Length > 1) // handle the last element of the list
                    {
                        double predictedNextItem = double.Parse(tokens2.Last());
                        double lastItem = list.Last();
                        Console.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {predictedNextItem}");
                        if (Math.Abs(predictedNextItem - lastItem) <= tolerance)
                        {
                            Console.WriteLine("Equal");
                        }
                        else
                        {
                            Console.WriteLine($"Not equal. Anomaly detected: predicted next element is {predictedNextItem}, actual next element is {lastItem}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("End of list. Anomaly can't be detected");
                    }
                }
                else
                {
                    Console.WriteLine("Nothing predicted. Anomaly can't be detected");
                }
            }
        }

        private static void RunMultiSimpleSequenceLearningExperiment()
        {
            Dictionary<string, List<double>> sequences = new Dictionary<string, List<double>>();

            List<double> testsequence = new List<double>();

            CSVFileReader cv = new CSVFileReader(@"D:\general\test_file2.csv", 2);

            testsequence.AddRange(cv.ReadFile());

            List<double> finaltestsequence1 = testsequence.GetRange(0, 50);
            List<double> finaltestsequence2 = testsequence.GetRange(50, 50);


            //sequences.Add("S1", new List<double>(new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, }));
            //sequences.Add("S1", new List<double>(finaltestsequence1));
            sequences.Add("S1", finaltestsequence1);
            //sequences.Add("S2", new List<double>(new double[] { 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0 }));

            sequences.Add("S2", finaltestsequence2);

            //
            // Prototype for building the prediction engine.
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);

        }

        private static void RunModifiedMultiSimpleSequenceLearningExperiment()
        {
            Dictionary<string, List<double>> sequences = new Dictionary<string, List<double>>();

            List<double> inputsequence = new List<double>();

            List<List<double>> listofsequences = new List<List<double>>();

            CSVFileReader cv = new CSVFileReader(@"D:\general\test_file2.csv", 2);

            inputsequence.AddRange(cv.ReadFile());

            //int[] rIndex = { 10, 15, 20, 25, 30, 35, 40, 45, 50 };
            //int[] rCount = { 5, 5, 5, 5, 5, 5, 5, 5, 5 };

            int step = 7;
            //rIndex start
            int rStart = 0;
            //number of rindexes
            int rNum = 25;
            //staticvalue
            int rStat = 7;
            int[] rIndex = Enumerable.Range(rStart, rNum).Select(x => x * step).ToArray();
            int[] rCount = Enumerable.Repeat(rStat, rNum).ToArray();

            for (int i = 0; i < rIndex.Length; i++)
            {
                List<double> singleseq = inputsequence.GetRange(rIndex[i], rCount[i]);
                listofsequences.Add(singleseq);
            }

            int countString = listofsequences.Count;


            List<string> stringstream = new List<string>();

            for (int i = 1; i <= countString; i++)
            {
                stringstream.Add("S" + i);
            }


            for (int i = 0; i < stringstream.Count; i++)
            {
                sequences.Add(stringstream[i], listofsequences[i]);
            }

            foreach (KeyValuePair<string, List<double>> item in sequences)
            {
                Console.WriteLine("Key: {0}", item.Key);
                Console.WriteLine("Values:");
                foreach (double value in item.Value)
                {
                    Console.WriteLine("  {0}", value);
                }
            }
            Console.WriteLine("Sequences fed into experiment");


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

            //sequences.Add("S1", new List<double>(new double[] { 0.0, 1.0, 2.0, 3.0, 4.0, 2.0, 5.0, }));
            //sequences.Add("S2", new List<double>(new double[] { 8.0, 1.0, 2.0, 9.0, 10.0, 7.0, 11.00 }));

            List<double> testsequence = new List<double>();

            CSVFileReader cv = new CSVFileReader(@"D:\general\test_file2.csv", 2);

            testsequence.AddRange(cv.ReadFile());

            List<double> finaltestsequence1 = testsequence.GetRange(0, 10);
            //List<double> finaltestsequence2 = testsequence.GetRange(600, 30);


            sequences.Add("S1", finaltestsequence1);
            //sequences.Add("S2", finaltestsequence2);



            //
            // Prototype for building the prediction engine.

            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);

            //
            // These list are used to see how the prediction works.
            // Predictor is traversing the list element by element. 
            // By providing more elements to the prediction, the predictor delivers more precise result.
            /*var list1 = new double[] { 1.0, 2.0, 3.0, 4.0, 2.0, 5.0 };
            var list2 = new double[] { 2.0, 3.0, 4.0 };
            var list3 = new double[] { 8.0, 1.0, 2.0 };*/

            //var list1_d = testsequence.GetRange(400, 30);
            //double[] list1 = list1_d.ToArray();

            //var list2_d = testsequence.GetRange(250, 6);
            //double[] list2 = list2_d.ToArray();
            //var list3_d = testsequence.GetRange(350, 6);
            //double[] list3 = list3_d.ToArray();



            //double[] list1 = TestAnomaly(1000, 10);

            //predictor.Reset();
            //PredictNextElement(predictor, list1);


            /*predictor.Reset();
            PredictNextElement(predictor, list2);

            predictor.Reset();
            PredictNextElement(predictor, list3);*/
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


        private static double[] TestAnomaly(int a, int b)
        {
            List<double> anomalytestsequence = new List<double>();

            CSVFileReader cv = new CSVFileReader(@"D:\general\test_file2.csv", 2);

            anomalytestsequence.AddRange(cv.ReadFile());

            var anomalytestlist1_d = anomalytestsequence.GetRange(a, b);

            double[] anomalytestlist1 = anomalytestlist1_d.ToArray();

            return anomalytestlist1;
            //return anomalytestlist1_d;

        }

                     

    }
}

