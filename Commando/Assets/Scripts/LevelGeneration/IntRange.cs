using System;

// Serializable so it will show up in the inspector.
[Serializable]
public class IntRange
{
    public int MMin;       // The minimum value in this range.
    public int MMax;       // The maximum value in this range.


    // Constructor to set the values.
    public IntRange(int min, int max)
    {
        MMin = min;
        MMax = max;
    }


    // Get a random value from the range.
    public int Random
    {
        get { return UnityEngine.Random.Range(MMin, MMax); }
    }
}