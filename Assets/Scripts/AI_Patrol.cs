// Patrol.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class AI_Patrol : MonoBehaviour
{
    [SerializeField] Transform[] patrolWaypoints;
    [Tooltip("The time bot will wait at a waypoint.")]
    [SerializeField] float bot_StartWaitTime = 4f;      //  Wait time of every action
    [Tooltip("The speed bot will walk at when not chasing.")]
    [SerializeField] float bot_WalkSpeed = 6f;
    [Tooltip("Activates autoBrakiing for bot, bot will slow down slightly as it nears waypoint if ON.")]
    [SerializeField] bool bot_WillBreak = true;
    [Tooltip("Set bot to face next waypoint or remain facing current direction.")] 
    [SerializeField] float detectionRange = 10f;         // distance before chace begins
    [Tooltip("Set to range at witch to be chased")]
    [SerializeField] bool faceNextLocation;         // Toggle to make bot rotate to face next waypoint if set On / True
    private Animator anim;

    public Transform Player;

    private NavMeshAgent bot_NavMeshAgent;      //  Refernce to Navmesh Agent assigned to object
    private int bot_CurrentWaypointIndex = 0;   //  Current waypoint where the bot is going to move to
    private float bot_WaitTimeTimer;            //  Variable used to count the wait time when Bot is waiting
    private Rigidbody rb;
    

    // -- Fov -- \\
    public float radius;
    [Range(0, 360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    // -- caught -- \\
    [SerializeField] GameObject CaughtScreen;

    void Start()
    {
        anim = GetComponent<Animator>();    
        bot_NavMeshAgent = GetComponent<NavMeshAgent>();  // asigns the attached NavMeshAgen component

        // --- Fov Detection --- //
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    void Update()
    {
        if (bot_NavMeshAgent.hasPath)
        {
            anim.SetBool("Walking", true );
            anim.SetBool("Idle", false );
        }
        else
        {
            anim.SetBool("Idle", true);
            anim.SetBool("Walking", false);
        }



        bot_NavMeshAgent.autoBraking = bot_WillBreak;  // if set tp true, bot will slow as it approaches destination
        // bot states defined below //  (add other states in switch/case or if statements as needed) 

        //if (PlayerInRange())
        //{
        //    CaughtScreen.SetActive(true);
        //    Time.timeScale = 0;
        //}

        if (canSeePlayer == true)
        {
            StopCoroutine("RotateToFaceNextWayPoint");
            bot_WaitTimeTimer = 0f; 
            ChacePlayer();
        }
        else
        {
            Patrolling();  // default state
        }

              
    }

    void Patrolling()
    {
        Move(bot_WalkSpeed);
        bot_NavMeshAgent.SetDestination(patrolWaypoints[bot_CurrentWaypointIndex].position);

        if (bot_NavMeshAgent.remainingDistance <= bot_NavMeshAgent.stoppingDistance)
        {
            if (faceNextLocation)  // only executed if Face Next Location is True, causes Bot to face next waypoint while waiting
            {
                int nextWaypoint = (bot_CurrentWaypointIndex + 1) % patrolWaypoints.Length; // location of waypoint after current destination
                Vector3 nextWayPointDirection = patrolWaypoints[nextWaypoint].position;

                StartCoroutine(RotateToFaceNextWayPoint(transform, nextWayPointDirection, bot_StartWaitTime / 2f));
            }
            else
            {
                StopCoroutine(RotateToFaceNextWayPoint(transform, new Vector3(0, 0, 0), 0f));
            }

            if (bot_WaitTimeTimer <= 0)
            {
                NextPoint();
                Move(bot_WalkSpeed);
                bot_WaitTimeTimer = bot_StartWaitTime;
            }
            else
            {
                Stop();
                bot_WaitTimeTimer -= Time.deltaTime;
            }
        }
    }

    bool PlayerInRange()
    {
        return Vector3.Distance(transform.position, Player.position) <= detectionRange;
        //Debug.Log(PlayerInRange());
    }

    void ChacePlayer()
    {
        //Vector3 direction = (Player.position - transform.position).normalized;
        //Debug.Log("PP: " + Player.position); 
        //Debug.Log("Dir: " + direction);
        bot_NavMeshAgent.SetDestination(Player.transform.position);
    }

    // --- Collision --- \\
    

    public void NextPoint()
    {
        if (patrolWaypoints.Length == 0)
        {
            return;  // traps for no waypoint set
        }
        Debug.Log(patrolWaypoints.Length);
        Debug.Log((bot_CurrentWaypointIndex + 1) % patrolWaypoints.Length);
        bot_CurrentWaypointIndex = (bot_CurrentWaypointIndex + 1) % patrolWaypoints.Length;
        bot_NavMeshAgent.SetDestination(patrolWaypoints[bot_CurrentWaypointIndex].position);
        // % is modulo operator which divides one number by another and returns the remainder
        // used to ensure when adding 1 to waytpoint index it doesn't go out of bounds
    }

    void Stop()
    {
        bot_NavMeshAgent.isStopped = true;
        bot_NavMeshAgent.speed = 0;
    }

    void Move(float speed)
    {
        bot_NavMeshAgent.isStopped = false;
        bot_NavMeshAgent.speed = speed;
    }

    //---------- Coroutines -----------\\
    // --- patrol rotation --- \\
    private IEnumerator RotateToFaceNextWayPoint(Transform transform, Vector3 positionToLook, float timeToRotate)
    {
        var startRotation = transform.rotation;                  // the bots current rotation as a Quaternion
        var direction = positionToLook - transform.position;     // subtracts the target location from objects current position
        var finalRotation = Quaternion.LookRotation(direction);  // creates a rotation amount in Quaternions based on the calculated direction

        // convert and strip out X and Z Quaternion values so Bot only rotates around Y axis
        // bot sometimes tilts on short movement runs without this
        Vector3 finRotAsEuler = finalRotation.eulerAngles;   // stores finalRotation as a Vector3 containing Eulers
        finRotAsEuler = new Vector3(0, finRotAsEuler.y, 0);  // strips out and replaces X and Z values with 0's
        finalRotation = Quaternion.Euler(finRotAsEuler);     // converts the modified value back into a Quarternion

        var t = 0f;      // used to keep track of elapsed time
        while (t <= 1f)  // loops while t value is less than 1
        {
            t += Time.deltaTime / timeToRotate;  // increments t counter value by deltatime divided by time specified for rotation  
            transform.rotation = Quaternion.Lerp(startRotation, finalRotation, t);
            //FieldOfViewCheck();
            //performs the rotation gradually based on value of 't' timer using Lerp
            yield return null; // waits for next update to continue this while loop in the co-routine
        }
        transform.rotation = finalRotation; // ensures the objects final rotation is locked to 
    }

    // --- FOV detection --- \\
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    Debug.Log("canSeePlayer");
                }
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }


    // Draw detection range
    private void OnDrawGizmosSelected()
    {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

}

