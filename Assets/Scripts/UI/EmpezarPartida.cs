using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EmpezarPartida : MonoBehaviour
{

    // Update is called once per frame
    public void Empezar()
    {
        SceneManager.LoadScene("Main");
    }
}
