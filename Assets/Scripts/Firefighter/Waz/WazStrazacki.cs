using System.Collections.Generic;
using UnityEngine;

public class WazStrazacki : MonoBehaviour
{
    [Header("Wyglad")]
    [SerializeField, Min(1)] private int numberOfInnerPoints = 50;     // ile punktów pomiedzy start i end
    [SerializeField, Min(0.01f)] private float space = 0.30f;         // docelowy odstep miedzy punktami
    [SerializeField, Min(0.01f)] private float size = 0.08f;          // skala punktow i grubosc laczników

    [Header("Material wê¿a (do renderu rury)")]
    [SerializeField] private Material materialWeza;
    public Material MaterialWeza => materialWeza;


    [Header("Fizyka")]
    [SerializeField, Min(1f)] private float springForce = 900f;
    [SerializeField, Min(0f)] private float damper = 90f;
    [SerializeField] private bool softDistance = true;

    [Header("VR stabilizacja")]
    [SerializeField, Min(0f)] private float maxPredkoscEnd = 3f;
    [SerializeField, Min(0f)] private float maxPredkoscKatowaEnd = 10f;

    [Header("Obiekty do podpiecia")]
    [SerializeField] private GameObject start;       // punkt przy pojezdzie
    [SerializeField] private GameObject end;         // punkt koniec
    [SerializeField] private GameObject connectorPrefab;
    [SerializeField] private GameObject pointPrefab;

    // wewnetrzne listy
    private readonly List<Transform> points = new();
    private readonly List<Transform> connectors = new();

    private const string cloneText = "Part";

    private Rigidbody startRB;
    private Rigidbody endRB;

    // Publiczne akcje w Inspectorze
    [ContextMenu("Zbuduj / Resetuj waz (2 punkty)")]
    public void BuildOrReset()
    {
        if (!Walidacja())
            return;

        UsunStareElementy();
        ZbudujPunkty();
        ZbudujLaczniki();
        OdswiezListy();
        // od razu poprawne pozycje lacznikow
        AktualizujLaczniki();
    }

    [ContextMenu("Dodaj punkt (wydluz)")]
    public void AddPoint()
    {
        if (!Walidacja())
            return;

        // Tworzymy nowy punkt przed end i przepinamy sprezyny
        // ... lastInner - newPoint - end
        if (points.Count < 2)
        {
            Debug.LogWarning("Waz nie jest zbudowany");
            return;
        }

        // points: [start, inner0..innerN, end]
        Transform lastInner = points.Count >= 3 ? points[^2] : points[0]; // ostatni przed end, brak innerow to start)

        // Usnñ sprezyne lastInner - end
        UsunSpringDo(lastInner.gameObject, endRB);

        // Nowy punkt
        GameObject newPoint = CreateNewPoint(numberOfInnerPoints);
        newPoint.transform.position = lastInner.position; // startowo tam gdzie ostatni
        newPoint.transform.rotation = transform.rotation;
        newPoint.transform.localScale = Vector3.one * size;

        Rigidbody newRB = newPoint.GetComponent<Rigidbody>();
        if (!newRB)
        {
            Debug.LogError("pointPrefab musi mieæ Rigidbody");
            DestroyImmediateSafe(newPoint);
            return;
        }

        // lastInner - newPoint
        SpringJoint sjA = PobierzLubDodajSpring(newPoint.gameObject);
        SetSpring(sjA, lastInner.GetComponent<Rigidbody>());

        // newPoint - end
        SpringJoint sjB = newPoint.AddComponent<SpringJoint>();
        SetSpring(sjB, endRB);

        // Nowy lacznik pomiedzy newPoint i end
        GameObject newConn = CreateNewConnector(numberOfInnerPoints + 1);

        // Aktualizacja licznikow i list
        numberOfInnerPoints++;
        OdswiezListy();
        AktualizujLaczniki();
    }

