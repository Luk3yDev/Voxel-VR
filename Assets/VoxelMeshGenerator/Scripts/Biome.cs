using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "ScriptableObjects/Biome")]
public class Biome : ScriptableObject
{
    public Voxel surfaceVoxel;
    public Voxel dirtVoxel;
    public Voxel undergroundVoxel;
    public Voxel sandVoxel;
    public Voxel[] plants;

    public float horizontalScale;
    public float verticalScale;
    public float oddityScale;
    public float oddity;
    public float oddityOffset;

    public bool generateTrees;
    public bool generateMushrooms;
    public bool generateCacti;
    public bool generatePlants;
    public bool generatePineTrees;
}