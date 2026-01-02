using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PanelCisnienia : MonoBehaviour
{
    [Header("Przycisk XR")]
    [SerializeField] private XRSimpleInteractable przycisk;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI napisOnOff;   // TextMeshPro - OFF/ON

    [Header("Woda")]
    [SerializeField] private StrumienWody strumienWody;

    [Header("Start")]
    [SerializeField] private bool startOn = false;

    private bool isOn;

    private void Awake()
    {
        UstawStan(startOn);
    }

    private void OnEnable()
    {
        if (przycisk != null)
            przycisk.selectEntered.AddListener(OnPressed);
    }

    private void OnDisable()
    {
        if (przycisk != null)
            przycisk.selectEntered.RemoveListener(OnPressed);
    }

    private void OnPressed(SelectEnterEventArgs args)
    {
        UstawStan(!isOn);
    }

    private void UstawStan(bool nowyStan)
    {
        isOn = nowyStan;

        if (napisOnOff != null)
            napisOnOff.text = isOn ? "ON" : "OFF";

        if (strumienWody != null)
            strumienWody.CisnienieWlaczone = isOn;
    }
}
