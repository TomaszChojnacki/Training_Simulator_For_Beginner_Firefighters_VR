using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// W¹¿/linia na punktach fizycznych (Rigidbody + SpringJoint) z wizualnymi ³¹cznikami.
/// Pod³¹czony w dwóch punktach: start i end.
/// </summary>
public class WazStrazacki : MonoBehaviour
{
    [Header("Wygl¹d")]
    [SerializeField, Min(1)] private int numberOfInnerPoints = 8;     // ile punktów pomiêdzy start i end
    [SerializeField, Min(0.01f)] private float space = 0.30f;         // docelowy odstêp miêdzy punktami
    [SerializeField, Min(0.01f)] private float size = 0.08f;          // skala punktów i gruboœæ ³¹czników

    [Header("Fizyka")]
    [SerializeField, Min(1f)] private float springForce = 250f;
    [SerializeField, Min(0f)] private float damper = 12f;
    [Tooltip("Jeœli true: minDistance=0, maxDistance=space (bardziej miêkkie i stabilne w VR). " +
             "Jeœli false: minDistance=space i maxDistance=space (bardziej sztywne).")]
    [SerializeField] private bool softDistance = true;

    [Header("VR stabilizacja (opcjonalne)")]
    [Tooltip("0 = wy³¹czone. Jeœli > 0, ogranicza prêdkoœæ Rigidbody obiektu end (np. dyszy).")]
    [SerializeField, Min(0f)] private float maxPredkoscEnd = 6f;

    [Tooltip("0 = wy³¹czone. Jeœli > 0, ogranicza prêdkoœæ k¹tow¹ Rigidbody obiektu end.")]
    [SerializeField, Min(0f)] private float maxPredkoscKatowaEnd = 20f;

    [Header("Obiekty do podpiêcia")]
    [SerializeField] private GameObject start;       // punkt przy pojeŸdzie (anchor)
    [SerializeField] private GameObject end;         // dysza/koniec

    [Tooltip("Prefab ³¹cznika (wizualny element miêdzy punktami). Powinien mieæ tylko mesh/renderer (bez RB).")]
    [SerializeField] private GameObject connectorPrefab;

    [Tooltip("Prefab punktu (musi mieæ Rigidbody + SpringJoint; Collider opcjonalnie).")]
    [SerializeField] private GameObject pointPrefab;

    // wewnêtrzne listy
    private readonly List<Transform> points = new();
    private readonly List<Transform> connectors = new();

    private const string cloneText = "Part";

    private Rigidbody startRB;
    private Rigidbody endRB;

    // -------------------------
    // Publiczne akcje w Inspectorze
    // -------------------------

    [ContextMenu("Zbuduj / Resetuj w¹¿ (2 punkty)")]
    public void BuildOrReset()
    {
        if (!Walidacja())
            return;

        UsunStareElementy();
        ZbudujPunkty();
        ZbudujLaczniki();
        OdswiezListy();

        // od razu ustaw poprawne pozycje ³¹czników
        AktualizujLaczniki();
    }

    [ContextMenu("Dodaj punkt (wyd³u¿)")]
    public void AddPoint()
    {
        if (!Walidacja())
            return;

        // Tworzymy nowy punkt przed end i przepinamy sprê¿yny:
        // ... lastInner -> newPoint -> end
        if (points.Count < 2)
        {
            Debug.LogWarning("W¹¿ nie jest zbudowany. U¿yj: Zbuduj / Resetuj w¹¿ (2 punkty).");
            return;
        }

        // points: [start, inner0..innerN, end]
        Transform lastInner = points.Count >= 3 ? points[^2] : points[0]; // ostatni przed end (jeœli brak innerów, to start)

        // Usuñ sprê¿ynê lastInner -> end
        UsunSpringDo(lastInner.gameObject, endRB);

        // Nowy punkt
        GameObject newPoint = CreateNewPoint(numberOfInnerPoints);
        newPoint.transform.position = lastInner.position; // startowo tam, gdzie ostatni
        newPoint.transform.rotation = transform.rotation;
        newPoint.transform.localScale = Vector3.one * size;

        Rigidbody newRB = newPoint.GetComponent<Rigidbody>();
        if (!newRB)
        {
            Debug.LogError("pointPrefab musi mieæ Rigidbody.");
            DestroyImmediateSafe(newPoint);
            return;
        }

        // lastInner -> newPoint
        SpringJoint sjA = PobierzLubDodajSpring(newPoint.gameObject);
        SetSpring(sjA, lastInner.GetComponent<Rigidbody>());

        // newPoint -> end
        SpringJoint sjB = newPoint.AddComponent<SpringJoint>();
        SetSpring(sjB, endRB);

        // Nowy ³¹cznik pomiêdzy newPoint i end
        GameObject newConn = CreateNewConnector(numberOfInnerPoints + 1);

        // Aktualizacja liczników i list
        numberOfInnerPoints++;
        OdswiezListy();
        AktualizujLaczniki();
    }

