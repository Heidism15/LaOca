using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int[] vectorCasillas;
    public int[] infoCasillas;
    public GameObject[] vectorObjetos;
    public Dado dado;

    private int rondaActual = 1;
    private bool turnoJugador = true;

    public GameObject fichaJugador;
    public GameObject fichaIA;

    private int posicionJugador = 0;
    private int posicionIA = 0;

    private bool esperandoTurno = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        InicializarVectores();
    }

    private void Start()
    {
        ActualizarColoresCasillas();
    }

    private void Update()
    {
        if (turnoJugador && !esperandoTurno && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(TurnoJugador());
        }
    }

    public void TirarDado()
    {
        StartCoroutine(ProcesarTurno());
    }

    public void InicializarVectores()
    {
        vectorCasillas = new int[22];
        infoCasillas = new int[22];
        vectorObjetos = new GameObject[22];

        for (int i = 0; i < vectorCasillas.Length; i++)
        {
            vectorCasillas[i] = 0;
        }

        infoCasillas[1] = 1;
        infoCasillas[6] = 1;
        infoCasillas[12] = 2;
        infoCasillas[18] = 2;
        infoCasillas[5] = -1;
        infoCasillas[10] = -1;
        infoCasillas[14] = -1;
        infoCasillas[19] = -1;
        infoCasillas[20] = -1;
        infoCasillas[21] = 99;
    }

    public void RegistrarCasilla(int indice, GameObject casilla)
    {
        if (indice >= 0 && indice < vectorObjetos.Length)
        {
            vectorObjetos[indice] = casilla;
        }
    }

    public void ActualizarColoresCasillas()
    {
        for (int i = 0; i < vectorObjetos.Length; i++)
        {
            if (vectorObjetos[i] != null)
            {
                Renderer meshRenderer = vectorObjetos[i].GetComponent<Renderer>();

                if (meshRenderer != null)
                {
                    Color nuevoColor =
                        (infoCasillas[i] == 1) ? new Color(1.0f, 0.5f, 0.0f) :
                        (infoCasillas[i] == -1) ? Color.red :
                        (infoCasillas[i] == 2) ? Color.green :
                        (infoCasillas[i] == 99) ? Color.yellow :
                        (i == 7 || i == 13) ? Color.blue :
                        Color.white;

                    meshRenderer.material.color = nuevoColor;
                }
            }
        }
    }

    IEnumerator ProcesarTurno()
    {
        dado.LanzarDado();
        yield return new WaitForSeconds(2.5f);  // Tiempo para el lanzamiento del dado

        int resultado = dado.ResultadoDado;

        if (turnoJugador)
        {
            // Mueve la ficha del jugador y espera a que termine
            yield return StartCoroutine(MoverFichaAnimado(fichaJugador, resultado));
        }
        else
        {
            // Mueve la ficha de la IA y espera a que termine
            yield return StartCoroutine(MoverFichaAnimado(fichaIA, resultado));
        }

        // Cambio de turno se hace aquí, al final de todo el proceso
        CambiarTurno();

        // Si ambos han jugado, cambiamos de ronda
        if (!turnoJugador)
        {
            rondaActual++;  // Cambiamos de ronda después de que ambos hayan jugado
            CanvasManager.Instance.ActualizarUI(rondaActual, turnoJugador);
        }

        // Si es el turno de la IA, lanzar el dado automáticamente
        if (!turnoJugador)
        {
            yield return new WaitForSeconds(1f); // Pequeña pausa antes de lanzar
            TirarDado(); // La IA lanza el dado automáticamente
        }
    }

    IEnumerator TurnoJugador()
    {
        esperandoTurno = true; // Evita que se vuelva a presionar "Espacio"
        yield return StartCoroutine(ProcesarTurno());
        esperandoTurno = false; // Permite lanzar el dado nuevamente en el siguiente turno
    }

    IEnumerator TurnoIA()
    {
        esperandoTurno = true; // Bloquea nuevos lanzamientos
        yield return StartCoroutine(ProcesarTurno());
        esperandoTurno = false; // Se desbloquea después del turno de la IA
    }


    public void MoverFicha(int resultadoDado)
    {
        // Mueve la ficha de jugador o IA según corresponda
        if (turnoJugador)
        {
            StartCoroutine(MoverFichaAnimado(fichaJugador, resultadoDado));

        }
        else
        {
            StartCoroutine(MoverFichaAnimado(fichaIA, resultadoDado));
        }
    }


    private void AplicarReglasCasilla(ref int posicion, GameObject ficha)
    {
        int tipoCasilla = infoCasillas[posicion];

        if (tipoCasilla == 1) // Teleport
        {
            // Si es la casilla 1, teletransportar a la 7
            int destino = (posicion == 1) ? 7 : 13;
            posicion = destino;
            ficha.transform.position = vectorObjetos[destino].transform.position;

            // Actualizar la posición en el GameManager
            if (ficha == fichaJugador)
            {
                posicionJugador = destino; // Actualizar la posición del jugador
            }
            else if (ficha == fichaIA)
            {
                posicionIA = destino; // Actualizar la posición de la IA
            }

            Debug.Log("Teletransportado a la casilla " + destino);
            CambiarTurno();
        }
        else if (tipoCasilla == -1) // Retroceder 3 casillas
        {
            posicion -= 3;
            if (posicion < 0) posicion = 0;
            ficha.transform.position = vectorObjetos[posicion].transform.position;
            // Actualizar la posición en el GameManager
            if (ficha == fichaJugador)
            {
                posicionJugador = posicion; // Actualizar la posición del jugador
            }
            else if (ficha == fichaIA)
            {
                posicionIA = posicion; // Actualizar la posición de la IA
            }
            CambiarTurno();
        }
        else if (tipoCasilla == 2) // Vuelve a tirar
        {
            Debug.Log("Vuelve a tirar!");
            return; // No se cambia el turno aquí, ya que el jugador vuelve a tirar
        }
        else if (tipoCasilla == 99) // Victoria
        {
            Debug.Log((turnoJugador ? "Jugador" : "IA") + " ha ganado la partida.");
            return; // No es necesario cambiar de turno si alguien ha ganado
        }
        else
        {
            // Cambiar turno de que todos los efectos hayan sido aplicados
            CambiarTurno();
        }
    }


    public void CambiarTurno()
    {
        // Cambiar el turno al siguiente jugador
        turnoJugador = !turnoJugador;

        // Aumentar la ronda después de que ambos hayan jugado
        if (!turnoJugador)
        {
            rondaActual++;
        }

        // Actualizar UI
        CanvasManager.Instance.ActualizarUI(rondaActual, turnoJugador);
    }

    IEnumerator MoverFichaAnimado(GameObject ficha, int pasos)
    {
        int posicionActual = turnoJugador ? posicionJugador : posicionIA;
        int nuevaPosicion = posicionActual + pasos;

        if (nuevaPosicion >= vectorObjetos.Length) nuevaPosicion = vectorObjetos.Length - 1;

        if (nuevaPosicion == (turnoJugador ? posicionIA : posicionJugador)) nuevaPosicion++;

        for (int i = posicionActual + 1; i <= nuevaPosicion; i++)
        {
            if (vectorObjetos[i] != null)
            {
                ficha.transform.position = vectorObjetos[i].transform.position;
                yield return new WaitForSeconds(0.3f);
            }
        }

        if (turnoJugador)
        {
            posicionJugador = nuevaPosicion;
        }
        else
        {
            posicionIA = nuevaPosicion;
        }

        AplicarReglasCasilla(ref nuevaPosicion, ficha);
    }

    IEnumerator MoverIA()
    {
        int posicionActual = posicionIA;
        int posicionAnterior = posicionActual - 1;
        int posicionSiguiente = posicionActual + 1;

        // Verificar si la IA está en la misma casilla que el jugador
        if (posicionIA == posicionJugador && posicionIA != 0)
        {
            // Elegir aleatoriamente entre moverse a una casilla más o menos
            bool moverAdelante = Random.Range(0, 2) == 0; // 50% de probabilidad
            yield return StartCoroutine(MoverFichaAnimado(fichaIA, moverAdelante ? 1 : -1));
        }
        else
        {
            // Evaluar las casillas adyacentes
            int efectoAnterior = posicionAnterior >= 0 ? infoCasillas[posicionAnterior] : 0;
            int efectoAdelante = posicionSiguiente < infoCasillas.Length ? infoCasillas[posicionSiguiente] : 0;

            // Priorizar casillas con modificadores positivos y evitar negativos
            if (efectoAdelante > 0 && efectoAnterior <= 0)
            {
                yield return StartCoroutine(MoverFichaAnimado(fichaIA, 1));
            }
            else if (efectoAnterior > 0 && efectoAdelante <= 0)
            {
                yield return StartCoroutine(MoverFichaAnimado(fichaIA, -1));
            }
            else
            {
                // Si no hay casillas especiales, elegir aleatoriamente
                bool moverAdelante = Random.Range(0, 2) == 0;
                yield return StartCoroutine(MoverFichaAnimado(fichaIA, moverAdelante ? 1 : -1));
            }
        }

        // Aplicar las reglas de la casilla después de mover la ficha
        int nuevaPosicion = turnoJugador ? posicionJugador : posicionIA;
        AplicarReglasCasilla(ref nuevaPosicion, fichaIA);
    }

    public void MoverFichaColision(bool moverAdelante)
    {
        StartCoroutine(MoverFichaAnimado(turnoJugador ? fichaJugador : fichaIA, moverAdelante ? 1 : -1));
    }
}
