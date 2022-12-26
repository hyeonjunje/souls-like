using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SE
{
    ChestOpen,
    PickUpItem,
    DoorOpen
}

public class WorldSoundManager : MonoBehaviour
{
    public static WorldSoundManager instance;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        _audioSource = GetComponent<AudioSource>();
    }

    public AudioClip bossBGM;

    public AudioClip[] soundEffect;
    public void ActiveBossBGM(bool active)
    {
        _audioSource.Stop();
        if (active)
        {
            _audioSource.clip = bossBGM;
            _audioSource.Play();
        }
    }

    public void PlaySoundEffect(SE se)
    {
        _audioSource.PlayOneShot(soundEffect[(int)se]);
    }
}
