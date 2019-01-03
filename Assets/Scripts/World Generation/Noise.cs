/*
 * Credit to Sebastian Lague for the tutorial on height map / low poly landscape generation
 * Find him on youtube at https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

    public enum NormalizeMode
    {
        local, global
    };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int seed, int octaves, float persistance, float lancunarity, Vector2 offset, NormalizeMode normalizeMode)
    {
        if (scale <= 0)
            scale = 0.0001f;

        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        //iterate over map
        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;

                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lancunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;

            }

        //normalize the values in the noise map 0 < x < 1
        for (int y = 0; y < mapHeight; y++) { 
            for (int x = 0; x < mapWidth; x++)
            {
                if (normalizeMode == NormalizeMode.local)
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]); //<---- not good for endless terrain chunks
                else
                {
                    float normalizedHeight = (noiseMap[x, y]  + 1) / (2f * maxPossibleHeight / 1.85f);
                    noiseMap[x, y] = Mathf.Clamp(0, normalizedHeight, int.MaxValue);
                }
            }
        }

                return noiseMap;
    }

    public static float[,] GenerateRoadMap(float[,] noiseMap)
    {
        float[,] road = new float[noiseMap.Length, noiseMap.Length];



        return road;
    }
}
