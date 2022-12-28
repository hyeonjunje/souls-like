using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundManager : MonoBehaviour
{
    [Header("Hit Sound")]
    public AudioClip[] hitSounds;
    private List<AudioClip> potentialHitSounds;
    private AudioClip lastHitSound;

    [Header("Footprint Sound")]
    public AudioClip[] footprintSounds;
    private List<AudioClip> potentialFootprintSounds;
    private AudioClip lastFootprintSound;

    [Header("Attack Sound")]
    public AudioClip[] attackSounds;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }



    public void PlayRandomFootprintSound()
    {
        potentialFootprintSounds = new List<AudioClip>();

        if (footprintSounds.Length == 1)
        {
            audioSource.PlayOneShot(footprintSounds[0]);
            return;
        }
        else if (footprintSounds.Length == 1)
            return;

        foreach(AudioClip clip in footprintSounds)
        {
            if (clip != lastFootprintSound)
                potentialFootprintSounds.Add(clip);
        }

        int index = Random.Range(0, potentialFootprintSounds.Count);


        audioSource.PlayOneShot(potentialFootprintSounds[index]);
        lastFootprintSound = potentialFootprintSounds[index];
    }


    public void PlayRandomHitSound()
    {
        potentialHitSounds = new List<AudioClip>();

        if (hitSounds.Length == 1)
        {
            audioSource.PlayOneShot(hitSounds[0]);
            return;
        }
        else if (hitSounds.Length == 1)
            return;

        foreach (AudioClip clip in hitSounds)
        {
            if (clip != lastHitSound)
                potentialHitSounds.Add(clip);
        }

        int index = Random.Range(0, potentialHitSounds.Count);

        audioSource.PlayOneShot(potentialHitSounds[index]);
        lastHitSound = potentialHitSounds[index];
    }


    public void PlayAttackSound(int index)
    {
        if (attackSounds.Length <= index)
            return;
        audioSource.PlayOneShot(attackSounds[index]);
    }
}
