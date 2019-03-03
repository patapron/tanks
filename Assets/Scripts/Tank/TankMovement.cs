using UnityEngine;

public class TankMovement : MonoBehaviour
{
    // Las variables publicas se muestran en el inspector
    public int m_PlayerNumber = 1;
    public float m_Speed = 12f;
    public float m_TurnSpeed = 180f;
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;
    public float m_PitchRange = 0.2f;


    private string m_MovementAxisName;
    private string m_TurnAxisName;
    private Rigidbody m_Rigidbody;
    private float m_MovementInputValue;
    private float m_TurnInputValue;
    private float m_OriginalPitch;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
    }


    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing. Obtenemos los valores de lo que se esté pulsando
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        // Actualizaremos el ruido del motor si se está moviendo
        EngineAudio();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.Comprobaremos que hay movimiento antes de cambiar el sonido
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            // No estoy pulsando ninguna tecla

            // Comprobamos que no se esté reproduciendo el sonido de movimiento, en esa caso debemos cambiarlo
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                // Cambiamos el clip
                m_MovementAudio.clip = m_EngineIdling;
                // Establecemos un pich aleatorio
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                // Lo reproducimos
                m_MovementAudio.Play();
            }
        }
        else
        {
            // Estamos pulsando una tecla de movimiento o giro

            // Comprobamos que no se esté reproduciendo el sonido de parado, en esa caso debemos cambiarlo
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                // Cambiamos el clip
                m_MovementAudio.clip = m_EngineDriving;
                // Establecemos un pich aleatorio
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                // Lo reproducimos
                m_MovementAudio.Play();
            }
        }
    }


    private void FixedUpdate()
    {
        // Move and turn the tank. Aplicamos el movimiento o el giro
        Move();
        Turn();
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input. Cuando queremos movel un objeto de sitio, 
        // necesitamos calcular un vector que nos dirá cuanto queremos cambiarlo o moverlo en cada uno de los ejes

        // Forward hacer referencia al eje z que es donde se movera hacia delante, multiplicamos por la velocidad de movimiento, 
        // por el valor de la tecla que se esté pulsando, adelante positivo y hacía atrás negativo, 
        //eso nos daría el valor para mover en un segudo pero al multiplicarlo por el delta tiem, nos devuelve el valor a mover en el este fotogram
        Vector3 movement = transform.forward * m_Speed * m_MovementInputValue * Time.deltaTime;

        // Aplicamos el movimiento a la situación actual
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        // Unity almacena los giros en unobjeto de ti`po quaternium, así que hay que transformarlo x,y,z
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        // Las rotaciones no van como los vectores en suma, hay que hacerlo con multiplicación.
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}