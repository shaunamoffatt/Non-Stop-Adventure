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
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        //spawn every 30 seconds
        yield return new WaitForSeconds(spawnTime);
        //increment i
        i = (i < spawnpoints.Length - 1) ? i + 1 : 0;
        Vector3 spawnPos = spawnpoints[i].transform.position;
        Instantiate(Enemy);
        NavMeshAgent agent = Enemy.GetComponentInChildren<NavMeshAgent>();
        if (!agent.isOnNavMesh)
        {
            agent.transform.position = spawnPos;
            Debug.Log("Spawn position : " + spawnPos);
            agent.enabled = false;
            agent.enabled = true;
        }

        Debug.Log("ENEMY SPAWNED " + i);
        StartCoroutine(Spawn());
    }
}
