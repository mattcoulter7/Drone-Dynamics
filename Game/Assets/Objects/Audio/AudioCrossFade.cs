using System;
using System.Collections;
using UnityEngine;

public class AudioCrossFade : MonoBehaviour
{
    public float crossFadeOffset = 1f;
    public float crossFadeDuration = 0.5f;

    private AudioSource player;
    private AudioSource fader;

    void Start()
    {
        player = gameObject.GetComponent<AudioSource>();
        fader = gameObject.AddComponent<AudioSource>();

        // Copy the audio source settings from source1 to source2
        fader.clip = player.clip;
        fader.loop = player.loop;
        fader.volume = 0f;
        fader.spatialBlend = player.spatialBlend;
        fader.playOnAwake = false;
        fader.Stop();

        StartCoroutine(CrossFadeTimer());
    }
    IEnumerator CrossFadeTimer()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(player.clip.length - crossFadeOffset);
            StartCoroutine(CrossFade());
        }
    }
    IEnumerator CrossFade()
    {
        float t = 0f;
        fader.Play();
        while (t < crossFadeDuration)
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / crossFadeDuration);
            player.volume = 1f - blend;
            fader.volume = blend;
            yield return null;
        }
        player.Stop();
        SwapAudioSource();
    }

    void SwapAudioSource()
    {
        AudioSource temp = player;
        player = fader;
        fader = temp;
    }
}