using UnityEngine;

public class AparatZestaw : MonoBehaviour
{
    [Header("Parametry butli")]
    [SerializeField, Min(1f)] private float pojemnoscTlenu = 100f;

    [Tooltip("Aktualny tlen w butli. Stan jest zapisywany w trakcie gry.")]
    [SerializeField] private float aktualnyTlen = 100f;

    public float PojemnoscTlenu => pojemnoscTlenu;

    public float AktualnyTlen
    {
        get => aktualnyTlen;
        set => aktualnyTlen = Mathf.Clamp(value, 0f, pojemnoscTlenu);
    }

    private void OnValidate()
    {
        AktualnyTlen = aktualnyTlen;
    }
}
