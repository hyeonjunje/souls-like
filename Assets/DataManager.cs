using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

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

        Debug.Log(Type + " " + PosX);
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
    }

    public Vector3 lastPosition = Vector3.zero;
    public Quaternion lastRotation = Quaternion.identity;

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
}
