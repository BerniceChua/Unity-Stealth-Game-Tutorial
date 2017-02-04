using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadeInOut : MonoBehaviour {
    [SerializeField] float m_fadeSpeed = 1.5f;

    private bool m_sceneStarting = true;
    private new GUITexture guiTexture;

    void Awake() {
        guiTexture = GetComponent<GUITexture>();
        guiTexture.pixelInset = new Rect(0.0f, 0.0f, Screen.width, Screen.height);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (m_sceneStarting) { StartScene(); }
	}

    void FadeToClear() {
        guiTexture.color = Color.Lerp(guiTexture.color, Color.clear, m_fadeSpeed * Time.deltaTime);
    }

    void FadeToBlack() {
        guiTexture.color = Color.Lerp(guiTexture.color, Color.black, m_fadeSpeed * Time.deltaTime);
    }

    void StartScene() {
        FadeToClear();

        if (guiTexture.color.a <= 0.05f) {
            guiTexture.color = Color.clear;
            guiTexture.enabled = false; // optimizes so that Unity won't keep calling this function.
            m_sceneStarting = false;
        }
    }

    public void EndScene() {
        guiTexture.enabled = true;
        FadeToBlack();

        if (guiTexture.color.a >= 0.95f) {
            SceneManager.LoadScene(0);
        }
    }
}