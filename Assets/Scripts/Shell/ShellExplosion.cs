using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    // Guarda un número de capas concretas (para que cuando explote solo afecte a ojetos de las capas indicadas)
    public LayerMask m_TankMask;
    // referencia al sistema de particulas
    public ParticleSystem m_ExplosionParticles;
    // referencia al audio source
    public AudioSource m_ExplosionAudio;
    // Daño máximo que se le hará a un objeto
    public float m_MaxDamage = 100f;
    // fuerza de empuje al repeler
    public float m_ExplosionForce = 1000f;
    // Tiempo que estará vivo proyectil en segundos
    public float m_MaxLifeTime = 2f;
    // radio de explosión
    public float m_ExplosionRadius = 5f;


    private void Start()
    {
        // que se destrulla automaticamente al pasar 2 segundos
        Destroy(gameObject, m_MaxLifeTime);
    }

    // cuando colisiona con otro collider
    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
        // creamos una esfera y dentro buscamos colliders que pertenezcan a la capa indicada
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);


        // si colisiona con un tanque
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            if (!targetRigidbody) continue;

            // Aplicamos la fuerza de empuje, duerza, posición y radio
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            // quitamos energia al tanque
            TankHealth tangetHealth = targetRigidbody.GetComponent<TankHealth>();
            if (!tangetHealth) continue;

            float damage = CalculateDamage(targetRigidbody.position);

            tangetHealth.TakeDamage(damage);

        }

        // Al colisionar con cualquier cosa
        m_ExplosionParticles.transform.SetParent(null);
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        Destroy(gameObject);
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;
        // Distancia menor entre los objetos
        float explosionDistance = Mathf.Min(explosionToTarget.magnitude, m_ExplosionRadius);
        // Porcentanje de daño de la distancia
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
        float damage = relativeDistance * m_MaxDamage;
        return damage;
    }
}