using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoController : MonoBehaviour
{
    public float velocidad = 5f;
    public float vida = 100f;

    public List<Transform> puntosMovimiento; // Puntos que definen el área

    private Vector3 puntoObjetivo; // Punto aleatorio dentro del área definida

    // Variables para el cooldown
    private bool enCooldown = false;
    private float tiempoCooldown = 1f; // Tiempo en segundos
    private Renderer rend; // Renderer del enemigo

    private void Start()
    {
        SeleccionarPuntoObjetivo();
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        MoverHaciaPuntoObjetivo();
    }

    private void SeleccionarPuntoObjetivo()
    {
        // Encuentra un punto objetivo aleatorio dentro del área definida
        float x = Random.Range(puntosMovimiento[0].position.x, puntosMovimiento[2].position.x);
        float z = Random.Range(puntosMovimiento[0].position.z, puntosMovimiento[2].position.z);

        puntoObjetivo = new Vector3(x, transform.position.y, z);
    }

    private void MoverHaciaPuntoObjetivo()
    {
        Vector3 direccion = (puntoObjetivo - transform.position).normalized;
        transform.position += direccion * velocidad * Time.deltaTime;

        // Si el enemigo está cerca del punto objetivo, seleccionamos un nuevo punto objetivo
        if (Vector3.Distance(transform.position, puntoObjetivo) < 0.5f)
        {
            SeleccionarPuntoObjetivo();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!enCooldown && collision.collider.CompareTag("Computador"))
        {
            vida -= 15f;
            Debug.Log("Vida: " + vida);
            if (vida <= 0)
            {
                vida = 100;
                transform.position = new Vector3(0, 0, 0);
                Invoke("Respawn", 5f);  // Utiliza Invoke en lugar de StartCoroutine
            }
            enCooldown = true;
            StartCoroutine(Cooldown());

            // Retroceso
            Vector3 direccionRetroceso = -collision.contacts[0].normal;
            transform.position += direccionRetroceso * 0.5f;

            // Iniciar parpadeo
            StartCoroutine(Parpadeo());
        }
    }


    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(tiempoCooldown + 0.2f); // Añadido un pequeño delay adicional
        enCooldown = false;
    }

    IEnumerator Parpadeo()
    {
        float tiempoInicio = Time.time;
        while (Time.time - tiempoInicio < tiempoCooldown)
        {
            rend.enabled = !rend.enabled; // Alternar visibilidad
            yield return new WaitForSeconds(0.1f); // Duración del parpadeo, puedes ajustarla
        }
        rend.enabled = true; // Asegurarse de que el enemigo sea visible al final
    }
}
