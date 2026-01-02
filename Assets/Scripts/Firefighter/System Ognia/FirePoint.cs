using UnityEngine;

public class FirePoint : MonoBehaviour
{
    [Header("Stan ognia")]
    [Range(0f, 1f)] public float intensity = 1f;

    [Tooltip("Jak szybko ogieñ gaœnie przy ci¹g³ym polewaniu (na sekundê).")]
    public float gaszenieNaSekunde = 0.35f;

    [Tooltip("Próg, przy którym punkt ognia zostaje usuniêty.")]
    public float progZgaszenia = 0.05f;

    [Header("VFX ognia (prefab zawiera te¿ dym)")]
    public Transform fireVfxRoot;

    [Header("Skalowanie VFX")]
    public float minScale = 0.15f;
    public float maxScale = 1.0f;

    private SystemOgniaManager manager;
    private bool dead;

    public void Init(SystemOgniaManager mgr, Transform fireVfx)
    {
        manager = mgr;
        fireVfxRoot = fireVfx;
        ZastosujWyglad();
    }

    /// <summary>Wywo³uj gdy woda trafia w punkt (amount = Time.deltaTime).</summary>
    public void ApplyWater(float amount)
    {
        if (dead) return;

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
