using System;
using UnityEngine;
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.


public static class Utils
{
    // Using Serializable allows us to embed a class with sub properties in the inspector.
    [Serializable]
    public class Range
    {
        public int minimum;  // minimum is inclusive
        public int maximum;  // maximum is inclusive
        public float distributionSkew;

        public Range(int min, int max, float distributionSkew = 1)
        {
            minimum = min;
            maximum = max;
            this.distributionSkew = distributionSkew;
        }

        // http://forum.unity3d.com/threads/random-range-with-decreasing-probability.50596/
        public int RandomInt()
        {
            int rand = Mathf.FloorToInt(Mathf.Pow(Random.value, distributionSkew) * (maximum - minimum + 1) + minimum);
            return (rand > maximum) ? rand - 1 : rand;
        }

    }
}