    [ContextMenu("Usuñ punkt (skróæ)")]
    public void RemovePoint()
    {
        if (!Walidacja())
            return;

        if (numberOfInnerPoints < 1)
        {
            Debug.LogWarning("Nie mo¿na skróciæ: brak punktów wewnêtrznych.");
            return;
        }

        OdswiezListy();
        if (points.Count < 3)
        {
            Debug.LogWarning("Brak punktów do usuniêcia.");
            return;
        }

        // points: [start, inner0..innerN, end]
        Transform lastInner = points[^2];
        Transform prevInnerOrStart = points.Count >= 4 ? points[^3] : points[0];

        // Usuñ sprê¿yny prowadz¹ce lastInner (do prev i do end)
        UsunSpringDo(lastInner.gameObject, prevInnerOrStart.GetComponent<Rigidbody>());
        UsunSpringDo(lastInner.gameObject, endRB);

        // Usuñ obiekt punktu i odpowiadaj¹cy mu ³¹cznik (ostatni connector)
        Transform lastConn = connectors.Count > 0 ? connectors[^1] : null;

        DestroyImmediateSafe(lastInner.gameObject);
        if (lastConn) DestroyImmediateSafe(lastConn.gameObject);

        // Teraz po³¹cz prev -> end
        SpringJoint newSJ = prevInnerOrStart.gameObject.AddComponent<SpringJoint>();
        SetSpring(newSJ, endRB);

        numberOfInnerPoints--;
        OdswiezListy();
        AktualizujLaczniki();
    }

    // -------------------------
    // Unity
    // -------------------------

    private void Awake()
    {
        if (Walidacja(false))
        {
            startRB = start.GetComponent<Rigidbody>();
            endRB = end.GetComponent<Rigidbody>();
        }
    }

    private void Start()
    {
        // Jeœli u¿ytkownik nie klikn¹³ Build, a chce automatycznie – mo¿esz odkomentowaæ:
         BuildOrReset();
        OdswiezListy();
    }

    private void Update()
    {
        if (points.Count == 0 || connectors.Count == 0)
            return;

        AktualizujLaczniki();
        StabilizujEndVR();
    }

    // -------------------------
    // Budowa
    // -------------------------

    private bool Walidacja(bool loguj = true)
    {
        if (!start || !end || !pointPrefab || !connectorPrefab)
        {
            if (loguj)
                Debug.LogWarning("Brakuje referencji: start, end, pointPrefab, connectorPrefab.");
            return false;
        }

        startRB = start.GetComponent<Rigidbody>();
        endRB = end.GetComponent<Rigidbody>();
        if (!startRB || !endRB)
        {
            if (loguj)
                Debug.LogWarning("start i end musz¹ mieæ Rigidbody (start zwykle IsKinematic=true).");
            return false;
        }

        // pointPrefab musi mieæ Rigidbody + SpringJoint (SpringJoint mo¿e dodaæ skrypt, ale prefab powinien mieæ Rigidbody)
        if (!pointPrefab.GetComponent<Rigidbody>())
        {
            if (loguj)
                Debug.LogWarning("pointPrefab musi mieæ Rigidbody.");
            return false;
        }

        return true;
    }

    private void UsunStareElementy()
    {
        // usuñ wszystkie dzieci tego obiektu, które zaczynaj¹ siê od "Part"
        int length = transform.childCount;
        for (int i = 0; i < length; i++)
        {
            Transform ch = transform.GetChild(i);
            if (ch.name.StartsWith(cloneText))
            {
                DestroyImmediateSafe(ch.gameObject);
                length--;
                i--;
            }
        }
    }

    private void ZbudujPunkty()
    {
        // Tworzymy punkty pomiêdzy start i end.
        // Rozk³adamy je na linii start->end, ale ich sprê¿yny trzymaj¹ docelowy dystans "space".
        // Dziêki temu w¹¿ nie musi byæ idealnie prosty, ale startowo bêdzie u³o¿ony sensownie.

        Vector3 a = start.transform.position;
        Vector3 b = end.transform.position;

        Vector3 dir = (b - a);
        float dist = dir.magnitude;
        if (dist < 0.001f) dir = transform.forward;
        else dir /= dist;

        // liczba segmentów = innerPoints + 1 (start->inner0 ... innerN->end)
        int segments = numberOfInnerPoints + 1;

        Rigidbody lastBody = startRB;

        for (int i = 0; i < numberOfInnerPoints; i++)
        {
            float t = (i + 1) / (float)segments;
            Vector3 pos = Vector3.Lerp(a, b, t);

            GameObject p = CreateNewPoint(i);
            p.transform.position = pos;
            p.transform.rotation = transform.rotation;
            p.transform.localScale = Vector3.one * size;

            Rigidbody rb = p.GetComponent<Rigidbody>();
            SpringJoint sj = PobierzLubDodajSpring(p);

            SetSpring(sj, lastBody);
            lastBody = rb;
        }

        // ostatnie po³¹czenie: ostatni punkt (lub start) -> end
        SpringJoint endSJ = lastBody.gameObject.AddComponent<SpringJoint>();
        SetSpring(endSJ, endRB);
    }

