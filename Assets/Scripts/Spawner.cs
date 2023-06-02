using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ElementASpawnejar;
    private float m_SpawnRate;
    private float m_SpawnRateDelta = 3f;

    [SerializeField]
    private int numeroEnemigosVivos;
    private int enemigosOleada;
    private int enemigosSpawneaos;
    private int oleada;

    private Coroutine spawnC;
  
    [SerializeField]
    private Transform[] m_SpawnPoints;

    void Start()
    {
        //m_SpawnRateDelta = m_SpawnRate;
        spawnC = StartCoroutine(SpawnCoroutine());
        oleada= 0;
        enemigosSpawneaos= 0;
        enemigosOleada = numeroEnemigosVivos;
    }


    IEnumerator SpawnCoroutine()
    {
        while (enemigosSpawneaos != (numeroEnemigosVivos + oleada))
        {
            m_SpawnRate = Random.Range(3,7);
            GameObject spawned = Instantiate(m_ElementASpawnejar);
            //GameObject spawned = m_PoolElements.GetElement();
            spawned.transform.position = m_SpawnPoints[Random.Range(0, m_SpawnPoints.Length)].position;
            enemigosSpawneaos++;
            yield return new WaitForSeconds(m_SpawnRate);
        }
    }
    public void enemigoMuerto()
    {
        enemigosOleada--;
        if(enemigosOleada == 0)
        {
            StopCoroutine(spawnC);
            StartCoroutine("empezarOtraOleada");
        }
    }
    private IEnumerator empezarOtraOleada()
    {
        enemigosSpawneaos= 0;
        oleada++;
        enemigosOleada = numeroEnemigosVivos + oleada;
        yield return new WaitForSeconds(5);
        spawnC = StartCoroutine(SpawnCoroutine());
    }

}
