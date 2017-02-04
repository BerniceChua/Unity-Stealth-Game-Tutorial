using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVPlayerDetection : MonoBehaviour {
    private GameObject m_player;
    private LastPlayerSighting m_lastPlayerSighting;

    private void Awake() {
        m_player = GameObject.FindGameObjectWithTag(Tags.player);
        m_lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
    }

    private void OnTriggerStay(Collider other) {
        // this if-statement checks if there is a wall between the source of the collider and the player.
        if (other.gameObject == m_player) {
            Vector3 relPlayerPosition = m_player.transform.position - transform.position;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, relPlayerPosition, out hit)) {
                // this inner if-statement checks if the collider of what was hit by the Raycast is the player.
                if (hit.collider.gameObject == m_player) {
                    // updates the last known position of the player.
                    m_lastPlayerSighting.m_position = m_player.transform.position;
                }
            }
        }
    }

}