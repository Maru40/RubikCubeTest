using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

#region パラメータ

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
    /// タイム終了時にする処理
    /// </summary>
    /// <param name="isEndAction">終了関数を呼び出すかどうか</param>
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

    #region コンストラクタ

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

    #region public関数

    /// <summary>
    /// 時間の更新
    /// </summary>
    /// <param name="countSpeed">更新時間のspeed</param>
    /// <returns></returns>
    public bool UpdateTimer(float countSpeed = 1.0f)
    {
        if (IsTimeUp)
        {  //経過時間が過ぎていたら加算をしない
            return true;
        }

        m_param.elapsedTime += Time.deltaTime * countSpeed; //経過時間のカウント

        if (IsTimeUp)
        {  //経過時間を過ぎたら
            m_param.EndTimer();
        }

        return IsTimeUp;
    }

    /// <summary>
    /// 時間経過リセット
    /// </summary>
    public void ResetTimer()
    {
        m_param.elapsedTime = 0.0f;
    }
    /// <summary>
    /// 時間経過リセット
    /// </summary>
    /// <param name="intervalTime">設定時間</param>
    public void ResetTimer(float intervalTime)
    {
        ResetTimer(intervalTime, null);
    }
    /// <summary>
    /// 時間経過リセット
    /// </summary>
    /// <param name="intervalTime">設定時間</param>
    /// <param name="endAction">終了後に呼んで欲しい関数</param>
    public void ResetTimer(float intervalTime, Action endAction)
    {
        m_param.intervalTime = intervalTime;
        m_param.endAction = endAction;
        m_param.elapsedTime = 0.0f;
    }

    /// <summary>
    /// 時間経過強制終了
    /// </summary>
    public void AbsoluteEndTimer(bool isEndAction)
    {
        m_param.EndTimer(isEndAction);
    }

    #endregion

    #region アクセッサ・プロパティ

    /// <summary>
    /// 経過時間を超えたかどうか
    /// </summary>
    public bool IsTimeUp
    {
        get => m_param.intervalTime <= m_param.elapsedTime;
    }

    /// <summary>
    /// 経過時間 / 設定時間 == 経過時間の割合
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
    /// 1.0f - ( 経過時間 / 設定時間 )
    /// </summary>
    public float IntervalTimeRate
    {
        get => 1.0f - TimeRate;
    }

    /// <summary>
    /// 残り時間
    /// </summary>
    public float LeftTime
    {
        get => m_param.intervalTime - m_param.elapsedTime;
    }

    /// <summary>
    /// 経過時間
    /// </summary>
    public float ElapsedTime => m_param.elapsedTime;

    /// <summary>
    /// 設定時間
    /// </summary>
    public float IntervalTime => m_param.intervalTime;

    #endregion

}