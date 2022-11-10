using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BinaryDataStream 
{
    public static void Save<T>(T serializedObj, string fileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        Directory.CreateDirectory(path);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(path + fileName + ".dat", FileMode.Create);

        try
        {
            bf.Serialize(fs, serializedObj);
        }
        catch (System.Exception e)
        {

            Debug.LogError("Save Failed: " + e.Message);
        }
        finally
        {
            fs.Close();
        }
    }
    
    public static bool Exist(string fileName)
    {
        return File.Exists(Application.persistentDataPath + "/saves/" + fileName + ".dat");
    }

    public static T Read<T>(string fileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(path + fileName + ".dat", FileMode.Open);
        T returnType = default(T);

        try
        {
            returnType = (T) bf.Deserialize(fs);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Read Failed: " + e.Message);
        }
        finally
        {
            fs.Close();
        }

        return returnType;
    }
}
