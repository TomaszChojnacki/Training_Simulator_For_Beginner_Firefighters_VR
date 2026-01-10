using UnityEngine;

public class StrefaTlenowaTrigger : MonoBehaviour
{
    [Header("Wykrywanie gracz")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private PlayerTlenZdrowie playerSystem;

    [Header("Warstwy gracza (dla OverlapBox)")]
    [SerializeField] private LayerMask maskaGracza = ~0;

    [Header("Boxy strefy")]
    [SerializeField] private BoxCollider[] boxy;

    [Header("Uwzglednianie triggera na graczu")]
    [SerializeField] private QueryTriggerInteraction queryTriggers = QueryTriggerInteraction.Collide;

    private bool bylWStrefie;

    private void Awake()
    {
        if (!playerSystem)
            playerSystem = FindFirstObjectByType<PlayerTlenZdrowie>();

        if (!playerRoot && playerSystem)
            playerRoot = playerSystem.transform.root;

        if (boxy == null || boxy.Length == 0)
            boxy = GetComponentsInChildren<BoxCollider>(true);
    }

    private void Update()
    {
        if (!playerSystem || !playerRoot) return;
        if (boxy == null || boxy.Length == 0) return;

        bool jestWStrefie = CzyPlayerWJakimkolwiekBoxie();

        if (jestWStrefie != bylWStrefie)
        {
            bylWStrefie = jestWStrefie;
            playerSystem.UstawWStrefie(jestWStrefie);
        }
    }

    private bool CzyPlayerWJakimkolwiekBoxie()
    {
        for (int b = 0; b < boxy.Length; b++)
        {
            var box = boxy[b];
            if (!box || !box.enabled) continue;

            // Parametry boxa w swiecie
            Vector3 centerWorld = box.transform.TransformPoint(box.center);

            // HalfExtents musi uwzgledniac skale konkretnego boxa
            Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, box.transform.lossyScale);

            Quaternion rot = box.transform.rotation;

            Collider[] hits = Physics.OverlapBox(
                centerWorld,
                halfExtents,
                rot,
                maskaGracza,
                queryTriggers
            );

            for (int i = 0; i < hits.Length; i++)
            {
                if (!hits[i]) continue;

                Transform t = hits[i].transform;

                // Upewnienie ze to XR Origin (dziecko -- rodzic)
                if (t == playerRoot || t.IsChildOf(playerRoot) || playerRoot.IsChildOf(t))
                    return true;
            }
        }

        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        BoxCollider[] drawBoxy = boxy;
        if (drawBoxy == null || drawBoxy.Length == 0)
            drawBoxy = GetComponentsInChildren<BoxCollider>(true);

        if (drawBoxy == null) return;

        for (int i = 0; i < drawBoxy.Length; i++)
        {
            var b = drawBoxy[i];
            if (!b) continue;

            Gizmos.matrix = Matrix4x4.TRS(
                b.transform.TransformPoint(b.center),
                b.transform.rotation,
                b.transform.lossyScale
            );

            Gizmos.DrawWireCube(Vector3.zero, b.size);
        }
    }
#endif
}
