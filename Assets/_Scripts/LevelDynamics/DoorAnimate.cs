using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimate : MonoBehaviour {
    [SerializeField] bool m_requireKey;
    [SerializeField] AudioClip m_doorSwishAudioClip;
    [SerializeField] AudioClip m_accessDeniedAudioClip;

    private Animator m_anim;
    private HashIDs m_hash;
    private GameObject m_player;
    private PlayerInventory m_playerInventory;
    private int m_countColliders;

    private AudioSource m_audioClipDoorAccessDenied;
    private AudioSource m_audioClipDoorOpenSwish;

    private void Awake() {
        m_anim = GetComponent<Animator>();
        m_hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        m_player = GameObject.FindGameObjectWithTag(Tags.player);
        m_playerInventory = m_player.GetComponent<PlayerInventory>();

        m_audioClipDoorAccessDenied = GetComponent<AudioSource>();
        m_audioClipDoorOpenSwish = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == m_player) {
            if (m_requireKey) {
                if (m_playerInventory.m_hasKey) {
                    m_countColliders++;
                } else {
                    m_audioClipDoorAccessDenied.clip = m_accessDeniedAudioClip;
                    m_audioClipDoorAccessDenied.Play();
                }
            } else {
                m_countColliders++;
            }
        } else if (other.gameObject.tag == Tags.enemy) {
            if (other is CapsuleCollider) {
                m_countColliders++;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject == m_player || (other.gameObject.tag == Tags.enemy && other is CapsuleCollider) ) {
            // this Mathf.Max will prevent our count colliders to be lower than zero.
            m_countColliders = Mathf.Max(0, m_countColliders - 1);
        }
    }

    // Update is called once per frame
	void Update () {
        m_anim.SetBool(m_hash.openBool, m_countColliders > 0);

        if (m_anim.IsInTransition(0) && !GetComponent<AudioSource>().isPlaying) {
            m_audioClipDoorOpenSwish.clip = m_doorSwishAudioClip;
            m_audioClipDoorOpenSwish.Play();
        }
	}
}
