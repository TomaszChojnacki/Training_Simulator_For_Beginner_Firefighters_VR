using UnityEngine;

public class ElektrycznaLampa : MonoBehaviour
{
    [Header("Referencje")]
    [SerializeField] private SystemElektrykiManager systemElektryki;
    [SerializeField] private PlayerTlenZdrowie player;

    [Header("Obrazenia od porazenia (na sekunde)")]
    [SerializeField] private float dmgNaSekunde = 25f;

    public void OnWaterHit(float dt)
    {
        if (!systemElektryki || !player) return;

        // Obrazenia tylko gdy prad wlaczony
        if (systemElektryki.PradWlaczony)
        {
            player.DodajObrazenia(dmgNaSekunde * dt);
        }
    }
}
