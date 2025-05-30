using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaverLoader : MonoBehaviour
{
    public Voxel[,,] mapData;

    MapBuilder map;
    Vector3Int goldenValue;

    private void Awake()
    {
        map = GetComponent<MapBuilder>();
        goldenValue.x = map.mapSize.x * map.chunkSize;
        goldenValue.y = map.mapSize.y * map.chunkSize;
        goldenValue.z = map.mapSize.z * map.chunkSize;
        mapData = new Voxel[goldenValue.x, goldenValue.y, goldenValue.z];
    }

    public void NewMap()
    {
        int sizeX = map.mapSize.x;
        int sizeY = map.mapSize.y;
        int sizeZ = map.mapSize.z;
        goldenValue.x = sizeX * map.chunkSize;
        goldenValue.y = sizeY * map.chunkSize;
        goldenValue.z = sizeZ * map.chunkSize;
        map.mapSize.x = sizeX;
        map.mapSize.y = sizeY;
        map.mapSize.z = sizeZ;

        mapData = new Voxel[goldenValue.x, goldenValue.y, goldenValue.z];
    }

    public void LoadMap()
    {
        Map map = SaveLoadMapSystem.LoadMapDataFromFile("world");
        int[,,] data = map.data;
        this.map.mapSize.x = map.sizeX;
        this.map.mapSize.y = map.sizeY;
        this.map.mapSize.z = map.sizeZ;
        goldenValue.x = map.sizeX * this.map.chunkSize;
        goldenValue.y = map.sizeY * this.map.chunkSize;
        goldenValue.z = map.sizeZ * this.map.chunkSize;

        mapData = new Voxel[goldenValue.x, goldenValue.y, goldenValue.z];

        if (data.Length <= 0) return;
        for (int x = 0; x < goldenValue.x; x++)
        {
            for (int y = 0; y < goldenValue.y; y++)
            {
                for (int z = 0; z < goldenValue.z; z++)
                {
                    mapData[x, y, z] = VoxelIndexer.IndexToVoxel(data[x, y, z]);
                }
            }
        }
    }

    public void SaveMap()
    {
        for (int x = 0; x < goldenValue.x; x++)
        {
            for (int y = 0; y < goldenValue.y; y++)
            {
                for (int z = 0; z < goldenValue.z; z++)
                {
                    mapData[x, y, z] = this.map.GetVoxel(new Vector3Int(x, y, z));
                }
            }
        }
        int[,,] data = new int[goldenValue.x, goldenValue.y, goldenValue.z];
        for (int x = 0; x < goldenValue.x; x++)
        {
            for (int y = 0; y < goldenValue.y; y++)
            {
                for (int z = 0; z < goldenValue.z; z++)
                {
                    data[x, y, z] = VoxelIndexer.VoxelToIndex(mapData[x, y, z]);
                }
            }
        }
        Map map = new Map();
        map.data = data;
        map.sizeX = this.map.mapSize.x;
        map.sizeY = this.map.mapSize.y;
        map.sizeZ = this.map.mapSize.z;
        map.name = "world";
        SaveLoadMapSystem.SaveMapDataToFile(map);
    }

    public Voxel GetVoxelAtPos(Vector3Int pos)
    {
        Voxel voxel = mapData[pos.x, pos.y, pos.z];
        if (voxel == null) return VoxelIndexer.IndexToVoxel(0);
        else return voxel;
    }
}
