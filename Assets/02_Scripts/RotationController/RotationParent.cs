using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;

public class RotationParent : MonoBehaviour
{
    [SerializeField]
    private float mTime = 1.0f;

    [SerializeField]
    private float mAngle = 90.0f;

    [SerializeField]
    private Vector3 mRotationEuler = Vector3.up;

    private GameTimer mGameTimer = new GameTimer();

    private void Update()
    {
        mGameTimer.UpdateTimer();

        if (IsRotation()) {
            RotationUpdate();
        }
    }

    public void StartRotation(Vector3 axis)
    {
        if (IsRotation()) {
            return;
        }

        mRotationEuler = axis;
        mGameTimer.ResetTimer(mTime);
    }

    private void RotationUpdate()
    {
        transform.Rotate(mRotationEuler * mTime * mAngle * Time.deltaTime);
    }

    public bool IsRotation()
    {
        return !mGameTimer.IsTimeUp;
    }

}
