using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.IO;

[System.Serializable]
public class SaveData
{
    public int clearCount = 0;
    public int deadCount = 0;
    public Vector3 lastPosition = Vector3.zero;
    public Quaternion lastRotation = Quaternion.identity;
    public List<bool> isClearBoss = new List<bool>() { false, false, false };
    public List<ItemData> currentItem = new List<ItemData>();
}

[System.Serializable]
public class EnemyData
{
    public int Type { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }
    public float RotX { get; set; }
    public float RotY { get; set; }
    public float RotZ { get; set; }

    public void SetData(List<string> data)
    {
        int index = 0;

        foreach (PropertyInfo p in typeof(EnemyData).GetProperties())
        {
            Debug.Log(p.PropertyType.Name);

            switch (p.PropertyType.Name)
            {
                case "Int32":
                    p.SetValue(this, int.Parse(data[index]));
                    break;
                case "Single":
                    p.SetValue(this, float.Parse(data[index]));
                    break;
            }
            index++;
        }
    }
}


public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        SetEnemyData();

        path = Path.Combine(Application.dataPath, "database.json");
        Load();
    }

    [Header("경로")]
    public string path;

    [Header("플레이어 정보")]
    public int clearCount = 0;
    public int deadCount = 0;
    public Vector3 lastPosition = Vector3.zero;
    public Quaternion lastRotation = Quaternion.identity;
    public List<ItemData> currentItem = new List<ItemData>();

    [Header("적 정보")]
    public List<bool> isClearBoss = new List<bool>() { false, false, false };
    public List<EnemyData> Monsters;

    private void SetEnemyData()
    {
        CSVReader.CSVRead("enemy.csv", (List<string> data) =>
        {
            EnemyData enemyData = new EnemyData();

            enemyData.SetData(data);
            Monsters.Add(enemyData);
        });
    }


    public void Load()
    {
        SaveData saveData = new SaveData();

        if(!File.Exists(path))
        {
            clearCount = 0;
            lastPosition = Vector3.zero;
            lastRotation = Quaternion.identity;
            isClearBoss = new List<bool>(3);
            Save();
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            if(saveData != null)
            {
                clearCount = saveData.clearCount;
                deadCount = saveData.deadCount;
                lastPosition = saveData.lastPosition;
                lastRotation = saveData.lastRotation;
                currentItem = saveData.currentItem;
                isClearBoss = saveData.isClearBoss;
            }
        }
    }


    public void Save()
    {
        SaveData saveData = new SaveData();

        saveData.clearCount = clearCount;
        saveData.deadCount = deadCount;
        saveData.lastPosition = lastPosition;
        saveData.lastRotation = lastRotation;
        saveData.currentItem = currentItem;
        saveData.isClearBoss = isClearBoss;

        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(path, json);
    }


    public void ClearSaveData()
    {
        SaveData saveData = new SaveData();

        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(path, json);

        Load();
    }
}
