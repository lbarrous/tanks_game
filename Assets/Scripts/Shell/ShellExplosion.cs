using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    //Referencia a la capa en la que estan los tanques
    public LayerMask m_TankMask;
    //referencia al sistame de particulas de la bala al explotar
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;           
    //Radio de accion de la explosion de la bala       
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        //Destruye la bala si tras 2 segundos no ha chocado con nada
        Destroy(gameObject, m_MaxLifeTime);
    }

    //Metodo que se llama con cada colision del collider de cada proyectil
    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.

        //Genera una esfera alrededor del objeto que captura todos los colliders que estan dentro y pertenezcan a la capa de "players" --> TankMask
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i= 0; i< colliders.Length; i++)
        {
            //Obtenemos el rigidbody del tanque correspondiente para realizar modificaciones fisicas en el
            Rigidbody targetRigidBody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigidBody) continue;

            targetRigidBody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            //Seria lo mismo poner colliders[i] que targetRigidBody
            //Pillamos el componente health del tanque para restarle la vida correspondiente
            TankHealth targetHealth = targetRigidBody.GetComponent<TankHealth>();

            if (!targetHealth) continue;

            float damage = CalculateDamage(targetRigidBody.position);

            targetHealth.TakeDamage(damage);
        }

        //Quitamos que el efecto de particulas sea hijo del proyectil, ya que si lo es, al destruirse este, no se ejecutaria
        m_ExplosionParticles.transform.SetParent(null);

        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        //Destruimos el proyectil, y un poco mas tarde, el efecto de particulas
        Destroy(gameObject);
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.

        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDistance = Mathf.Min(explosionToTarget.magnitude, m_ExplosionRadius);
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        float damage = relativeDistance * m_MaxDamage;
        return damage;
    }
}