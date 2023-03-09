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

    [SerializeField]
    GameObject deathParticle;

    public float lookRadius = 10f;
    Transform target;
    NavMeshAgent agent;

    [SerializeField]
    public float patrolSpeed = 2f;

    [SerializeField]
    public float chaseSpeed = 5f;

    float patrolStartTime = 0;

    [SerializeField]
    float rotateSpeed = 2.5f;

    // used for patrolling
    private Vector3 randomSpot;
    public float randomRange = 10f;

    //set wait times for patroling
    private float waitTime;
    public float startWaitTime = 1f;

    Rigidbody rb;

    private const int DEADLAYER = 16;

    //Control the different sound of ememies
    SoundManager.Sound soundDie,
        soundAlert;

    // used to briefly stop the enemy chasing the player onCollision
    bool collided = false;

    // Start is called before the first frame update
    void Start()
    {
        InitializeSound();

        //init deathparticle
        if (deathParticle == null)
        {
            Debug.LogError("EnemyControlller has no deathParticle");
        }
        //Play death particles once when spawing
        deathParticle.SetActive(true);

        //Set the ragdoll to false
        SetRigidbodyState(true);
        SetColliderState(false);
        GetComponentInChildren<Animator>().enabled = true;

        InitializeRigidBody();

        waitTime = startWaitTime;
        //SET the target to be that player using th playerManager
        target = PlayerManager.instance.player.transform;

        EnableNavMeshAgent();
    }

    private void EnableNavMeshAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        if (!agent.isOnNavMesh)
        {
            agent.transform.position = transform.position;
            agent.enabled = false;
            agent.enabled = true;
        }
    }

    Vector3 ChooseRandomSpot()
    {
        float randomSpotx = transform.position.x + Random.Range(-randomRange, +randomRange);
        float randomSpotz = transform.position.z + Random.Range(-randomRange, +randomRange);

        return new Vector3(randomSpotx, 0, randomSpotz);
    }

    void InitializeRigidBody()
    {
        //Init Rigidbody
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void InitializeSound()
    {
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

        if (agent.isOnNavMesh && !collided)
        {
            //Check Distance from player target
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= lookRadius)
            {
                ChaseEnemy();
            }
            if (distance > lookRadius)
            {
                Patrol();
            }
        }
    }

    void ChaseEnemy()
    {
        //Chase player
        agent.destination = target.transform.position;
        agent.speed = chaseSpeed;
        //Play Alert sound
        SoundManager.PlaySound(soundAlert, transform.position);
        //FaceTarget
        FaceTarget(target.position);
    }

    void Patrol()
    {
        ///move to a random position
        agent.SetDestination(randomSpot);
        FaceTarget(randomSpot);
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.1f || patrolStartTime < 5f)
        {
            patrolStartTime += Time.deltaTime;
            if (waitTime <= 0)
            {
                randomSpot = ChooseRandomSpot();
                waitTime = startWaitTime;
                patrolStartTime = 0f;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        //Using the players tag 17 to stop chase briefly
        if (collision.gameObject.layer == 17)
        {
            collided = true;
            Debug.Log("Collided with player");
            StartCoroutine(Rotate());
        }
    }

    //Rotate 360degrees
    IEnumerator Rotate()
    {
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + 360.0f;
        float t = 0.0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / 1f) % 360.0f;
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                yRotation,
                transform.eulerAngles.z
            );
            yield return null;
        }
        yield return new WaitForSeconds(2);
        collided = false;
    }

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 dir = (targetPosition - transform.position).normalized;
        if (dir.x != 0 && dir.z != 0)
        {
            //rotation to look at target
            Quaternion lookAtRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookAtRotation,
                Time.deltaTime * rotateSpeed
            );
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("hit the Enemy");
        //Play Enemy die sound
        SoundManager.PlaySound(soundDie, transform.position);

        //Add an implulse force and disable NavMesh agent
        agent.enabled = false;

        rb.AddForce(-transform.forward * 200, ForceMode.Impulse);

        StartCoroutine(KillEnemy());
    }

    IEnumerator KillEnemy()
    {
        //Play death particles
        deathParticle.SetActive(true);
        //Change Layer so that it cant hurt the  player
        //TODO Set the layers up in const game manager DEADPLAYER == 16
        SetLayerRecursively(gameObject, DEADLAYER);

        //Update Player Score
        PlayerManager.playerScore += 50;

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

        GetComponent<Rigidbody>().isKinematic = !state;
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
        Gizmos.DrawWireSphere(randomSpot, 3);
    }
}
