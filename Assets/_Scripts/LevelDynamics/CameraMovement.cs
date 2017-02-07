using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    [SerializeField] float m_smooth = 1.5f;

    private Transform m_player;
    private Vector3 m_relCameraPos;
    private float m_relCameraPosMagnitude;
    private Vector3 m_newPos;

    private void Awake() {
        m_player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        m_relCameraPos = transform.position - m_player.position;
        m_relCameraPosMagnitude = m_relCameraPos.magnitude - 0.5f;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // using FixedUpdated instead of regular Update because even though camera is NOT a physics object, it is trying to follow a physics object.
    void FixedUpdate() {
        // stores the standard camera position
        Vector3 standardPos = m_player.position + m_relCameraPos;

        // store position of camera when it's looking down directly on the player
        Vector3 abovePos = m_player.position + Vector3.up * m_relCameraPosMagnitude;

        // in-between positions: points that check if camera can see the player.
        Vector3[] checkpoints = new Vector3[5];
        checkpoints[0] = standardPos;
        checkpoints[1] = Vector3.Lerp(standardPos, abovePos, 0.25f);
        checkpoints[2] = Vector3.Lerp(standardPos, abovePos, 0.50f);
        checkpoints[3] = Vector3.Lerp(standardPos, abovePos, 0.75f);
        checkpoints[4] = abovePos;

        // loops through the camera positions to see which ones works when the camera can see the player.
        for (int i = 0; i < checkpoints.Length; i++) {
            if (ViewingPositionCheck(checkpoints[i])) {
                break;
            }
        }

        transform.position = Vector3.Lerp(transform.position, m_newPos, m_smooth * Time.deltaTime);
        SmoothLookAt();
    }

    bool ViewingPositionCheck(Vector3 checkPosition) {
        RaycastHit hit;

        if (Physics.Raycast(checkPosition, m_player.position - checkPosition, out hit, m_relCameraPosMagnitude)) {
            if (hit.transform != m_player) {
                return false;
            }
        }

        m_newPos = checkPosition;
        return true;
    }

    void SmoothLookAt() {
        Vector3 relPlayerPosition = m_player.position - transform.position;
        Quaternion lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, m_smooth * Time.deltaTime);
    }

}