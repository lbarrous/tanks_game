using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BotonSalir : MonoBehaviour {

    public bool salir;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(salir)
            {
                Debug.Log("Salimos del juego");
                Application.Quit();
            }
            else
            {
                SceneManager.LoadScene("Portada");
            }
            
        }
	}
}
