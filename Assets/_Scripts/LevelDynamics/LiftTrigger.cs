using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftTrigger : MonoBehaviour {
    [SerializeField] float m_timeToDoorsClose = 2.0f;
    [SerializeField] float m_timeToLiftStart = 3.0f;
    [SerializeField] float m_timeToEndLevel = 6.0f;
    [SerializeField] float m_liftSpeed = 3.0f;

    private GameObject m_player;
    private Animator m_playerAnimator;  // this is here so we can stop the player from moving when the lift moves.
    private HashIDs m_hash; // this is here so we can get the tags from the animator more easily.
    private CameraMovement m_cameraMovement;  // so we can disable when the lift moves.
    private SceneFadeInOut m_sceneFadeInOut;
    private LiftDoorsTracking m_liftDoorsTracking;
    private bool m_playerInLift;
    private float m_timer;

    private AudioSource m_elevatorSound;

    private void Awake() {
        m_player = GameObject.FindGameObjectWithTag(Tags.player);
        m_playerAnimator = m_player.GetComponent<Animator>();
        m_hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        m_cameraMovement = Camera.main.gameObject.GetComponent<CameraMovement>();
        m_sceneFadeInOut = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<SceneFadeInOut>();
        m_liftDoorsTracking = GetComponent<LiftDoorsTracking>();

        m_elevatorSound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == m_player) {
            m_playerInLift = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject == m_player) {
            m_playerInLift = false;
            m_timer = 0.0f;
        }
    }

    // Update is called once per frame
	void Update () {
        if (m_playerInLift)
            LiftActivation();

        if (m_timer <= m_timeToDoorsClose) {
            m_liftDoorsTracking.DoorFollowing();
        } else {
            m_liftDoorsTracking.CloseDoors();
        }

	}

    // LiftActivation() will be called every frame once the player is in the lift ^_^
    void LiftActivation() {
        m_timer += Time.deltaTime;

        // this if-statement will be called once the lift starts moving.
        if (m_timer >= m_timeToLiftStart) {
            m_playerAnimator.SetFloat(m_hash.speedFloat, 0.0f);
            m_cameraMovement.enabled = false;
            m_player.transform.parent = transform; // this is so the player character will move upward with the lift.

            transform.Translate(Vector3.up * m_liftSpeed * Time.deltaTime);

            if (!m_elevatorSound.isPlaying)
                m_elevatorSound.Play();

            if (m_timer >= m_timeToEndLevel)
                m_sceneFadeInOut.EndScene();
        }
    }

}