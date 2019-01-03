using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public TerrainData terrainData;
    public NoiseData noiseData;
    public TextureData textureData;

    public Material terrainMaterial;

    public enum DrawMode
    {
        NoiseMap, /*ColorMap,*/ Mesh, FalloffMap
    }
    public DrawMode drawMode;

    public static int mapChunkSize = 239;//95;

    [Range(0,6)]
    public int editorPreviewLevelOfDetail;

    public bool autoUpdate;

    //public TerrainType[] regions;

    Queue<MapThreadInfo<MapData>> mapDataTheadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataTheadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    private void Awake()
    {
        textureData.ApplyToMaterial(terrainMaterial);
        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
    }

    float[,] falloffMap;

    void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }

    public void DrawMapInEditor()
    {
        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);

        MapData mapData = GenerateMapData(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        /*else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }*/
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, editorPreviewLevelOfDetail, terrainData.useFlatShading)/*, TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize)*/);
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(falloffMap));
        }
    }

    public MapData GenerateMapData(Vector2 center)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.noiseScale, noiseData.seed, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, center + noiseData.offset, noiseData.normMode);

        if (terrainData.useFalloff)
        {

            if (falloffMap == null)
            {
                falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize + 2);
            }

            //color the map
            //Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
            for (int y = 0; y < mapChunkSize+2; y++)
            {
                for (int x = 0; x < mapChunkSize+2; x++)
                {
                    if (terrainData.useFalloff)
                        noiseMap[x, y] = Mathf.Clamp(0, noiseMap[x, y] - falloffMap[x, y], 1);

                    //float currentHeight = noiseMap[x, y];

                    /*for (int i = 0; i < regions.Length; i++)
                    {
                        if (currentHeight >= regions[i].height)
                        {
                            colorMap[y * mapChunkSize + x] = regions[i].color;
                        }
                        else
                        {
                            break;
                        }
                    }*/
                }
            }
        }

        return new MapData(noiseMap/*, colorMap*/);
    }

    public void ReqestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    //inside of thread
    void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(center);
        lock (mapDataTheadInfoQueue)
        {
            mapDataTheadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int levelOfDetail, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, levelOfDetail, callback);
        };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, int levelOfDetail, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, levelOfDetail, terrainData.useFlatShading);
        lock(meshDataTheadInfoQueue)
        {
            meshDataTheadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    void Update()
    {
        if (terrainData.reset || noiseData.reset)
        {
            textureData.ApplyToMaterial(terrainMaterial);   
            terrainData.reset = false;
            noiseData.reset = false;
        }


        if (mapDataTheadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataTheadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataTheadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataTheadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataTheadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataTheadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    private void OnValidate()
    {
        if (terrainData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }
        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

/*[System.Serializable]
public struct TerrainType
{
    public float height;
    public Color color;
    public string name;
}*/

public struct MapData
{
    public readonly float[,] heightMap;
    //public readonly Color[] colorMap;

    public MapData(float[,] heightMap/*, Color[] colorMap*/)
    {
        this.heightMap = heightMap;
        //this.colorMap = colorMap;
    }
}