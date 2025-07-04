using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public int chunkSize;
    public Vector3Int mapSize;
    [SerializeField] VoxelMeshBuilder chunkPrefab;
    [SerializeField] Voxel airVoxel;

    NoiseGenerator noiseGenerator;
    public Voxel[,,] voxelData;
    Dictionary<Vector3Int, VoxelMeshBuilder> builtChunks;
    [HideInInspector] public int realChunkSize;

    TerrainFeatureGen featureGen;
    NetworkWorld netWorld;
    [SerializeField] GameObject spawnRoom;
    [SerializeField] GameObject loading;
    [SerializeField] int renderDistance;
    float squareRenderDistance;


    List<GameObject> chunkObjects = new List<GameObject>();
    Transform player;

    private void Awake()
    {
        noiseGenerator = GetComponent<NoiseGenerator>();
        featureGen = GetComponent<TerrainFeatureGen>();
        netWorld = GetComponent<NetworkWorld>();
        player = GameObject.Find("Player").transform;
    }

    private void Start()
    {
        //CreateWorld();
    }

    public void NetworkCreateWorld()
    {
        netWorld.NetworkCreateWorld();
    }

    public void CreateWorld()
    {
        /* THIS IS COSTLY
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        */

        GenerateMap();
        BuildMap();     

        VoxelMeshBuilder[] collection = builtChunks.Values.ToArray();
        foreach (VoxelMeshBuilder chunk in collection)
        {
            chunkObjects.Add(chunk.gameObject);
        }

        spawnRoom.SetActive(false);
    }

    private void Update()
    {
        squareRenderDistance = renderDistance * renderDistance;
        //LoadUnloadChunks();
    }

    private void LoadUnloadChunks()
    {
        foreach (GameObject chunk in chunkObjects)
        {
            //float distance = Vector3.Distance(player.position, chunk.transform.position);
            float distance = (player.position - chunk.transform.position).sqrMagnitude;
            if (distance >= squareRenderDistance)
            {
                if (chunk.activeSelf) chunk.SetActive(false);
            }
            else
            {
                if (!chunk.activeSelf) chunk.SetActive(true);
            }
        }
    }

    public void SetVoxel(Vector3Int voxelPos, Voxel voxel)
    {
        if (voxelPos.x >= 1 && voxelPos.x < voxelData.GetLength(0) - 1)
            if (voxelPos.y >= 1 && voxelPos.y < voxelData.GetLength(1) - 1)
                if (voxelPos.z >= 1 && voxelPos.z < voxelData.GetLength(2) - 1)
                {
                    voxelData[voxelPos.x, voxelPos.y, voxelPos.z] = voxel;
                    Vector3Int chunkPos = GetChunkPosOfVoxel(voxelPos);
                    Vector3Int[] neighbourChunks = new Vector3Int[]
                    {
                        new Vector3Int(0, 0, 0),
                        new Vector3Int(1, 0, 0),
                        new Vector3Int(0, 1, 0),
                        new Vector3Int(0, 0, 1),
                        new Vector3Int(-1, 0, 0),
                        new Vector3Int(0, -1, 0),
                        new Vector3Int(0, 0, -1),
                    };
                    foreach (Vector3Int offset in neighbourChunks )
                    {
                        if (builtChunks.TryGetValue((chunkPos + offset) * realChunkSize, out VoxelMeshBuilder chunk))
                        {
                            chunk.BuildChunk(getChunkDataFromChunkPos((chunkPos + offset) * realChunkSize));
                        }
                    }
                }
    }

    public Voxel GetVoxel(Vector3Int voxelPos)
    {
        if (voxelPos.x >= 1 && voxelPos.x < voxelData.GetLength(0) - 1)
            if (voxelPos.y >= 1 && voxelPos.y < voxelData.GetLength(1) - 1)
                if (voxelPos.z >= 1 && voxelPos.z < voxelData.GetLength(2) - 1)
                {
                    return voxelData[voxelPos.x, voxelPos.y, voxelPos.z];
                }
        return null;
    }

    void GenerateMap()
    {
        realChunkSize = chunkSize - 2;
        voxelData = new Voxel[mapSize.x * realChunkSize + 2, mapSize.y * realChunkSize + 2, mapSize.z * realChunkSize + 2];
        for (int x = 0; x < mapSize.x * realChunkSize + 2; x++)
        {
            for (int y = 0; y < mapSize.y * realChunkSize + 2; y++)
            {
                for (int z = 0; z < mapSize.z * realChunkSize + 2; z++)
                {
                    voxelData[x, y, z] = noiseGenerator.GetVoxelAtPos(new Vector3Int(x, y, z));
                    if (x == 0 || x == mapSize.x * realChunkSize + 1) voxelData[x, y, z] = airVoxel;
                    if (y == 0 || y == mapSize.y * realChunkSize + 1) voxelData[x, y, z] = airVoxel;
                    if (z == 0 || z == mapSize.z * realChunkSize + 1) voxelData[x, y, z] = airVoxel;
                }
            }
        }

        featureGen.GenerateFeatures();
    }

    void BuildMap()
    {
        builtChunks = new Dictionary<Vector3Int, VoxelMeshBuilder>();
        for (int chunkX = 0; chunkX < mapSize.x; chunkX++)
        {
            for (int chunkY = 0; chunkY < mapSize.y; chunkY++)
            {
                for (int chunkZ = 0; chunkZ < mapSize.z; chunkZ++)
                {
                    BuildChunk(chunkX, chunkY, chunkZ);
                }
            }
        }
    }

    Voxel[,,] getChunkDataFromChunkPos(Vector3Int chunkPos)
    {
        Voxel[,,] chunkData = new Voxel[chunkSize, chunkSize, chunkSize];
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    Vector3Int voxelWorldPos = new Vector3Int(chunkPos.x + x, chunkPos.y + y, chunkPos.z + z);
                    chunkData[x, y, z] = voxelData[voxelWorldPos.x, voxelWorldPos.y, voxelWorldPos.z];
                }
            }
        }
        return chunkData;
    }

    void BuildChunk(int chunkX, int chunkY, int chunkZ)
    {
        Vector3Int chunkPos = new Vector3Int(chunkX * realChunkSize, chunkY * realChunkSize, chunkZ * realChunkSize);
        Voxel[,,] chunkData = getChunkDataFromChunkPos(chunkPos);
        VoxelMeshBuilder chunkI = Instantiate(chunkPrefab);
        chunkI.transform.parent = transform;
        chunkI.transform.position = chunkPos;
        chunkI.BuildChunk(chunkData);
        builtChunks.Add(chunkPos, chunkI);
    }

    Vector3Int GetChunkPosOfVoxel(Vector3Int voxelPos)
    {
        int x = Mathf.FloorToInt((float)(voxelPos.x - 1) / realChunkSize);
        int y = Mathf.FloorToInt((float)(voxelPos.y - 1) / realChunkSize);
        int z = Mathf.FloorToInt((float)(voxelPos.z - 1) / realChunkSize);
        return new Vector3Int(x, y, z);
    }

    
}
