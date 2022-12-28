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
            UIController.instance.ShowInteractiveEnterText("ÈÞ½ÄÇÏ±â");
    }

    public void ExitInteractZone()
    {
        if(isActive)
            UIController.instance.HideInteractiveExitText();
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
            UIController.instance.HideInteractiveExitText();
            _player.RestInBonfire();

            DataManager.instance.lastPosition = _player.transform.position;
            DataManager.instance.lastRotation = _player.transform.rotation;

            DataManager.instance.Save();
        }
    }


    public void ActiveBonfire()
    {
        bonfireEffect.Play();

        isActive = true;
    }
}
