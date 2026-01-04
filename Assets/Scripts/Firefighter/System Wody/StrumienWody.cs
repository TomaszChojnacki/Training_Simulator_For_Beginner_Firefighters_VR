using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StrumienWody : MonoBehaviour
{
    [Header("XR")]
    [SerializeField] private XRGrabInteractable grab;
    [SerializeField] private bool wymagajTrzymania = true;
    [SerializeField, Range(0f, 1f)] private float progTriggera = 0.2f;

    [Header("Trigger (Input System / XRI)")]
    [SerializeField] private InputActionProperty activateLeft;
    [SerializeField] private InputActionProperty activateRight;

    [Header("Anchor / Muzzle")]
    [SerializeField] private Transform waterMuzzle;

    [Header("Prefaby efektow")]
    [SerializeField] private ParticleSystem waterPrefab;
    [SerializeField] private ParticleSystem splashPrefab;

    [Header("Skracanie widocznego strumienia (¿eby nie przechodzi³ przez œciany)")]
    [SerializeField, Min(0.1f)] private float minimalnyZasiegWizualny = 0.5f;
    [SerializeField, Min(0.1f)] private float maksymalnyZasiegWizualny = 25f;
    [SerializeField, Min(0f)] private float zapasOdSplasha = 0.05f; // ma³y margines, ¿eby nie "wchodzi³o" w œcianê

    private float domyslnyLifetime = -1f;
    private float domyslnaPredkosc = -1f;


    [Header("Zasieg i trafienie")]
    [SerializeField, Min(0.1f)] private float zasieg = 18f;
    [SerializeField] private LayerMask maskaTrafien = ~0;
    [SerializeField, Min(0f)] private float sila = 0f;

    [Header("Stabilizacja splash")]
    [SerializeField, Min(0f)] private float progRuchuHit = 0.01f;
    [SerializeField, Min(0f)] private float offsetOdPowierzchni = 0.01f;

    [Header("Zabezpieczenie na szybkie puszczenie/wcisniecie")]
    [SerializeField, Min(0f)] private float splashDelayPoWlaczeniu = 0.03f;

    // Blokada wody do przycisku na pojezdzie
    [Header("Cisnienie (panel ON/OFF)")]
    [SerializeField] private bool cisnienieWlaczone = false;

    [Header("Dzwiek wody")]
    [SerializeField] private AudioSource audioWody;
    [SerializeField] private AudioClip dzwiekWodyLoop;
    [SerializeField, Range(0f, 1f)] private float glosnoscWody = 0.7f;
    [SerializeField] private float predkoscWygaszania = 8f;



    public bool CisnienieWlaczone
    {
        get => cisnienieWlaczone;
        set
        {
            cisnienieWlaczone = value;

            // jesli panel wylaczy cisnienie wylacz wode
            if (!cisnienieWlaczone)
                WylaczNatychmiast(true);
        }
    }


    // stan
    private bool trzymane;
    private bool leje;

    // instancje
    private ParticleSystem waterInstance;
    private ParticleSystem splashInstance;

    private bool mamHit;
    private Vector3 ostatniHitPoint;

    private float blokujSplashDoCzasu = 0f;

    private void Reset()
    {
        grab = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (!grab) grab = GetComponent<XRGrabInteractable>();

        if (grab)
        {
            grab.selectEntered.AddListener(OnGrabbed);
            grab.selectExited.AddListener(OnReleased);
        }

        if (activateLeft.action != null) activateLeft.action.Enable();
        if (activateRight.action != null) activateRight.action.Enable();
    }

    private void OnDisable()
    {
        if (grab)
        {
            grab.selectEntered.RemoveListener(OnGrabbed);
            grab.selectExited.RemoveListener(OnReleased);
        }

        if (activateLeft.action != null) activateLeft.action.Disable();
        if (activateRight.action != null) activateRight.action.Disable();
    }

    private void Start()
    {
        WylaczNatychmiast(true);
        UstawAudioWody();
    }

    private void Update()
    {
        // BLOKADA WODY jesli cisnienie OFF
        if (!cisnienieWlaczone)
        {
            if (leje) WylaczNatychmiast(false);
            return;
        }

        if (wymagajTrzymania && !trzymane)
        {
            if (leje) Wylacz();
            return;
        }
        // ----- blokoda koniec

        float tL = ReadFloat(activateLeft);
        float tR = ReadFloat(activateRight);
        bool triggerOk = Mathf.Max(tL, tR) > progTriggera;

        if (triggerOk && !leje) Wlacz();
        else if (!triggerOk && leje) Wylacz();

        if (leje)
        {
            AktualizujWaterNaMuzzle();

            // Dopiero po chwili od startu wody jest mozliwosc na splash
            if (Time.time >= blokujSplashDoCzasu)
                AktualizujTrafienieISplash();
            else
                UkryjSplash();
        }

        AktualizujDzwiekWody();

    }

    private float ReadFloat(InputActionProperty prop)
    {
        if (prop.action == null) return 0f;
        return prop.action.ReadValue<float>();
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        trzymane = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        trzymane = false;
        WylaczNatychmiast(false);
    }

    private void Wlacz()
    {
        leje = true;
        mamHit = false;
        blokujSplashDoCzasu = Time.time + splashDelayPoWlaczeniu;
        // sprawdzanie czy water istnieje
        if (waterPrefab != null && waterInstance == null)
        {
            waterInstance = Instantiate(waterPrefab);
            waterInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // ustaw water na muzzle
        AktualizujWaterNaMuzzle();

        // reset, zeby szybkie klikniecie triggera zawsze startowalo Water
        if (waterInstance)
        {
            // zatrzymaj i wyczysc (ogon poprzedniego strumienia)
            waterInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            waterInstance.Clear(true);

            waterInstance.Play(true);
            ZapamietajParametryWater();
            UstawDlugoscStrumienia(maksymalnyZasiegWizualny);

        }
        WlaczDzwiekWody();

        // Na start lania nie pokazuje splasha dopoki nie ma swiezego hita
        UkryjSplash();
    }

    private void Wylacz()
    {
        leje = false;
        if (waterInstance)
        {
            UstawDlugoscStrumienia(maksymalnyZasiegWizualny);
            waterInstance.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        UkryjSplash();
        mamHit = false;
    }


    private void WylaczNatychmiast(bool clear)
    {
        leje = false;
        mamHit = false;

        if (waterInstance)
        {
            if (clear) waterInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            else waterInstance.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        if (splashInstance)
        {
            if (clear) splashInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            else splashInstance.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void AktualizujWaterNaMuzzle()
    {
        if (!waterInstance || !waterMuzzle) return;
        waterInstance.transform.SetPositionAndRotation(waterMuzzle.position, waterMuzzle.rotation);

        if (audioWody && waterMuzzle)
        {
            audioWody.transform.position = waterMuzzle.position;
            audioWody.transform.rotation = waterMuzzle.rotation;
        }

    }

    private void AktualizujTrafienieISplash()
    {
        if (!waterMuzzle) return;

        Ray ray = new Ray(waterMuzzle.position, waterMuzzle.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, zasieg, maskaTrafien, QueryTriggerInteraction.Ignore))
        {
            UstawDlugoscStrumienia(hit.distance - zapasOdSplasha);

            Vector3 p = hit.point + hit.normal * offsetOdPowierzchni;

            if (!mamHit || (p - ostatniHitPoint).sqrMagnitude > (progRuchuHit * progRuchuHit))
            {
                ostatniHitPoint = p;
                mamHit = true;
                UstawSplash(hit, p);
            }
            else
            {
                if (splashInstance && !splashInstance.isPlaying)
                    splashInstance.Play(true);
            }

            if (sila > 0f && hit.rigidbody != null)
                hit.rigidbody.AddForceAtPosition(waterMuzzle.forward * sila, hit.point, ForceMode.Force);

            var fire = hit.collider.GetComponentInParent<FirePoint>();
            if (fire != null)
                fire.ApplyWater(Time.deltaTime);
        }
        else
        {
            UstawDlugoscStrumienia(maksymalnyZasiegWizualny);

            mamHit = false;
            UkryjSplash();
        }
    }

    private void UstawSplash(RaycastHit hit, Vector3 pozycja)
    {
        if (!splashPrefab) return;

        if (splashInstance == null)
        {
            splashInstance = Instantiate(splashPrefab);
            splashInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        splashInstance.transform.position = pozycja;
        splashInstance.transform.rotation = Quaternion.LookRotation(hit.normal);

        // zabezpieczenie gdy start zawsze czysc przed Play
        if (!splashInstance.isPlaying)
        {
            splashInstance.Clear(true);
            splashInstance.Play(true);
        }
    }

    private void UkryjSplash()
    {
        if (splashInstance && splashInstance.isPlaying)
            splashInstance.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void UstawAudioWody()
    {
        if (!audioWody)
            audioWody = GetComponent<AudioSource>();

        if (!audioWody)
            audioWody = gameObject.AddComponent<AudioSource>();

        audioWody.playOnAwake = false;
        audioWody.loop = true;
        audioWody.spatialBlend = 1f;   // dzwiek 3D
        audioWody.dopplerLevel = 0f;
        audioWody.volume = 0f;

        if (dzwiekWodyLoop)
            audioWody.clip = dzwiekWodyLoop;
    }

    private void WlaczDzwiekWody()
    {
        if (!audioWody || !audioWody.clip) return;

        if (!audioWody.isPlaying)
            audioWody.Play();
    }

    private void AktualizujDzwiekWody()
    {
        if (!audioWody) return;

        float docelowaGlosnosc = leje ? glosnoscWody : 0f;

        audioWody.volume = Mathf.MoveTowards(
            audioWody.volume,
            docelowaGlosnosc,
            predkoscWygaszania * Time.deltaTime
        );

        // Gdy ucichnie i nie leje sie woda to -- zatrzyma AudioSource
        if (!leje && audioWody.volume <= 0.01f && audioWody.isPlaying)
            audioWody.Stop();
    }

    private void ZapamietajParametryWater()
    {
        if (!waterInstance) return;
        if (domyslnyLifetime > 0f && domyslnaPredkosc > 0f) return; // ju¿ zapamiêtane

        var main = waterInstance.main;

        // UWAGA: najlepiej ustaw w prefabie sta³e StartSpeed i StartLifetime
        domyslnyLifetime = main.startLifetime.constant;
        domyslnaPredkosc = main.startSpeed.constant;

        if (domyslnaPredkosc < 0.01f)
            domyslnaPredkosc = 0.01f;

        // Dla bezpieczeñstwa – ¿eby maksymalny zasiêg wizualny nie by³ wiêkszy ni¿ zasieg raycastu
        if (maksymalnyZasiegWizualny < zasieg)
            maksymalnyZasiegWizualny = zasieg;
    }

    private void UstawDlugoscStrumienia(float odleglosc)
    {
        if (!waterInstance) return;

        ZapamietajParametryWater();

        odleglosc = Mathf.Clamp(odleglosc, minimalnyZasiegWizualny, maksymalnyZasiegWizualny);

        var main = waterInstance.main;

        float speed = main.startSpeed.constant;
        if (speed < 0.01f) speed = domyslnaPredkosc;
        if (speed < 0.01f) speed = 0.01f;

        // lifetime = dystans / predkosc
        float lifetime = odleglosc / speed;

        // bezpieczny clamp, ¿eby nie robiæ absurdalnych wartoœci
        lifetime = Mathf.Clamp(lifetime, 0.02f, 10f);

        main.startLifetime = lifetime;
    }



}
