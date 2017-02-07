using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastPlayerSighting : MonoBehaviour {
    public Vector3 m_position = new Vector3(1000.0f, 1000.0f, 1000.0f);
    public Vector3 m_resetPosition = new Vector3(1000.0f, 1000.0f, 1000.0f);
    // the 2 above are arbitrary positions, but they are just positions that the player cannot get to.

    [SerializeField] float m_lightHighIntensity = 0.25f;
    [SerializeField] float m_lightLowIntensity = 0.0f;
    [SerializeField] float m_fadeSpeed = 7.0f;
    [SerializeField] float m_musicFadeSpeed = 1.0f;

    private AlarmLight m_alarm;
    private Light m_mainLight;
    private AudioSource m_audio;
    private AudioSource m_panicAudio;
    private AudioSource[] m_sirens;

    private void Awake() {
        m_audio = GetComponent<AudioSource>();

        m_alarm = GameObject.FindGameObjectWithTag(Tags.alarm).GetComponent<AlarmLight>();
        m_mainLight = GameObject.FindGameObjectWithTag(Tags.mainLight).GetComponent<Light>();
        m_panicAudio = transform.Find("secondaryMusicGameObject").GetComponent<AudioSource>();
        GameObject[] sirenGameObjects = GameObject.FindGameObjectsWithTag(Tags.siren);
        m_sirens = new AudioSource[sirenGameObjects.Length];

        for (int i = 0; i < m_sirens.Length; i++) {
            m_sirens[i] = sirenGameObjects[i].GetComponent<AudioSource>();
        }
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        SwitchAlarms();
        MusicFading();
    }

    void SwitchAlarms() {
        m_alarm.m_alarmOn = (m_position != m_resetPosition);

        float newIntensity;
        //if (m_position != m_resetPosition) {
        //    newIntensity = m_lightLowIntensity;
        //}
        //else {
        //    newIntensity = m_lightHighIntensity;
        //}
        newIntensity = m_position != m_resetPosition ? m_lightLowIntensity : m_lightHighIntensity;

        m_mainLight.intensity = Mathf.Lerp(m_mainLight.intensity, newIntensity, m_fadeSpeed * Time.deltaTime);

        for (int i = 0; i < m_sirens.Length; i++) {
            if (m_position != m_resetPosition && !m_sirens[i].isPlaying) {
                m_sirens[i].Play();
            } else if (m_position == m_resetPosition) {
                m_sirens[i].Stop();
            }
        }
    }

    // handles music fading in and out
    void MusicFading() {
        if (m_position != m_resetPosition) {
            m_audio.volume = Mathf.Lerp(m_audio.volume, 0.0f, m_musicFadeSpeed * Time.deltaTime);
            m_panicAudio.volume = Mathf.Lerp(m_panicAudio.volume, 0.8f, m_musicFadeSpeed * Time.deltaTime);
        } else {
            m_audio.volume = Mathf.Lerp(m_audio.volume, 0.8f, m_musicFadeSpeed * Time.deltaTime);
            m_panicAudio.volume = Mathf.Lerp(m_panicAudio.volume, 0.0f, m_musicFadeSpeed * Time.deltaTime);
        }
    }

}