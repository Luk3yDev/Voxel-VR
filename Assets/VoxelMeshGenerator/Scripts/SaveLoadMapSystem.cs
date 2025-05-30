using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class Map {
    public int[,,] data;
    public int sizeX;
    public int sizeY;
    public int sizeZ;
    public string name;
}

public static class SaveLoadMapSystem
{
    public static string fileFormat = ".vvr";

    public static void SaveMapDataToFile(Map map)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "\\" + map.name + fileFormat;
        
        FileStream stream = File.Create(path);

        formatter.Serialize(stream, map);
        stream.Close();
    }

    public static Map LoadMapDataFromFile(string path)
    {      
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = File.OpenRead(path);
            Map map = formatter.Deserialize(stream) as Map;
            stream.Close();
            return map;

        } else
        {
            Debug.LogError("Attempted to load file that does not exist at " + path);
            return null;
        }
    }
}
