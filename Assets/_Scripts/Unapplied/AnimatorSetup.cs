using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSetup {
    // Damping will make the animations not so jerky when moving. That's why we have this helper class.

    public float speedDampTime = 0.1f;
    public float angularDampTime = 0.7f;
    public float angleResponseTime = 0.6f;

    private Animator anim;
    private HashIDs hash;

    // the constructor
    public AnimatorSetup(Animator animator, HashIDs hashIDs) {
        anim = animator;
        hash = hashIDs;
    }

    public void Setup(float speed, float angle) {
        float angularSpeed = angle / angleResponseTime;

        anim.SetFloat(hash.speedFloat, speed, speedDampTime, Time.deltaTime);
        anim.SetFloat(hash.angularSpeedFloat, angularSpeed, angularDampTime, Time.deltaTime);
    }
}