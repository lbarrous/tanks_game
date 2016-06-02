using System;
using UnityEngine;


//Clase que se encarga de agrupar en arrays los tanques que estan en la escena y que son manipulados por GameManager
[Serializable]
public class TankManager
{
    public Color m_PlayerColor;            
    public Transform m_SpawnPoint;         
    [HideInInspector] public int m_PlayerNumber;             
    [HideInInspector] public string m_ColoredPlayerText;
    //Instancia del tanque
    [HideInInspector] public GameObject m_Instance;          
    [HideInInspector] public int m_Wins;                     

    //Referencias a los scripts de movimiento y disparo, ademas del canvas donde se alojan los sliders de disparo y vida para manipularlos.
    private TankMovement m_Movement;       
    private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject;


    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            //Pinta cada elemento de renderizado del tanque del color del jugador
            renderers[i].material.color = m_PlayerColor;
        }
    }


    public void DisableControl()
    {
        //Desactiva movimiento, disparo y sliders
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        //Activa movimiento, disparo y sliders
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    public void Reset()
    {
        //Pone a los tanques en la posicion de respawn, los desactiva y vuelve a activar --> Hay que hacerlo ya que uno siempre queda activo al ser ganador de la partida
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
