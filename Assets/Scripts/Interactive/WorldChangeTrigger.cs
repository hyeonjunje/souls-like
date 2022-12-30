using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChangeTrigger : MonoBehaviour
{
    [SerializeField] private string worldName;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("플레이어 감지");
            WorldUIController.instance.EnterOtherWorld(worldName);
        }
    }
}
