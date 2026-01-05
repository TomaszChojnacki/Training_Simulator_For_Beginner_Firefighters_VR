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
    [SerializeField, Min(0f)] private float spadekBezAparatu = 18f;
    [SerializeField, Min(0f)] private float spadekZAparatem = 0.217f;

    // Butla 100 bar -- 7,4 min 
    // SpadekZAparatem = 0.217 bar/s
    // Gwizdek = 25 bar start koniec przy 22.5 bara
    // Gwizdek ----------
    [Header("Spadek tlenu Gwizdek")]
    [SerializeField] private float progGwizdka = 25f;
    [SerializeField] private float koniecGwizdka = 23.5f;

    [Header("Dzwiek gwizdka")]
    [SerializeField] private AudioSource audioGwizdka;
    [SerializeField] private AudioClip dzwiekGwizdka;
    [SerializeField, Range(0f, 1f)] private float glosnoscGwizdka = 0.9f;
    private bool gwizdekAktywny = false;
    // -------

    [Header("Spadek zdrowia gdy tlen = 0 (na sekunde)")]
    [SerializeField, Min(0f)] private float spadekZdrowia = 10f;

    [Header("Regeneracja poza strefa (na sekunde)")]
    [SerializeField, Min(0f)] private float regenPowietrza = 20f;
    [SerializeField, Min(0f)] private float regenZdrowia = 20f;

    [Header("Zestaw aparatu (zapis tlenu)")]
    [SerializeField] private float maxPowietrzeBezAparatu = 100f;
    private AparatZestaw aktualnyZestaw;

    [Header("Koniec gry")]
    [SerializeField] private int scenaPoSmierciBuildIndex = 0;
    private bool zaladowanoScene = false;

    private float powietrze;
    private float zdrowie;

    private bool wStrefie;
    private bool maAparat;

    public bool MaAparat => maAparat;
    public bool WStrefie => wStrefie;
    public float Powietrze01 => Mathf.Clamp01(powietrze / maxPowietrze);
    public float Zdrowie01 => Mathf.Clamp01(zdrowie / maxZdrowie);

    private void Start()
    {
        maxPowietrze = maxPowietrzeBezAparatu;
        powietrze = maxPowietrze;
        zdrowie = maxZdrowie;

        UstawTrybAparatu(false);
        UstawGwizdek();
        OdswiezUI();
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        if (wStrefie)
        {
            // Tlen spada tylko w strefie
            float rate = maAparat ? spadekZAparatem : spadekBezAparatu;
            powietrze = Mathf.Max(0f, powietrze - rate * dt);

            // Zapis do butli gdy mamy aparat
            if (maAparat && aktualnyZestaw != null)
                aktualnyZestaw.AktualnyTlen = powietrze;

            bool wPrzedzialeGwizdka = maAparat && powietrze <= progGwizdka && powietrze >= koniecGwizdka;

            if (wPrzedzialeGwizdka)
            {
                if (!gwizdekAktywny)
                {
                    if (audioGwizdka && audioGwizdka.clip)
                        audioGwizdka.Play();

                    gwizdekAktywny = true;
                }
            }
            else
            {
                ZatrzymajGwizdek();
            }


            // Zdrowie spada tylko w strefie i tylko gdy tlen = 0
            if (powietrze <= 0f)
            {
                zdrowie = Mathf.Max(0f, zdrowie - spadekZdrowia * dt);

                if (zdrowie <= 0f)
                {
                    ZakonczGreIZaladujScene();
                    return;
                }
            }
        }
        else
        {
            // Poza strefa zdrowie wraca
            zdrowie = Mathf.Min(maxZdrowie, zdrowie + regenZdrowia * dt);

            // Poza strefa tlen regeneruje sie chyba ze jest butla wtedy pozostaje w 0 jak byla 0
            if (!maAparat)
                powietrze = Mathf.Min(maxPowietrze, powietrze + regenPowietrza * dt);
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
        ZatrzymajGwizdek();
        if (zaladowanoScene) return;
        zaladowanoScene = true;
        SceneManager.LoadScene(scenaPoSmierciBuildIndex);
    }

    public AparatZestaw AktualnyZestaw => aktualnyZestaw;

    public void ZalozZestaw(AparatZestaw zestaw)
    {
        aktualnyZestaw = zestaw;
        UstawTrybAparatu(true);

        maxPowietrze = zestaw.PojemnoscTlenu;
        powietrze = zestaw.AktualnyTlen;

        // Clamp na wypadek zlych wartosci
        powietrze = Mathf.Clamp(powietrze, 0f, maxPowietrze);

        OdswiezUI();
        ZatrzymajGwizdek();

    }

    public void ZdejmijZestaw()
    {

        // zapisz stan w butli
        if (aktualnyZestaw != null)
            aktualnyZestaw.AktualnyTlen = powietrze;

        aktualnyZestaw = null;
        UstawTrybAparatu(false);

        // clampujemy aktualna wartosc tlenu po zdjeciu
        maxPowietrze = maxPowietrzeBezAparatu;
        powietrze = Mathf.Clamp(powietrze, 0f, maxPowietrze);

        OdswiezUI();
        ZatrzymajGwizdek();

    }

    private void UstawGwizdek()
    {
        if (!audioGwizdka)
            audioGwizdka = gameObject.AddComponent<AudioSource>();

        audioGwizdka.clip = dzwiekGwizdka;
        audioGwizdka.loop = true;          
        audioGwizdka.playOnAwake = false;
        audioGwizdka.spatialBlend = 0f;    
        audioGwizdka.volume = glosnoscGwizdka;
    }

    private void ZatrzymajGwizdek()
    {
        if (gwizdekAktywny)
        {
            if (audioGwizdka.isPlaying)
                audioGwizdka.Stop();

            gwizdekAktywny = false;
        }
    }


}
