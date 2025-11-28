using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

public class PokazRayNaUI : MonoBehaviour
{
    [Header("Referencje")]
    public XRRayInteractor rayInteractor;
    public XRInteractorLineVisual lineVisual;

    private void Reset()
    {
        // Automatyczne pobranie komponentów z tego samego obiektu
        if (rayInteractor == null)
            rayInteractor = GetComponent<XRRayInteractor>();

        if (lineVisual == null)
            lineVisual = GetComponent<XRInteractorLineVisual>();
    }

    private void Update()
    {
        if (rayInteractor == null || lineVisual == null)
            return;

        // Sprawdzamy, czy ray aktualnie trafia w UI
        bool trafiaWUI = false;

        if (rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult wynik))
        {
            if (wynik.gameObject != null)
            {
                trafiaWUI = true;
            }
        }

        // Pokazujemy liniê tylko, jeœli celujemy w UI
        lineVisual.enabled = trafiaWUI;

        // Jeœli u¿ywasz te¿ LineRenderera, mo¿esz go te¿ prze³¹czaæ:
        var lr = GetComponent<LineRenderer>();
        if (lr != null)
            lr.enabled = trafiaWUI;
    }
}
