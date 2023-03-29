# ML22/23-12: Implement Anomaly Detection Sample


# Introduction:

HTM (Hierarchical Temporal Memory) is a machine learning algorithm, which uses a hierarchical network of nodes to process time-series data in a distributed way. Each nodes, or columns, can be trained to learn, and recognize patterns in input data. This can be used in identifying anomalies/deviations from normal patterns. It is a promising approach for anomaly detection and prediction in a variety of applications. In our project, we are going to use multisequencelearning class in NeoCortex API to implement an anomaly detection system, such that numerical sequences are read from multiple csv files inside a folder, train our HTM Engine using the same class, and use the trained engine for learning patterns and detect anomalies.  

# Requirements

To run this project, we need.
* [.NET 7.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
* Nuget package: [NeoCortexApi Version= 1.1.4](https://www.nuget.org/packages/NeoCortexApi/)

For code debugging, we recommend using visual studio IDE/visual studio code. This project can be run on [github codespaces](https://github.com/features/codespaces) as well.

# Usage

To run this project, 

* Install .NET SDK. Then using code editor/IDE of your choice, create a new console project and place all the C# codes inside your project folder. 
* Add/reference nuget package NeoCortexApi v1.1.4 to this project.
* Place numerical sequence CSV Files (datasets) under relevant folders respectively. All the folders should be inside the project folder. More details given below.

Our project is based on NeoCortex API. More details [here](https://github.com/ddobric/neocortexapi/blob/master/source/Documentation/gettingStarted.md).

# Details

We have used [MultiSequenceLearning](https://github.com/ddobric/neocortexapi/blob/master/source/Samples/NeoCortexApiSample/MultisequenceLearning.cs) class in NeoCortex API for training our HTM Engine. We are going to start by reading and using data from both our training (learning) folder (present as numerical sequences in CSV Files in 'training' folder inside project directory) and predicting folder (present as numerical sequences in CSV Files in 'predicting' folder inside project directory) to train HTM Engine. For testing purposes, we are going to read numerical sequence data from predicting folder and remove the first few elements (essentially, making it subsequence of the original sequence; we already added anomalies in this data at random indexes), and then use it to detect anomalies.

Please note that all files are read with .csv extension inside the folders, and exception handlers are in place if the format of the files are not in proper order.

For this project, we are using artificial integer sequence data of network load (rounded off to nearest integer, in precentage), which are stored inside the csv files. Example of a csv file within training folder.

```
49,52,55,48,52,47,46,50,52,47
49,52,55,48,52,47,46,50,49,47
.............................
.............................
48,54,55,48,52,47,46,50,49,45
51,54,55,48,52,47,46,50,49,45
```
Normally, the values stay within the range of 45 to 55. For testing, we consider anything outside this range to be an anomaly. We have uploaded the graphs of our data in this repository for reference. 

1. Graph for numerical sequence data from training folder (without anomalies) can be found [here](https://github.com/SouravPaulSumit/Team_anomaly/blob/master/mySEProject/AnomalyDetectionSample/output/training_data_for_supervised_learn_plot.jpg).
2. Graph of combined numerical sequence data from training folder (without anomalies) and predicting folder (with anomalies) can be found [here](https://github.com/SouravPaulSumit/Team_anomaly/blob/master/mySEProject/AnomalyDetectionSample/output/combined_data_for_unsup_learn_data_plot.jpg).

### Encoding:

Encoding of our input data is very important, such that it can be processed by our HTM Engine. More on [this](https://github.com/ddobric/neocortexapi/blob/master/source/Documentation/Encoders.md). 

As we are going to train and test data between the range of integer values between 0-100 with no periodicity, we are using the following settings. Minimum and maximum values are set to 0 and 100 respectively, as we are expecting all the values to be in this range only. In other used cases, these values need to be changed.

```csharp

int inputBits = 121;
int numColumns = 1210;
.......................
.......................
double max = 100;

Dictionary<string, object> settings = new Dictionary<string, object>()
            {
                { "W", 21},
                ...........
                { "MinVal", 0.0},
                ...........
                { "MaxVal", max}
            };
 ```
 
 Complete settings:
 
 ```csharp

Dictionary<string, object> settings = new Dictionary<string, object>()
            {
                { "W", 21},
                { "N", inputBits},
                { "Radius", -1.0},
                { "MinVal", 0.0},
                { "Periodic", false},
                { "Name", "integer"},
                { "ClipInput", false},
                { "MaxVal", max}
            };
```

### HTM Configuration:

We have used the following configuration. More on [this](https://github.com/ddobric/neocortexapi/blob/master/source/Documentation/SpatialPooler.md#parameter-desription)

```csharp
{
                Random = new ThreadSafeRandom(42),

                CellsPerColumn = 25,
                GlobalInhibition = true,
                LocalAreaDensity = -1,
                NumActiveColumnsPerInhArea = 0.02 * numColumns,
                PotentialRadius = (int)(0.15 * inputBits),
                //InhibitionRadius = 15,

                MaxBoost = 10.0,
                DutyCyclePeriod = 25,
                MinPctOverlapDutyCycles = 0.75,
                MaxSynapsesPerSegment = (int)(0.02 * numColumns),

                ActivationThreshold = 15,
                ConnectedPermanence = 0.5,

                // Learning is slower than forgetting in this case.
                PermanenceDecrement = 0.25,
                PermanenceIncrement = 0.15,

                // Used by punishing of segments.
                PredictedSegmentDecrement = 0.1
};
```

### Multisequence learning

The [RunExperiment](https://github.com/SouravPaulSumit/Team_anomaly/blob/be27813af65f611df7cbd33009d72a3ee72e3756/mySEProject/AnomalyDetectionSample/multisequencelearning.cs#L75) method inside the [multisequencelearning](https://github.com/SouravPaulSumit/Team_anomaly/blob/master/mySEProject/AnomalyDetectionSample/multisequencelearning.cs) class file demonstrates how multisequence learning works. To summarize, 

* HTM Configuration is taken and memory of connections are initialized. After that, HTM Classifier, Cortex layer and HomeostaticPlasticityController are initialized.
```csharp
.......
var mem = new Connections(cfg);
.......
HtmClassifier<string, ComputeCycle> cls = new HtmClassifier<string, ComputeCycle>();
CortexLayer<object, object> layer1 = new CortexLayer<object, object>("L1");
HomeostaticPlasticityController hpc = new HomeostaticPlasticityController(mem, numUniqueInputs * 150, (isStable, numPatterns, actColAvg, seenInputs) => ..
.......
.......
```

* After that, Spatial Pooler and Temporal Memory is initialized.
```csharp
.....
TemporalMemory tm = new TemporalMemory();
SpatialPoolerMT sp = new SpatialPoolerMT(hpc);
.....
```
* After that, spatial pooler memory is added to cortex layer and trained for maximum number of cycles.
```csharp
.....
layer1.HtmModules.Add("sp", sp);
int maxCycles = 3500;
for (int i = 0; i < maxCycles && isInStableState == false; i++)
.....
`````
* After that, temporal memory is added to cortex layer to learn all the input sequences.
```csharp
.....
layer1.HtmModules.Add("tm", tm);
foreach (var sequenceKeyPair in sequences){
.....
}
.....
```
* Finally, the trained cortex layer and HTM classifier is returned.
```csharp
.....
return new Predictor(layer1, mem, cls)
.....
`````
We will use this for prediction in later parts of our project.

## Execution of the project

Our project is executed in the following way. 

* In the beginning, we use ReadFolder method of [CSVFolderReader](https://github.com/SouravPaulSumit/Team_anomaly/blob/master/mySEProject/AnomalyDetectionSample/CSVFolderReader.cs) class to read all the files placed inside a folder. Alternatively, we can use ReadFile method of [CSVFileReader](https://github.com/SouravPaulSumit/Team_anomaly/blob/master/mySEProject/AnomalyDetectionSample/CSVFileReader.cs) to read a single file; it works in a similar way, except that it reads a single file. These classes store the read sequences to a list of numeric sequences, which will be used in a number of occasions later. These classes have exception handling implemented inside for handling non-numeric data. Data can be trimmed using TrimSequences method, which will be used in our unsupervised approach. Trimsequences method trims one to four elements(Number 1 to 4 is decided randomly) from the beginning of a numeric sequence and returns it.

```csharp
 public List<List<double>> ReadFolder()
        {
         ....  
          return folderSequences;
        }

public static List<List<double>> TrimSequences(List<List<double>> sequences)
        {
        ....
          return trimmedSequences;
        }
```

* After that, the method BuildHTMInput of [CSVToHTMInput](https://github.com/SouravPaulSumit/Team_anomaly/blob/master/mySEProject/AnomalyDetectionSample/CSVToHTMInput.cs) class converts all the read sequences to a format suitable for HTM training.
```csharp
Dictionary<string, List<double>> dictionary = new Dictionary<string, List<double>>();
for (int i = 0; i < sequences.Count; i++)
    {
     // Unique key created and added to dictionary for HTM Input                
     string key = "S" + (i + 1);
     List<double> value = sequences[i];
     dictionary.Add(key, value);
    }
     return dictionary;
```
* After that, we use RunHTMModelLearning method of [HTMModeltraining](https://github.com/SouravPaulSumit/Team_anomaly/blob/master/mySEProject/AnomalyDetectionSample/HTMModeltraining.cs) class to train our model using the converted sequences in supervised approach. This class returns our trained model object predictor.
```csharp
.....
MultiSequenceLearning learning = new MultiSequenceLearning();
predictor = learning.Run(htmInput);
.....
```

For unsupervised approach, we use RunHTMModelLearning method of [UnsupervisedHTMModeltraining](https://github.com/SouravPaulSumit/Team_anomaly/blob/master/mySEProject/AnomalyDetectionSample/UnsupervisedHTMModeltraining.cs), which works in a similar manner, except of one major feature. It combines the numerical data sequences from training (for learning) and predicting folders. RunHTMModelLearning method also takes one extra parameter (path of predicting folder)
```csharp
.....
List<List<double>> combinedSequences = new List<List<double>>(sequences1);
combinedSequences.AddRange(sequences2);
.....
```
* In the end, we use [HTMAnomalyTesting](https://github.com/SouravPaulSumit/Team_anomaly/blob/master/mySEProject/AnomalyDetectionSample/HTMAnomalyTesting.cs) to detected anomalies in sequences read from files inside testing folder using supervised approach. All the steps explained earlier- CSV files reading, converting them for HTM training and training the HTM engine using HTMModelTraining class are done here. We use the same class (CSVFolderReader) to read files for our testing sequences.
```csharp
CSVFolderReader testseq = new CSVFolderReader(_testingFolderPath);
var inputtestseq = testseq.ReadFolder();
```
Path to training and testing folder is set as default and passed on the constructor, or can be set inside the class manually.
```csharp
.....
_trainingFolderPath = Path.Combine(projectbaseDirectory, trainingFolderPath);
_testingFolderPath = Path.Combine(projectbaseDirectory, testingFolderPath);
.....
```

In case of our unsupervised approach, we use [UnsupervisedHTMModelTesting](https://github.com/SouravPaulSumit/Team_anomaly/blob/master/mySEProject/AnomalyDetectionSample/UnsupervisedHTMAnomalyTesting.cs) class, which also works in a similar fashion as HTMAnomalyTesting class, except for the following.

We pass paths to training and predicting folder to the constructor.

```csharp
.....
 _trainingFolderPath = Path.Combine(projectbaseDirectory, trainingFolderPath);
_predictingFolderPath = Path.Combine(projectbaseDirectory, predictingFolderPath);
.....
```
and, TrimSequences method is used to trim sequences for testing. Method for trimming is already explained earlier.

In the end, DetectAnomaly method is used to detect anomalies in our test sequences one by one, using our trained HTM Model predictor. For supervise learning, the numerical sequences from testing data is passed to the loop. 
```csharp
foreach (List<double> list in inputtestseq)
       {
         .....
         double[] lst = list.ToArray();
         DetectAnomaly(myPredictor, lst);
       }
```
For unsupervised approach, trimmed sequences are passed to the loop. 

Exception handling is coded, such that errors thrown from DetectAnomaly method can be handled(like passing of non-numeric values, or number of elements in list less than two).

DetectAnomaly method works similarly for both our approaches. It is the main method which detects anoamlies in our data. It traverses each value of a list one by one in a sliding window manner, and uses trained model predictor to predict the next element for comparison. We use an anomalyscore to quantify the comparison and detect anomalies; if the prediction crosses a certain tolerance level, it is declared as an anomaly.

In our sliding window approach, naturally the first element is skipped, so we ensure that the first element is checked for anomaly in the beginning.

We can get our prediction in a list of results in format of "NeoCortexApi.Classifiers.ClassifierResult`1[System.String]" from our trained model Predictor using the following:

```csharp
var res = predictor.Predict(item);
```
Here, assume that item passed to the model is of int type with value 8. We can use this to analyze how prediction works. When this is executed,
```csharp
foreach (var pred in res)
 {
   Console.WriteLine($"{pred.PredictedInput} - {pred.Similarity}");
    }
```
We get the following output.
```
S2_2-9-10-7-11-8-1 - 100
S1_1-2-3-4-2-5-0 - 5
S1_-1.0-0-1-2-3-4 - 0
S1_-1.0-0-1-2-3-4-2 - 0
```
We know that the item we passed here is 8. The first line gives us the best prediction with similarity accuracy. We can easily get the predicted value which will come after 8 (here, it is 1), and previous value (11, in this case). We use basic string operations to get our required values.

We will then use this to detect anomalies.

* When we iteratively pass values to DetectAnomaly method using our sliding window approach, we will not be able to detect anomaly in the first element. So, in the beginning, we use the second element of the list to predict and compare the previous element(which is the first element). A flag is set to control the command execution; if the first element has anomaly, then we will not use it to detect our second element. We will directly start from first element. Otherwise, we will start from first element as usual.

* Now, when we traverse the list one by one to the right, we pass the value to the predictor to get the next value and compare the prediction with the actual value. If there's anomaly, then it is outputted to the user, and the anomalous element is skipped. Upon reaching to the last element, we can end our traversal and move on to next list.

In both the scenarios, we use anomalyscore (difference ratio) for comparison with our preset threshold. When it exceeds, probable anomalies are found.

To test out data using supervised, or unsupervised approach, just use the relevant class/methods given in [Program.cs](https://github.com/SouravPaulSumit/Team_anomaly/blob/master/mySEProject/AnomalyDetectionSample/Program.cs).

```csharp
  //This is the supervised approach
 HTMAnomalyTesting tester = new HTMAnomalyTesting();
 tester.Run();

 // This uses the unsupervised approach.
 UnsupervisedHTMAnomalyTesting tester1 = new UnsupervisedHTMAnomalyTesting();
 tester1.Run();
```
 
# Results

After running this project, we got the following output:

* Supervised approach:

Our supervised approach got the following [results](https://github.com/SouravPaulSumit/Team_anomaly/tree/master/mySEProject/AnomalyDetectionSample/output/supervised).

* Unsupervised approach:

Our unsupervised approach got the following [results](https://github.com/SouravPaulSumit/Team_anomaly/tree/master/mySEProject/AnomalyDetectionSample/output/unsupervised).

We can observed that lower false positive rate is observed in our supervised approach (Avg of both runs: 0.15) than unsupervised one (0.24), It is desired that false positive rate should be as lower as possible in an anomaly detection program. Lower false negative rate is also desirable in some cases.

Although, it depends on a number of factors, like quantity(the more, the better) and quality of data, and hyperparameters used to tune and train model. In both our approaches, more data should be used for training, and hyperparameters should be further tuned to find the most optimal setting for training to get the best results. We were using less amount of data sequences to demonstrate our sample project due to time and computational constraints, but that can be improved if we use better resources, like cloud.
