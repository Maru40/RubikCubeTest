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

    private int mDirection = 1;

    private GameTimer mGameTimer = new GameTimer();

    private void Update()
    {
        mGameTimer.UpdateTimer();

        if (IsRotation()) {
            RotationUpdate();
        }
    }

    public void StartRotation(Vector3 axis, int direction)
    {
        if (IsRotation()) {
            return;
        }

        mRotationEuler = axis;
        mDirection = direction;
        mGameTimer.ResetTimer(mTime);
    }

    private void RotationUpdate()
    {
        transform.Rotate(mRotationEuler * mTime * mAngle * Time.deltaTime * mDirection);
    }

    public bool IsRotation()
    {
        return !mGameTimer.IsTimeUp;
    }

}
