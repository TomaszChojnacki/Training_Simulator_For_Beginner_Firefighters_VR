using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PrzyciskAparatuXR : MonoBehaviour
{
    [SerializeField] private AparatTlenowyManager manager;
    [SerializeField] private XRSimpleInteractable interactable;

    private void Reset()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        if (!interactable) interactable = GetComponent<XRSimpleInteractable>();
        if (interactable)
            interactable.selectEntered.AddListener(_ => Klik());
    }

    private void OnDisable()
    {
        if (interactable)
            interactable.selectEntered.RemoveListener(_ => Klik());
    }

    private void Klik()
    {
        if (manager) manager.ToggleAparat();
    }
}
