using UnityEngine;

public class FirePoint : MonoBehaviour
{
    [Header("Stan ognia")]
    [Range(0f, 1f)] public float intensity = 1f;
    public float gaszenieNaSekunde = 0.35f;
    public float progZgaszenia = 0.05f;

    [Header("Regeneracja ognia")]
    public float opoznienieRegeneracji = 2.0f;
    public float regeneracjaNaSekunde = 0.15f;

    [Range(0f, 1f)] public float docelowaIntensywnosc = 1f;
    [Header("VFX ognia")]
    public Transform fireVfxRoot;

    [Header("Skalowanie VFX")]
    public float minScale = 0.15f;
    public float maxScale = 1.0f;

    private SystemOgniaManager manager;
    private bool dead;

    private float lastWaterTime = -999f; // kiedy ostatnio trafiono woda
    private float startIntensity = 1f;   // intensywnosc poczatkowa dla tego punktu

    public void Init(SystemOgniaManager mgr, Transform fireVfx)
    {
        manager = mgr;
        fireVfxRoot = fireVfx;

        // zapamietaj stan poczatkowy
        startIntensity = Mathf.Clamp01(intensity);
        if (docelowaIntensywnosc <= 0f) docelowaIntensywnosc = startIntensity;
        else docelowaIntensywnosc = Mathf.Clamp01(docelowaIntensywnosc);

        ZastosujWyglad();
    }

    private void Update()
    {
        if (dead) return;

        // jesli nie jest polewane przez pewien czas -- regeneruj
        if (Time.time - lastWaterTime >= opoznienieRegeneracji)
        {
            // wracamy do intensywnosci 1
            if (intensity < docelowaIntensywnosc)
            {
                intensity = Mathf.Min(docelowaIntensywnosc, intensity + regeneracjaNaSekunde * Time.deltaTime);
                ZastosujWyglad();
            }
        }
    }

    // Wywolywanie gdy woda trafia w punkt ognia
    public void ApplyWater(float amount)
    {
        if (dead) return;

        lastWaterTime = Time.time;

        intensity = Mathf.Max(0f, intensity - gaszenieNaSekunde * amount);
        ZastosujWyglad();

        if (intensity <= progZgaszenia)
            Zgas();
    }

    private void ZastosujWyglad()
    {
        if (!fireVfxRoot) return;

        float scale = Mathf.Lerp(minScale, maxScale, intensity);
        fireVfxRoot.localScale = Vector3.one * scale;
    }

    private void Zgas()
    {
        if (dead) return;
        dead = true;

        if (manager)
            manager.NotifyExtinguished(this);

        Destroy(gameObject);
    }
}
