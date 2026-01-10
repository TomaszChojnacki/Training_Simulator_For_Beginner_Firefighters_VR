using UnityEngine;
using UnityEngine.SceneManagement;

public class PowrotDoMenu : MonoBehaviour
{
    [Header("Ladowanie Sceny")]
    [SerializeField] private int scenaIndex = 0;
    public void ZaladujScene()
    {
        SceneManager.LoadScene(scenaIndex);
    }
}
