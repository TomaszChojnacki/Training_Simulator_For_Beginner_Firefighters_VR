using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

public class TylkoJakUiWidoczne : MonoBehaviour
{
    [Header("Referencje do interakcji XR - UI")]
    public XRRayInteractor rayInteractor;
    public XRInteractorLineVisual lineVisual;
    public CanvasGroup menuCanvasGroup;

    void Reset()
    {
        // Automatyczne pobranie komponentow z tego samego obiektu
        if (rayInteractor == null)
            rayInteractor = GetComponent<XRRayInteractor>();

        if (lineVisual == null)
            lineVisual = GetComponent<XRInteractorLineVisual>();
    }

    void LateUpdate()
    {
        // Jesli brakuje referencji –- nie rob nic
        if (menuCanvasGroup == null || rayInteractor == null || lineVisual == null)
            return;

        // Czy menu jest widoczne (alpha > 0)
        bool menuWidoczne = menuCanvasGroup.alpha > 0.01f;

        if (!menuWidoczne)
        {
            // Wylaczenie interakcji i linii
            rayInteractor.enabled = false;
            lineVisual.enabled = false;

            // Jesli uzywany jest LineRenderer tez jest wylaczany
            var lr = GetComponent<LineRenderer>();
            if (lr != null)
                lr.enabled = false;
        }
        else
        {
            // MENU WIDOCZNE wlaczamy ray
            rayInteractor.enabled = true;
        }
    }
}
