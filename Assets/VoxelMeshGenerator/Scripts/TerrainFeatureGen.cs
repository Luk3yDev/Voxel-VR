using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFeatureGen : MonoBehaviour
{
    [Header("Roots")]
    [SerializeField] Voxel rootVoxel;
    [SerializeField] Voxel mushRootVoxel;
    [SerializeField] Voxel cactiRootVoxel;
    [SerializeField] Voxel pineRootVoxel;

    [Header("Woods")]
    [SerializeField] Voxel woodVoxel;
    [SerializeField] Voxel woodVoxel2;
    [SerializeField] Voxel woodVoxel3;
    [SerializeField] Voxel cactiVoxel;

    [Header("Leaves")]
    [SerializeField] Voxel leavesVoxel;
    [SerializeField] Voxel leavesVoxel2;
    [SerializeField] Voxel leavesVoxel3;  

    [Header("Extra")]
    [SerializeField] Voxel cactiFlowerVoxel;
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
                    else if (mb.voxelData[x, y, z] == cactiRootVoxel)
                    {
                        FeatureTree3(new Vector3Int(x, y, z));
                    }
                    else if (mb.voxelData[x, y, z] == pineRootVoxel)
                    {
                        FeatureTree4(new Vector3Int(x, y, z));
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

    void FeatureTree3(Vector3Int pos)
    {
        if (pos.y < (mb.realChunkSize * mb.mapSize.y) - 16)
        {
            int i = 1;
            for (i = 1; i < 5; i++)
            {
                mb.voxelData[pos.x, pos.y + i, pos.z] = cactiVoxel;
            }
            if (Random.Range(0, 2) == 0) mb.voxelData[pos.x, pos.y + i, pos.z] = cactiFlowerVoxel;
        }
    }

    void FeatureTree4(Vector3Int pos)
    {
        if (pos.y < (mb.realChunkSize * mb.mapSize.y) - 16)
        {
            for (int i = 1; i < 8; i++)
            {
                mb.voxelData[pos.x, pos.y + i, pos.z] = woodVoxel3;

                if (i == 3)
                {
                    for (int x = -2; x < 3; x++)
                    {
                        for (int z = -2; z < 3; z++)
                        {
                            if (pos.x + x >= 1 && pos.x + x < mb.voxelData.GetLength(0) - 1)
                            {
                                if (pos.z + z >= 1 && pos.z + z < mb.voxelData.GetLength(2) - 1)
                                {
                                    mb.voxelData[pos.x + x, pos.y + i, pos.z + z] = leavesVoxel3;
                                }
                            }
                        }
                    }
                }
                if (i == 5)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        for (int z = -1; z < 2; z++)
                        {
                            if (pos.x + x >= 1 && pos.x + x < mb.voxelData.GetLength(0) - 1)
                            {
                                if (pos.z + z >= 1 && pos.z + z < mb.voxelData.GetLength(2) - 1)
                                {
                                    mb.voxelData[pos.x + x, pos.y + i, pos.z + z] = leavesVoxel3;
                                }
                            }
                        }
                    }
                }
                if (i == 7)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        for (int z = -1; z < 2; z++)
                        {
                            if (pos.x + x >= 1 && pos.x + x < mb.voxelData.GetLength(0) - 1)
                            {
                                if (pos.z + z >= 1 && pos.z + z < mb.voxelData.GetLength(2) - 1)
                                {
                                    mb.voxelData[pos.x + x, pos.y + i, pos.z + z] = leavesVoxel3;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}