using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class NetworkWorld : NetworkBehaviour
{
    MapBuilder world;

    void Awake()
    {
        world = GetComponent<MapBuilder>();
    }

    [Rpc(SendTo.Everyone)]
    public void SetVoxelClientRpc(int x, int y, int z, int voxelIndex)
    {
        world.SetVoxel(new Vector3Int(x, y, z), VoxelIndexer.IndexToVoxel(voxelIndex));
    }

    public void NetworkSetVoxel(Vector3Int voxelPos, Voxel voxel)
    {
        if (world == null) return;
        if (!IsOwner) return;
        SetVoxelClientRpc(voxelPos.x, voxelPos.y, voxelPos.z, VoxelIndexer.VoxelToIndex(voxel));
    }

    [ClientRpc]
    public void CreateWorldClientRpc()
    {
        if (!IsOwner) return;
        world?.CreateWorld();
    }
}