using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]

public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject deathParticle;
    public float lookRadius = 10f;
    Transform target;
    NavMeshAgent agent;
    [SerializeField] float rotateSpeed = 5f;

    [SerializeField] public Transform[] moveSpots;
    private int randomSpot;

    //set wait times for patroling
    private float waitTime;
    public float startWaitTime = 3f;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        //init deathparticle
        if(deathParticle == null)
        {
            Debug.LogError("EnemyControlller has no deathParticle");
        }
        waitTime = startWaitTime;
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        //Set a random start spot
        if (moveSpots != null)
            randomSpot = Random.Range(0, moveSpots.Length - 1);

        //Init Rigidbody
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Check Distance from player target
        float distance = Vector3.Distance(transform.position, target.position);
        if(distance <= lookRadius)
        {
            Debug.Log("Enemy Follow Player");
            //Chase player
            agent.SetDestination(target.position);
            //FaceTarget
            FaceTarget();
        }
        else
        {
            Debug.Log("Enemy Patrolling Move to spot : " + moveSpots[randomSpot]);
            //Patrol
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                if (waitTime <= 0)
                {
                    randomSpot = Random.Range(0, moveSpots.Length - 1);
                    waitTime = startWaitTime;
                    Patrol();
                }
                else
                {
                    waitTime -= Time.deltaTime;
                }
            Patrol();
        }
    }

    void Patrol()
    {
        // Returns if no points have been set up
        if (moveSpots.Length == 0)
            return;

        ///move to a random position
        agent.destination = moveSpots[randomSpot].position;

    }

    void FaceTarget()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        //rotation to look at target
        Quaternion lookAtRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtRotation, Time.deltaTime * rotateSpeed);
    }

    public void DieOnWhipCollide()
    {
        Debug.Log("Going to Destroy Enemy");
        //TODO kill enemy
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("hit the Enemy");
        //play blood particel effect
        //bloodParticles.SetActive(true);
        //play die animation
        //animator.SetTrigger("Die");
        //Set kinematic to true to allow the enemy to fall
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        StartCoroutine(KillEnemy());

    }

    IEnumerator KillEnemy()
    {
        //Play death particles
        deathParticle.SetActive(true);
        //Stop collisions
        gameObject.GetComponent<Collider>().enabled = false;
        yield return new WaitForSecondsRealtime(0.5f);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);

    }
}
