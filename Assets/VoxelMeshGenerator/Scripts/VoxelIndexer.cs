using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VoxelIndexer
{
    static Voxel[] voxels;

    private static void GetVoxels()
    {
        voxels = Resources.LoadAll<Voxel>("Voxels");
    }

    public static Voxel IndexToVoxel(int id)
    {
        if (voxels == null) GetVoxels();
        if (id < 0) return null;
        if (id >= voxels.Length) return null;
        return voxels[id];
    }

    public static int VoxelToIndex(Voxel voxel)
    {
        if (voxels == null) GetVoxels();
        for (int i = 0; i < voxels.Length; i++)
        {
            if (voxels[i] == voxel) return i;
        }
        return -1;
    }
}
