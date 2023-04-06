using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class GranularSynthesis : MonoBehaviour
{
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    [SerializeField, Range(0.0f, 1.0f)] private float rev = 0;
    [SerializeField] private float variance = 0.05f;
    [SerializeField] private float grainTime = 300.0f;
    [SerializeField] private float envelopeTime = 50.0f;
    [SerializeField] private float clipMargin = 0.1f;

    private AudioSource source;
    private AudioSource source2;
    private bool useAudioSource2 = false;
    private Dictionary<AudioClip, float[][]> clipGrains = new Dictionary<AudioClip, float[][]>();
    private AudioClip selectedClip;
    private LinkedList<int> grainSchedule = new LinkedList<int>();
    private int currentGrain = 0;
    private double lastPlayTime = AudioSettings.dspTime;
    private double lastPlayDuration = 0;

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source2 = gameObject.AddComponent<AudioSource>();

        if (clips.Count == 0)
            Debug.LogWarning($"The input audio clips of {gameObject.name} are empty. This will likely result in an error. Please assign clips in the inspector.");

        if (envelopeTime > grainTime)
        {
            envelopeTime = grainTime / 2.0f;
            Debug.LogWarning("The envelope size was higher than half the grainTime and has been adjusted.");
        }

        foreach (AudioClip clip in clips)
        {
            GenerateGrains(clip);
        }
    }

    private void Update()
    {
        // Just cycles through throttle for debug
        rev = 1.0f - Mathf.Abs(Mathf.Sin(Time.time / 5.0f));

        ScheduleGrains();

        PlayScheduled();
    }

    public void SetRev(float rev)
    {
        this.rev = Mathf.Clamp(rev, 0.0f, 1.0f);
    }

    private void GenerateGrains(AudioClip clip)
    {
        int samples = clip.samples;
        float[] data = new float[samples];
        clip.GetData(data, 0);

        int grainSize = (int)((double)grainTime / 1000.0 * clip.frequency);
        LinkedList<int> grainIndices = new LinkedList<int>();
        int envelopeSize = (int)((double)envelopeTime / 1000.0 * clip.frequency);

        source.clip = AudioClip.Create("grain", (int)(grainSize * 2.0f), 1, clip.frequency, false);
        source2.clip = AudioClip.Create("grain2", (int)(grainSize * 2.0f), 1, clip.frequency, false);

        grainIndices.AddLast(0);
        for (int index = grainSize; index < samples; index += grainSize)
        {
            for (int r = 0; r < grainSize / 2; r++)
            {
                int right = index - r;

                if (Mathf.Abs(data[r]) < clipMargin)
                {
                    index = right;
                    break;
                }
            }

            data[index] = 0;
            if (index + 1 < samples) data[index + 1] *= 0.5f;
            if (index > 0) data[index - 1] *= 0.5f;

            grainIndices.AddLast(index);
        }

        float[][] grains = new float[grainIndices.Count][];
        LinkedListNode<int> node = grainIndices.First;
        for (int i = 0; i < grainIndices.Count; i++)
        {
            LinkedListNode<int> next = node.Next;

            int index = node.Value;
            int nextIndex;

            if (next == null)
            {
                nextIndex = samples;

                if (nextIndex - index <= (grainSize + envelopeSize) / 2)
                    index = node.Previous.Value;
            }
            else
                nextIndex = next.Value;

            grains[i] = new float[nextIndex - index];

            for (int sampleNumber = 0, runningIndex = index; runningIndex < nextIndex; runningIndex++, sampleNumber++)
            {
                float fadeIn = Mathf.Clamp((float)sampleNumber / (float)envelopeSize, 0.0f, 1.0f);
                float fadeOut = Mathf.Clamp((float)(sampleNumber - grains[i].Length) / -(float)envelopeSize, 0.0f, 1.0f);
                float envMul = fadeIn * fadeOut;
                if (float.IsNaN(envMul) || float.IsInfinity(envMul)) envMul = 1.0f;
                if (envelopeTime == 0) envMul = 1.0f;

                grains[i][sampleNumber] = data[runningIndex] * envMul;
            }

            node = next;
        }

        clipGrains[clip] = grains;
    }
    private void ScheduleGrains()
    {
        while (grainSchedule.Count < Mathf.Max(1, (2 * Time.deltaTime / (grainTime / 1000.0f))))
        {
            selectedClip = clips[Random.Range(0, clips.Count)];
            float[][] selectedGrains = clipGrains[selectedClip];

            int grainIndex = (int)(selectedGrains.Length * (rev + (Random.value * 2.0f - 1.0f) * variance));
            grainIndex = Mathf.Clamp(grainIndex, 0, selectedGrains.Length - 1);

            grainSchedule.AddLast(grainIndex);
        }
    }

    private void PlayScheduled()
    {
        if (AudioSettings.dspTime >= lastPlayTime + lastPlayDuration * 0.75 - (double)envelopeTime / 1000.0)
        {
            int first = grainSchedule.First.Value;
            grainSchedule.RemoveFirst();

            clipGrains.TryGetValue(selectedClip, out float[][] grains);

            double duration = (double)grains[currentGrain].Length / selectedClip.frequency;
            double playTime = lastPlayTime + lastPlayDuration - (double)envelopeTime / 1000.0 - 0.1;

            AudioSource useSource = (useAudioSource2) ? source2 : source;
            useSource.clip.SetData(grains[first], 0);
            useSource.PlayScheduled(playTime);

            lastPlayDuration = duration;
            lastPlayTime = playTime;
            currentGrain = first;
            useAudioSource2 = !useAudioSource2;
        }
    }
}