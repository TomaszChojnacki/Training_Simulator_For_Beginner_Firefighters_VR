using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HandGrabCollisionFilter : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] XRBaseInteractor interactor;          // np. XRDirectInteractor na rêce
    [SerializeField] List<Collider> handColliders = new(); // fizyczne collidery rêki (te, które maj¹ NIE byæ w kolizji z trzymanym obiektem)

    // zapamiêtujemy pary, które ignorujemy, ¿eby je potem przywróciæ
    readonly HashSet<(Collider a, Collider b)> ignoredPairs = new();

    void Reset()
    {
        interactor = GetComponent<XRBaseInteractor>();
        // domyœlnie zbierz wszystkie collidery z rêki
        GetComponentsInChildren(true, handColliders);
    }

    void OnEnable()
    {
        if (!interactor) interactor = GetComponent<XRBaseInteractor>();
        if (interactor is XRBaseInteractor i)
        {
            i.selectEntered.AddListener(OnSelectEntered);
            i.selectExited.AddListener(OnSelectExited);
        }
    }

    void OnDisable()
    {
        if (interactor is XRBaseInteractor i)
        {
            i.selectEntered.RemoveListener(OnSelectEntered);
            i.selectExited.RemoveListener(OnSelectExited);
        }
        RestoreAll(); // porz¹dek na wypadek stopu Play Mode
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        var otherCols = args.interactableObject.transform.GetComponentsInChildren<Collider>(true);
        foreach (var hc in handColliders)
        {
            if (!hc || !hc.enabled) continue;
            foreach (var oc in otherCols)
            {
                if (!oc || !oc.enabled) continue;
                Physics.IgnoreCollision(hc, oc, true);
                ignoredPairs.Add(Norm(hc, oc));
            }
        }
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        var otherCols = args.interactableObject.transform.GetComponentsInChildren<Collider>(true);
        foreach (var hc in handColliders)
        {
            if (!hc) continue;
            foreach (var oc in otherCols)
            {
                if (!oc) continue;
                var key = Norm(hc, oc);
                if (ignoredPairs.Remove(key))
                    Physics.IgnoreCollision(hc, oc, false);
            }
        }
    }

    void RestoreAll()
    {
        foreach (var (a, b) in ignoredPairs)
            if (a && b) Physics.IgnoreCollision(a, b, false);
        ignoredPairs.Clear();
    }

    static (Collider, Collider) Norm(Collider a, Collider b)
        => (a.GetInstanceID() < b.GetInstanceID()) ? (a, b) : (b, a);
}
