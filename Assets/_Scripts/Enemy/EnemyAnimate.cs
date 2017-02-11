using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimate : MonoBehaviour {
    // the deadZone stops the NPC AI from oversteering and walking in crooked lines
    [SerializeField] float m_deadZone;

    private Transform m_playerTransform;
    private EnemySight m_enemySight;
    private NavMeshAgent m_nav;
    private Animator m_anim;
    private HashIDs m_hash;
    private AnimatorSetup m_animSetup;

    private void Awake() {
        m_playerTransform = GameObject.FindGameObjectWithTag(Tags.player).transform;
        m_enemySight = GetComponent<EnemySight>();
        m_nav = GetComponent<NavMeshAgent>();
        m_anim = GetComponent<Animator>();
        m_hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

        // makes sure that enemy's rotation is set by Animator, not by 
        // NavMeshAgent, so it will reduce appearance of foot slipping when turning corners.
        m_nav.updateRotation = false;

        // create instance of helper function & call its constructor.
        m_animSetup = new AnimatorSetup(m_anim, m_hash);

        // sets layer weight of Animator, that controls balance of animations from Animator
        // 1 is totally overriding the animation layers under them.

        // in .SetLayerWeight(), first parameter is the layer number.  
        // layer 0 is the base layer.  since we want to use the mask, we set this integer to layer 1.
        m_anim.SetLayerWeight(1, 1.0f);  // Layer 1 is the "Shooting" layer.
        m_anim.SetLayerWeight(2, 1.0f);  // Layer 2 is the "Gun" layer.

        // convert m_deadZone from degrees to radians.
        m_deadZone *= Mathf.Deg2Rad;
    }

    // Update is called once per frame
    void Update () {
        NavAnimSetup();
    }

    // called after Update() every frame
    // this allows us to control root rotation directly.
    private void OnAnimatorMove() {
        m_nav.velocity = m_anim.deltaPosition / Time.deltaTime;
        transform.rotation = m_anim.rootRotation;
    }

    void NavAnimSetup() {
        float speed;
        float angle;

        if (m_enemySight.m_playerInSight) {
            speed = 0.0f;

            angle = FindAngleToTurn(transform.forward, m_playerTransform.position - transform.position, transform.up);
        } else {
            speed = Vector3.Project(m_nav.desiredVelocity, transform.forward).magnitude;

            angle = FindAngleToTurn(transform.forward, m_nav.desiredVelocity, transform.up);

            // since we're using damping...
            // if the angle is small, don't need to rotate character w/ the Animator controller
            if (Mathf.Abs(angle) < m_deadZone) {
                // look at desired velocity from its own position
                transform.LookAt(transform.position + m_nav.desiredVelocity);
                angle = 0.0f;
            }
        }

        // now that speed & angle are calculated, we can have damping applied to them and passed into the animator controller
        m_animSetup.Setup(speed, angle);
    }

    float FindAngleToTurn(Vector3 fromVector, Vector3 toVector, Vector3 upVector) {
        // this is here so that it won't give errors when the desired velocity & direction is zero.
        if (toVector == Vector3.zero)
            return 0.0f;

        // find Absolute value of angle
        float angle = Vector3.Angle(fromVector, toVector);

        // gets cross product of 2 vectors to know if to turn right or left
        Vector3 normal = Vector3.Cross(fromVector, toVector);

        // gets dot product of 2 vectors: the normal & the upVector.
        // if they are the same direction (upwards), then the result is bigger than zero,
        // so we can multiply the angle calculated w/ the sign of the dot product.
        // negative is left of forwardVector
        // positive is right of forwardVector
        angle *= Mathf.Sign(Vector3.Dot(normal, upVector));
        angle *= Mathf.Deg2Rad;

        return angle;
    }

}
