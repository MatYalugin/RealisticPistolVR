using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public Animator animator;
    private bool isDown;
    public float delayToTargetUp = 5f;
    public void TargetDown()
    {
        if (!isDown)
        {
            animator.Play("TargetPlateDown");
            isDown = true;
            Invoke("TargetUp", delayToTargetUp);
        }
    }
    public void TargetUp()
    {
        animator.Play("TargetPlateUp");
        isDown = false;
    }
}
