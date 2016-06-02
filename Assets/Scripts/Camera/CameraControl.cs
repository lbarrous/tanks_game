using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //Tiempo que queremos que pase para hacer un cambio de posicion o zoom
    public float m_DampTime = 0.2f;              
    //Intervalo minimo de uinidades de espacio que apareceran (Para evitar que una cama corte a la figura en medio de la escena)
    public float m_ScreenEdgeBuffer = 4f;           
    public float m_MinSize = 6.5f;                  
    [HideInInspector] public Transform[] m_Targets; 

    //Referencia a la camara
    private Camera m_Camera;     
    //Velocidad con la q hacemos el zoom                   
    private float m_ZoomSpeed;      
    //Velocidad con la que movemos la camara                
    private Vector3 m_MoveVelocity;      
    //Posicion a la que queremos que apunte la camara --> Media de posicion de todos los tanques de la escena           
    private Vector3 m_DesiredPosition;              


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
        Move();
        Zoom();
    }


    private void Move()
    {
        //Busca la posicion media de todos los tanques
        FindAveragePosition();

        //Este metodo devolvera la posicion avanzada hacia la posicion deseada con respecto a la velocidad de la camara y el tiempo que necesite. --> Posicion que tendra que utilizar para llegar
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        //averagePos sera aproximadamente el centro de la posicion intermedia entre todos los tanques
        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();

        //Este metodo devolvera el zoom hacia la posicion deseada con respecto a la velocidad de la camara y el tiempo que necesite. --> Zoom que tendra que utilizar para llegar
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        //posicion deseada en el sistema de coordenadas local del cameraRig (soporte de la camara) --> posicion deseada relativa a la camara
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition); // --> hacemos inversetransform para pillar las coordenadas locales de las que recibimos de la camara en global

        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            //Calcula la posicion del tanque i relativo a la camerarig
            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

            //Diferencia entre donde queremos colocar la camara y el tanque
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            //Devuelve el maximo valor de y que tenga un tanque con respecto a la camerarig
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

            //Devuelve el maximo valor de ancho que tenga un tanque con respecto a la camerarig
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);

            //al final termina devolviendo el mayor size que es capaz de agrupar en pantalla a todos los tanques
        }
        
        //Añadimos el valor minimo de unidades que debe de haber en el borde para que asi, no quede ningun tanque cortado y aumente el size para contenerlos a todos completos en pantalla
        size += m_ScreenEdgeBuffer;

        //Si tenemos un valor menor de size al minsize, devolvemos el valor minsize de size para evitar que la camara este demasiado cerca
        size = Mathf.Max(size, m_MinSize);

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();

        //Poner la pantalla en el sitio adecuado cuando acabe la fase sin hacer movimiento fluido, es decir, poner la camara en el sitio correcto al acabar.
    }
}