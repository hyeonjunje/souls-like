using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public Transform enemyParent;
    public List<GameObject> monstersPrefab;

    private List<EnemyData> Monsters => DataManager.instance.Monsters;


    public void SpawnEnemy()
    {
        ClearEnemy();

        foreach (EnemyData data in Monsters)
        {
            Transform monster = Instantiate(monstersPrefab[data.Type]).transform;
            monster.GetComponent<NavMeshAgent>().Warp(new Vector3(data.PosX, data.PosY, data.PosZ));
            monster.localEulerAngles = new Vector3(data.RotX, data.RotY, data.RotZ);
            monster.SetParent(enemyParent);
        }
    }


    private void ClearEnemy()
    {
        Transform[] child = enemyParent.GetComponentsInChildren<Transform>();

        foreach (Transform iter in child)
        {
            if (iter != enemyParent)
                Destroy(iter.gameObject);
        }
    }
}
