using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string dataDirPath;
    private string subDir;
    private string dataFileName;
    private string codeWord;
    private bool useEncription;

    public FileDataHandler(string dataFileName, string subDir = "", bool useEncription = true)
    {
        this.dataFileName = dataFileName;
        this.dataDirPath = Application.persistentDataPath;
        this.subDir = subDir;
        this.codeWord = "Árvíztûrõtükörfúrógép";
        this.useEncription = useEncription;
	}

    public void SaveData<T>(T data, bool prettyPrint = false)
    {
        string fullPath = Path.Combine(dataDirPath, subDir, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonUtility.ToJson(data, prettyPrint);
            //Debug.Log(dataToStore);
            if (useEncription)
            {
                dataToStore = EncryptDecript(dataToStore);
            }
            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Save fail: " + e);
        }
        //Debug.Log("Saved to " + dataFileName);
    }
    public T LoadData<T>(object obj = null)
    {
        T loadedData = (T)obj;
        string fullPath = Path.Combine(dataDirPath, subDir, dataFileName);
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream fs = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        dataToLoad = sr.ReadToEnd();
                    }
                }
                if (useEncription)
                {
                    dataToLoad = EncryptDecript(dataToLoad);
                }

                if (loadedData != null)
                {
                    JsonUtility.FromJsonOverwrite(dataToLoad, loadedData);
                }
                else
                {
                    loadedData = JsonUtility.FromJson<T>(dataToLoad);
                }
                Debug.Log(dataFileName + " loaded");
                return loadedData;

            }
            catch (Exception e)
            {
                Debug.LogWarning("Load fail: " + e);
                return default;
            }
        }
        Debug.LogWarning("File " + dataFileName + " not found! [Normal behaviour on first startup]");
        return default;
    }

    string EncryptDecript(string data)
    {
        string modData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modData += (char)(data[i] ^ codeWord[i % codeWord.Length]);
        }
        return modData;
    }
}