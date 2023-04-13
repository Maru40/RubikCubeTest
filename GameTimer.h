
/*!
@file GameTimer.h
@brief GameTimerクラス
担当：丸山裕喜
*/

#pragma once
#include "stdafx.h"

namespace basecross {

	//--------------------------------------------------------------------------------------
	///	前方宣言
	//--------------------------------------------------------------------------------------
	namespace maru {
		enum class DeltaType;
	}

	//--------------------------------------------------------------------------------------
	///	ゲームタイム管理クラス
	//--------------------------------------------------------------------------------------
	class GameTimer {
	public:
		/// <summary>
		/// パラメータ
		/// </summary>
		struct Parametor {
			float intervalTime = 0.0f;		//設定時間
			float elapsedTime = 0.0f;		//経過時間
			std::function<void()> endFunc;	//時間経過後に呼び出すイベント

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="intervalTime">設定時間</param>
			/// <param name="func">時間経過後に呼び出すイベント</param>
			Parametor(float intervalTime, const std::function<void()>& func);

			/// <summary>
			/// 時間経過後に呼び出す処理
			/// </summary>
			/// <param name="isEndFunc">終了イベントを呼び出すかどうか</param>
			void EndTimer(bool isEndFunc = true);
		};

	private:
		Parametor m_param;	//パラメータ

	public:
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="intervalTime">設定時間</param>
		/// <param name="func">時間経過後に呼び出すイベント</param>
		GameTimer(const float& intervalTime = 0.0f, const std::function<void()>& func = nullptr);

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="param">パラメータ</param>
		GameTimer(const Parametor& param = Parametor(0.0f, nullptr));

		/// <summary>
		/// デストラクタ
		/// </summary>
		~GameTimer();

		/// <summary>
		/// 時間の更新
		/// </summary>
		/// <param name="countSpeed">更新時間のspeed</param>
		/// <returns>Update終了時にtrueを返す</returns>
		bool UpdateTimer(const float& countSpeed = 1.0f, const maru::DeltaType& deltaType = maru::DeltaType(0));

		/// <summary>
		/// 経過時間リセット
		/// </summary>
		void ResetTimer() {
			ResetTimer(0.0f);
		}

		/// <summary>
		/// 経過時間リセット
		/// </summary>
		/// <param name="intervalTime">設定時間</param>
		void ResetTimer(const float& intervalTime) {
			ResetTimer(intervalTime, nullptr);
		}

		/// <summary>
		/// 経過時間リセット
		/// </summary>
		/// <param name="intervalTime">設定時間</param>
		/// <param name="func">終了時に呼び出したいイベント</param>
		void ResetTimer(const float& intervalTime, const std::function<void()>& func) {
			m_param.intervalTime = intervalTime;
			m_param.endFunc = func;
			m_param.elapsedTime = 0.0f;

			if (IsTimeUp()) {
				m_param.EndTimer();
			}
		}

		/// <summary>
		/// 強制終了
		/// </summary>
		/// <param name="isEndFunc">終了時に呼び出すイベントを呼び出すかどうか</param>
		void ForceEndTimer(const bool isEndFunc) {
			m_param.EndTimer(isEndFunc);
		}

		//--------------------------------------------------------------------------------------
		///	アクセッサ
		//--------------------------------------------------------------------------------------

		/// <summary>
		/// 経過時間を超えたかどうか
		/// </summary>
		/// <returns>経過時間を超えたらtrue</returns>
		bool IsTimeUp() const {
			return m_param.intervalTime <= m_param.elapsedTime;
		}

		/// <summary>
		/// 経過時間 / 設定時間 == 経過時間の割合
		/// </summary>
		/// <returns>経過時間 / 設定時間 == 経過時間の割合</returns>
		float GetTimeRate() const {
			if (IsTimeUp()) {
				return 1.0f;
			}

			return m_param.elapsedTime / m_param.intervalTime;
		}

		/// <summary>
		/// 1.0f - ( 経過時間 / 設定時間 )
		/// </summary>
		/// <returns>1.0f - ( 経過時間 / 設定時間 )</returns>
		float GetIntervalTimeRate() const {
			return 1.0f - GetTimeRate();
		}

		/// <summary>
		/// 残り時間
		/// </summary>
		/// <returns>残り時間</returns>
		float GetLeftTime() const {
			return m_param.intervalTime - m_param.elapsedTime;
		}

		/// <summary>
		/// 経過時間
		/// </summary>
		/// <returns>経過時間</returns>
		float GetElapsedTime() const noexcept {
			return m_param.elapsedTime;
		}

		/// <summary>
		/// 設定時間
		/// </summary>
		/// <returns>設定時間</returns>
		float GetIntervalTime() const noexcept {
			return m_param.intervalTime;
		}
	};

}

//endbasecross