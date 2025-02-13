using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFeatureGen : MonoBehaviour
{
    [SerializeField] Voxel rootVoxel;
    [SerializeField] Voxel mushRootVoxel;
    [SerializeField] Voxel woodVoxel;
    [SerializeField] Voxel woodVoxel2;
    [SerializeField] Voxel leavesVoxel;
    [SerializeField] Voxel leavesVoxel2;
    MapBuilder mb;

    private void Awake()
    {
        mb = GetComponent<MapBuilder>();   
    }

    public void GenerateFeatures()
    {
        for (int x = 0; x < mb.mapSize.x * mb.realChunkSize + 2; x++)
        {
            for (int y = 0; y < mb.mapSize.y * mb.realChunkSize + 2; y++)
            {
                for (int z = 0; z < mb.mapSize.z * mb.realChunkSize + 2; z++)
                {
                    if (mb.voxelData[x, y, z] == rootVoxel)
                    {
                        FeatureTree(new Vector3Int(x, y, z));
                    }
                    else if (mb.voxelData[x, y, z] == mushRootVoxel)
                    {
                        FeatureTree2(new Vector3Int(x, y, z));
                    }
                }
            }
        }
    }              

    void FeatureTree(Vector3Int pos)
    {
        if (pos.y < (mb.realChunkSize * mb.mapSize.y) - 16)
        {
            for (int i = 1; i < 5; i++)
            {
                mb.voxelData[pos.x, pos.y + i, pos.z] = woodVoxel;
            }
            for (int x = -1; x < 2; x++)
            {
                for (int y = 5; y < 8; y++)
                {
                    for (int z = -1; z < 2; z++)
                    {
                        if (pos.x + x >= 1 && pos.x + x < mb.voxelData.GetLength(0) - 1)
                        {
                            if (pos.y + y >= 1 && pos.y + y < mb.voxelData.GetLength(1) - 1)
                            {
                                if (pos.z + z >= 1 && pos.z + z < mb.voxelData.GetLength(2) - 1)
                                {
                                    mb.voxelData[pos.x + x, pos.y + y, pos.z + z] = leavesVoxel;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void FeatureTree2(Vector3Int pos)
    {
        if (pos.y < (mb.realChunkSize * mb.mapSize.y) - 16)
        {
            for (int i = 1; i < 5; i++)
            {
                mb.voxelData[pos.x, pos.y + i, pos.z] = woodVoxel2;
            }
            for (int x = -2; x < 3; x++)
            {
                for (int z = -2; z < 3; z++)
                {
                    if (pos.x + x >= 1 && pos.x + x < mb.voxelData.GetLength(0) - 1)
                    {                        
                        if (pos.z + z >= 1 && pos.z + z < mb.voxelData.GetLength(2) - 1)
                        {
                            mb.voxelData[pos.x + x, pos.y + 5, pos.z + z] = leavesVoxel2;
                        }
                    }
                }
            }
        }
    }
}