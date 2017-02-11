using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {
    public float m_patrolSpeed = 2.0f;
    public float m_chaseSpeed = 5.0f;

    // enemies pause briefly when reaching the last sighting of the player OR when reaching the waypoints of their patrol route
    public float m_chaseWaitTime = 5.0f;
    public float m_patrolWaitTime = 1.0f;

    public Transform[] m_patrolWaypoints;

    private EnemySight m_enemySight;
    private NavMeshAgent m_nav;
    private GameObject m_player;
    private Transform m_playerTransform;
    private PlayerHealth m_playerHealth;
    private LastPlayerSighting m_lastPlayerSighting;
    private float m_chaseTimer;
    private float m_patrolTimer;
    
    // waypoints that keep track of the enemy guard's current destination.
    private int m_waypointIndex;

    // Use this for initialization
    void Awake () {
        m_enemySight = GetComponent<EnemySight>();
        m_nav = GetComponent<NavMeshAgent>();
        m_player = GameObject.FindGameObjectWithTag(Tags.player);
        //m_playerTransform = m_player.transform;
        m_playerHealth = m_player.GetComponent<PlayerHealth>();
        m_lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
    }
	
	// Update is called once per frame
	void Update () {
        if (m_enemySight.m_playerInSight && m_playerHealth.m_health > 0.0f)
            Shooting();
        else if (m_enemySight.m_personalLastSighting != m_lastPlayerSighting.m_resetPosition && m_playerHealth.m_health > 0.0f)
            Chasing();
        else
            Patrolling();
    }

    void Shooting() {
        m_nav.Stop(); // enemy guard will stop to shoot
        // the rest is handled by EnemyShooting.cs
    }

    void Chasing() {
        Vector3 sightingDeltaPosition = m_enemySight.m_personalLastSighting - transform.position;
        if (sightingDeltaPosition.sqrMagnitude > 4.0f)
            m_nav.destination = m_enemySight.m_personalLastSighting;

        m_nav.speed = m_chaseSpeed;

        if (m_nav.remainingDistance < m_nav.stoppingDistance) {
            m_chaseTimer += Time.deltaTime; // makes enemy wait when it reaches the last sighting.

            if (m_chaseTimer > m_chaseWaitTime) { // if enemy has waited long enough, it will go on patrol route again.
                m_lastPlayerSighting.m_position = m_lastPlayerSighting.m_resetPosition;
                m_enemySight.m_personalLastSighting = m_lastPlayerSighting.m_resetPosition;

                m_chaseTimer = 0.0f;
            }
        } else {
            m_chaseTimer = 0.0f;
        }

    }

    void Patrolling() {
        m_nav.speed = m_patrolSpeed;

        // if the enemy guard is near the destination [aka .remainingDistance < .stoppingDistance] or has no destination (meaning it lost track of the player) aka .m_resetPosition
        if (m_nav.destination == m_lastPlayerSighting.m_resetPosition || m_nav.remainingDistance < m_nav.stoppingDistance) {
            m_patrolTimer += Time.deltaTime;

            // if enemy guard is finished waiting for patrol time
            if (m_patrolTimer >= m_patrolWaitTime) {
                if (m_waypointIndex == m_patrolWaypoints.Length - 1) {
                    // if enemy is already at the end of the array of waypoints, we reset the waypoint to zero.
                    m_waypointIndex = 0;
                } else {
                    // if not, we keep iterating through the array of waypoints.
                    m_waypointIndex++;
                }

                m_patrolTimer = 0.0f;
            }
        } else {
            // if we are NOT near the destination or destination is NOT the m_resetPosition, reset timer.
            m_patrolTimer = 0.0f;
        }

            // update the destination
            m_nav.destination = m_patrolWaypoints[m_waypointIndex].position;
    }

}
