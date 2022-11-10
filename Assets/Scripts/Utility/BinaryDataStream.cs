using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class BinaryDataStream 
{
    public static void Save<T>(T serializedObj, string fileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        Directory.CreateDirectory(path);

        FileStream fs = new FileStream(path + fileName + ".dat", FileMode.Create);
        BinaryFormatter bf = new BinaryFormatter();
        try
        {
            bf.Serialize(fs, serializedObj);
        }
        catch (System.Exception)
        {

            Debug.LogError("Save failed");
        }
        finally
        {
           
            fs.Close();
        }
    }

    public static T Read<T>(string fileName)
    {
        string path = Application.persistentDataPath + "/saves/";

        FileStream fs = new FileStream(path + fileName + ".dat", FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        T returnType = default(T);
        try
        {
            returnType = (T)bf.Deserialize(fs);
        }
        catch (System.Exception)
        {

            Debug.LogError("Read failed");
        }
        finally
        {
            fs.Close();
        }

        return returnType;
    }

    public static bool Exist(string fileName)
    {
        return File.Exists(Application.persistentDataPath + "/saves/" + fileName + ".dat");
    }
}
