using UnityEngine;
using TMPro;

public class Ustawienia : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    private void Awake()
    {
        // Jeœli zapomnisz przypisaæ w Inspectorze, spróbuje znaleŸæ sam
        if (dropdown == null)
            dropdown = GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        // Ustaw dropdown na aktualny poziom jakoœci
        int currentQuality = QualitySettings.GetQualityLevel();
        dropdown.value = currentQuality;
        dropdown.RefreshShownValue();

        // Reaguj na zmianê opcji w menu
        dropdown.onValueChanged.AddListener(ChangeLevel);
    }

    private void OnDestroy()
    {
        dropdown.onValueChanged.RemoveListener(ChangeLevel);
    }

    private void ChangeLevel(int value)
    {
        // Zmieniamy jakoœæ grafiki
        QualitySettings.SetQualityLevel(value, true);
        //Debug.Log("Zmieniono jakoœæ na: " + QualitySettings.names[value]);
    }
}
