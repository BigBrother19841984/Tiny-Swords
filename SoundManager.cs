using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [Header("--- Audio ---")]
    public AudioSource audioSource;
    public AudioClip[] popUpSounds;
    public AudioClip selectSound;

    public void PopUpSound()
    {
        audioSource.clip = popUpSounds[0];
        audioSource.Play();
    }

    public void PopUpClose()
    {
        audioSource.clip = popUpSounds[1];
        audioSource.Play();
    }

    public void PlaySelectSound()
    {
        audioSource.clip = selectSound;
        audioSource.Play();
    }

}
