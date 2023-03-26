using NeoCortexApi;
using System;
using System.Diagnostics;

namespace AnomalyDetectionSample
{
    /// <summary>
    /// This class is responsible for training an HTM model.
    /// The trained model will be further used to detect anomalies. 
    /// </summary>
    public class HTMModeltraining
    {
        /// <summary>
        /// Runs the HTM model learning experiment on a folder containing CSV files, and returns the trained model.
        /// </summary>
        /// <param name="folderPath">The path to the folder containing the CSV files to be used for training the model.</param>
        /// <param name="predictor">The trained model that will be used for prediction.</param>
        public void RunHTMModelLearning(string folderPath, out Predictor predictor)
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine();
            Console.WriteLine("Starting our anomaly detection experiment!!");
            Console.WriteLine();
            Console.WriteLine("------------------------------");
            Console.WriteLine();
            Console.WriteLine("HTM Model training initiated...................");
            // Using stopwatch to calculate the total training time
            Stopwatch swh = Stopwatch.StartNew();

            // Read sequences from CSV files in the specified folder
            // CSVFileReader class can also be used for single files
            CSVFolderReader reader = new CSVFolderReader(folderPath);
            var sequences = reader.ReadFolder();

            // Convert sequences to HTM input format
            CSVToHTMInput converter = new CSVToHTMInput();
            var htmInput = converter.BuildHTMInput(sequences);

            // Starting multi-sequence learning experiment to generate predictor model
            // by passing htmInput 
            MultiSequenceLearning learning = new MultiSequenceLearning();
            predictor = learning.Run(htmInput);

            // Our HTM model training concludes here

            swh.Stop();

            Console.WriteLine();
            Console.WriteLine("------------------------------");
            Console.WriteLine();
            Console.WriteLine("HTM Model trained!! Training time is: " + swh.Elapsed.TotalSeconds + " seconds.");
            Console.WriteLine();
            Console.WriteLine("------------------------------");
        }
    }
}
