using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioMotorPitch : MonoBehaviour
{
    public float pitchScalar = 2.5f;
    public float manufacturingTolerance = 0.2f;
    private Motor motor = null;
    private AudioSource[] audioSources;
    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponentInParent<Motor>();
        pitchScalar += UnityEngine.Random.Range(-manufacturingTolerance, manufacturingTolerance);
    }

    // Update is called once per frame
    void Update()
    {
        audioSources = GetComponents<AudioSource>();

        float rpmScalar = motor.rpm / motor.maxRpm;
        float pitch = pitchScalar * rpmScalar;
        foreach (var source in audioSources)
            source.pitch = pitch;
    }
}
