using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMarkLookAt : MonoBehaviour
{
    private Transform _player;

    private void Start()
    {
        _player = GameObject.Find("PlayerCam").transform;
    }


    private void Update()
    {
        transform.LookAt(_player.position);
    }
}
