using UnityEngine;

public class TankMovement : MonoBehaviour
{
    //Numero de jugador
    public int m_PlayerNumber = 1;        
    //Velocidad en m/s 
    public float m_Speed = 12f;         
    //Grado de variacion del eje/s   
    public float m_TurnSpeed = 180f;      
    
    //Recrusos de audio del movimiento del tanque 
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      

    //Variacion del tono --> Para que varios tanques suenen con distinto tono
    public float m_PitchRange = 0.2f;

    
    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;       
    //Valores entre -1 y 1 para el eje de movimiento del tanque --> Movimiento  
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;         


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        //Permitimos al rigidbody , que cuando aparezca en la escena, sea afectado por las fisicas, colisiones...
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        //Eliminamos el acceso a fisicas del rigidbody al desactivar el tanque de la escena
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        //Permitimos la entrada de usuario de 2 jugadores, 2 tanques con sus correspondientes ejes creados en edit/input
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
    }
    

    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.

        //Pasamos la entrada de los ejes del usuario a los ejes del tanque
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        //Si los valores de los ejes son 0 es que no se esta pulsando ningun boton.

        EngineAudio();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.

        //Si lo que recibo del eje de entrada de movimiento y giro es muy similar a 0:
        if(Mathf.Abs(m_MovementInputValue)<0.1f && Mathf.Abs(m_TurnInputValue)<0.1f)
        {
            //No estoy pulsando ninguna tecla
            if(m_MovementAudio.clip != m_EngineIdling)
            {
                //Si en ese momento estaba reproduciendo el sonido de tanque moviendose, cambiamos al sonido de tanque quieto --> Solo lo estableceriamos por tanto, la primera vez
                changeTankAudio(m_EngineIdling);
            }
        }
        else
        {
            //Estamos moviendonos o girando
            if (m_MovementAudio.clip != m_EngineDriving)
            {
                //Si en ese momento estaba reproduciendo el sonido de tanque moviendose, cambiamos al sonido de tanque quieto --> Solo lo estableceriamos por tanto, la primera vez
                changeTankAudio(m_EngineDriving);
            }

        }
    }
    
    //Funcion para aplicar al tanque el audio correspondiente (Movimiento o quieto)
    private void changeTankAudio(AudioClip audio_tank)
    {
        m_MovementAudio.clip = audio_tank;
        m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
        m_MovementAudio.Play();
    }


    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.

        //Calculamos el vector de movimiento utilizando:
        // forward --> Vector unitario en el eje z
        // velocidad
        // Valor del eje (-1 o 1 para saber si es positivo o negativo)
        // deltatime --> Tiempo que dura el fotograma
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        //Para calcular el movimiento absoluto sobre el objeto en ese momento se lo añadimos al movimiento actual SUMANDO.
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.

        //Calculamos el quaternion para el giro utilizando:
        // velocidad de rotacion
        // Valor del eje (-1 o 1 para saber si es positivo o negativo)
        // deltatime --> Tiempo que dura el fotograma
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        //Para calcular la rotacion absoluta sobre el objeto en ese momento se lo añadimos a la rotacion actual MULTIPLICANDO.
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}