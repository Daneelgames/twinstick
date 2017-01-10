using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicPlayerController : MonoBehaviour
{
    public List<AudioSource> au;
    public AudioSource activeAu;
    bool crossfade = false;

    public void SetBgm(AudioClip c)
    {
        print (c + "; au clip is " + activeAu.clip);
        foreach (AudioSource j in au)
        {
            if (!j.clip)
            {
                activeAu = j;
                activeAu.clip = c;
                activeAu.Play();
                StartCoroutine("Crossfade");
                break;
            }
        }
    }

    IEnumerator Crossfade()
    {
        crossfade = true;
        yield return new WaitForSeconds(2f);
        foreach (AudioSource j in au)
        {
            if (j == activeAu)  
            {
                j.volume = 0.5f;
            }
            else
            {
                j.volume = 0;
                j.Stop();
                j.clip = null;
            }
        }
        crossfade = false;
    }

    void Update()
    {
        if (crossfade)
        {
            foreach (AudioSource j in au)
            {
                if (j == activeAu)
                {
                    j.volume = Mathf.Lerp(j.volume, 0.5f, Time.unscaledDeltaTime);
                }
                else
                {
                    j.volume = Mathf.Lerp(j.volume, 0, Time.unscaledDeltaTime);
                }
            }
        }
    }
}