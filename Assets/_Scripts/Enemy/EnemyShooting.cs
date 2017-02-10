using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour {
    // damage scales with proximity

    [SerializeField] float m_maxDamage = 120.0f;
    [SerializeField] float m_minDamage = 45.0f;
    [SerializeField] AudioClip m_shotClip;
    [SerializeField] float m_flashIntensity = 3.0f;
    [SerializeField] float m_fadeSpeed = 10.0f;

    private Animator m_anim;
    private HashIDs m_hash;
    private LineRenderer m_laserShotLine;
    private Light m_laserShotLight;
    private SphereCollider m_sphereCollider;
    private Transform m_player;
    private PlayerHealth m_playerHealth;
    private bool m_shooting;
    private float m_scaledDamage;

    // Use this for initialization
    void Awake () {
        m_anim = GetComponent<Animator>();
        m_laserShotLine = GetComponentInChildren<LineRenderer>();
        //m_laserShotLight = m_laserShotLine.gameObject.light; // this version is obsolete, so it was replaced with the line of code below
        m_laserShotLight = m_laserShotLine.GetComponent<Light>();
        m_sphereCollider = GetComponent<SphereCollider>();
        m_player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        m_playerHealth = m_player.GetComponent<PlayerHealth>();
        m_hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

        m_laserShotLine.enabled = false;
        m_laserShotLight.intensity = 0.0f;

        // this scales the damage
        m_scaledDamage = m_maxDamage - m_minDamage;
    }

    // Update is called once per frame
    void Update() {
        float shot = m_anim.GetFloat(m_hash.shotFloat);

        if (shot > 0.5f && !m_shooting)
            Shoot();

        if (shot < 0.5f) {
            m_shooting = false;
            m_laserShotLine.enabled = false;
        }

        m_laserShotLight.intensity = Mathf.Lerp(m_laserShotLight.intensity, 0.0f, m_fadeSpeed * Time.deltaTime);
    }

    // this makes the enemy point the gun at the player when shooting.
    // it uses Inverse Kinematics.
    private void OnAnimatorIK(int layerIndex) {
        // stores the value of the aim weight curve
        float aimWeight = m_anim.GetFloat(m_hash.aimWeightFloat);

        // sets IK position of animator
        // 1st parameter is an enumeration called "Avatar IK Goal".
        // an enumeration is a collection of named constants.
        // 2nd parameter is where the animation should be aiming at.
        m_anim.SetIKPosition(AvatarIKGoal.RightHand, m_player.position + Vector3.up * 1.5f);

        // sets weight for IK
        m_anim.SetIKPositionWeight(AvatarIKGoal.RightHand, aimWeight);
    }

    void Shoot() {
        m_shooting = true;

        // max distance that enemy can shoot is radius of sphere collider.
        // this calculates the distance of the player from the enemy as a fraction of m_sphereCollider's radius.
        // max value (farthest) of fractionalDistance is 0, and when the player is nearest the value is 1.
        // we're getting the fractional distance instead of the actual distance so we can scale the damage.
        float fractionalDistance = (m_sphereCollider.radius - Vector3.Distance(transform.position, m_player.position)) / m_sphereCollider.radius;
        float damage = m_scaledDamage * fractionalDistance + m_minDamage;

        m_playerHealth.TakeDamage(damage);

        ShotEffects();
    }

    void ShotEffects() {
        // the m_laserShotLine should begin at end of the gun barrel, and end at where the player is when it hits the player.
        m_laserShotLine.SetPosition(0, m_laserShotLine.transform.position);
        m_laserShotLine.SetPosition(1, m_player.position + Vector3.up * 1.5f);
        m_laserShotLine.enabled = true;

        m_laserShotLight.intensity = m_flashIntensity;

        AudioSource.PlayClipAtPoint(m_shotClip, m_laserShotLight.transform.position);
    }
}
