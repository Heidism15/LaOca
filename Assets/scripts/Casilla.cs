using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casilla : MonoBehaviour
{
    public int numeroCasilla;

    private void Awake()
    {
        // Asegurar que el nombre tiene el formato correcto
        if (!gameObject.name.StartsWith("Casilla_"))
        {
            Debug.LogError("Error: El GameObject '" + gameObject.name + "' no tiene el formato esperado (Casilla_X)");
            return;
        }

        // Extraer el n�mero despu�s del guion bajo
        string casillaString = gameObject.name.Substring(8); // Cambi� a 8 porque "Casilla_" tiene 8 caracteres

        // Verificar que el n�mero sea v�lido
        if (int.TryParse(casillaString, out int numero))
        {
            numeroCasilla = numero;
            GameManager.Instance.RegistrarCasilla(numeroCasilla, this.gameObject);
        }
        else
        {
            Debug.LogError("Error: No se pudo convertir '" + casillaString + "' a un n�mero en la casilla '" + gameObject.name + "'");
        }
    }

}

