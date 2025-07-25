using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class NoiseGenerator : MonoBehaviour
{
    // Voxels section for the Inspector window in the engine GUI
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
    
    // Dictionary for previously sampled noise to speed up load time by
    // only calculating the 2D noise and biome noise for one vertical slice of
    // the world and not recalculating it every new Y value - which would
    // be redudant.
    Dictionary<Vector2Int, float> previouslySampledNoise = new();
    Dictionary<Vector2Int, int> sampledBiomeNoise = new();
    float[] biomeSeeds;

    // Function that generates the seed for world generation using a Hash
    // from the Join Code string.
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

    // Unity built in function for getting references *before* the first frame update.
    private void Awake()
    {
        // Load biomes from file
        biomes = Resources.LoadAll<Biome>("Biomes");
        mb = GetComponent<MapBuilder>();
    }

    // Function that is called by the MapBuilder to generate the world, it returns a voxel
    // and is the main driver of the terrain generator.
    public Voxel GetVoxelAtPos(Vector3Int pos)
    {
        // Check if the current biome noise has already been sampled
        if (!sampledBiomeNoise.TryGetValue(new Vector2Int(pos.x, pos.z), out int chosenBiome))
        {
            Dictionary<int, float> biomeNoise = new();
            float bestNoise = -1;
            // Give each biome a noise value for comparison.
            for (int b = 0; b < biomes.Length; b++)
            {
                float bnoise = Mathf.PerlinNoise((float)(pos.x + biomeSeeds[b]) / biomeScale, (float)(pos.z + biomeSeeds[b]) / biomeScale);

                biomeNoise.Add(b, bnoise);
            }
            // Compare the biome's noise values to determine which biome to use at this position
            foreach (KeyValuePair<int, float> dBiome in biomeNoise)
            {
                if (dBiome.Value > bestNoise)
                {
                    bestNoise = dBiome.Value;
                    chosenBiome = dBiome.Key;
                }
            }
            // Add the sampled noise to the sample dictionary
            sampledBiomeNoise.Add(new Vector2Int(pos.x, pos.z), chosenBiome);
        }
        biome = biomes[chosenBiome];

        // Find the centre of the map for falloff calculations
        int centerX = (mb.mapSize.x * mb.realChunkSize) / 2;
        int centerZ = (mb.mapSize.z * mb.realChunkSize) / 2;
        // Calculate falloff for island-like terrain
        float distanceFromCenter = Mathf.Sqrt((pos.x - centerX) * (pos.x - centerX) + (pos.z - centerZ) * (pos.z - centerZ));
        float falloff = Mathf.Clamp01(Mathf.Pow(distanceFromCenter / centerX, 2));

        // Check if the 2D noise for this slice has already been sampled and exists in the dictionary
        if (!previouslySampledNoise.TryGetValue(new Vector2Int(pos.x, pos.z), out float noiseVal))
        {
            noiseVal = noise.snoise(new float3((float)pos.x + seed, 0, (float)pos.z + seed) / biome.horizontalScale);
            previouslySampledNoise.Add(new Vector2Int(pos.x, pos.z), noiseVal);
        }

        // This value is 3 dimensional noise and thus does not benefit from a resampling dictionary
        float noiseValT = noise.snoise(new float3((float)pos.x + odditySeed, (float)pos.y + odditySeed, (float)pos.z + odditySeed) / biome.oddityScale);

        // Apply 3D noise to the 2D noise value
        noiseVal += (noiseValT + (biome.oddityOffset - 0.5f)) * biome.oddity;

        // Value for determining which voxel to generate
        float height = ((noiseVal + 1) * biome.verticalScale * (1f - falloff));

        // Compare height values to Y position values
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
            // Sample noise for tree generation
            float treeRand = UnityEngine.Random.Range(0, 50 * falloff);

            // Generate different structures based on biome and noise
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

            // if no structures are generated, return the surface voxel
            return biome.surfaceVoxel;
        }
        // Generate plants if the biome requires them
        if (height > pos.y - 1 && biome.generatePlants)
        {
            if (UnityEngine.Random.Range(0, 150) == 0)
            {
                return biome.plants[UnityEngine.Random.Range(0, biome.plants.Length)];
            }
        }
        // If nothing else, return the air voxel and close the function,
        // this is also good for catching any edge cases and making sure
        // that the function will always return a value.
        return airVoxel;
    }
}
