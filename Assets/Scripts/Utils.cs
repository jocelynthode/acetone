using System;
using UnityEngine;
using Random = UnityEngine.Random;  // Use Unity's RNG


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
        // skew < 1: bigger numbers are favored
        // skew = 1: uniform distribution
        // skew > 1: smaller numbers are favored
        public int RandomInt()
        {
            int rand = Mathf.FloorToInt(Mathf.Pow(Random.value, distributionSkew) * (maximum - minimum + 1) + minimum);
            // In some rare cases rand could be bigger than max
            return (rand > maximum) ? rand - 1 : rand;
        }

    }

    public static void PlaySound(string name)
    {
        GameObject.Find("Sound").transform.FindChild(name).GetComponent<AudioSource>().Play();
    }
}

