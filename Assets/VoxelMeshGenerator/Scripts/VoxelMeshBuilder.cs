using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class VoxelMeshBuilder : MonoBehaviour
{
    [SerializeField] int atlasSize;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    public struct MeshData
    {
        public Vector3[] vertices;
        public Vector2[] uvs;
        public int[] triangles;

        public Vector3[] colliderVertices;
        public int[] colliderTriangles;
    }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public async void BuildChunkAsync(Voxel[,,] voxelData)
    {
        MeshData meshData = await Task.Run(() => GenerateMeshData(voxelData));
        ApplyMesh(meshData);
    }

    MeshData GenerateMeshData(Voxel[,,] voxelData)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        List<Vector3> colliderVertices = new List<Vector3>();
        List<int> colliderTriangles = new List<int>();

        Vector3[] VertPos = new Vector3[8]{
            new Vector3(-1, 1, -1), new Vector3(-1, 1, 1),
            new Vector3(1, 1, 1), new Vector3(1, 1, -1),
            new Vector3(-1, -1, -1), new Vector3(-1, -1, 1),
            new Vector3(1, -1, 1), new Vector3(1, -1, -1),
        };

        int[,] Faces = new int[10, 7]{
            {0, 1, 2, 3, 0, 1, 0},   //top
            {7, 6, 5, 4, 0, -1, 0},  //bottom
            {2, 1, 5, 6, 0, 0, 1},   //right
            {0, 3, 7, 4, 0, 0, -1},  //left
            {3, 2, 6, 7, 1, 0, 0},   //front
            {1, 0, 4, 5, -1, 0, 0},  //back
            {0, 2, 6, 4, 0, 0, 0},   // +X +Z
            {2, 0, 4, 6, 0, 0, 0},   // -X -Z
            {1, 3, 7, 5, 0, 0, 0},   // -X +Z
            {3, 1, 5, 7, 0, 0, 0}    // +X -Z
        };

        for (int x = 1; x < voxelData.GetLength(0) - 1; x++)
            for (int y = 1; y < voxelData.GetLength(1) - 1; y++)
                for (int z = 1; z < voxelData.GetLength(2) - 1; z++)
                {
                    var voxel = voxelData[x, y, z];
                    if (voxel == null || voxel.isAir) continue;

                    if (voxel.modelType == Voxel.ModelType.Cube)
                    {
                        for (int o = 0; o < 6; o++)
                        {
                            var neighbor = voxelData[x + Faces[o, 4], y + Faces[o, 5], z + Faces[o, 6]];
                            if (neighbor != null && neighbor.transparent)
                                AddQuad(o, x, y, z, voxel, Faces, VertPos);
                        }
                    }
                    else if (voxel.modelType == Voxel.ModelType.Cross)
                    {
                        for (int o = 6; o < 10; o++)
                            AddQuad(o, x, y, z, voxel, Faces, VertPos);
                    }
                }

        void AddQuad(int facenum, int x, int y, int z, Voxel voxel, int[,] Faces, Vector3[] VertPos)
        {
            int v = vertices.Count;
            int cv = colliderVertices.Count;

            for (int i = 0; i < 4; i++) vertices.Add(new Vector3(x, y, z) + VertPos[Faces[facenum, i]] / 2f);
            triangles.AddRange(new int[] { v, v + 1, v + 2, v, v + 2, v + 3 });

            if (voxel.collide)
            {
                for (int i = 0; i < 4; i++) colliderVertices.Add(new Vector3(x, y, z) + VertPos[Faces[facenum, i]] / 2f);
                colliderTriangles.AddRange(new int[] { cv, cv + 1, cv + 2, cv, cv + 2, cv + 3 });
            }

            Vector2 bottomleft = voxel.uvCoordinate / atlasSize;
            uvs.AddRange(new Vector2[] {
                bottomleft + new Vector2(0, 1f) / atlasSize,
                bottomleft + new Vector2(1f, 1f) / atlasSize,
                bottomleft + new Vector2(1f, 0) / atlasSize,
                bottomleft
            });
        }

        return new MeshData
        {
            vertices = vertices.ToArray(),
            uvs = uvs.ToArray(),
            triangles = triangles.ToArray(),
            colliderVertices = colliderVertices.ToArray(),
            colliderTriangles = colliderTriangles.ToArray()
        };
    }

    void ApplyMesh(MeshData meshData)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.vertices;
        mesh.uv = meshData.uvs;
        mesh.triangles = meshData.triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.OptimizeReorderVertexBuffer();

        Mesh colliderMesh = new Mesh();
        colliderMesh.vertices = meshData.colliderVertices;
        colliderMesh.triangles = meshData.colliderTriangles;
        colliderMesh.RecalculateNormals();
        colliderMesh.RecalculateBounds();
        colliderMesh.OptimizeReorderVertexBuffer();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = colliderMesh;
    }
}
