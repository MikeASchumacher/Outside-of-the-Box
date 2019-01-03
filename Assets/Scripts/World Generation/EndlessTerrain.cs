using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour {

    const float updateThreshold = 25f;
    const float updateThresholdSqr = updateThreshold * updateThreshold;

    public Transform viewer;
    public CarControl car;

    public LevelOfDetailInfo[] detailLevels;
    public static float maxViewDistance;

    public static Vector2 viewerPosition;
    static Vector2 prevViewerPosition;

    int chunkSize;
    int chunksVisibleInViewDistance;

    Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    static MapGenerator mapGenerator;

    public Material mapMaterial;
    public TextureData textureData;

    public TerrainChunk GetBelowChunk(Vector2 pos)
    {
        foreach (TerrainChunk chunk in terrainChunkDict.Values)
        {
            if (chunk.ContainsPoint(pos))
                return chunk;
        }

        return null;
    }

    public void Start()
    {
        maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);
        mapGenerator = FindObjectOfType<MapGenerator>();

        textureData.ApplyToMaterial(mapMaterial);

        UpdateVisibleChunks();
        car.TerrainFinish();
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / mapGenerator.terrainData.uniformScale;
        if ((prevViewerPosition - viewerPosition).sqrMagnitude > updateThresholdSqr)
        {
            prevViewerPosition = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {

        //go through chunks visible last update
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOff = -chunksVisibleInViewDistance; yOff <= chunksVisibleInViewDistance; yOff++)
        {
            for (int xOff = -chunksVisibleInViewDistance; xOff <= chunksVisibleInViewDistance; xOff++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOff, currentChunkCoordY + yOff);

                if(terrainChunkDict.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDict[viewedChunkCoord].UpdateTerrainChunk();
                }
                else
                {
                    //instantiate new chunk 
                    terrainChunkDict.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk
    {
        public Vector2 position;
        GameObject meshObject;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;

        LevelOfDetailInfo[] detailLevels;
        LevelOfDetailMesh[] levelOfDetailMeshes;
        LevelOfDetailMesh collisionMesh;

        public MapData mapData;
        public bool mapDataReceived;

        int prevLevelOfDetailIndex = -1;
        Material material2;
        Transform par;

        public TerrainChunk(Vector2 coord, int size, LevelOfDetailInfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;

            material2 = material;
            par = parent;

            position = coord * size;
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);
            bounds = new Bounds(position, Vector2.one * size);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;
            meshCollider = meshObject.AddComponent<MeshCollider>();

            meshObject.transform.position = positionV3 * mapGenerator.terrainData.uniformScale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * mapGenerator.terrainData.uniformScale;
            SetVisible(false);

            levelOfDetailMeshes = new LevelOfDetailMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                levelOfDetailMeshes[i] = new LevelOfDetailMesh(detailLevels[i].LevelOfDetail, UpdateTerrainChunk);
                if (detailLevels[i].useForCollider)
                {
                    collisionMesh = levelOfDetailMeshes[i];
                }
            }

            mapGenerator.ReqestMapData(position, OnMapDataReceived);
        }

        public void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceived = true;
            /*Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;*/

            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (mapDataReceived)
            {
                float viewDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewDistanceFromNearestEdge <= maxViewDistance;

                if (visible)
                {
                    int levelOfDetailIndex = 0;
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewDistanceFromNearestEdge > detailLevels[i].visibleDistanceThreshold)
                        {
                            levelOfDetailIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (levelOfDetailIndex != prevLevelOfDetailIndex)
                    {
                        LevelOfDetailMesh LODMesh = levelOfDetailMeshes[levelOfDetailIndex];
                        if (LODMesh.hasMesh)
                        {
                            prevLevelOfDetailIndex = levelOfDetailIndex;
                            meshFilter.mesh = LODMesh.mesh;
                        }
                        else if (!LODMesh.hasRequestedMesh)
                        {
                            LODMesh.RequestMesh(mapData);
                        }

                    }

                    if (levelOfDetailIndex == 0)
                    {
                        if (collisionMesh.hasMesh)
                        {
                            meshCollider.sharedMesh = collisionMesh.mesh;
                        }
                        else if (!collisionMesh.hasRequestedMesh)
                        {
                            collisionMesh.RequestMesh(mapData);
                        }
                    }

                    //add to list of chunks visible last update
                    terrainChunksVisibleLastUpdate.Add(this);
                }

                SetVisible(visible);
            }
        }

        public void SetVisible(bool visible)
        {

            if (meshObject == null)
            {
                meshObject = new GameObject("Terrain Chunk");
                meshRenderer = meshObject.AddComponent<MeshRenderer>();
                meshFilter = meshObject.AddComponent<MeshFilter>();
                meshRenderer.material = material2;
                meshCollider = meshObject.AddComponent<MeshCollider>();
                Vector3 positionV3 = new Vector3(position.x, 0, position.y);
                meshObject.transform.position = positionV3 * mapGenerator.terrainData.uniformScale;
                meshObject.transform.parent = par;
                meshObject.transform.localScale = Vector3.one * mapGenerator.terrainData.uniformScale;
            }

            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }

        public Vector3[] GetVertices()
        {
            List<Vector3> vertices = new List<Vector3>();
            meshFilter.mesh.GetVertices(vertices);
            return vertices.ToArray();
        }

        public bool ContainsPoint(Vector2 pos)
        {
            return bounds.Contains(pos);
        }

        public Vector3 GetNearestVertex(Vector3 point)
        {
            float minDistanceSqr = Mathf.Infinity;
            Vector3 nearestVertex = Vector3.zero;

            foreach (Vector3 vertex in GetVertices())
            {
                Vector3 diff = point - vertex;
                float distSqr = diff.sqrMagnitude;

                if (distSqr < minDistanceSqr)
                {
                    minDistanceSqr = distSqr;
                    nearestVertex = vertex;
                }
            }

            return nearestVertex;
        }
    }

    //fetching the mesh from the mapgenerator
    class LevelOfDetailMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int levelOfDetail;
        System.Action updateCallback;

        public LevelOfDetailMesh(int LOD, System.Action updateCallback)
        {
            this.levelOfDetail = LOD;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;

            updateCallback();
        }

        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, levelOfDetail, OnMeshDataReceived);
        }
    }

    [System.Serializable]
    public struct LevelOfDetailInfo
    {
        public int LevelOfDetail;
        public float visibleDistanceThreshold;
        public bool useForCollider;
    }

}
