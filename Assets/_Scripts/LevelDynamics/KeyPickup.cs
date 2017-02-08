using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour {
    [SerializeField] AudioClip m_keyGrab;

    private GameObject m_player;
    private PlayerInventory m_playerInventory;

    private void Awake() {
        m_player = GameObject.FindGameObjectWithTag(Tags.player);
        m_playerInventory = m_player.GetComponent<PlayerInventory>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == m_player) {
            // using AudioSource instead of AudioClip because the keycard GameObject will be destroyed.
            // if GameObject is destroyed, it won't be able to play the sound.
            AudioSource.PlayClipAtPoint(m_keyGrab, transform.position);

            m_playerInventory.m_hasKey = true;
            Destroy(gameObject);
        }
    }

}
