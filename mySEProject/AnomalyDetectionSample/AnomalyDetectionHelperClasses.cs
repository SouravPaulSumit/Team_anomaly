using NeoCortexApi;
using System.Diagnostics;

namespace NeoCortexApiSample
{
    /// <summary>
    /// Reads a single CSV file and returns its contents as a list of sequences.
    /// </summary>
    public class CSVFileReader
    {
        private string _filePathToCSV;

        /// <summary>
        /// Creates a new instance of the CSVFileReader class with the provided file path to the constructor.
        /// </summary>
        /// <param name="filePathToCSV">The path to the CSV file to be read.</param>
        public CSVFileReader(string filePathToCSV)
        {
            _filePathToCSV = filePathToCSV;
        }

        /// <summary>
        /// Reads the CSV file at the file path specified in the constructor,
        /// and returns its contents as a list of sequences.
        /// </summary>
        /// <returns>A list of sequences contained in the CSV file.</returns>
        public List<List<double>> ReadFile()
        {
            List<List<double>> sequences = new List<List<double>>();
            string[] csvLines = File.ReadAllLines(_filePathToCSV);
            // Loop through each line in the CSV File
            for (int i = 0; i < csvLines.Length; i++)
            {
                // Current line is split into an array of columns
                string[] columns = csvLines[i].Split(new char[] { ',' });
                List<double> sequence = new List<double>();
                // Loop through each column in the current line
                for (int j = 0; j < columns.Length; j++)
                {
                    // Parsing the current column as double,then adding it to the current sequence
                    sequence.Add(double.Parse(columns[j]));
                }
                sequences.Add(sequence);
            }
            return sequences;
        }

