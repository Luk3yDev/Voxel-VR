using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Voxel", menuName = "ScriptableObjects/Voxel")]
public class Voxel : ScriptableObject
{
    public Vector2 uvCoordinate;
    public bool isAir;
    public AudioClip breakSound;
    public int index;

    public Voxel(Vector2 uvCoord, bool air = false, AudioClip sound = null, int i = 0)
    {
        uvCoordinate = uvCoord;
        isAir = air;
        breakSound = sound;
        index = i;
    }
}
