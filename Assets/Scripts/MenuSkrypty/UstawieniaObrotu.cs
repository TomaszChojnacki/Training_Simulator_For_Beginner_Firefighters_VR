using UnityEngine;
using TMPro;

// Zarz¹dza systemem obracania: Continuous Turn / Snap Turn.
// - Domyœlnie (po uruchomieniu gry) ustawia obracanie ci¹g³e.
// - Zmiana w trakcie gry dzia³a we wszystkich scenach (w tej sesji).
// - Po zamkniêciu gry ustawienie wraca do ci¹g³ego (brak zapisu na dysk).
public class UstawieniaObrotu : MonoBehaviour
{
    [Header("Skrypty obracania")]
    public MonoBehaviour continuousTurn; 
    public MonoBehaviour snapTurn;       

    [Header("UI (opcjonalne)")]
    public TMP_Dropdown dropdownTrybObrotu;

    // 0 = continuous, 1 = snap
    // Static = wspólne dla wszystkich scen w czasie JEDNEGO uruchomienia gry
    private static int globalTrybObrotu = 0; // domyœlnie ci¹g³e obracanie

    private void Awake()
    {
        // Ustawiamy tryb na podstawie globalnej zmiennej (wspólnej miêdzy scenami)
        UstawTrybObrotu(globalTrybObrotu);

        // Jeœli w tej scenie mamy dropdown, podpinamy go
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

    // Wywo³ywane przez dropdown (OnValueChanged).
    public void OnDropdownZmianaTrybu(int index)
    {
        // Zapisujemy do zmiennej statycznej (dzia³a w ca³ej sesji gry)
        globalTrybObrotu = index;

        // W³¹czamy odpowiedni tryb
        UstawTrybObrotu(index);
    }

    // W³¹cza/wy³¹cza odpowiednie skrypty obracania.
    // 0 = continuous, 1 = snap.
    private void UstawTrybObrotu(int index)
    {
        bool continuous = (index == 0);

        if (continuousTurn != null)
            continuousTurn.enabled = continuous;

        if (snapTurn != null)
            snapTurn.enabled = !continuous;

        if (dropdownTrybObrotu != null)
        {
            dropdownTrybObrotu.value = index;
            dropdownTrybObrotu.RefreshShownValue();
        }
    }
}
