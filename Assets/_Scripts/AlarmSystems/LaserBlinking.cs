using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBlinking : MonoBehaviour {
    [SerializeField] float m_timeOn;
    [SerializeField] float m_timeOff;

    private float m_timer;
    private Renderer m_renderer;
    private Light m_light;

    // Use this for initialization
    void Start() {
        m_renderer = GetComponent<Renderer>();
        m_light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update () {
        m_timer += Time.deltaTime;

        if (m_renderer.enabled && m_timer >= m_timeOn) {
            SwitchBeam();
        }

        if (!m_renderer.enabled && m_timer >= m_timeOff) {
            SwitchBeam();
        }
	}

    void SwitchBeam() {
        m_timer = 0.0f;

        m_renderer.enabled = !m_renderer.enabled;
        m_light.enabled = !m_light.enabled;
    }

}