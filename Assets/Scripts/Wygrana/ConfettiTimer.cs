using UnityEngine;

public class ConfettiTimer : MonoBehaviour
{
    public ParticleSystem confetti;
    public float interval = 3f;

    void Start()
    {
        InvokeRepeating(nameof(PlayConfetti), 0f, interval);
    }

    void PlayConfetti()
    {
        confetti.Play();
    }
}
