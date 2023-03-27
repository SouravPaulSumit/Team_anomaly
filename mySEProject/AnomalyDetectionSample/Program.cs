using System;

namespace AnomalyDetectionSample
{

    class Program
    {
        static void Main(string[] args)
        {
            // Starts experiment that demonstrates how to perform anomaly detection using multisequencelearning.
            HTMAnomalyTesting tester = new HTMAnomalyTesting();
            tester.Run();

            // Starts experiment that demonstrates how to perform anomaly detection using multisequencelearning.
            // This uses the unsupervised approach.
            // UnsupervisedHTMAnomalyTesting tester1 = new UnsupervisedHTMAnomalyTesting();
            // tester.Run();

        }

    }
}