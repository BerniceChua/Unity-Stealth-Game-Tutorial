using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadeInOut : MonoBehaviour {
    [SerializeField] float m_fadeSpeed = 1.5f;

    private bool m_sceneStarting = true;
    private GUITexture m_guiTexture;

    void Awake() {
        m_guiTexture = GetComponent<GUITexture>();
        m_guiTexture.pixelInset = new Rect(0.0f, 0.0f, Screen.width, Screen.height);
    }

    // Update is called once per frame
	void Update () {
		if (m_sceneStarting) { StartScene(); }
	}

    void FadeToClear() {
        m_guiTexture.color = Color.Lerp(m_guiTexture.color, Color.clear, m_fadeSpeed * Time.deltaTime);
    }

    void FadeToBlack() {
        m_guiTexture.color = Color.Lerp(m_guiTexture.color, Color.black, m_fadeSpeed * Time.deltaTime);
    }

    void StartScene() {
        FadeToClear();

        if (m_guiTexture.color.a <= 0.05f) {
            m_guiTexture.color = Color.clear;
            m_guiTexture.enabled = false; // optimizes so that Unity won't keep calling this function.
            m_sceneStarting = false;
        }
    }

    public void EndScene() {
        m_guiTexture.enabled = true;
        FadeToBlack();

        if (m_guiTexture.color.a >= 0.95f) {
            SceneManager.LoadScene(0);
        }
    }
}