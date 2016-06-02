using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Rondas para ganar
    public int m_NumRoundsToWin = 5;  
    //Mensajes de inicio y final de juego      
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;            
    //Referencia a los objetos tank  
    public GameObject m_TankPrefab;   
    //Array de los tanques --> Con su tankmanager para manipularlos      
    public TankManager[] m_Tanks;           


    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;       


    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

        //Rutina de juego --> LLeva la carga de estados posibles en el juego
        StartCoroutine(GameLoop());
    }


    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            //Instanciamos un tanque a traves de su prefab en la posicion de spawn correspondiente al tanque que corresponda en el array con sus correspondientes parametros
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            //Setup del correspondiente TankManager
            m_Tanks[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        //Creamos un array de tantos elementos Transform como tanques tengamos --> Para pasarle todos los elementos y que situe la camara correctamente
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            //Le pasamos a cada elemento del array la posicion (Transform) de la instancia del tankmanager para pasarselos a la camara
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {
        //Ejecuta las corutinas de inicio de partida, partida en curso y partida acabada para despues comprobar si hay ganador y reiniciar el juego
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();

        m_CameraControl.SetStartPositionAndSize();
        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();
        m_MessageText.text = string.Empty;

        //Bucle en cada fotograma que comprueba si hay un solo tanque restante
        while(!OneTankLeft())
        {
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        DisableTankControl();
        m_RoundWinner = GetRoundWinner();

        PlayerPrefs.SetInt("n_rondas_jugadas", PlayerPrefs.GetInt("n_rondas_jugadas") + 1);

        if (m_RoundWinner.m_PlayerNumber == 1)
        {
            PlayerPrefs.SetInt("n_wins_rojo", PlayerPrefs.GetInt("n_wins_rojo")+1);
        }
        else if(m_RoundWinner.m_PlayerNumber == 2)
        {
            PlayerPrefs.SetInt("n_wins_azul", PlayerPrefs.GetInt("n_wins_azul") + 1);
        }
        if(m_RoundWinner != null)
        {
            //Aumentamos el numero de veces ganadas de la referencia al tankmanager del ganador
            m_RoundWinner.m_Wins++;
        }
        m_GameWinner = GetGameWinner();

        if(m_GameWinner != null)
        {
            if (m_RoundWinner.m_PlayerNumber == 1)
            {
                PlayerPrefs.SetString("last_winner", "Rojo");
            }
            else if (m_RoundWinner.m_PlayerNumber == 2)
            {
                PlayerPrefs.SetString("last_winner", "Azul");
            }
        }

        string message = EndMessage();
        m_MessageText.text = message;

        yield return m_EndWait;
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        //mira en cada elemento del array de tanques si su estancia esta activa --> Si esta desactivada es que esta muerto
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        //Devolveria null si ambos tanques han muerto en la ronda --> Disparo mutuo
        return null;
    }


    private TankManager GetGameWinner()
    {
        //Comprueba para cada tanque si su numero de victorias es igual al numero de rondas a ganar
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "EMPATE!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " GANA LA RONDA!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " VICTORIAS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " GANA EL JUEGO!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}