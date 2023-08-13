using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject enemigoPrefab; // Asigna tu prefab de enemigo aquí desde el editor de Unity
    private float tiempoDeEspera = 5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckAndSpawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator CheckAndSpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoDeEspera);

            // Busca si hay algún objeto con la etiqueta "Enemigo" en la escena
            GameObject currentEnemy = GameObject.FindGameObjectWithTag("Enemigo");

            // Si no se encuentra ningún enemigo, instanciamos uno
            if (currentEnemy == null)
            {
                Instantiate(enemigoPrefab, new Vector3(0, 0.5f, 0), Quaternion.identity); // Cambia el Vector3 a la posición inicial deseada
            }
        }
    }
}
