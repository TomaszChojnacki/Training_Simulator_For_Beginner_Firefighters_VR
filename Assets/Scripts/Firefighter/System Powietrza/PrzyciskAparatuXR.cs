using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PrzyciskAparatuXR : MonoBehaviour
{
    public enum KtoryZestaw
    {
        Zestaw1 = 1,
        Zestaw2 = 2
    }

    [SerializeField] private AparatTlenowyManager manager;
    [SerializeField] private KtoryZestaw zestaw = KtoryZestaw.Zestaw1;
    [SerializeField] private XRSimpleInteractable interactable;

    private void Reset()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void Awake()
    {
        if (!interactable)
            interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        if (interactable)
            interactable.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        if (interactable)
            interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!manager) return;

        if (zestaw == KtoryZestaw.Zestaw1)
            manager.KlikZestaw1();
        else
            manager.KlikZestaw2();
    }
}
