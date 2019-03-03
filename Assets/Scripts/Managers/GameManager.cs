using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public CameraControl m_CameraControl;
    public Text m_MessageText;
    public GameObject m_TankPrefab;
    public TankManager[] m_Tanks;

    // Numero de ronda que se juega
    private int m_RoundNumber;
    // Tiempo de espera inicial
    private WaitForSeconds m_StartWait;
    // Tiempo de espera del mensaje de victoria
    private WaitForSeconds m_EndWait;
    // Referencia al tanque ganador de la ronda
    private TankManager m_RoundWinner;
    // Referencia al tanque ganador del juego
    private TankManager m_GameWinner;

    // Método inicial
    private void Start()
    {
        // Creamos los objetos de espera
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        // Llamamos al metodo que instancia todos los tanques
        SpawnAllTanks();
        // Llamamos al metodo que coloca la referencia a todos esos tanques para la camara
        SetCameraTargets();
        // Corutina que se encarga de llevar todo el estado del juego
        StartCoroutine(GameLoop());
    }


    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // Recorre todos los tanques le añada la referencia con el prefab del tanque colocado en el spawnpoint con su rotación
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            // Le ponemos el número al tanque
            m_Tanks[i].m_PlayerNumber = i + 1;
            // hacemos el setup de ese tanque manager, se inicializa todo lo que hay en el setup del script
            m_Tanks[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        // creamos un array de transform por cada elemento en el array de tanques
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            // recorremos el array y le colocamos el transform de la instancia del gameobject del tanque que está en esa misma posición del array de tanques
            targets[i] = m_Tanks[i].m_Instance.transform;
        }
        // se le asigna a la variable de targets de camera control para que los siga
        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {
        // incia la ejecución de...
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        // si hay alguna refrencia ya tenemos tanque ganador del juego
        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
            //Application.LoadedLevel(Application.LoadedLevel);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        // Reinicia la partida

        // reseteamos todos los tanques
        ResetAllTanks();

        // desactiva todos los controles de los tanques
        DisableTankControl();

        // llamamos al componete de la camara para que se inicialice en posición y tamaño
        m_CameraControl.SetStartPositionAndSize();

        // incrementamos el numero de ronda
        m_RoundNumber++;

        // cambiamos el texto por el de la ronda correspondiente
        m_MessageText.text = "ROUND " + m_RoundNumber;

        // esperara tantos segundos como tenga la variable para inicializarse y se vea el texto
        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        // metodo que activa los controles de los tanques
        EnableTankControl();

        // quitamos el texto con una cadean vacia
        m_MessageText.text = string.Empty;

        // mientras que no quede un tanque en la escena esperamos al siguiente fotograma
        while (!OneTankLeft())
        {
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        // deshabilitamos los controles de los tanques
        DisableTankControl();

        // vemos si hay algun ganador de la ronda
        m_RoundWinner = GetRoundWinner();

        // mirar si hay un ganador, le incrementamos una victoria
        if (m_RoundWinner!=null)
        {
            m_RoundWinner.m_Wins++;
        }
        // Comprobamos si hay algún ganador del juego
        m_GameWinner = GetGameWinner();

        // se encarga de ver que mensaje se muestra en pantalla
        string message = EndMessage();

        // y lo colocamos en el canvas
        m_MessageText.text = message;

        // esperamos mostrando el texto el tiempo que tenga esa variable
        yield return m_EndWait;
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;
        // recorre todo los tanques que hay viendo si su tankmanager está activo en la escena
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }
        //devuelve true si quedan menos de dos en la escena
        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

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
        // recorre todos los tankmanager y llama al metodo que activa el componente de movimiento y estado del tanque
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