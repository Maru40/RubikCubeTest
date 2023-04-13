
/*!
@file GameTimer.h
@brief GameTimer�N���X
�S���F�ێR�T��
*/

#pragma once
#include "stdafx.h"

namespace basecross {

	//--------------------------------------------------------------------------------------
	///	�O���錾
	//--------------------------------------------------------------------------------------
	namespace maru {
		enum class DeltaType;
	}

	//--------------------------------------------------------------------------------------
	///	�Q�[���^�C���Ǘ��N���X
	//--------------------------------------------------------------------------------------
	class GameTimer {
	public:
		/// <summary>
		/// �p�����[�^
		/// </summary>
		struct Parametor {
			float intervalTime = 0.0f;		//�ݒ莞��
			float elapsedTime = 0.0f;		//�o�ߎ���
			std::function<void()> endFunc;	//���Ԍo�ߌ�ɌĂяo���C�x���g

			/// <summary>
			/// �R���X�g���N�^
			/// </summary>
			/// <param name="intervalTime">�ݒ莞��</param>
			/// <param name="func">���Ԍo�ߌ�ɌĂяo���C�x���g</param>
			Parametor(float intervalTime, const std::function<void()>& func);

			/// <summary>
			/// ���Ԍo�ߌ�ɌĂяo������
			/// </summary>
			/// <param name="isEndFunc">�I���C�x���g���Ăяo�����ǂ���</param>
			void EndTimer(bool isEndFunc = true);
		};

	private:
		Parametor m_param;	//�p�����[�^

	public:
		/// <summary>
		/// �R���X�g���N�^
		/// </summary>
		/// <param name="intervalTime">�ݒ莞��</param>
		/// <param name="func">���Ԍo�ߌ�ɌĂяo���C�x���g</param>
		GameTimer(const float& intervalTime = 0.0f, const std::function<void()>& func = nullptr);

		/// <summary>
		/// �R���X�g���N�^
		/// </summary>
		/// <param name="param">�p�����[�^</param>
		GameTimer(const Parametor& param = Parametor(0.0f, nullptr));

		/// <summary>
		/// �f�X�g���N�^
		/// </summary>
		~GameTimer();

		/// <summary>
		/// ���Ԃ̍X�V
		/// </summary>
		/// <param name="countSpeed">�X�V���Ԃ�speed</param>
		/// <returns>Update�I������true��Ԃ�</returns>
		bool UpdateTimer(const float& countSpeed = 1.0f, const maru::DeltaType& deltaType = maru::DeltaType(0));

		/// <summary>
		/// �o�ߎ��ԃ��Z�b�g
		/// </summary>
		void ResetTimer() {
			ResetTimer(0.0f);
		}

		/// <summary>
		/// �o�ߎ��ԃ��Z�b�g
		/// </summary>
		/// <param name="intervalTime">�ݒ莞��</param>
		void ResetTimer(const float& intervalTime) {
			ResetTimer(intervalTime, nullptr);
		}

		/// <summary>
		/// �o�ߎ��ԃ��Z�b�g
		/// </summary>
		/// <param name="intervalTime">�ݒ莞��</param>
		/// <param name="func">�I�����ɌĂяo�������C�x���g</param>
		void ResetTimer(const float& intervalTime, const std::function<void()>& func) {
			m_param.intervalTime = intervalTime;
			m_param.endFunc = func;
			m_param.elapsedTime = 0.0f;

			if (IsTimeUp()) {
				m_param.EndTimer();
			}
		}

		/// <summary>
		/// �����I��
		/// </summary>
		/// <param name="isEndFunc">�I�����ɌĂяo���C�x���g���Ăяo�����ǂ���</param>
		void ForceEndTimer(const bool isEndFunc) {
			m_param.EndTimer(isEndFunc);
		}

		//--------------------------------------------------------------------------------------
		///	�A�N�Z�b�T
		//--------------------------------------------------------------------------------------

		/// <summary>
		/// �o�ߎ��Ԃ𒴂������ǂ���
		/// </summary>
		/// <returns>�o�ߎ��Ԃ𒴂�����true</returns>
		bool IsTimeUp() const {
			return m_param.intervalTime <= m_param.elapsedTime;
		}

		/// <summary>
		/// �o�ߎ��� / �ݒ莞�� == �o�ߎ��Ԃ̊���
		/// </summary>
		/// <returns>�o�ߎ��� / �ݒ莞�� == �o�ߎ��Ԃ̊���</returns>
		float GetTimeRate() const {
			if (IsTimeUp()) {
				return 1.0f;
			}

			return m_param.elapsedTime / m_param.intervalTime;
		}

		/// <summary>
		/// 1.0f - ( �o�ߎ��� / �ݒ莞�� )
		/// </summary>
		/// <returns>1.0f - ( �o�ߎ��� / �ݒ莞�� )</returns>
		float GetIntervalTimeRate() const {
			return 1.0f - GetTimeRate();
		}

		/// <summary>
		/// �c�莞��
		/// </summary>
		/// <returns>�c�莞��</returns>
		float GetLeftTime() const {
			return m_param.intervalTime - m_param.elapsedTime;
		}

		/// <summary>
		/// �o�ߎ���
		/// </summary>
		/// <returns>�o�ߎ���</returns>
		float GetElapsedTime() const noexcept {
			return m_param.elapsedTime;
		}

		/// <summary>
		/// �ݒ莞��
		/// </summary>
		/// <returns>�ݒ莞��</returns>
		float GetIntervalTime() const noexcept {
			return m_param.intervalTime;
		}
	};

}

//endbasecross