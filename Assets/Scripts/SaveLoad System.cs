using System.IO;
using UnityEngine;


public static class SaveLoadSystem 
{
    private const string SAVE_EXTENTION = ".txt";
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";

    private static bool isInit = false;

    public static void Init(string folder)
    {
        if (!isInit)
        {
            isInit = true;
            //if not creates one
            if (!Directory.Exists(SAVE_FOLDER + folder + "/"))
            {
                Directory.CreateDirectory(SAVE_FOLDER + folder + "/");
            }
        }
    }
 

    public static void Save(string folder, string fileName, string saveString, bool overwrite) 
    {
        Init(folder);
        string saveFileName = fileName;
        if (!overwrite)
        {
            int saveNumber = 1;
            while (File.Exists(SAVE_FOLDER + folder + "/" + saveFileName + SAVE_EXTENTION))
            {
                saveNumber++;
                saveFileName = fileName + "_" + saveNumber;
            }
        }
        
        File.WriteAllText(SAVE_FOLDER + folder + "/" + saveFileName + SAVE_EXTENTION, saveString);

    }
    public static string Load(string folder, string fileName)
    {
        Init(folder);
        if (File.Exists(SAVE_FOLDER + folder + "/" + fileName + SAVE_EXTENTION))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + folder + "/" + fileName + SAVE_EXTENTION);
            return saveString;
        }
        else
        {
            return null;
        }

    }

    public static string LoadMostRecentFile(string folder) 
    {
        Init(folder);
        DirectoryInfo directory = new DirectoryInfo(SAVE_FOLDER + folder + "/");
        FileInfo[] saveFiles = directory.GetFiles("*" + SAVE_EXTENTION);
        FileInfo mostRecentFile = null;

        foreach(FileInfo file in saveFiles)
        {
            if(mostRecentFile == null)
            {
                mostRecentFile = file;
            }
            else
            {
                if (file.LastWriteTime > mostRecentFile.LastWriteTime)
                {
                    mostRecentFile = file;
                }
            }
        }
        if(mostRecentFile != null) 
        {
            string saveString = File.ReadAllText(mostRecentFile.FullName);
            return saveString;
        }
        else
        {
            return null;
        }    
    }

    public static void SaveObject(string folder, string fileName, object saveObject, bool overWrite) 
    {
        Init(folder);
        string json = JsonUtility.ToJson(saveObject);
        Save(folder, fileName, json, overWrite);
    }

    public static TSaveObject LoadMostRecentObject<TSaveObject>(string folder)
    {
        Init(folder);
        string saveString = LoadMostRecentFile(folder);
        if (saveString != null)
        {
            TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(saveString);
            return saveObject;
        }
        
        return default(TSaveObject);
        
    }

    public static TSaveObject LoadObject<TSaveObject>(string folder, string fileName)
    {
        Init(folder);
        string saveString = Load(folder, fileName);
        if(saveString != null)
        {
            TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(saveString);
            return saveObject;
        }
        else
        {
            return default(TSaveObject);
        }
    }
}
