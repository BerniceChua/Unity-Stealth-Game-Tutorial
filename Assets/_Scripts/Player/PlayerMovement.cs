using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public AudioClip m_shoutingClip;
    public float m_turnSmoothing = 15.0f;
    public float m_speedDampTime = 0.1f;

    private Animator m_anim;
    private HashIDs m_hash;

    private Rigidbody m_rigidbody;
    private AudioSource m_audio;

    // Use this for initialization
    void Start () {
        m_anim = GetComponent<Animator>();
        m_hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

        m_rigidbody = GetComponent<Rigidbody>();
        m_audio = GetComponent<AudioSource>();

        // the weight of a layer is its effectiveness of overriding the layers beneath it.
        m_anim.SetLayerWeight(1, 1.0f);
	}
	
	// Player is a Physics object, so we use FixedUpdate instead of regular Update.
	void FixedUpdate () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool sneak = Input.GetButton("Sneak");

        MovementManager(h, v, sneak);

	}

    private void Update() {
        bool shout = Input.GetButtonDown("Attract");
        m_anim.SetBool(m_hash.shoutingBool, shout);
        AudioManagement(shout);
    }

    void MovementManager(float horizontal, float vertical, bool sneaking) {
        m_anim.SetBool(m_hash.sneakingBool, sneaking);

        if (horizontal != 0.0f || vertical != 0.0f) {
            Rotating(horizontal, vertical);
            m_anim.SetFloat(m_hash.speedFloat, 5.5f, m_speedDampTime, Time.deltaTime);
        } else {
            m_anim.SetFloat(m_hash.speedFloat, 0.0f);
        }
    }

    void Rotating(float horizontal, float vertical) {
        Vector3 targetDirection = new Vector3(horizontal, 0.0f, vertical);
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        Quaternion newRotation = Quaternion.Lerp(m_rigidbody.rotation, targetRotation, m_turnSmoothing * Time.deltaTime);
        m_rigidbody.MoveRotation(newRotation);
    }

    void AudioManagement(bool shout) {
        // plays the footsteps sounds
        //if (m_anim.GetCurrentAnimatorStateInfo(0).nameHash == m_hash.locomotionState) { // nameHash is obsolete, so it was replaced
        if (m_anim.GetCurrentAnimatorStateInfo(0).fullPathHash == m_hash.locomotionState) {
            
            // this is inside its own if-statement, because we are making sure that the sound does not play over itself.
            if (!m_audio.isPlaying) {
                m_audio.Play();
            }
        } else {
            m_audio.Stop();
        }

        if (shout) {
            AudioSource.PlayClipAtPoint(m_shoutingClip, transform.position);
        }
    }

}
