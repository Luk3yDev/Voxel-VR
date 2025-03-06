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

    [ServerRpc(RequireOwnership = false)]
    public void SetVoxelServerRpc(int x, int y, int z, int voxelIndex)
    {
        SetVoxelClientRpc(x, y, z, voxelIndex);
    }

    [ClientRpc]
    public void SetVoxelClientRpc(int x, int y, int z, int voxelIndex)
    {
        world.SetVoxel(new Vector3Int(x, y, z), VoxelIndexer.IndexToVoxel(voxelIndex));
    }

    public void NetworkSetVoxel(Vector3Int voxelPos, Voxel voxel)
    {
        SetVoxelServerRpc(voxelPos.x, voxelPos.y, voxelPos.z, VoxelIndexer.VoxelToIndex(voxel));
    }
    public void NetworkCreateWorld()
    {
        if (!IsServer) return;
        CreateWorldServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateWorldServerRpc()
    {
        try
        {
            CreateWorldClientRpc();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error in CreateWorldServerRpc: {ex.Message}");
        }
    }

    [ClientRpc]
    public void CreateWorldClientRpc()
    {
        world?.CreateWorld();
    }
}