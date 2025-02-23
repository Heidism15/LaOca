using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dado : MonoBehaviour
{
    public static Dado Instance;

    private Rigidbody rb;
    private bool puedeLanzar = true;
    private Transform caraInferior; // Almacena la cara que toca el suelo
    private int resultadoDado = 1;

    // Array con las caras del dado (deben asignarse en el Inspector)
    public Transform[] caras; // 0 = Cara1, 1 = Cara2, ..., 5 = Cara6

    // Diccionario de caras opuestas
    private int[] carasOpuestas = { 5, 4, 3, 2, 1, 0 }; // 1↔6, 2↔5, 3↔4

    public int ResultadoDado { get { return resultadoDado; } } // Añadir propiedad para acceder al resultado

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void LanzarDado()
    {
        puedeLanzar = false; // Bloquear nuevas tiradas

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Aplicar fuerza y torque aleatorios
        rb.AddForce(new Vector3(Random.Range(-2f, 2f), Random.Range(4f, 7f), Random.Range(-2f, 2f)), ForceMode.Impulse);
        rb.AddTorque(new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)), ForceMode.Impulse);

        // Esperar antes de detectar la cara en contacto
        StartCoroutine(EsperarYDetectarCara());
    }

    IEnumerator EsperarYDetectarCara()
    {
        yield return new WaitForSeconds(2f); // Espera para evitar detecciones falsas

        // Buscar la cara que está en contacto con el suelo
        caraInferior = ObtenerCaraInferior();

        if (caraInferior != null)
        {
            // Obtener el índice de la cara en el array
            int indiceCaraInferior = System.Array.IndexOf(caras, caraInferior);

            if (indiceCaraInferior != -1)
            {
                // Obtener la cara opuesta y su número
                int indiceCaraSuperior = carasOpuestas[indiceCaraInferior];
                resultadoDado = indiceCaraSuperior + 1; // Convertir índice a número real
                Debug.Log("Resultado del dado: " + resultadoDado);

                // Llamar a MoverFicha después de tirar el dado
                GameManager.Instance.MoverFicha(resultadoDado);
            }
        }

        puedeLanzar = true; // Permitir lanzar de nuevo
    }

    private Transform ObtenerCaraInferior()
    {
        Transform caraInferiorDetectada = null;
        float menorAltura = float.MaxValue;

        // Recorrer todas las caras y detectar la más cercana al suelo
        foreach (Transform cara in caras)
        {
            if (cara.position.y < menorAltura)
            {
                menorAltura = cara.position.y;
                caraInferiorDetectada = cara;
            }
        }

        return caraInferiorDetectada;
    }
}