    [ContextMenu("Usun punkt w linie")]
    public void RemovePoint()
    {
        if (!Walidacja())
            return;

        if (numberOfInnerPoints < 1)
        {
            Debug.LogWarning("Brak punktow wewnetrznych");
            return;
        }

        OdswiezListy();
        if (points.Count < 3)
        {
            Debug.LogWarning("Brak punktow do usuniecia");
            return;
        }

        // points - start, inner0..innerN, end
        Transform lastInner = points[^2];
        Transform prevInnerOrStart = points.Count >= 4 ? points[^3] : points[0];

        // Usun sprezyny prowadzace lastInner (do prev i do end)
        UsunSpringDo(lastInner.gameObject, prevInnerOrStart.GetComponent<Rigidbody>());
        UsunSpringDo(lastInner.gameObject, endRB);

        // Usun obiekt punktu i odpowiadajacy mu lacznik - ostatni connector
        Transform lastConn = connectors.Count > 0 ? connectors[^1] : null;

        DestroyImmediateSafe(lastInner.gameObject);
        if (lastConn) DestroyImmediateSafe(lastConn.gameObject);

        // Teraz polacz prev - end
        SpringJoint newSJ = prevInnerOrStart.gameObject.AddComponent<SpringJoint>();
        SetSpring(newSJ, endRB);

        numberOfInnerPoints--;
        OdswiezListy();
        AktualizujLaczniki();
    }
    // Unity run
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
        BuildOrReset(); // budowanie weza przy start
        OdswiezListy();
    }

    private void Update()
    {
        if (points.Count == 0 || connectors.Count == 0)
            return;

        AktualizujLaczniki();
        StabilizujEndVR();
    }
    // Budowa
    private bool Walidacja(bool loguj = true)
    {
        if (!start || !end || !pointPrefab || !connectorPrefab)
        {
            if (loguj)
                Debug.LogWarning("Brakuje referencji start - end - pointPrefab - connectorPrefab");
            return false;
        }

        startRB = start.GetComponent<Rigidbody>();
        endRB = end.GetComponent<Rigidbody>();
        if (!startRB || !endRB)
        {
            if (loguj)
                Debug.LogWarning("start i end musza miec Rigidbody - start zwykle IsKinematic=true");
            return false;
        }

        // pointPrefab musi miec Rigidbody
        if (!pointPrefab.GetComponent<Rigidbody>())
        {
            if (loguj)
                Debug.LogWarning("pointPrefab musi miec Rigidbody");
            return false;
        }

        return true;
    }

    private void UsunStareElementy()
    {
        // usun wszystkie dzieci tego obiektu, które zaczynaja siê od "Part"
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
        // Tworzymy punkty pomiedzy start i end
        // Rozkladamy je na linii start - end ale ich sprezyny trzymaja docelowy dystans "space"
        // Waz nie jest prosty ale startowo jest ustawiony

        Vector3 a = start.transform.position;
        Vector3 b = end.transform.position;

        Vector3 dir = (b - a);
        float dist = dir.magnitude;
        if (dist < 0.001f) dir = transform.forward;
        else dir /= dist;

        // liczba segmentow = innerPoints + 1 (start - inner0 ... innerN - end)
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

        // ostatnie polaczenie - ostatni punkt albo start - end
        SpringJoint endSJ = lastBody.gameObject.AddComponent<SpringJoint>();
        SetSpring(endSJ, endRB);
    }

    private void ZbudujLaczniki()
    {
        // liczba lacznikow = innerPoints + 1  -- start-1, 1-2 -- last-end
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

        // punkty wewnetrzne
        for (int i = 0; i < numberOfInnerPoints; i++)
        {
            Transform p = transform.Find(PointName(i));
            if (p) points.Add(p);
        }

        points.Add(end.transform);

        // laczniki
        for (int i = 0; i < numberOfInnerPoints + 1; i++)
        {
            Transform c = transform.Find(ConnectorName(i));
            if (c) connectors.Add(c);
        }
    }

    // Aktualizacja wizualnych lacznikow
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
                // Skala -- X,Y = grubosc, Z = polowa dlugoœci
                conn.localScale = new Vector3(size, size, d * 0.5f);
            }

            lastPoint = nextPoint;
        }
    }
    // Spring -- stabilizacja
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
            spring.minDistance = 0f;
            spring.maxDistance = space;
        }
        else
        {
            // Bardziej sztywne t³umienie drgan przy szarpnieciu
            spring.minDistance = space;
            spring.maxDistance = space;
        }
    }

    private void StabilizujEndVR()
    {
        if (maxPredkoscEnd <= 0f && maxPredkoscKatowaEnd <= 0f)
            return;

        if (!endRB) return;

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
    // Prefaby -- nazwy
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

    public IReadOnlyList<Transform> PunktyDoRenderu => points;


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
