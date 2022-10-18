using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor.PackageManager;
using UnityEngine;

public class BinaryDataStream
{
    private static string FOLDER_PATH = "/saves/";
    private static string FILE_EXTENSION = ".dat";


    public static void Save<T>(T serializableObject, string fileName)
    {
        string path = Application.persistentDataPath + FOLDER_PATH;
        Directory.CreateDirectory(path);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path + fileName + FILE_EXTENSION, FileMode.Create);

        try
        {
            formatter.Serialize(fileStream, serializableObject);
        }
        catch (SerializationException e)
        {
            Debug.Log("Save failed. Error: " + e.Message);
        }
        finally
        {
            fileStream.Close();
        }
    }

    public static bool Exists(string fileName)
    {
        string path = Application.persistentDataPath + FOLDER_PATH;
        string fullFileName = fileName + FILE_EXTENSION;

        return File.Exists(path + fullFileName);
    }

    public static T Read<T>(string fileName)
    {
        string path = Application.persistentDataPath + FOLDER_PATH;
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path + fileName + FILE_EXTENSION, FileMode.Open);
        T resultType = default(T);

        try
        {
            resultType = (T)formatter.Deserialize(fileStream);
        }
        catch (SerializationException e)
        {
            Debug.Log("Read failed. Error: " + e.Message);
        }
        finally
        {
            fileStream.Close();
        }

        return resultType;
    }
}
