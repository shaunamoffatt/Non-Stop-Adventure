using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawn : MonoBehaviour
{
    public GameObject[] spawnpoints;
    public GameObject Enemy;
    private int i = 0;
    public float spawnTime = 30f;

    void Start()
    {
        spawnpoints = GameObject.FindGameObjectsWithTag("spawn");
        Debug.Log("SpawnPoints: " + spawnpoints.Length);
        for (int i = 0; i < spawnpoints.Length; i++)
        {
            CreateEnemy();
        }
        StartCoroutine(Spawn());

    }

    private IEnumerator Spawn()
    {
        //spawn every 30 seconds
        yield return new WaitForSeconds(spawnTime);
        CreateEnemy();
        Debug.Log("ENEMY SPAWNED " + i);
        StartCoroutine(Spawn());
    }

    void CreateEnemy()
    {
        //increment i
        i = (i < spawnpoints.Length - 1) ? i + 1 : 0;
        Vector3 spawnPos = spawnpoints[i].transform.position;
        Instantiate(Enemy);
        //Enemy.transform.position = spawnPos;
        NavMeshAgent agent = Enemy.GetComponentInChildren<NavMeshAgent>();
        if (!agent.isOnNavMesh)
        {
            agent.transform.position = spawnPos;
            //bool t = agent.Warp(spawnPos);
            //Debug.Log("WARP SUCCESS: " + t);
            agent.enabled = false;
            agent.enabled = true;
        }
    }
}