    private void ZbudujLaczniki()
    {
        // liczba ³¹czników = innerPoints + 1 (start-1, 1-2 ... last-end)
        int connectorsCount = numberOfInnerPoints + 1;
        for (int i = 0; i < connectorsCount; i++)
        {
            GameObject c = CreateNewConnector(i);
            c.transform.localScale = Vector3.one * size;
        }
    }

    private void OdswiezListy()
    {
        points.Clear();
        connectors.Clear();

        points.Add(start.transform);

        // punkty wewnêtrzne
        for (int i = 0; i < numberOfInnerPoints; i++)
        {
            Transform p = transform.Find(PointName(i));
            if (p) points.Add(p);
        }

        points.Add(end.transform);

        // ³¹czniki
        for (int i = 0; i < numberOfInnerPoints + 1; i++)
        {
            Transform c = transform.Find(ConnectorName(i));
            if (c) connectors.Add(c);
        }
    }

    // -------------------------
    // Aktualizacja wizualnych ³¹czników
    // -------------------------

    private void AktualizujLaczniki()
    {
        int parts = connectors.Count;
        if (points.Count != parts + 1)
            return;

        Transform lastPoint = points[0];
        for (int i = 0; i < parts; i++)
        {
            Transform nextPoint = points[i + 1];
            Transform conn = connectors[i];

            Vector3 a = lastPoint.position;
            Vector3 b = nextPoint.position;

            conn.position = (a + b) * 0.5f;

            Vector3 dir = (b - a);
            float d = dir.magnitude;

            if (d < 0.0001f)
            {
                conn.localScale = Vector3.zero;
            }
            else
            {
                conn.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
                // Skala: X,Y = gruboœæ, Z = po³owa d³ugoœci (jak w Twoim kodzie)
                conn.localScale = new Vector3(size, size, d * 0.5f);
            }

            lastPoint = nextPoint;
        }
    }

    // -------------------------
    // Spring / stabilizacja
    // -------------------------

    private void SetSpring(SpringJoint spring, Rigidbody connectedBody)
    {
        spring.connectedBody = connectedBody;
        spring.spring = springForce;
        spring.damper = damper;

        spring.autoConfigureConnectedAnchor = false;
        spring.anchor = Vector3.zero;
        spring.connectedAnchor = Vector3.zero;

        if (softDistance)
        {
            // Bardziej "wê¿owe" i stabilne w VR
            spring.minDistance = 0f;
            spring.maxDistance = space;
        }
        else
        {
            // Bardziej sztywne (³atwiej o drgania przy szarpniêciu)
            spring.minDistance = space;
            spring.maxDistance = space;
        }
    }

    private void StabilizujEndVR()
    {
        if (maxPredkoscEnd <= 0f && maxPredkoscKatowaEnd <= 0f)
            return;

        if (!endRB) return;

        // Unity 6: linearVelocity
        if (maxPredkoscEnd > 0f)
        {
            float v = endRB.linearVelocity.magnitude;
            if (v > maxPredkoscEnd)
                endRB.linearVelocity = endRB.linearVelocity.normalized * maxPredkoscEnd;
        }

        if (maxPredkoscKatowaEnd > 0f)
        {
            float w = endRB.angularVelocity.magnitude;
            if (w > maxPredkoscKatowaEnd)
                endRB.angularVelocity = endRB.angularVelocity.normalized * maxPredkoscKatowaEnd;
        }
    }

    private SpringJoint PobierzLubDodajSpring(GameObject go)
    {
        SpringJoint sj = go.GetComponent<SpringJoint>();
        if (!sj) sj = go.AddComponent<SpringJoint>();
        return sj;
    }

    private void UsunSpringDo(GameObject go, Rigidbody target)
    {
        var springs = go.GetComponents<SpringJoint>();
        for (int i = springs.Length - 1; i >= 0; i--)
        {
            if (springs[i] && springs[i].connectedBody == target)
                DestroyImmediateSafe(springs[i]);
        }
    }

    // -------------------------
    // Prefaby / nazwy
    // -------------------------

    private string ConnectorName(int index) => $"{cloneText}_{index}_Conn";
    private string PointName(int index) => $"{cloneText}_{index}_Point";

    private GameObject CreateNewPoint(int index)
    {
        GameObject temp = Instantiate(pointPrefab);
        temp.name = PointName(index);
        temp.transform.SetParent(transform, true);
        return temp;
    }

    private GameObject CreateNewConnector(int index)
    {
        GameObject temp = Instantiate(connectorPrefab);
        temp.name = ConnectorName(index);
        temp.transform.SetParent(transform, true);
        return temp;
    }

    private void DestroyImmediateSafe(Object obj)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) DestroyImmediate(obj);
        else Destroy(obj);
#else
        Destroy(obj);
#endif
    }
}
