using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SafetyZoneAnimator : MonoBehaviour
{
	protected SafetyZone m_SafetyZone;

	#region 내부 컴포넌트
	protected Animator m_Animator;
	protected TextMeshPro m_Text;
	#endregion
	#region 내부 프로퍼티
	#region 매니져
	protected __GameManager M_Game => __GameManager.Instance;
	#endregion
	#endregion
	#region 외부 프로퍼티
	public TextMeshPro text => m_Text;
	#endregion
	#region 외부 함수
	public void __Initialize(SafetyZone safetyZone)
	{
		m_SafetyZone = safetyZone;

		M_Game.onPlayModeEnter += OnPlayEnter;
		M_Game.onPlayModeExit += OnPlayExit;

		if (null == m_Animator)
		{
			m_Animator = GetComponent<Animator>();
		}
		if (null == m_Text)
		{
			m_Text = transform.GetComponentInChildren<TextMeshPro>();
		}
		m_Text.text = m_SafetyZone.safetyZoneCount.ToString();
	}
	#endregion
	#region 이벤트 함수
	public void OnPlayEnter()
	{
		m_Animator.SetTrigger("GameStart");
	}
	public void OnPlayExit()
	{
		m_Animator.SetTrigger("GameEnd");
	}
	#endregion
}
