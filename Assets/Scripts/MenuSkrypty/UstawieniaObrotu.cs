using UnityEngine;
using TMPro;

// Zarz¹dza systemem obracania: Snap Turn / Continuous Turn.
// - Domyœlnie po uruchomieniu gry: SNAP TURN (index 0).
// - Zmiana dzia³a we wszystkich scenach w trakcie jednej sesji.
// - Po zamkniêciu gry ustawienie wraca do SNAP TURN.

public class UstawieniaObrotu : MonoBehaviour
{
    [Header("Skrypty obracania")]
    public MonoBehaviour snapTurn;        // index 0
    public MonoBehaviour continuousTurn;  // index 1

    [Header("UI (opcjonalne)")]
    public TMP_Dropdown dropdownTrybObrotu;

    // 0 = snap, 1 = continuous
    // static wspólne dla wszystkich scen w tej sesji
    private static int globalTrybObrotu = 0; // DOMYŒLNIE SNAP TURN

    private void Awake()
    {
        // Ustawiamy aktualny tryb obrotu
        UstawTrybObrotu(globalTrybObrotu);

        // Konfiguracja dropdowna (jeœli istnieje w tej scenie)
        if (dropdownTrybObrotu != null)
        {
            dropdownTrybObrotu.onValueChanged.AddListener(OnDropdownZmianaTrybu);
            dropdownTrybObrotu.value = globalTrybObrotu;
            dropdownTrybObrotu.RefreshShownValue();
        }
    }

    private void OnDestroy()
    {
        if (dropdownTrybObrotu != null)
        {
            dropdownTrybObrotu.onValueChanged.RemoveListener(OnDropdownZmianaTrybu);
        }
    }

    // Wywo³ywane przez TMP_Dropdown (OnValueChanged)

    public void OnDropdownZmianaTrybu(int index)
    {
        globalTrybObrotu = index;
        UstawTrybObrotu(index);
    }

    // W³¹cza odpowiedni system obracania
    // 0 = Snap Turn
    // 1 = Continuous Turn
    private void UstawTrybObrotu(int index)
    {
        bool snap = (index == 0);

        if (snapTurn != null)
            snapTurn.enabled = snap;

        if (continuousTurn != null)
            continuousTurn.enabled = !snap;

        if (dropdownTrybObrotu != null && dropdownTrybObrotu.value != index)
        {
            dropdownTrybObrotu.value = index;
            dropdownTrybObrotu.RefreshShownValue();
        }
    }
}
