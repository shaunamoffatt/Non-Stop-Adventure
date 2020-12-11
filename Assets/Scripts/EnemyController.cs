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

    private const int DEADLAYER = 16;

    // Start is called before the first frame update
    void Start()
    {
        //init deathparticle
        if(deathParticle == null)
        {
            Debug.LogError("EnemyControlller has no deathParticle");
        }
        //Set the ragdoll to false
        SetRigidbodyState(true);
        SetColliderState(false);
        GetComponentInChildren<Animator>().enabled = true;

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
        //If they are dead return
        if (gameObject.layer == DEADLAYER)
            return;

        //Check Distance from player target
        float distance = Vector3.Distance(transform.position, target.position);
        if(distance <= lookRadius)
        {
            //Chase player
            agent.SetDestination(target.position);
            //FaceTarget
            FaceTarget();
        }
        else
        {
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
        agent.SetDestination(moveSpots[randomSpot].position);

    }

    void FaceTarget()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        //rotation to look at target
        Quaternion lookAtRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtRotation, Time.deltaTime * rotateSpeed);
    }


    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("hit the Enemy");

        //TODO: play die animation (have non yet)

        //Add an implulse force and disable NavMesh agent
        agent.enabled = false;
        Vector3 pointOfExplosion = new Vector3(transform.position.x - 0.5f, transform.position.y - 0.5f, transform.position.z - 0.5f);

        //rb.AddExplosionForce(500f, pointOfExplosion, 10, 13);
        rb.AddForce(-transform.forward * 20, ForceMode.Impulse);
        //transform.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(0, 0, 100f), ForceMode.Impulse);

        StartCoroutine(KillEnemy());

    }

    IEnumerator KillEnemy()
    {
        //Play death particles
        deathParticle.SetActive(true);
        //Change Layer so that it cant hurt the  player
        //TODO Set the layers up in const game manager DEADPLAYER == 16
        SetLayerRecursively(gameObject, DEADLAYER);
        Debug.Log("LAyer : " + gameObject.layer);
        //Stop  Animations
        GetComponentInChildren<Animator>().enabled = false;
        //Start RagDollEffect
        SetRigidbodyState(false);
        SetColliderState(true);
        //Wait 5 secs before destroying
        yield return new WaitForSecondsRealtime(5f);
        Destroy(gameObject);
    }

    void SetRigidbodyState(bool state)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }

       //GetComponent<Rigidbody>().isKinematic = !state;
    }

    //Change all the child objects layers to Dead which is 16
    public static void SetLayerRecursively(GameObject g, int layerNumber)
    {
        if (null == g)
            return;
        g.layer = layerNumber;
        foreach (Transform child in g.transform)
        {
            if (null == child)
                continue;
            //child.gameobject contains the current child you can do whatever you want like add it to an array
            child.gameObject.layer = layerNumber;
            SetLayerRecursively(child.gameObject, layerNumber);
        }
    }

    void SetColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }

        GetComponent<Collider>().enabled = !state;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
