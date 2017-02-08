using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftDoorsTracking : MonoBehaviour {
    [SerializeField] float m_doorSpeed = 7.0f;

    private Transform m_leftOuterDoor;
    private Transform m_rightOuterDoor;
    private Transform m_leftInnerDoor;
    private Transform m_rightInnerDoor;
    private float m_leftClosedPosX;
    private float m_rightClosedPosX;

    private void Awake() {
        m_leftOuterDoor = GameObject.Find("door_exitOuter_left_001").transform;
        m_rightOuterDoor = GameObject.Find("door_exitOuter_right_001").transform;
        m_leftInnerDoor = GameObject.Find("door_exitInner_left_001").transform;
        m_rightInnerDoor = GameObject.Find("door_exitInner_right_001").transform;

        m_leftClosedPosX = m_leftInnerDoor.position.x;
        m_rightClosedPosX = m_rightInnerDoor.position.x;
    }

    void MoveDoors(float newLeftXTarget, float newRightXTarget) {
        float newX = Mathf.Lerp(m_leftInnerDoor.position.x, newLeftXTarget, m_doorSpeed * Time.deltaTime);
        m_leftInnerDoor.position = new Vector3(newX, m_leftInnerDoor.position.y, m_leftInnerDoor.position.z);

        newX = Mathf.Lerp(m_rightInnerDoor.position.x, newRightXTarget, m_doorSpeed * Time.deltaTime);
        m_rightInnerDoor.position = new Vector3(newX, m_rightInnerDoor.position.y, m_rightInnerDoor.position.z);
    }

    // calls MoveDoors() w/ parameters of outer doors' x-positions so inner doors will follow them.
    public void DoorFollowing() {
        MoveDoors(m_leftOuterDoor.position.x, m_rightOuterDoor.position.x);
    }
    
    // calls MoveDoors() w/ parameters of closed X-positions for when player is inside the lift, and closes the doors to seal them inside.
    public void CloseDoors() {
        MoveDoors(m_leftClosedPosX, m_rightClosedPosX);
    }

}