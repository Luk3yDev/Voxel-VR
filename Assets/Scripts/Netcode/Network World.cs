using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class NetworkWorld : NetworkBehaviour
{
    MapBuilder world;
    [SerializeField] Voxel testVoxel;

    private void Start()
    {
        world = GetComponent<MapBuilder>();
    }

    [Rpc(SendTo.NotMe)]
    void SetVoxelClientRpc(int x, int y, int z, string index)
    {
        world.SetVoxel(new Vector3Int(x, y, z), testVoxel);
    }

    public void NetworkSetVoxel(Vector3Int voxelPos, Voxel voxel)
    {
        SetVoxelClientRpc(voxelPos.x, voxelPos.y, voxelPos.z, "");
    }
}