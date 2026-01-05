using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerTlenZdrowie : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider powietrzeBar;
    [SerializeField] private Slider zdrowieBar;
    [SerializeField] private GameObject zAparatemText;
    [SerializeField] private GameObject bezAparatuText;

    [Header("Wartosci max")]
    [SerializeField, Min(1f)] private float maxPowietrze = 100f;
    [SerializeField, Min(1f)] private float maxZdrowie = 100f;

    [Header("Spadek tlenu w strefie (na sekunde)")]
    [SerializeField, Min(0f)] private float spadekBezAparatu = 18f;   // szybko
    [SerializeField, Min(0f)] private float spadekZAparatem = 5f;     // wolno

    [Header("Spadek zdrowia gdy tlen = 0 (na sekunde)")]
    [SerializeField, Min(0f)] private float spadekZdrowia = 10f;      // taki sam dla obu wariantow

    [Header("Regeneracja poza strefa (na sekunde)")]
    [SerializeField, Min(0f)] private float regenPowietrza = 20f;     // wraca do max
    [SerializeField, Min(0f)] private float regenZdrowia = 20f;       // wraca do max

    [Header("Koniec gry")]
    [SerializeField] private int scenaPoSmierciBuildIndex = 0;
    private bool zaladowanoScene = false;

    // stan
    private float powietrze;
    private float zdrowie;

    private bool wStrefie;
    private bool maAparat;

    public bool MaAparat => maAparat;
    public bool WStrefie => wStrefie;
    public float Powietrze01 => Mathf.Clamp01(powietrze / maxPowietrze);
    public float Zdrowie01 => Mathf.Clamp01(zdrowie / maxZdrowie);

    // event dla managera aparatu (gdy tlen sie skonczyl i trzeba przejsc na "bez aparatu")
    public System.Action OnPowietrzeWyczerpaneZAparatem;

    private void Start()
    {
        powietrze = maxPowietrze;
        zdrowie = maxZdrowie;
        UstawTrybAparatu(false); // start: bez aparatu
        OdswiezUI();
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        if (wStrefie)
        {
            // tlen spada tylko w strefie
            float rate = maAparat ? spadekZAparatem : spadekBezAparatu;
            powietrze = Mathf.Max(0f, powietrze - rate * dt);

            // gdy tlen = 0 w strefie -> zdrowie spada zawsze tak samo
            if (powietrze <= 0f)
            {
                zdrowie = Mathf.Max(0f, zdrowie - spadekZdrowia * dt);

                // jeœli mial aparat i skonczyl sie tlen -> przelacz na "bez aparatu"
                if (maAparat)
                {
                    maAparat = false;
                    OnPowietrzeWyczerpaneZAparatem?.Invoke();
                    OdswiezIkony();
                }
            }
            if (zdrowie <= 0f)
            {
                ZakonczGreIZaladujScene();
                return;
            }

        }
        else
        {
            // poza strefa: powietrze i zdrowie wracaja do max w takim samym tempie (wg ustawien)
            powietrze = Mathf.Min(maxPowietrze, powietrze + regenPowietrza * dt);
            zdrowie = Mathf.Min(maxZdrowie, zdrowie + regenZdrowia * dt);
        }

        OdswiezUI();
    }

    public void UstawWStrefie(bool value)
    {
        wStrefie = value;
    }

    public void UstawTrybAparatu(bool czyMaAparat)
    {
        maAparat = czyMaAparat;
        OdswiezIkony();
    }

    public void ResetDoPelna()
    {
        powietrze = maxPowietrze;
        zdrowie = maxZdrowie;
        OdswiezUI();
    }

    private void OdswiezUI()
    {
        if (powietrzeBar)
        {
            powietrzeBar.minValue = 0f;
            powietrzeBar.maxValue = 1f;
            powietrzeBar.value = Powietrze01;
        }

        if (zdrowieBar)
        {
            zdrowieBar.minValue = 0f;
            zdrowieBar.maxValue = 1f;
            zdrowieBar.value = Zdrowie01;
        }
    }

    private void OdswiezIkony()
    {
        if (zAparatemText) zAparatemText.SetActive(maAparat);
        if (bezAparatuText) bezAparatuText.SetActive(!maAparat);
    }

    private void ZakonczGreIZaladujScene()
    {
        if (zaladowanoScene) return;
        zaladowanoScene = true;
        SceneManager.LoadScene(scenaPoSmierciBuildIndex);
    }

}
