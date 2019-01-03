using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdateableData {

    public float noiseScale;

    public int octaves;

    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;

    public Noise.NormalizeMode normMode;

    public bool reset = false;

    #if UNITY_EDITOR

    protected override void OnValidate()
    {
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;

        base.OnValidate();
    }
    #endif
}
