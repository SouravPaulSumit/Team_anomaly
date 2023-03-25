# ML22/23-12: Implement Anomaly Detection Sample

# This page is currently under development.

## Project members:

* Anurag De (Matriculation Number: 1400450)
* Sourav Paul Sumit (Matriculation Number: 1344118)

# Introduction:

The [MultiSequenceLearning](https://github.com/ddobric/neocortexapi/blob/master/source/Samples/NeoCortexApiSample/MultisequenceLearning.cs) sample demonstrates how multiple sequences can be learned and used for prediction by using the experimental class [Predictor](https://github.com/ddobric/neocortexapi/blob/master/source/Samples/NeoCortexApiSample/Program.cs). The way how that works is that, once all the sequences are learned by the HTM engine, the trained engine can be used to guess the next element by presenting a list of elements using the predictor. 

We have used this to create a [program](https://github.com/SouravPaulSumit/Team_anomaly/blob/master/mySEProject/AnomalyDetectionSample/Program.cs), that can be used to detect anomalies in sequences intended for testing. In the beginning, our HTM engine learns the sequences which are present in csv files placed inside the [training](https://github.com/SouravPaulSumit/Team_anomaly/tree/master/mySEProject/AnomalyDetectionSample/training) folder, under the [project](https://github.com/SouravPaulSumit/Team_anomaly/tree/master/mySEProject/AnomalyDetectionSample) folder. The program can be used to read multiple csv files, and then convert the read sequences to a suitable [format](https://github.com/SouravPaulSumit/Team_anomaly/blob/b2ab0330d07dfec1c8ccdf051a6a14da37dd8558/mySEProject/AnomalyDetectionSample/AnomalyDetectionHelperClasses.cs#L135) for HTM training. After that, our HTM engine is trained using those sequences, and then the sequences read from [testing](https://github.com/SouravPaulSumit/Team_anomaly/tree/master/mySEProject/AnomalyDetectionSample/testing) folder is used for prediction. Generally, the predictor predicts the next value when some value from predicting file is presented. The next appearing value is compared with the last predicted value. If presenting value and matching value are the same (including defined tolerance), there is no anomaly in the input sequence. If values mismatch, the anomaly is detected. 

We have used the same principle in our program to check anomalies in our testing sequences. We have implemented a [method](https://github.com/SouravPaulSumit/Team_anomaly/blob/b2ab0330d07dfec1c8ccdf051a6a14da37dd8558/mySEProject/AnomalyDetectionSample/AnomalyDetectionHelperClasses.cs#L278) for anomaly detection, which uses a sliding window approach to read through a single tested sequence one by one, and then compare the predicted value from our trained HTM engine with the actual value. We use [anomalyscore](https://github.com/SouravPaulSumit/Team_anomaly/blob/b2ab0330d07dfec1c8ccdf051a6a14da37dd8558/mySEProject/AnomalyDetectionSample/AnomalyDetectionHelperClasses.cs#L308) to check anomalies in our tested sequence. The tolerance level is preset to 10%, and if the actual value deviates by more than 10%, then anomaly is detected and displayed to the user.  

# Requirements

To run this project, we need.
* [.NET 7.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
* Nuget package: [NeoCortexApi Version= 1.1.4](https://www.nuget.org/packages/NeoCortexApi/)

For code debugging, we recommend using visual studio IDE/visual studio code. This project can be run on [github codespaces](https://github.com/features/codespaces) as well.

# Usage

To run this project, 

* Install .NET SDK. Then using code editor/IDE of your choice, create a new console project and place all the C# codes inside your project folder. 
* Add/reference nuget package NeoCortexApi v1.1.4 to this project.
* Place training and testing datasets under training and testing folder respectively. Both the folders should be inside the project folder.

Our project is based on NeoCortex API. More details [here](https://github.com/ddobric/neocortexapi/blob/master/source/Documentation/gettingStarted.md).

# Details

We have used [MultiSequenceLearning](https://github.com/ddobric/neocortexapi/blob/master/source/Samples/NeoCortexApiSample/MultisequenceLearning.cs) as the base of this project.

For this project, we are using a sample time series data of network load (rounded off to nearest integer, in precentage) for training. The data is kept inside the csv files in the following format:

```
49,52,55,48,52,47,46,50,52,47
49,52,55,48,52,47,46,50,49,47
.............................
.............................
48,54,55,48,52,47,46,50,49,45
51,54,55,48,52,47,46,50,49,45
```
Plotting the graph of the sequences from our training data, we get the following. Normally, the values stay within the range of 45 to 55. 

![alt text](https://github.com/SouravPaulSumit/Team_anomaly/blob/Sourav_Paul_Sumit/mySEProject/AnomalyDetectionSample/output/training_data_plot.jpg)

For testing our project, we have taken parts of sequences of our training data, and added anomalies to this data for predicting and testing.
```
55,48,52,47,46,99,52,47
52,55,48,52,47,46,90,49,97
......................
......................
55,48,52,47,16,50,49,45
97,46,50,49,75
```

Plot of this data is given below. Marked red values indicate the anomalies in the testing data.

![alt text](https://github.com/SouravPaulSumit/Team_anomaly/blob/Sourav_Paul_Sumit/mySEProject/AnomalyDetectionSample/output/testing_data_plot.jpg)

## Encoding:

Encoding of this data is very important. More on [this](https://github.com/ddobric/neocortexapi/blob/master/source/Documentation/Encoders.md).

As we are going to train and test data between the range of integer values between 0-100 with no periodicity, we are using the following settings.

Minimum and maximum values are set to 0 and 100 respectively.

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

## HTM Configuration:

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


## Multisequence learning

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
We will use this for prediction.
