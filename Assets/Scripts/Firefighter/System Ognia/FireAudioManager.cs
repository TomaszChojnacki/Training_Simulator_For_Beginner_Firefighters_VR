using System.Collections.Generic;
using UnityEngine;

public class FireAudioManager : MonoBehaviour
{
    [Header("Referencje")]
    [SerializeField] private Transform gracz;           // np. Main Camera
    [SerializeField] private AudioClip fireLoop;        // clip ognia (loop)

    [Header("èrÛd≥a düwiÍku (dok≥adnie 2)")]
    [SerializeField] private AudioSource sourceA;
    [SerializeField] private AudioSource sourceB;

    [Header("Ustawienia")]
    [SerializeField, Min(0.05f)] private float updateInterval = 0.2f;

    [Header("G≥oúnoúÊ")]
    [SerializeField, Range(0f, 1f)] private float baseVolume = 1f;
    [SerializeField] private bool volumeOdIntensywnosci = true;

    [Header("Wyg≥adzanie prze≥πczeÒ (fade)")]
    [SerializeField, Min(0f)] private float fadeSpeed = 8f; // im wiÍksze, tym szybciej

    private readonly List<FirePoint> fires = new();

    private FirePoint nearest1;
    private FirePoint nearest2;

    private float timer;

    private void Awake()
    {
        SetupSource(sourceA);
        SetupSource(sourceB);
    }

    private void SetupSource(AudioSource s)
    {
        if (!s) return;
        s.loop = true;
        s.playOnAwake = false;
        if (fireLoop) s.clip = fireLoop;
        s.volume = 0f;
    }

    public void Register(FirePoint fp)
    {
        if (fp && !fires.Contains(fp))
            fires.Add(fp);
    }

    public void Unregister(FirePoint fp)
    {
        fires.Remove(fp);
        if (nearest1 == fp) nearest1 = null;
        if (nearest2 == fp) nearest2 = null;
    }

    private void Update()
    {
        if (!gracz) return;

        // regularny update wyboru najbliøszych
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = updateInterval;
            CleanupNulls();
            FindTwoNearest(gracz.position, out nearest1, out nearest2);
        }

        // ustawienia ürÛde≥ (pozycja + g≥oúnoúÊ z fade)
        UpdateSource(sourceA, nearest1);
        UpdateSource(sourceB, nearest2);
    }

    private void CleanupNulls()
    {
        for (int i = fires.Count - 1; i >= 0; i--)
            if (fires[i] == null) fires.RemoveAt(i);
    }

    private void FindTwoNearest(Vector3 from, out FirePoint a, out FirePoint b)
    {
        a = null;
        b = null;

        float best1 = float.MaxValue;
        float best2 = float.MaxValue;

        foreach (var f in fires)
        {
            if (!f) continue;
            float d = (f.transform.position - from).sqrMagnitude;

            if (d < best1)
            {
                // przesuwamy najlepszy do drugiego
                best2 = best1;
                b = a;

                best1 = d;
                a = f;
            }
            else if (d < best2 && f != a)
            {
                best2 = d;
                b = f;
            }
        }
    }

    private void UpdateSource(AudioSource s, FirePoint target)
    {
        if (!s) return;

        if (target == null)
        {
            // fade out
            s.volume = Mathf.MoveTowards(s.volume, 0f, fadeSpeed * Time.deltaTime);
            if (s.volume <= 0.001f && s.isPlaying) s.Stop();
            return;
        }

        // pozycja düwiÍku = pozycja ognia
        s.transform.position = target.transform.position;

        // docelowa g≥oúnoúÊ
        float v = baseVolume;
        if (volumeOdIntensywnosci) v *= Mathf.Clamp01(target.intensity);

        // fade do docelowej
        s.volume = Mathf.MoveTowards(s.volume, v, fadeSpeed * Time.deltaTime);

        if (!s.isPlaying) s.Play();
    }
}
