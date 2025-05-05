using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MapUpdater : MonoBehaviour
{
    public float tickSpeed;
    public Vector3Int chunkPos;

    [SerializeField] Voxel water;
    [SerializeField] Voxel air;
    MapBuilder mb;

    float tick = 0;
    bool updateInProgress = false;

    ConcurrentQueue<Vector3Int[]> waterUpdates = new ConcurrentQueue<Vector3Int[]>();

    private void Awake()
    {
        mb = FindFirstObjectByType<MapBuilder>();
        chunkPos.x = Mathf.FloorToInt((float)((int)transform.position.x - 1) / mb.realChunkSize);
        chunkPos.y = Mathf.FloorToInt((float)((int)transform.position.y - 1) / mb.realChunkSize);
        chunkPos.z = Mathf.FloorToInt((float)((int)transform.position.z - 1) / mb.realChunkSize);
    }

    private void Update()
    {
        tick += Time.deltaTime;

        // Check if there's a pending result to apply on the main thread
        if (waterUpdates.TryDequeue(out var newWaterVoxels))
        {
            mb.FillVoxels(newWaterVoxels, water);
        }

        // Skip if ticking too fast or update still running
        if (tick < tickSpeed || updateInProgress) return;

        tick = 0;
        updateInProgress = true;

        // Start background task
        Task.Run(() =>
        {
            List<Vector3Int> newWater = new List<Vector3Int>();

            for (int x = chunkPos.x; x < chunkPos.x + mb.realChunkSize; x++)
            {
                for (int y = chunkPos.y; y < chunkPos.y + mb.realChunkSize; y++)
                {
                    for (int z = chunkPos.z; z < chunkPos.z + mb.realChunkSize; z++)
                    {
                        if (mb.GetVoxel(new Vector3Int(x, y, z)) == water)
                        {
                            if (mb.voxelData[x, y - 1, z] == air)
                            {
                                newWater.Add(new Vector3Int(x, y - 1, z));
                            }
                            else if (mb.voxelData[x, y - 1, z] != water)
                            {
                                if (mb.voxelData[x + 1, y, z] == air)
                                    newWater.Add(new Vector3Int(x + 1, y, z));
                                if (mb.voxelData[x - 1, y, z] == air)
                                    newWater.Add(new Vector3Int(x - 1, y, z));
                                if (mb.voxelData[x, y, z + 1] == air)
                                    newWater.Add(new Vector3Int(x, y, z + 1));
                                if (mb.voxelData[x, y, z - 1] == air)
                                    newWater.Add(new Vector3Int(x, y, z - 1));
                            }
                        }
                    }
                }
            }

            if (newWater.Count > 0)
            {
                waterUpdates.Enqueue(newWater.ToArray());
            }

            updateInProgress = false;
        });
    }
}
