using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonfire : MonoBehaviour, IInteractable
{
    [SerializeField] private ParticleSystem bonfireEffect;

    public bool isActive = true;

    private Player _player;

    private void Start()
    {
        _player = GameObject.FindObjectOfType<Player>();

        if (isActive)
        {
            bonfireEffect.Play();
        }
        else
            bonfireEffect.Stop();
    }

    public void EnterInteractZone()
    {
        if(isActive)
            Debug.Log("E�� ������ ���ϴ�.");
    }

    public void ExitInteractZone()
    {
        if(isActive)
            Debug.Log("�����ϴ�.");
    }

    public Vector3 GetPos()
    {
        if (isActive)
            return transform.position;
        else
            return Vector3.zero;
    }

    public void Interact()
    {
        if(isActive)
        {
            _player.RestInBonfire();
            Debug.Log("�޽�");
        }
    }


    public void ActiveBonfire()
    {
        bonfireEffect.Play();

        isActive = true;
    }
}
