using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class NoiseGenerator : MonoBehaviour
{
    [Header("Voxels")]
    [SerializeField] Voxel airVoxel;
    [SerializeField] Voxel rootVoxel;
    [SerializeField] Voxel mushRootVoxel;
    [SerializeField] Voxel sandstoneVoxel;
    [SerializeField] Voxel pineRoot;

    [Header("Noise Parameters")]
    [SerializeField] float biomeScale;
    Biome biome;
    Biome[] biomes;

    int seed;
    int odditySeed;
    MapBuilder mb;
    Dictionary<Vector2Int, float> previouslySampledNoise = new();
    Dictionary<Vector2Int, int> sampledBiomeNoise = new();
    float[] biomeSeeds;

    public void GenerateSeed(string joinCode)
    {
        int code = joinCode.GetHashCode();
        if (joinCode == "!singeplayer!")
        {
            code = UnityEngine.Random.Range(-10000, 10000);
        }
        
        Debug.Log($"World seed: {code}");
        UnityEngine.Random.InitState(code);
        seed = UnityEngine.Random.Range(-1000, 1000);
        odditySeed = UnityEngine.Random.Range(-1000, 1000);
        
        biomeSeeds = new float[biomes.Length];
        for (int b = 0; b < biomes.Length; b++)
        {
            biomeSeeds[b] = UnityEngine.Random.Range(-1000, 1000);
        }
    }

    private void Awake()
    {
        biomes = Resources.LoadAll<Biome>("Biomes");
        mb = GetComponent<MapBuilder>();
    }

    public Voxel GetVoxelAtPos(Vector3Int pos)
    {
        if (!sampledBiomeNoise.TryGetValue(new Vector2Int(pos.x, pos.z), out int chosenBiome))
        {
            Dictionary<int, float> biomeNoise = new();
            float bestNoise = -1;

            for (int b = 0; b < biomes.Length; b++)
            {
                float bnoise = Mathf.PerlinNoise((float)(pos.x + biomeSeeds[b]) / biomeScale, (float)(pos.z + biomeSeeds[b]) / biomeScale);

                biomeNoise.Add(b, bnoise);
            }
            foreach (KeyValuePair<int, float> dBiome in biomeNoise)
            {
                if (dBiome.Value > bestNoise)
                {
                    bestNoise = dBiome.Value;
                    chosenBiome = dBiome.Key;
                }
            }
            sampledBiomeNoise.Add(new Vector2Int(pos.x, pos.z), chosenBiome);
        }
        biome = biomes[chosenBiome];

        int centerX = (mb.mapSize.x * mb.realChunkSize) / 2;
        int centerZ = (mb.mapSize.z * mb.realChunkSize) / 2;

        float distanceFromCenter = Mathf.Sqrt((pos.x - centerX) * (pos.x - centerX) + (pos.z - centerZ) * (pos.z - centerZ));
        float falloff = Mathf.Clamp01(Mathf.Pow(distanceFromCenter / centerX, 2));

        if (!previouslySampledNoise.TryGetValue(new Vector2Int(pos.x, pos.z), out float noiseVal))
        {
            noiseVal = noise.snoise(new float3((float)pos.x + seed, 0, (float)pos.z + seed) / biome.horizontalScale);
            previouslySampledNoise.Add(new Vector2Int(pos.x, pos.z), noiseVal);
        }

        float noiseValT = noise.snoise(new float3((float)pos.x + odditySeed, (float)pos.y + odditySeed, (float)pos.z + odditySeed) / biome.oddityScale);
        noiseVal += (noiseValT + (biome.oddityOffset - 0.5f)) * biome.oddity;

        float height = ((noiseVal + 1) * biome.verticalScale * (1f - falloff));
        
        if (height > pos.y + 4)
        {
            return biome.undergroundVoxel;
        }
        if (height > pos.y && pos.y < 3)
        {
            return biome.sandVoxel;
        }
        if (height > pos.y + 2)
        {
            return biome.dirtVoxel;
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
            if (biome.generatePineTrees && UnityEngine.Random.Range(0, 300) == 0)
            {
                return pineRoot;
            }

            return biome.surfaceVoxel;
        }
        if (height > pos.y - 1 && biome.generatePlants)
        {
            if (UnityEngine.Random.Range(0, 150) == 0)
            {
                return biome.plants[UnityEngine.Random.Range(0, biome.plants.Length)];
            }
        }
        return airVoxel;
    }
}
