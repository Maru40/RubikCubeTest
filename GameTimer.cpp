
/*!
@file GameTimer.cpp
@brief GameTimerクラス実体
担当：丸山裕喜
*/

#include "stdafx.h"
#include "Project.h"

#include "GameTimer.h"
#include "TimeHelper.h"

namespace basecross {

	//--------------------------------------------------------------------------------------
	///	パラメータ
	//--------------------------------------------------------------------------------------

	GameTimer::Parametor::Parametor(float intervalTime, const std::function<void()>& func)
		:intervalTime(intervalTime), endFunc(func)
	{}

	void GameTimer::Parametor::EndTimer(bool isEndFunc) {
		if (isEndFunc && endFunc) {
			endFunc();
		}
	}

	//--------------------------------------------------------------------------------------
	///	ゲームタイム管理クラス
	//--------------------------------------------------------------------------------------

	GameTimer::GameTimer(const float& intervalTime, const std::function<void()>& func)
		:GameTimer(Parametor(intervalTime, func))
	{}

	GameTimer::GameTimer(const Parametor& param)
		:m_param(param)
	{}

	GameTimer::~GameTimer() = default;

	bool GameTimer::UpdateTimer(const float& countSpeed, const maru::DeltaType& deltaType) {
		if (IsTimeUp()) {  //経過時間が過ぎていたら加算しない。
			return true;
		}

		auto delta = maru::TimeHelper::GetElapsedTime(deltaType);
		m_param.elapsedTime += countSpeed * delta;

		if (IsTimeUp()) {
			m_param.EndTimer();
		}

		return IsTimeUp();
	}

}

//endbasecross