        /// <summary>
        /// This method reads the CSV file at the file path passed on to the constructor,
        /// and outputs its contents to the console.
        /// </summary>
        public void CSVSequencesConsoleOutput()
        {
            List<List<double>> sequences = ReadFile();
            // Looping through each sequence and displaying it in the console
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

    /// <summary>
    /// Reads the CSV files inside a folder and returns its contents as a list of sequences.
    /// </summary>
    public class CSVFolderReader
    {
        private string _folderPathToCSV;

        /// <summary>
        /// Creates a new instance of the CSVFolderReader class with the provided file path to the constructor.
        /// </summary>
        /// <param name="folderPathToCSV">The path to the folder containing the CSV files.</param>
        public CSVFolderReader(string folderPathToCSV)
        {
            _folderPathToCSV = folderPathToCSV;
        }

        /// <summary>
        /// Reads all CSV files in the folder, path to the folder is specified in the constructor,
        /// and returns its contents as a list of sequences.
        /// </summary>
        /// <returns>A list of sequences contained in the CSV files present in the folder.</returns>
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

        /// <summary>
        /// This method reads all CSV files in the folder path passed on to the constructor,
        /// and outputs its contents to the console.
        /// </summary>
        public void CSVSequencesConsoleOutput()
        {
            List<List<double>> sequences = ReadFolder();
            // Looping through each sequence and displaying it in the console
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

    /// <summary>
    /// Converts a list of sequences to a dictionary of sequences for facilitating HTM Engine training.
    /// </summary>
    public class CSVToHTMInput
    {
        /// <summary>
        /// Builds a dictionary of sequences from a list of sequences.
        /// An unique key is added, which is later used as an input for HtmClassifier.
        /// </summary>
        /// <param name="sequences">A list of sequences read from CSV files/files in a folder.</param>
        /// <returns>A dictionary of sequences required for HTM Engine training.</returns>
        public Dictionary<string, List<double>> BuildHTMInput(List<List<double>> sequences)
        {
            Dictionary<string, List<double>> dictionary = new Dictionary<string, List<double>>();
            for (int i = 0; i < sequences.Count; i++)
            {
                //Unique key created and added to dictionary for HTM Input                
                string key = "S" + (i + 1);
                List<double> value = sequences[i];
                dictionary.Add(key, value);
            }
            return dictionary;
        }
    }

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
            //Using stopwatch to calculate the total training time
            Stopwatch swh = Stopwatch.StartNew();

            //Read sequences from CSV files in the specified folder
            CSVFolderReader reader = new CSVFolderReader(folderPath);
            var sequences = reader.ReadFolder();

            //Convert sequences to HTM input format
            CSVToHTMInput converter = new CSVToHTMInput();
            var htmInput = converter.BuildHTMInput(sequences);

            //Starting multi-sequence learning experiment to generate predictor model
            //by passing htmInput 
            MultiSequenceLearning learning = new MultiSequenceLearning();
            predictor = learning.Run(htmInput);
            //Our HTM model training concludes here

            swh.Stop();

            Console.WriteLine();
            Console.WriteLine("------------------------------");
            Console.WriteLine();
            Console.WriteLine("HTM Model trained!! Training time is: " + swh.Elapsed.TotalSeconds + " seconds.");
            Console.WriteLine();
            Console.WriteLine("------------------------------");
        }
    }

    /// <summary>
    /// This class is responsible for testing an HTM model.
    /// Default training/ testing folder path is passed on to the constructor.
    /// Testing is carried out by training model using HTMModeltraining class,
    /// then CSVFolderReader is used to read all the sequences from all the CSV files inside testing folder,
    /// after that they are tested sequence by sequence. In the end, PredictAnomalyElement method will be used for
    /// testing a sequence as sliding window, one by one item in sequence.
    /// </summary>
    public class HTMAnomalyTesting
    {
        private readonly string _trainingFolderPath;
        private readonly string _testingFolderPath;

        /// <summary>
        /// Creates a new instance of the CSVFolderReader class with the provided file path to the constructor.
        /// Default training/ testing folder path is passed on to the constructor.
        /// </summary>
        /// <param name="trainingFolderPath">The path to the folder containing the CSV files for training.</param>
        ///<param name="testingFolderPath">The path to the folder containing the CSV files for testing.</param>
        public HTMAnomalyTesting(string trainingFolderPath = "training", string testingFolderPath = "testing")
        {
            //Folder directory set to location of C# files. This is the relative path.
            string projectbaseDirectory = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
            _trainingFolderPath = Path.Combine(projectbaseDirectory, trainingFolderPath);
            _testingFolderPath = Path.Combine(projectbaseDirectory, testingFolderPath);

        }

        /// <summary>
        /// Runs the anomaly detection experiment.
        /// </summary>
        public void AnomalyTestRun()
        {
            //HTM model training initiated
            HTMModeltraining myModel = new HTMModeltraining();
            Predictor myPredictor;

            myModel.RunHTMModelLearning(_trainingFolderPath, out myPredictor);

            Console.WriteLine("------------------------------");
            Console.WriteLine();
            Console.WriteLine("Started testing our trained HTM Engine...................");
            Console.WriteLine();

            //Starting to test our trained HTM model
            CSVFolderReader testseq = new CSVFolderReader(_testingFolderPath);
            var seq2seq = testseq.ReadFolder();
            myPredictor.Reset();

            //Testing the sequences one by one
            //Our anomaly detection experiment is complete after all the lists are traversed iteratively
            foreach (List<double> list in seq2seq)
            {
                double[] lst = list.ToArray();
                PredictAnomalyElement(myPredictor, lst);
            }

            Console.WriteLine();
            Console.WriteLine("------------------------------");
            Console.WriteLine();
            Console.WriteLine("Anomaly detection experiment complete!!.");
            Console.WriteLine();
            Console.WriteLine("------------------------------");
        }

        /// <summary>
        /// Predicts anomalies in the input list using the HTM trained model.
        /// The anomaly score is calculated using a sliding window approach.
        /// The difference between the predicted value and the actual value is used to calculate the anomaly score.
        /// If the difference exceeds a certain tolerance set earlier, anomaly is detected.
        /// </summary>
        /// <param name="predictor">Trained HTM model, used for prediction.</param>
        /// <param name="list">Input list which will be used to detect anomalies.</param>
        private static void PredictAnomalyElement(Predictor predictor, double[] list)
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine();
            Console.WriteLine("Testing the sequence for anomaly detection: " + string.Join(", ", list) + ".");

            //Tolerance level set to 10%.
            double tolerance = 0.1;

            //Input list will be traversed one by one, like a sliding window 
            for (int i = 0; i < list.Length; i++)
            {
                var item = list[i];

                //Using our trained HTM model predictor to predict next item.
                var res = predictor.Predict(item);
                Console.WriteLine("Current element in the testing sequence from input list: " + item);

                if (res.Count > 0)
                {
                    var tokens = res.First().PredictedInput.Split('_');
                    var tokens2 = res.First().PredictedInput.Split('-');
                    var tokens3 = res.First().Similarity;

                    if (i < list.Length - 1) // excluding the last element of the list
                    {
                        int nextIndex = i + 1;
                        double nextItem = list[nextIndex];
                        double predictedNextItem = double.Parse(tokens2.Last());
                        //Anomalyscore variable will be used to check the deviation from predicted item
                        var AnomalyScore = Math.Abs(predictedNextItem - nextItem);
                        var deviation = AnomalyScore / nextItem;

                        if (deviation <= tolerance)
                        {
                            Console.WriteLine("Anomaly not detected in the next element!! HTM Engine found similarity to be: " + tokens3 + "%.");
                        }
                        else
                        {
                            Console.WriteLine($"****Anomaly detected**** in the next element. HTM Engine predicted it to be {predictedNextItem} with similarity: {tokens3}%, but the actual value is {nextItem}.");
                            i++; // skip to the next element for checking
                            Console.WriteLine("As anomaly was detected, so we are skipping to the next element in our testing sequence.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("End of input list. Further anomaly testing cannot be continued.");
                        Console.WriteLine();
                        Console.WriteLine("------------------------------");
                    }
                }
                else
                {
                    Console.WriteLine("Nothing predicted from HTM Engine. Anomaly cannot be detected.");
                }
            }


        }
    }

}