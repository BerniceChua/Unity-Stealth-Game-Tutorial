using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySight : MonoBehaviour {
    /*
     * if player is outside field of view,
     * enemy will not react.
     */
    [SerializeField] float m_fieldOfViewAngle = 110.0f;
    [SerializeField] bool m_playerInSight;
    [SerializeField] Vector3 m_personalLastSighting;

    private NavMeshAgent m_nav;
    private SphereCollider m_fieldOfViewTriggerSphereCollider;
    private Animator m_anim;
    private LastPlayerSighting m_lastPlayerSighting;
    private GameObject m_player;
    private Animator m_playerAnim;
    private PlayerHealth m_playerHealth;
    private HashIDs m_hash;

    private Vector3 m_previousSighting;

    private void Awake() {
        m_nav = GetComponent<NavMeshAgent>();
        m_fieldOfViewTriggerSphereCollider = GetComponent<SphereCollider>();
        m_anim = GetComponent<Animator>();
        m_lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
        m_player = GameObject.FindGameObjectWithTag(Tags.player);
        m_playerAnim = m_player.GetComponent<Animator>();
        m_playerHealth = m_player.GetComponent<PlayerHealth>();
        m_hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

        m_personalLastSighting = m_lastPlayerSighting.m_resetPosition;
        // these are initialized separately so when the game begins, the enemy guards are not automatically chasing the player.
        m_previousSighting = m_lastPlayerSighting.m_resetPosition;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // need to check if global sighting has changed, and if it has, update personal sighting of player.
        if (m_lastPlayerSighting.m_position != m_previousSighting)
            m_personalLastSighting = m_lastPlayerSighting.m_position;

        m_previousSighting = m_lastPlayerSighting.m_position;

        // We only want the enemy guards to chase the player if the player is doing science and is still alive.
        if (m_playerHealth.m_health > 0.0f)
            m_anim.SetBool(m_hash.playerInSightBool, m_playerInSight);
        else
            m_anim.SetBool(m_hash.playerInSightBool, false);
    }

    /* 
     * For "m_playerInSight" to be true, it must satisfy these 3 conditions:
     * 1) m_player is within the trigger zone
     * 2) m_player is in front of enemy's field of view
     * 3) nothing is blocking enemy's view of player
     */
    private void OnTriggerStay(Collider other) {
        if (other.gameObject == m_player) {
            // if m_player is within trigger zone, but the other 2 conditions are false, it's still false.
            m_playerInSight = false;

            // if m_player's raycast hit result is within LESS than half of the m_fieldOfViewAngle, then the player is within the field of view
            // other.transform.position is the player's transform position.  transform.position is the enemy guard's own transform position
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);  // transform.forward is the enemy guard's own transform facing forward

            if (angle < m_fieldOfViewAngle * 0.5f) {
                RaycastHit hit;

                /* Origin of this and most character models is at its feet (y = 0), so we'll need to raise this by transform.up (1 unit up) & transform.position as starting point for raycast
                 * direction vector of a raycast is always normalized so it will always have a magnitude of 1
                 * distance of raycast is m_fieldOfViewTriggerSphereCollider so if player is outside it, enemy can't detect player
                 */
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, m_fieldOfViewTriggerSphereCollider.radius)) {
                    // check if the thing that the raycast has hit is indeed the player, then the guard has seen the player.
                    if (hit.collider.gameObject == m_player) {
                        m_playerInSight = true;
                        m_lastPlayerSighting.m_position = m_player.transform.position;
                    }
                }
            }

            // check if guard has heard the player if the player is running or shouting within the trigger m_fieldOfViewTriggerSphereCollider
            //int playerLayerZeroStateHash = m_playerAnim.GetCurrentAnimatorStateInfo(0).nameHash;
            //int playerLayerOneStateHash = m_playerAnim.GetCurrentAnimatorStateInfo(1).nameHash;
            // the above 2 use "nameHash" which is obsolete, so they were replaced with "fullPathHash" which is its equivalent in version 5.x
            int playerLayerZeroStateHash = m_playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash;
            int playerLayerOneStateHash = m_playerAnim.GetCurrentAnimatorStateInfo(1).fullPathHash;

            // if the enemy guard can hear the player it will go to the source of the sound to inspect.
            if (playerLayerZeroStateHash == m_hash.locomotionState || playerLayerOneStateHash == m_hash.shoutState) {
                if (CalculatePathLength(m_player.transform.position) <= m_fieldOfViewTriggerSphereCollider.radius) {
                    m_personalLastSighting = m_player.transform.position;
                }
            }
        }

    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject == m_player)
            m_playerInSight = false;
    }

    float CalculatePathLength(Vector3 targetPosition) {
        NavMeshPath path = new NavMeshPath();

        if (m_nav.enabled)
            m_nav.CalculatePath(targetPosition, path);

        // part of NaveMeshPath() is an array of Vector3s called "corners"
        Vector3[] allWaypoints = new Vector3[path.corners.Length + 2]; // Length+2 to allow for enemy & player positions.
        allWaypoints[0] = transform.position;
        allWaypoints[allWaypoints.Length - 1] = targetPosition;

        // for-loop assigns each corner
        for (int i = 1; i < allWaypoints.Length; i++) {
            allWaypoints[i] = path.corners[i - 1];
        }

        float pathLength = 0;

        // this for-loop iterates over the lengths between each waypoint and gets their sum
        for (int i = 0; i < allWaypoints.Length-1; i++) {
            pathLength += Vector3.Distance(allWaypoints[i], allWaypoints[i + 1]);
        }

        return pathLength;
    }

}
