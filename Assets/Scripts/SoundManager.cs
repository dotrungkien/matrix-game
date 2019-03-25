using UnityEngine;
using System.Collections;

public class SoundManager : Singleton<SoundManager>
{
    public AudioClip clickSound;
    public AudioClip tileSound;
    public AudioSource mySources;

    private bool soundOn;

    public void EnableSound()
    {
        MakeClickSound();
        mySources.volume = 1f;
        soundOn = true;
    }

    public void DisableSound()
    {
        mySources.volume = 0f;
        soundOn = false;
    }

    public void MakeClickSound()
    {
        mySources.clip = clickSound;
        MakeSound();
    }

    public void MakeTileSound()
    {
        mySources.clip = tileSound;
        MakeSound();
    }

    private void MakeSound()
    {
        mySources.Play();
    }

    private void StopSound()
    {
        mySources.Stop();
    }
}
