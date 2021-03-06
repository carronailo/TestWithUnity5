﻿using UnityEngine;

[ExecuteInEditMode]
public class RealtimeHumanoidPosing : MonoBehaviour
{
    private GameObject CHARACTER;
    private GameObject HIP;
    private GameObject SPINE;
    private GameObject SHOULDER_CENTER;
    private GameObject HEAD;
    private GameObject SHOULDER_LEFT;
    private GameObject ELBOW_LEFT;
    private GameObject WRIST_LEFT;
    private GameObject HAND_LEFT;
    private GameObject SHOULDER_RIGHT;
    private GameObject ELBOW_RIGHT;
    private GameObject WRIST_RIGHT;
    private GameObject HAND_RIGHT;
    private GameObject HIP_LEFT;
    private GameObject KNEE_LEFT;
    private GameObject ANKLE_LEFT;
    private GameObject FOOT_LEFT;
    private GameObject HIP_RIGHT;
    private GameObject KNEE_RIGHT;
    private GameObject ANKLE_RIGHT;
    private GameObject FOOT_RIGHT;

    private GameObject[] joints;

    private Vector3 startingPosition;

    public void Awake()
    {
        CHARACTER = this.gameObject;

        if (CHARACTER != null)
        {
            HIP = CHARACTER.transform.Find("HIP").gameObject;

            HIP_LEFT = HIP.transform.Find("HIP_LEFT").gameObject;
            KNEE_LEFT = HIP_LEFT.transform.Find("KNEE_LEFT").gameObject;
            ANKLE_LEFT = KNEE_LEFT.transform.Find("ANKLE_LEFT").gameObject;
            FOOT_LEFT = ANKLE_LEFT.transform.Find("FOOT_LEFT").gameObject;

            HIP_RIGHT = HIP.transform.Find("HIP_RIGHT").gameObject;
            KNEE_RIGHT = HIP_RIGHT.transform.Find("KNEE_RIGHT").gameObject;
            ANKLE_RIGHT = KNEE_RIGHT.transform.Find("ANKLE_RIGHT").gameObject;
            FOOT_RIGHT = ANKLE_RIGHT.transform.Find("FOOT_RIGHT").gameObject;

            SPINE = HIP.transform.Find("SPINE").gameObject;
            SHOULDER_CENTER = SPINE.transform.Find("SHOULDER_CENTER").gameObject;
            HEAD = SHOULDER_CENTER.transform.Find("HEAD").gameObject;

            SHOULDER_LEFT = SHOULDER_CENTER.transform.Find("SHOULDER_LEFT").gameObject;
            ELBOW_LEFT = SHOULDER_LEFT.transform.Find("ELBOW_LEFT").gameObject;
            WRIST_LEFT = ELBOW_LEFT.transform.Find("WRIST_LEFT").gameObject;
            HAND_LEFT = WRIST_LEFT.transform.Find("HAND_LEFT").gameObject;

            SHOULDER_RIGHT = SHOULDER_CENTER.transform.Find("SHOULDER_RIGHT").gameObject;
            ELBOW_RIGHT = SHOULDER_RIGHT.transform.Find("ELBOW_RIGHT").gameObject;
            WRIST_RIGHT = ELBOW_RIGHT.transform.Find("WRIST_RIGHT").gameObject;
            HAND_RIGHT = WRIST_RIGHT.transform.Find("HAND_RIGHT").gameObject;
        }

        joints = new GameObject[25] {
			null, HIP, SPINE, SHOULDER_CENTER,
			null, SHOULDER_LEFT, ELBOW_LEFT, WRIST_LEFT,
			null, SHOULDER_RIGHT, ELBOW_RIGHT, WRIST_RIGHT,
			HIP, HIP_LEFT, KNEE_LEFT, ANKLE_LEFT,
			null, HIP_RIGHT, KNEE_RIGHT, ANKLE_RIGHT,
			HEAD, HAND_LEFT, HAND_RIGHT, FOOT_LEFT, FOOT_RIGHT};
    }

    public void SetWorldPosition(Vector3 position)
    {
        if (startingPosition == Vector3.zero)
        {
            startingPosition = position - CHARACTER.transform.position;
        }
        CHARACTER.transform.position = position - startingPosition;
    }

    public void SetRotations(Quaternion[] rotations)
    {
        for (int i = 0; i < rotations.Length; i++)
        {
            if (joints[i] != null)
            {
                joints[i].transform.localRotation = rotations[i];
            }
        }
    }
}

