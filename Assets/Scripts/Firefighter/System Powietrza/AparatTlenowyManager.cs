using UnityEngine;

public class AparatTlenowyManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PlayerTlenZdrowie playerSystem;

    [Header("Zestawy w scenie")]
    [SerializeField] private GameObject zestaw1;
    [SerializeField] private GameObject zestaw2;

    [Header("Ikony UI (pobrane z PlayerTlenZdrowie)")]
    [SerializeField] private GameObject zAparatemText;
    [SerializeField] private GameObject bezAparatuText;

    private AparatZestaw zestaw1Data;
    private AparatZestaw zestaw2Data;

    private void Awake()
    {
        if (!playerSystem)
            playerSystem = FindFirstObjectByType<PlayerTlenZdrowie>();

        if (zestaw1) zestaw1Data = zestaw1.GetComponent<AparatZestaw>();
        if (zestaw2) zestaw2Data = zestaw2.GetComponent<AparatZestaw>();

        if (zestaw1 && !zestaw1Data)
            Debug.LogWarning("[AparatTlenowyManager] Zestaw_1 nie ma komponentu AparatZestaw.");

        if (zestaw2 && !zestaw2Data)
            Debug.LogWarning("[AparatTlenowyManager] Zestaw_2 nie ma komponentu AparatZestaw.");

        OdswiezUIIkon();
    }

    public void KlikZestaw1()
    {
        ToggleZestaw(zestaw1, zestaw1Data);
    }

    public void KlikZestaw2()
    {
        ToggleZestaw(zestaw2, zestaw2Data);
    }

    private void ToggleZestaw(GameObject obiektZestawu, AparatZestaw data)
    {
        if (!playerSystem || !obiektZestawu || data == null)
            return;

        // Jesli gracz ma ju¿ ten zestaw -- zdejmij (i pokaz go w scenie)
        if (playerSystem.AktualnyZestaw == data)
        {
            playerSystem.ZdejmijZestaw();
            obiektZestawu.SetActive(true);
            OdswiezUIIkon();
            return;
        }

        // Jesli gracz ma inny zestaw -- zdejmij go i pokaz go w scenie
        if (playerSystem.AktualnyZestaw != null)
        {
            AparatZestaw stary = playerSystem.AktualnyZestaw;
            playerSystem.ZdejmijZestaw();

            // pokaz z powrotem wlasciwy obiekt starego zestawu
            if (zestaw1Data == stary && zestaw1) zestaw1.SetActive(true);
            if (zestaw2Data == stary && zestaw2) zestaw2.SetActive(true);
        }

        playerSystem.ZalozZestaw(data);
        obiektZestawu.SetActive(false);

        OdswiezUIIkon();
    }

    private void OdswiezUIIkon()
    {
        bool maAparat = playerSystem != null && playerSystem.AktualnyZestaw != null;

        if (zAparatemText) zAparatemText.SetActive(maAparat);
        if (bezAparatuText) bezAparatuText.SetActive(!maAparat);
    }
}
