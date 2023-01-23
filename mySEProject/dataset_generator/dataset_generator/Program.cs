using System;
using System.Drawing;

namespace doublesequencearraygenerator;


public class DoubleSequenceArrayGenerator
{
    private Random _random;

    public DoubleSequenceArrayGenerator()
    {
        _random = new Random();
    }

    public double[] GenerateArray(int size)
    {
        double[] array = new double[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = _random.NextDouble();
        }
        return array;
    }

    static void Main(string[] args)
    {
        DoubleSequenceArrayGenerator generator = new DoubleSequenceArrayGenerator();
        double[] array = generator.GenerateArray(10);

        for (int i = 0; i < array.Length; i++)
        {
            Console.WriteLine(array[i]);
        }
    }



}



