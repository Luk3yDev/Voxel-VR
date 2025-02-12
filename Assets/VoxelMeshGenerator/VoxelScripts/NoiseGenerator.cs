using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class NoiseGenerator : MonoBehaviour
{
    [Header("Voxels")]
    [SerializeField] Voxel airVoxel;
    [SerializeField] Voxel grassVoxel;
    [SerializeField] Voxel dirtVoxel;
    [SerializeField] Voxel stoneVoxel;
    [SerializeField] Voxel rootVoxel;
    [SerializeField] Voxel sandVoxel;

    [Header("Noise Params")]
    [SerializeField] float horizontalScale;
    [SerializeField] float verticalScale;
    [SerializeField] float oddityScale;
    [SerializeField] float oddity;
    [SerializeField] float oddityOffset;
    int seed;
    int odditySeed;
    MapBuilder mb;
    Dictionary<Vector2Int, float> previouslySampledNoise = new Dictionary<Vector2Int, float>();

    private void Awake()
    {
        UnityEngine.Random.InitState(System.DateTime.Now.Second);
        seed = UnityEngine.Random.Range(-1000, 1000);
        odditySeed = UnityEngine.Random.Range(-1000, 1000);
        mb = GetComponent<MapBuilder>();
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
        
        if (height > pos.y + 3)
        {
            return stoneVoxel;
        }
        if (height > pos.y && pos.y < 3)
        {
            return sandVoxel;
        }
        if (height > pos.y + 1)
        {
            return dirtVoxel;
        }
        if (height > pos.y)
        {           
            if (UnityEngine.Random.Range(0, 200) == 0)
            {
                return rootVoxel;
            }

            return grassVoxel;
        }
        return airVoxel;
    }
}
