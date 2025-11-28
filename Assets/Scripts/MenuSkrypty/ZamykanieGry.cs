using UnityEngine;

public class ZamykanieGry : MonoBehaviour
{

    public void ZamknijGre()
    {
        Application.Quit();


#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }
}
