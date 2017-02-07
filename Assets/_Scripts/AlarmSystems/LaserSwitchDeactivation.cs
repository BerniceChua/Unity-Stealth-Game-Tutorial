using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSwitchDeactivation : MonoBehaviour {
    [SerializeField] GameObject m_laser;
    [SerializeField] Material m_unlockedMaterial;
    
    private GameObject m_player;
    private AudioSource m_audioSourceDeactivateLaser;

    private void Awake() {
        m_player = GameObject.FindGameObjectWithTag(Tags.player);
        m_audioSourceDeactivateLaser = GetComponent<AudioSource>();
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject == m_player) {
            if (Input.GetButton("Switch")) {
                LaserDeactivation();
            }
        }
    }

    void LaserDeactivation() {
        m_laser.SetActive(false);

        Renderer screen = transform.Find("prop_switchUnit_screen").GetComponent<Renderer>();
        screen.material = m_unlockedMaterial;
        m_audioSourceDeactivateLaser.Play();
    }

}
