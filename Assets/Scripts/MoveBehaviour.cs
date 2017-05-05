using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBehaviour : StateMachineBehaviour
{
	public float runSpeed = 6f;
	public float walkSpeed = 2f;

	private int speedParameterHash = 0;
	private int turnParameterHash = 0;

	private void OnEnable()
	{
		speedParameterHash = Animator.StringToHash("Speed");
		turnParameterHash = Animator.StringToHash("Turn");
	}

	private void OnDisable()
	{
	}

	public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
	{
		LogConsole.Log("OnStateMachineEnter");
	}

	public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
	{
		LogConsole.Log("OnStateMachineExit");
	}

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		LogConsole.Log("OnStateEnter");
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		float moveSpeed = 0f;
		float runFactor = animator.GetFloat(speedParameterHash);
		if (runFactor > 0.0001f)
		{
			if (runFactor > 0.30001)
				//moveSpeed = Mathf.Lerp(walkSpeed, runSpeed, Mathf.Min(1f, (runFactor - Constants.controllerDirectionThresholdForWalk) / (Constants.controllerDirectionThresholdForRun - Constants.controllerDirectionThresholdForWalk)));
				moveSpeed = runSpeed;
			else
				moveSpeed = walkSpeed;
		}
		else
			moveSpeed = 0f;
		if (moveSpeed > 0f)
		{
			Vector3 moveVector = Vector3.forward * moveSpeed * Time.deltaTime;
			//if (myRigidbody != null)
			//{
			//	moveVector = myTransform.TransformDirection(moveVector);
			//	myRigidbody.MovePosition(myRigidbody.position + moveVector);
			//}
			//else
			animator.transform.Translate(moveVector, Space.Self);
		}
		float turnAngle = animator.GetFloat(turnParameterHash);
		animator.transform.eulerAngles = new Vector3(0f, CameraUtil.GetMainCameraForward().y + turnAngle, 0f);

	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		LogConsole.Log("OnStateExit");
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	//{
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	//{
	//
	//}

}
