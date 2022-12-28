using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public Transform enemyParent;
    public List<GameObject> monstersPrefab;
    public List<BossEvent> bosses;
    public List<Chest> chests;

    private List<EnemyData> Monsters => DataManager.instance.Monsters;

    private List<Vector3> bossesPos = new List<Vector3>();
    private List<Quaternion> bossesRot = new List<Quaternion>();

    public void SpawnEnemy()
    {
        ClearEnemy();

        // 적 설정
        foreach (EnemyData data in Monsters)
        {
            Transform monster = Instantiate(monstersPrefab[data.Type]).transform;
            monster.GetComponent<NavMeshAgent>().Warp(new Vector3(data.PosX, data.PosY, data.PosZ));
            monster.localEulerAngles = new Vector3(data.RotX, data.RotY, data.RotZ);
            monster.SetParent(enemyParent);
        }

        // 보스 설정
        for(int i = 0; i < DataManager.instance.isClearBoss.Count; i++)
        {
            if (bossesPos.Count != bosses.Count)
            {
                bossesPos.Add(bosses[i].boss.transform.localPosition);
                bossesRot.Add(bosses[i].boss.transform.localRotation);
            }
            bosses[i].boss.transform.localPosition = bossesPos[i];
            bosses[i].boss.transform.localRotation = bossesRot[i];

            if (DataManager.instance.isClearBoss[i])
            {
                bosses[i].gameObject.SetActive(false);
                if (bosses[i].bonfire != null)
                    bosses[i].bonfire.ActiveBonfire();

                if (bosses[i].chair != null)
                    bosses[i].chair.ActiveChair();
            }
        }

        // 보물상자 설정
        for (int i = 0; i < DataManager.instance.currentItem.Count; i++)
        {
            for(int j = 0; j < chests.Count; j++)
            {
                if (DataManager.instance.currentItem[i].itemId == chests[j].itemData.itemId)
                    chests[j].gameObject.SetActive(false);
            }
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
