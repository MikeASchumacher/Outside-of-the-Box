using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdateableData {

    public bool useFalloff;
    public bool useFlatShading;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public float uniformScale = 6f;

    public bool reset = false;

    public float minHeight
    {
        get
        {
            return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(1);
        }
    }
}
