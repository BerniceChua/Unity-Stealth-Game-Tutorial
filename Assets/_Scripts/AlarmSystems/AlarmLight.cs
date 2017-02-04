using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmLight : MonoBehaviour {
    [SerializeField] float m_fadeSpeed = 2.0f;
    [SerializeField] float m_highIntensity = 2.0f;
    [SerializeField] float m_lowIntensity = 0.5f;
    [SerializeField] float m_changeMargin = 0.2f;

    public bool m_alarmOn;

    private float targetIntensity;
    private Light light;

    private void Awake() {
        light = GetComponent<Light>();
        light.intensity = 0.0f;
        targetIntensity = m_highIntensity;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (m_alarmOn) {
            light.intensity = Mathf.Lerp(light.intensity, targetIntensity, m_fadeSpeed * Time.deltaTime);
            CheckTargetIntensity();
        } else {
            light.intensity = Mathf.Lerp(light.intensity, 0.0f, m_fadeSpeed * Time.deltaTime);
        }
	}

    void CheckTargetIntensity() {
        if (Mathf.Abs(targetIntensity - light.intensity) < m_changeMargin) {
            targetIntensity = targetIntensity == m_highIntensity ? m_lowIntensity : m_highIntensity;
        }

        // originally:
        //if (Mathf.Abs(targetIntensity - light.intensity) < m_changeMargin) {
        //    if (targetIntensity == m_highIntensity) {
        //        targetIntensity = m_lowIntensity;
        //    } else {
        //        targetIntensity = m_highIntensity;
        //    }
        //}
    }

}