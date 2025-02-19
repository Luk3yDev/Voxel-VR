using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class NetworkWorld : NetworkBehaviour
{
    MapBuilder world;

    private void Start()
    {
        world = GetComponent<MapBuilder>();
    }

    [Rpc(SendTo.NotMe)]
    void SetVoxelClientRpc(int x, int y, int z, int voxelIndex)
    {
        world.SetVoxel(new Vector3Int(x, y, z), VoxelIndexer.IndexToVoxel(voxelIndex));
    }

    public void NetworkSetVoxel(Vector3Int voxelPos, Voxel voxel)
    {
        SetVoxelClientRpc(voxelPos.x, voxelPos.y, voxelPos.z, VoxelIndexer.VoxelToIndex(voxel));
    }
}