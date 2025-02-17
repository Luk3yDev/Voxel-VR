using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class NoiseGenerator : MonoBehaviour
{
    [Header("Voxels")]
    [SerializeField] Voxel airVoxel;
    [SerializeField] Voxel rootVoxel;
    [SerializeField] Voxel sandVoxel;
    [SerializeField] Voxel mushRootVoxel;
    [SerializeField] Voxel sandstoneVoxel;

    [SerializeField] Biome biome;

    float horizontalScale;
    float verticalScale;
    float oddityScale;
    float oddity;
    float oddityOffset;

    Voxel surfaceVoxel;
    Voxel dirtVoxel;
    Voxel undergroundVoxel;

    int seed;
    int odditySeed;
    MapBuilder mb;
    Dictionary<Vector2Int, float> previouslySampledNoise = new Dictionary<Vector2Int, float>();

    public void GenerateSeed(string joinCode)
    {
        int code = joinCode.GetHashCode();
        Debug.Log($"World seed: {code}");
        UnityEngine.Random.InitState(code);
        seed = UnityEngine.Random.Range(-1000, 1000);
        odditySeed = UnityEngine.Random.Range(-1000, 1000);
    }

    private void Awake()
    {       
        mb = GetComponent<MapBuilder>();

        horizontalScale = biome.horizontalScale;
        verticalScale = biome.verticalScale;
        oddityScale = biome.oddityScale;
        oddity = biome.oddity;
        oddityOffset = biome.oddityOffset;
        surfaceVoxel = biome.surfaceVoxel;
        dirtVoxel = biome.dirtVoxel;
        undergroundVoxel = biome.undergroundVoxel;
    }

    public Voxel GetVoxelAtPos(Vector3Int pos)
    {
        int centerX = (mb.mapSize.x * mb.realChunkSize) / 2;
        int centerZ = (mb.mapSize.z * mb.realChunkSize) / 2;

        float distanceFromCenter = Mathf.Sqrt((pos.x - centerX) * (pos.x - centerX) + (pos.z - centerZ) * (pos.z - centerZ));
        float falloff = Mathf.Clamp01(Mathf.Pow(distanceFromCenter / centerX, 2));

        float noiseVal = 0;
        if (!previouslySampledNoise.TryGetValue(new Vector2Int(pos.x, pos.z), out noiseVal))
        {
            noiseVal = noise.snoise(new float3((float)pos.x + seed, 0, (float)pos.z + seed) / horizontalScale);
            previouslySampledNoise.Add(new Vector2Int(pos.x, pos.z), noiseVal);
        }       
        
        float noiseValT = noise.snoise(new float3((float)pos.x + odditySeed, (float)pos.y + odditySeed, (float)pos.z + odditySeed) / oddityScale);
        noiseVal += (noiseValT + (oddityOffset - 0.5f)) * oddity;

        float height = ((noiseVal + 1) * verticalScale * (1f - falloff));
        
        if (height > pos.y + 4)
        {
            return undergroundVoxel;
        }
        if (height > pos.y && pos.y < 3)
        {
            return sandVoxel;
        }
        if (height > pos.y + 2)
        {
            return dirtVoxel;
        }
        if (height > pos.y)
        {           
            float treeRand = UnityEngine.Random.Range(0, 50 * falloff);

            if (biome.generateTrees && treeRand > 0.5f && UnityEngine.Random.Range(0, 300) == 0)
            {
                return rootVoxel;
            }
            if (biome.generateMushrooms && treeRand < 0.5f && UnityEngine.Random.Range(0, 300) == 0)
            {
                return mushRootVoxel;
            }
            if (biome.generateCacti && UnityEngine.Random.Range(0, 300) == 0)
            {
                return sandstoneVoxel;
            }

            return surfaceVoxel;
        }
        return airVoxel;
    }
}
