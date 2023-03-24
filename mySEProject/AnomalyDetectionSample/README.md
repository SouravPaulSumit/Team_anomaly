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
* Place training and testing datasets under training and testing folder respectively.

To run this project using github codespaces, set up .NET and Nuget manager in your environment. Place the training and testing folders in your project folder. The path to training and testing should be modified accordingly.

