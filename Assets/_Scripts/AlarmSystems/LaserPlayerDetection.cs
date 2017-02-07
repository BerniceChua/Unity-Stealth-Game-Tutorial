using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPlayerDetection : MonoBehaviour {
    private GameObject m_player;
    private LastPlayerSighting m_lastPlayerSighting;
    private Renderer m_renderer;

    void Awake() {
        m_renderer = GetComponent<Renderer>();
        m_player = GameObject.FindGameObjectWithTag(Tags.player);
        m_lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
    }

    void OnTriggerStay(Collider other) {   // can also be "void OnTriggerEnter(Collider other) {"
        if (m_renderer.enabled) {
            if (other.gameObject == m_player) {
                m_lastPlayerSighting.m_position = m_player.transform.position;
            }
        }
    }

}
