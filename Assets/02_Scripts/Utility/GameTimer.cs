using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

#region �p�����[�^

public class GameTimerParametor
{
    public float intervalTime = 0.0f;
    public float elapsedTime = 0.0f;
    public Action endAction = null;

    public GameTimerParametor(float intervalTime, Action endAction)
    {
        this.intervalTime = intervalTime;
        this.endAction = endAction;
    }

    /// <summary>
    /// �^�C���I�����ɂ��鏈��
    /// </summary>
    /// <param name="isEndAction">�I���֐����Ăяo�����ǂ���</param>
    public void EndTimer(bool isEndAction = true)
    {
        if (isEndAction)
        {
            endAction?.Invoke();
        }
        endAction = null;
    }
}

#endregion

public class GameTimer
{
    private GameTimerParametor m_param = new GameTimerParametor(0.0f, null);

    #region �R���X�g���N�^

    public GameTimer()
        : this(new GameTimerParametor(0.0f, null))
    { }

    public GameTimer(float intervalTime)
        : this(new GameTimerParametor(intervalTime, null))
    { }

    public GameTimer(float intervalTime, Action endAction)
        : this(new GameTimerParametor(intervalTime, endAction))
    { }

    public GameTimer(GameTimerParametor param)
    {
        m_param = param;
    }

    #endregion

    #region public�֐�

    /// <summary>
    /// ���Ԃ̍X�V
    /// </summary>
    /// <param name="countSpeed">�X�V���Ԃ�speed</param>
    /// <returns></returns>
    public bool UpdateTimer(float countSpeed = 1.0f)
    {
        if (IsTimeUp)
        {  //�o�ߎ��Ԃ��߂��Ă�������Z�����Ȃ�
            return true;
        }

        m_param.elapsedTime += Time.deltaTime * countSpeed; //�o�ߎ��Ԃ̃J�E���g

        if (IsTimeUp)
        {  //�o�ߎ��Ԃ��߂�����
            m_param.EndTimer();
        }

        return IsTimeUp;
    }

    /// <summary>
    /// ���Ԍo�߃��Z�b�g
    /// </summary>
    public void ResetTimer()
    {
        m_param.elapsedTime = 0.0f;
    }
    /// <summary>
    /// ���Ԍo�߃��Z�b�g
    /// </summary>
    /// <param name="intervalTime">�ݒ莞��</param>
    public void ResetTimer(float intervalTime)
    {
        ResetTimer(intervalTime, null);
    }
    /// <summary>
    /// ���Ԍo�߃��Z�b�g
    /// </summary>
    /// <param name="intervalTime">�ݒ莞��</param>
    /// <param name="endAction">�I����ɌĂ�ŗ~�����֐�</param>
    public void ResetTimer(float intervalTime, Action endAction)
    {
        m_param.intervalTime = intervalTime;
        m_param.endAction = endAction;
        m_param.elapsedTime = 0.0f;
    }

    /// <summary>
    /// ���Ԍo�ߋ����I��
    /// </summary>
    public void AbsoluteEndTimer(bool isEndAction)
    {
        m_param.EndTimer(isEndAction);
    }

    #endregion

    #region �A�N�Z�b�T�E�v���p�e�B

    /// <summary>
    /// �o�ߎ��Ԃ𒴂������ǂ���
    /// </summary>
    public bool IsTimeUp
    {
        get => m_param.intervalTime <= m_param.elapsedTime;
    }

    /// <summary>
    /// �o�ߎ��� / �ݒ莞�� == �o�ߎ��Ԃ̊���
    /// </summary>
    public float TimeRate
    {
        get
        {
            if (IsTimeUp)
            {
                return 1.0f;
            }

            return m_param.elapsedTime / m_param.intervalTime;
        }
    }

    /// <summary>
    /// 1.0f - ( �o�ߎ��� / �ݒ莞�� )
    /// </summary>
    public float IntervalTimeRate
    {
        get => 1.0f - TimeRate;
    }

    /// <summary>
    /// �c�莞��
    /// </summary>
    public float LeftTime
    {
        get => m_param.intervalTime - m_param.elapsedTime;
    }

    /// <summary>
    /// �o�ߎ���
    /// </summary>
    public float ElapsedTime => m_param.elapsedTime;

    /// <summary>
    /// �ݒ莞��
    /// </summary>
    public float IntervalTime => m_param.intervalTime;

    #endregion

}