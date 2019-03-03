using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    // nivel de energia inicial
    public float m_StartingHealth = 100f;
    // referencia del color de la vida
    public Slider m_Slider;
    // referencia a la imagen de relleno
    public Image m_FillImage;
    public Color m_FullHealthColor = Color.green;
    public Color m_ZeroHealthColor = Color.red;
    // referencia al prefab de la explosión del tanque
    public GameObject m_ExplosionPrefab;

    // Referencia al sonido de la explosión
    private AudioSource m_ExplosionAudio;
    // referencia al sistema de particulas de la explosión
    private ParticleSystem m_ExplosionParticles;
    // Vida actual del tanque
    private float m_CurrentHealth;
    // si está muerto o no
    private bool m_Dead;


    private void Awake()
    {
        // se instancia el prefab de la explosión
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        // se instancia el sonido de la explosión
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        // se desactiva al inicio
        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        // valor de vida inicial
        m_CurrentHealth = m_StartingHealth;
        // le decimos que no está muerto
        m_Dead = false;

        // actualice la barra de energia del tanque
        SetHealthUI();
    }


    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        // a la vida actual le restamos el daño
        m_CurrentHealth -= amount;

        SetHealthUI();

        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        // actualiza el valor y color de la barra de energia
        m_Slider.value = m_CurrentHealth;
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true;
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        gameObject.SetActive(false);
    }
}