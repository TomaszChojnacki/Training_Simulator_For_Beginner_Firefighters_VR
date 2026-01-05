using UnityEngine;

public class AparatTlenowyManager : MonoBehaviour
{
    [Header("Referencje")]
    [SerializeField] private PlayerTlenZdrowie playerSystem;

    [Header("Zestawy w scenie")]
    [SerializeField] private GameObject zestaw1; // Zestaw_1
    [SerializeField] private GameObject zestaw2; // Zestaw_2

    [Header("Ikony UI (opcjonalnie, moze byc tez tylko w PlayerTlenZdrowie)")]
    [SerializeField] private GameObject zAparatemText;
    [SerializeField] private GameObject bezAparatuText;

    private GameObject ostatnioZabrany;

    private void OnEnable()
    {
        if (playerSystem != null)
            playerSystem.OnPowietrzeWyczerpaneZAparatem += WymusPrzelaczNaBezAparatu;
    }

    private void OnDisable()
    {
        if (playerSystem != null)
            playerSystem.OnPowietrzeWyczerpaneZAparatem -= WymusPrzelaczNaBezAparatu;
    }

    public void ToggleAparat()
    {
        if (!playerSystem) return;

        // jeœli juz ma aparat -> zdejmij (i odloz ostatnio zabrany zestaw z powrotem)
        if (playerSystem.MaAparat)
        {
            playerSystem.UstawTrybAparatu(false);
            PokazOstatniZestaw();
            OdswiezIkony();
            return;
        }

        // jeœli nie ma aparatu -> spróbuj "wzi¹æ" zestaw
        GameObject doZabrania = ZnajdzDostepnyZestaw();
        if (doZabrania == null)
        {
            // brak zestawow - zostaje bez aparatu
            playerSystem.UstawTrybAparatu(false);
            OdswiezIkony();
            return;
        }

        // "zabieramy" (znika ze sceny)
        ostatnioZabrany = doZabrania;
        doZabrania.SetActive(false);

        playerSystem.UstawTrybAparatu(true);
        OdswiezIkony();
    }

    private GameObject ZnajdzDostepnyZestaw()
    {
        if (zestaw1 && zestaw1.activeInHierarchy) return zestaw1;
        if (zestaw2 && zestaw2.activeInHierarchy) return zestaw2;
        return null;
    }

    private void PokazOstatniZestaw()
    {
        if (ostatnioZabrany != null)
            ostatnioZabrany.SetActive(true);

        ostatnioZabrany = null;
    }

    private void WymusPrzelaczNaBezAparatu()
    {
        // Gdy tlen spadnie do 0 w strefie – gracz ma automatycznie przejœæ w tryb bez aparatu
        // (zestaw zostaje "zu¿yty", czyli nie wraca automatycznie)
        OdswiezIkony();
    }

    private void OdswiezIkony()
    {
        bool ma = playerSystem != null && playerSystem.MaAparat;

        if (zAparatemText) zAparatemText.SetActive(ma);
        if (bezAparatuText) bezAparatuText.SetActive(!ma);
    }
}
