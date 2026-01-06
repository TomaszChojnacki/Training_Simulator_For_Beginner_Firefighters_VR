using UnityEngine;

public class SystemElektrykiManager : MonoBehaviour
{
    [Header("Obiekty oswietlenia")]
    [SerializeField] private GameObject oswietlenieHali;          // wlaczone gdy prad ON
    [SerializeField] private GameObject blendaOswietlenieHali;     // wlaczenie gdy prad OFF

    [Header("Przelaczniki (6 szt.)")]
    [SerializeField] private PrzelacznikElektrykiXR[] przelaczniki;

    public bool PradWlaczony { get; private set; } = true;

    private void Start()
    {
        OdswiezStanPradu();
    }

    public void OdswiezStanPradu()
    {
        // Prad jest wlaczony - jesli chociaz jeden przelacznik jest ON
        bool jakikolwiekOn = false;

        if (przelaczniki != null)
        {
            foreach (var p in przelaczniki)
            {
                if (p != null && p.CzyOn)
                {
                    jakikolwiekOn = true;
                    break;
                }
            }
        }

        PradWlaczony = jakikolwiekOn;

        if (oswietlenieHali) oswietlenieHali.SetActive(PradWlaczony);
        if (blendaOswietlenieHali) blendaOswietlenieHali.SetActive(!PradWlaczony);

        //Debug.Log($"[SystemElektryki] PradWlaczony = {PradWlaczony}");
    }
}
