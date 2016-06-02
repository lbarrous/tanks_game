using UnityEngine;
using UnityEngine.UI;

public class Sonido : MonoBehaviour
{
    public AudioSource music;

    private bool boolToogleButton;

    public void ToggleSound()

    {

        if (boolToogleButton == false)
        {
            music.Pause();
            boolToogleButton = true;
            PlayerPrefs.SetInt("Sonido", 0);
        }
        else
        {
            music.Play();
            boolToogleButton = false;
            PlayerPrefs.SetInt("Sonido", 1);
        }
    }

}