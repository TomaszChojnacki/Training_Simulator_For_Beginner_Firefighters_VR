using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PrzelacznikElektrykiXR : MonoBehaviour
{
    [Header("Referencje")]
    [SerializeField] private SystemElektrykiManager manager;
    [SerializeField] private XRSimpleInteractable interactable;

    [Header("Napis na Canvasie (jedno z nich)")]
    [SerializeField] private TMP_Text tmpText;

    [Header("Stan startowy")]
    [SerializeField] private bool startOn = true;

    public bool CzyOn { get; private set; }

    private void Reset()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void Awake()
    {
        CzyOn = startOn;
        OdswiezNapis();
    }

    private void OnEnable()
    {
        if (!interactable) interactable = GetComponent<XRSimpleInteractable>();
        if (interactable)
            interactable.selectEntered.AddListener(OnPressed);
    }

    private void OnDisable()
    {
        if (interactable)
            interactable.selectEntered.RemoveListener(OnPressed);
    }

    private void OnPressed(SelectEnterEventArgs args)
    {
        // toggle ON/OFF
        CzyOn = !CzyOn;
        OdswiezNapis();

        if (manager)
            manager.OdswiezStanPradu();
    }

    private void OdswiezNapis()
    {
        string s = CzyOn ? "ON" : "OFF";

        if (tmpText) tmpText.text = s;
    }
}
