using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;

    public TextMeshProUGUI textoRonda;
    public TextMeshProUGUI textoJugador;

    public GameObject opcionesPanel; // Panel con botones de opci�n

    public void Start()
    {
        opcionesPanel.SetActive(false);
        ActualizarUI(1, true);
    }

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
    }

    public void ActualizarUI(int ronda, bool esTurnoJugador)
    {
        if (textoRonda != null)
            textoRonda.text = "Ronda: " + ronda;

        if (textoJugador != null)
            textoJugador.text = (esTurnoJugador ? "Jugador" : "IA");
    }

    // Mostrar el panel de opciones (adelante/atr�s)
    public void MostrarOpcionesMovimiento()
    {
        opcionesPanel.SetActive(true);
    }

    // Ocultar el panel de opciones
    public void OcultarOpcionesMovimiento()
    {
        opcionesPanel.SetActive(false);
    }

    // Funci�n para mover adelante
    public void OnElegirAdelante()
    {
        OcultarOpcionesMovimiento(); // Ocultar el panel
        GameManager.Instance.MoverFichaColision(true); // Mover adelante
    }

    // Funci�n para mover atr�s
    public void OnElegirAtras()
    {
        OcultarOpcionesMovimiento(); // Ocultar el panel
        GameManager.Instance.MoverFichaColision(false); // Mover atr�s
    }
}