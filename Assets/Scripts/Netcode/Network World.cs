using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class NetworkWorld : NetworkBehaviour
{
    MapBuilder world;
    Voxel voxelToSet;

    private void Start()
    {
        world = GetComponent<MapBuilder>();
    }

    [Rpc(SendTo.NotMe)]
    void SetVoxelClientRpc(int x, int y, int z)
    {
        world.SetVoxel(new Vector3Int(x, y, z), voxelToSet);
    }

    public void NetworkSetVoxel(Vector3Int voxelPos, Voxel voxel)
    {
        voxelToSet = voxel;
        SetVoxelClientRpc(voxelPos.x, voxelPos.y, voxelPos.z);
    }
}