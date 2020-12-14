using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]

public class EnemyController : MonoBehaviour
{
    enum Enemy
    {
        Tiki,
        Mummy,
        Eskimo
    }

    [SerializeField] GameObject deathParticle;

    public float lookRadius = 10f;
    Transform target;
    NavMeshAgent agent;

    [SerializeField] public float patrolSpeed = 2f;
    [SerializeField] public float chaseSpeed = 5f;
    public float chaseTimer = 3f;
    public float chaseWaitTime = 5f;

    //Acceleration to help stop the enemy sliding
    public float acceleration = 10f;
    public float deceleration = 20f;

    [SerializeField] float rotateSpeed = 5f;

    [SerializeField] public Transform[] moveSpots;
    private int randomSpot;

    //set wait times for patroling
    private float waitTime;
    public float startWaitTime = 1f;

    Rigidbody rb;

    private const int DEADLAYER = 16;
    //switch to Let the enemy follow the player for a certain length of time
    bool following = false;

    //Control the different sound of ememies
    SoundManager.Sound soundDie, soundAlert;

    // Start is called before the first frame update
    void Start()
    {
        InitializeSound();

        //init deathparticle
        if (deathParticle == null)
        {
            Debug.LogError("EnemyControlller has no deathParticle");
        }

        //Set the ragdoll to false
        SetRigidbodyState(true);
        SetColliderState(false);
        GetComponentInChildren<Animator>().enabled = true;

        //Init Rigidbody
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        waitTime = startWaitTime;
        //SET the target to be that player using th playerManager
        target = PlayerManager.instance.player.transform;
        try
        {
            agent = GetComponent<NavMeshAgent>();
        }catch{
            Debug.LogError("Cant get the navmesh agent");
        }
        
        agent.enabled = true;

        ChooseStartingPatrolSpot();
    }

    void ChooseStartingPatrolSpot()
    {
        //Set a random start spot
        if (moveSpots != null)
            randomSpot = Random.Range(0, moveSpots.Length - 1);
        else
            Debug.LogError("Forgot to add move spots for the enemy");
    }

    void InitializeSound()
    {
        //soundDie = SoundManager.Sound.tikaDie;
        //soundAlert = SoundManager.Sound.tikaAlert;
        //TODO different sounds for different enemyies
        switch (LevelController.currentLevel)
        {
            case (LevelController.LEVEL.FOREST):
                soundDie = SoundManager.Sound.tikaDie;
                soundAlert = SoundManager.Sound.tikaAlert;
                break;
            case (LevelController.LEVEL.DESERT):
                soundDie = SoundManager.Sound.mummyDie;
                soundAlert = SoundManager.Sound.mummyAlert;
                break;
            case (LevelController.LEVEL.SNOW):
                soundDie = SoundManager.Sound.eskimoDie;
                soundAlert = SoundManager.Sound.eskimoAlert;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //If they are dead return
        if (gameObject.layer == DEADLAYER)
            return;
        // speed up slowly, but stop quickly
        if (agent.hasPath)
            agent.acceleration = (agent.remainingDistance < 0.2f) ? deceleration : acceleration;

        if (following == false)
        {
            Patrol();
        }
        //Check Distance from player target
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= lookRadius)
        {
            following = true;
            ChaseEnemy();
        }
    }

    void ChaseEnemy()
    {
        //Chase player
        agent.SetDestination(target.position);
        agent.speed = chaseSpeed;
        //Play Alert sound
        SoundManager.PlaySound(soundAlert, transform.position);
        //FaceTarget
        FaceTarget(target.position);

        if (agent.remainingDistance < agent.stoppingDistance)
        {
            // increment the timer.
            chaseTimer += Time.deltaTime;

            // If the timer exceeds the wait time
            if (chaseTimer >= chaseWaitTime)
            {
                following = false;
                chaseTimer = 0f;
            }
        }
        else
        {
            // If not reset the timer.
            chaseTimer = 0f;
            following = false;
        }

    }

    void Patrol()
    {
        agent.speed = patrolSpeed;
       
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            if (waitTime <= 0)
            {
                randomSpot = Random.Range(0, moveSpots.Length - 1);
                waitTime = startWaitTime;
                FaceTarget(moveSpots[randomSpot].position);

            }
            else
            {
                waitTime -= Time.deltaTime;

            }
       
        ///move to a random position
        agent.SetDestination(moveSpots[randomSpot].position);
    }

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 dir = (targetPosition - transform.position).normalized;
        //rotation to look at target
        Quaternion lookAtRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtRotation, Time.deltaTime * rotateSpeed);
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("hit the Enemy");
        //Play Enemy die sound
        SoundManager.PlaySound(soundDie, transform.position);

        //Add an implulse force and disable NavMesh agent
        agent.enabled = false;
      
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
