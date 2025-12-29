using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WazStrazacki))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[DisallowMultipleComponent]
public class WazStrazackiRuraRenderer : MonoBehaviour
{
    [Header("Geometria rury")]
    [SerializeField, Min(0.001f)] private float promien = 0.04f;
    [SerializeField, Range(3, 24)] private int boki = 10;

    [Header("UV / tekstura")]
    [SerializeField, Min(0.01f)] private float uvTilingNaMetr = 1.5f;

    [Header("Stabilizacja")]
    [SerializeField] private bool stabilizujOrientacje = true;
    [SerializeField] private bool filtrujZeroweForward = true;

    [Header("Widocznoœæ fizyki (opcjonalne)")]
    [SerializeField] private bool ukryjRenderPunktow = true;
    [SerializeField] private bool ukryjRenderLacznikow = true;

    private WazStrazacki waz;
    private MeshFilter mf;
    private MeshRenderer mr;
    private Mesh mesh;

    private int ostatniaLiczbaPunktow = -1;

    private readonly List<Vector3> vertices = new();
    private readonly List<Vector3> normals = new();
    private readonly List<Vector2> uvs = new();
    private readonly List<int> triangles = new();

    private void Awake()
    {
        waz = GetComponent<WazStrazacki>();
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();

        mesh = new Mesh { name = "Waz_Rura_Mesh" };
        mesh.MarkDynamic();
        mf.sharedMesh = mesh;

        UstawMaterial();
        UkryjRenderyJesliTrzeba(force: true);
    }

    private void LateUpdate()
    {
        if (!waz) return;

        UstawMaterial();

        var pts = waz.PunktyDoRenderu;
        if (pts == null || pts.Count < 2)
        {
            mesh.Clear();
            return;
        }

        bool full = pts.Count != ostatniaLiczbaPunktow;
        if (full)
        {
            ostatniaLiczbaPunktow = pts.Count;
            UkryjRenderyJesliTrzeba(force: true);
        }

        ZbudujSiatkePoPunktach(pts, full);
    }

    private void UstawMaterial()
    {
        if (waz && waz.MaterialWeza && mr.sharedMaterial != waz.MaterialWeza)
            mr.sharedMaterial = waz.MaterialWeza;
    }

    private void UkryjRenderyJesliTrzeba(bool force)
    {
        if (!force) return;

        if (ukryjRenderPunktow)
        {
            foreach (Transform t in transform)
            {
                if (!t) continue;
                if (t.name.Contains("_Point"))
                {
                    var r = t.GetComponentInChildren<Renderer>();
                    if (r) r.enabled = false;
                }
            }
        }

        if (ukryjRenderLacznikow)
        {
            foreach (Transform t in transform)
            {
                if (!t) continue;
                if (t.name.Contains("_Conn"))
                {
                    var r = t.GetComponentInChildren<Renderer>();
                    if (r) r.enabled = false;
                }
            }
        }
    }

    private void ZbudujSiatkePoPunktach(IReadOnlyList<Transform> punkty, bool pelnyRebuild)
    {
        vertices.Clear();
        normals.Clear();
        uvs.Clear();
        if (pelnyRebuild) triangles.Clear();

        int n = punkty.Count;
        int ring = boki;

        // Startowy forward
        Vector3 f0 = punkty[1].position - punkty[0].position;
        if (f0.sqrMagnitude < 1e-8f) f0 = transform.forward;
        f0.Normalize();

        // Startowy up  -- nie rownolegly do forward
        Vector3 up = Vector3.up;
        if (Mathf.Abs(Vector3.Dot(up, f0)) > 0.95f) up = Vector3.right;

        Vector3 right = Vector3.Cross(up, f0).normalized;
        up = Vector3.Cross(f0, right).normalized;

        float dlugoscNarastajaco = 0f;
        Vector3 prevForward = f0;

        // wszystko zapisane w LOCAL SPACE obiektu z Meshem
        Transform meshTr = transform;

        for (int i = 0; i < n; i++)
        {
            Vector3 pW = punkty[i].position;

            // kierunek styczny -- jak LineRenderer – po punktach
            Vector3 forward;
            if (i == 0) forward = punkty[i + 1].position - pW;
            else if (i == n - 1) forward = pW - punkty[i - 1].position;
            else forward = punkty[i + 1].position - punkty[i - 1].position;

            if (forward.sqrMagnitude < 1e-8f)
            {
                forward = filtrujZeroweForward ? prevForward : transform.forward;
            }
            forward.Normalize();

            if (stabilizujOrientacje && i > 0)
            {
                // Minimalny obrot ramki (parallel transport)
                Quaternion q = Quaternion.FromToRotation(prevForward, forward);
                up = q * up;
                right = q * right;

                // Orytonormalizacja
                right = Vector3.Cross(up, forward).normalized;
                up = Vector3.Cross(forward, right).normalized;
            }
            else
            {
                Vector3 tempUp = Vector3.up;
                if (Mathf.Abs(Vector3.Dot(tempUp, forward)) > 0.95f) tempUp = Vector3.right;
                right = Vector3.Cross(tempUp, forward).normalized;
                up = Vector3.Cross(forward, right).normalized;
            }

            if (i > 0)
                dlugoscNarastajaco += Vector3.Distance(punkty[i - 1].position, pW);

            float v = dlugoscNarastajaco * uvTilingNaMetr;

            // Ring wierzcholkow
            for (int j = 0; j < ring; j++)
            {
                float ang = (j / (float)ring) * Mathf.PI * 2f;

                Vector3 offsetW = (Mathf.Cos(ang) * right + Mathf.Sin(ang) * up) * promien;
                Vector3 vertW = pW + offsetW;

                // KONWERSJA WORLD -> LOCAL
                Vector3 vertL = meshTr.InverseTransformPoint(vertW);
                Vector3 normalL = meshTr.InverseTransformDirection(offsetW.normalized);

                vertices.Add(vertL);
                normals.Add(normalL);

                float u = j / (float)ring;
                uvs.Add(new Vector2(u, v));
            }

            prevForward = forward;
        }

        if (pelnyRebuild)
        {
            for (int i = 0; i < n - 1; i++)
            {
                int a = i * ring;
                int b = (i + 1) * ring;

                for (int j = 0; j < ring; j++)
                {
                    int j1 = (j + 1) % ring;

                    int a0 = a + j;
                    int a1 = a + j1;
                    int b0 = b + j;
                    int b1 = b + j1;

                    triangles.Add(a0); triangles.Add(b0); triangles.Add(a1);
                    triangles.Add(a1); triangles.Add(b0); triangles.Add(b1);
                }
            }
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();
    }
}
