using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Voxel", menuName = "ScriptableObjects/Voxel")]
public class Voxel : ScriptableObject
{
    public Vector2 uvCoordinate;
    public bool isAir;
    public bool transparent;
    public ModelType modelType = ModelType.Cube;
    public AudioClip breakSound;
    public bool collide = true;

    public enum ModelType
    {
        Cube,
        Cross
    }

    public Voxel(Vector2 uvCoord, bool air = false, AudioClip sound = null)
    {
        uvCoordinate = uvCoord;
        isAir = air;
        breakSound = sound;
    }
}
