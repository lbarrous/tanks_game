using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    //salud inicial
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;  
    //Colores de la salud                    
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
    private bool m_Dead;            


    private void Awake()
    {
        //Instanciamos el objeto explosion del tanque y referenciamos el sistema de particulas
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        //Pilla el componente del sistema de particulas y accede al componente de audio
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        //Desactiva la explosion instanciada --> Sera activada luego cuando el tanque muera
        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }
    

    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.

        m_CurrentHealth -= amount;
        SetHealthUI();

        if(m_CurrentHealth <=0 && !m_Dead)
        {
            OnDeath();
        }
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.

        m_Slider.value = m_CurrentHealth;
        //Devuelve un color interpolado en base a la vida actual
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.

        m_Dead = true;

        //Colocamos el sistema de particulas de la explosion en el lugar del tanque, realziamos la animacion, hacemos sonar el audio y desactivamos el tanque
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);
        m_ExplosionAudio.Play();

        gameObject.SetActive(false);
    }
}