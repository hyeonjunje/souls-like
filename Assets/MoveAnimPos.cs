using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnimPos : StateMachineBehaviour
{

    public AnimationCurve moveCurve;

    private float _moveTimer;
    private float timer;

    private Rigidbody _rigid;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _rigid = animator.GetComponent<Rigidbody>();
        _rigid.isKinematic = false;
        Keyframe moveLastFrame = moveCurve[moveCurve.length - 1];
        _moveTimer = moveLastFrame.time;
        timer = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        float speed = moveCurve.Evaluate(timer);
        Debug.Log(speed);
        Vector3 dir = animator.transform.forward * speed;
        _rigid.velocity = dir;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _rigid.velocity = Vector3.zero;
        _rigid.isKinematic = true;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
