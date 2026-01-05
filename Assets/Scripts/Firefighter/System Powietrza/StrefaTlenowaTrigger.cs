using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class StrefaTlenowaOverlap : MonoBehaviour
{
    [Header("Kogo wykrywamy")]
    [SerializeField] private Transform playerRoot; // XR Origin (VR) albo Camera Offset
    [SerializeField] private PlayerTlenZdrowie playerSystem;

    [Header("Warstwy gracza (dla OverlapBox)")]
    [Tooltip("Ustaw na warstwê gracza, np. Player. Mo¿e byæ te¿ Everything, ale lepiej zawêziæ.")]
    [SerializeField] private LayerMask maskaGracza = ~0;

    [Header("Diagnostyka")]
    [SerializeField] private bool loguj = true;

    private BoxCollider box;
    private bool bylWStrefie;

    private void Awake()
    {
        box = GetComponent<BoxCollider>();

        // OverlapBox nie wymaga triggera, ale mo¿esz zostawiæ trigger jak chcesz.
        // box.isTrigger = true;

        if (!playerSystem)
            playerSystem = FindFirstObjectByType<PlayerTlenZdrowie>();

        if (!playerRoot && playerSystem)
            playerRoot = playerSystem.transform.root;
    }

    private void Update()
    {
        if (!playerSystem || !playerRoot) return;

        bool jestWStrefie = CzyPlayerWBoxie();

        if (jestWStrefie != bylWStrefie)
        {
            bylWStrefie = jestWStrefie;
            playerSystem.UstawWStrefie(jestWStrefie);

            if (loguj)
                Debug.Log(jestWStrefie ? "[StrefaTlenu] ENTER (OverlapBox)" : "[StrefaTlenu] EXIT (OverlapBox)");
        }
    }

    private bool CzyPlayerWBoxie()
    {
        // œwiatowe parametry boxa
        Vector3 centerWorld = transform.TransformPoint(box.center);
        Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, transform.lossyScale);
        Quaternion rot = transform.rotation;

        // sprawdzamy czy w boxie jest jakikolwiek collider z warstwy gracza
        Collider[] hits = Physics.OverlapBox(centerWorld, halfExtents, rot, maskaGracza, QueryTriggerInteraction.Ignore);

        // i upewniamy siê, ¿e to faktycznie coœ z XR Origin (dziecko/rodzic)
        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i]) continue;

            Transform t = hits[i].transform;
            if (t == playerRoot || t.IsChildOf(playerRoot) || playerRoot.IsChildOf(t))
                return true;
        }

        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        var b = GetComponent<BoxCollider>();
        if (!b) return;

        Gizmos.matrix = Matrix4x4.TRS(transform.TransformPoint(b.center), transform.rotation, transform.lossyScale);
        Gizmos.DrawWireCube(Vector3.zero, b.size);
    }
#endif
}
