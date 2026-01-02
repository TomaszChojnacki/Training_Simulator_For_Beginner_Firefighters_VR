using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemOgniaManager : MonoBehaviour
{
    [Header("Prefab ognia")]
    [SerializeField] private GameObject prefabOgnia;

    [Header("Opcje")]
    [SerializeField] private string prefixNazwyPunktu = "punkt_ognia";
    [SerializeField] private bool resetujVfxPrzyStarcie = true;

    [Header("Koniec zadania")]
    [SerializeField] private int sceneIndexWhenDone = 0;

    private readonly HashSet<FirePoint> active = new();
    private bool sceneLoaded;

    private void Start()
    {
        ZainstalujOgnieNaPunktach();
        LogCount();
    }

    private void ZainstalujOgnieNaPunktach()
    {
        if (!prefabOgnia)
        {
            Debug.LogWarning("[SystemOgnia] Brak prefabOgnia.");
            return;
        }

        foreach (Transform child in transform)
        {
            if (!child) continue;

            if (!child.name.ToLowerInvariant().StartsWith(prefixNazwyPunktu.ToLowerInvariant()))
                continue;

            if (resetujVfxPrzyStarcie)
            {
                for (int i = child.childCount - 1; i >= 0; i--)
                {
                    Transform c = child.GetChild(i);
                    if (c && c.name == "VFX_Ogien")
                        Destroy(c.gameObject);
                }
            }

            FirePoint fp = child.GetComponent<FirePoint>();
            if (!fp) fp = child.gameObject.AddComponent<FirePoint>();

            // Dodaj prefab ognia
            var fireGO = Instantiate(prefabOgnia, child);
            fireGO.name = "VFX_Ogien";
            fireGO.transform.localPosition = Vector3.zero;
            fireGO.transform.localRotation = Quaternion.identity;
            fireGO.transform.localScale = Vector3.one;

            active.Add(fp);
            fp.Init(this, fireGO.transform);

            if (!child.GetComponent<Collider>())
                Debug.LogWarning($"[SystemOgnia] Punkt {child.name} nie ma Collidera.");
        }
    }

    public void NotifyExtinguished(FirePoint fp)
    {
        if (!fp) return;

        active.Remove(fp);
        LogCount();

        if (!sceneLoaded && active.Count == 0)
        {
            sceneLoaded = true;
            Debug.Log("[SystemOgnia] Wszystkie punkty ognia ugaszone. £adujê scenê 0.");
            SceneManager.LoadScene(sceneIndexWhenDone);
        }
    }

    private void LogCount()
    {
        Debug.Log($"[SystemOgnia] Pozosta³o punktów ognia: {active.Count}");
    }
}
