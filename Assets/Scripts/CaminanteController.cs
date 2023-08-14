using System.Collections.Generic;
using UnityEngine;
using Panda;

public class CaminanteController : MonoBehaviour
{
    public Transform enemigo;
    public float velocidad = 5f;
    public float distanciaDeteccion = 10f;
    public float rangoAtaque = 0f;
    public float energia = 100f;
    public float costoAtaque = 0.25f;
    public float energiaRegeneradaPorSegundo = 10f;
    public List<Transform> puntosPerimetro = new List<Transform>();
    private int puntoActual = 0;
    private bool estaDescansando = false;
    private Renderer rend;
    public float rotacionSuavizada = 10f;
    public GameObject playerCamera;
    private float tiempoUltimoAtaque;
    public float intervaloAtaque = 1f;
    public Transform zonaSegura;
    public float radioZonaSegura = 5f;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Enemigo")
        {
            Debug.Log("Colisionando...");
        }
    }

    private void Start()
    {
        rend = GetComponent<Renderer>();

        // Si no hay puntos definidos, crea algunos por default
        if (puntosPerimetro == null || puntosPerimetro.Count == 0)
        {
            DefinirPuntosPorDefault();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightAlt))
        {
            playerCamera.SetActive(false);
        }
        else if (Input.GetKeyUp(KeyCode.RightAlt))
        {
            playerCamera.SetActive(true);
        }
    }



    [Task]
    private void EnemigoCerca()
    {
        if (enemigo != null && Vector3.Distance(transform.position, enemigo.position) < distanciaDeteccion)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    private void IrEnemigo()
    {
        if (estaDescansando || enemigo == null)
        {
            Task.current.Fail();
            return;
        }

        if (Vector3.Distance(transform.position, enemigo.position) <= rangoAtaque)
        {
            Task.current.Succeed();
            return;
        }
        Vector3 direccion = (enemigo.position - transform.position).normalized;
        transform.position += direccion * velocidad * Time.deltaTime;
        Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, rotacionSuavizada * Time.deltaTime);
    }

    [Task]
    private void Atacar()
    {
        if (estaDescansando)
        {
            Task.current.Fail();
            return;
        }

        // Verificar el tiempo desde el último ataque
        if (Time.time - tiempoUltimoAtaque < intervaloAtaque)
        {
            Task.current.Fail();
            return;
        }

        energia -= costoAtaque;
        Debug.Log("Atacando, energía restante:" + energia);

        tiempoUltimoAtaque = Time.time; // Actualizar el tiempo del último ataque

        if (energia <= 0)
        {
            Task.current.Fail();
            estaDescansando = true;
            return;
        }

        Task.current.Succeed();
    }


    [Task]
    private void Descansar()
    {
        if (!estaDescansando)
        {
            Task.current.Fail();
            return;
        }

        // Si no está en la zona segura, moverse hacia ella.
        if (Vector3.Distance(transform.position, zonaSegura.position) > radioZonaSegura)
        {
            Vector3 direccion = (zonaSegura.position - transform.position).normalized;
            transform.position += direccion * velocidad * Time.deltaTime;
            Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, rotacionSuavizada * Time.deltaTime);
            return; // Regresa después de moverse, para no ejecutar el código de descanso a continuación.
        }

        // Si está dentro de la zona segura, realizar las acciones de descanso.

        // Girar sobre su eje.
        transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime);

        // Cambiar de color en arcoíris.
        float t = Mathf.PingPong(Time.time, 1);
        Color arcoiris = Color.HSVToRGB(t, 1, 1);
        rend.material.color = arcoiris;

        // Cargar energía.
        energia += energiaRegeneradaPorSegundo * Time.deltaTime;
        Debug.Log("Cargando: " + energia + "%"); // Imprime el porcentaje de carga.

        if (energia >= 100)
        {
            energia = 100;
            estaDescansando = false;
            rend.material.color = Color.white; // Vuelve al color original cuando termina.
            Task.current.Succeed();
        }
    }




    [Task]
    private void RecorrerPerimetro()
    {
        if (estaDescansando || (enemigo != null && Vector3.Distance(transform.position, enemigo.position) < distanciaDeteccion))
        {
            Task.current.Fail();
            return;
        }

        if (Vector3.Distance(transform.position, puntosPerimetro[puntoActual].position) < 1f)
        {
            puntoActual = (puntoActual + 1) % puntosPerimetro.Count;
        }

        MoverHaciaPuntoPerimetro();
        Task.current.Succeed();
    }

    private void MoverHaciaPuntoPerimetro()
    {
        Vector3 direccion = (puntosPerimetro[puntoActual].position - transform.position).normalized;
        transform.position += direccion * velocidad * Time.deltaTime;
        Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, rotacionSuavizada * Time.deltaTime);
    }

    private void DefinirPuntosPorDefault()
    {
        // Aquí definirás los puntos por default. Por ejemplo:
        Transform punto1 = new GameObject("Punto1").transform;
        punto1.position = new Vector3(18, -0.5f, -22);
        puntosPerimetro.Add(punto1);

        Transform punto2 = new GameObject("Punto2").transform;
        punto2.position = new Vector3(18, -0.5f, -58);
        puntosPerimetro.Add(punto2);

        Transform punto3 = new GameObject("Punto3").transform;
        punto3.position = new Vector3(55, -0.5f, -58);
        puntosPerimetro.Add(punto3);

        Transform punto4 = new GameObject("Punto4").transform;
        punto4.position = new Vector3(55, -0.5f, -22);
        puntosPerimetro.Add(punto4);
    }

}
