using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class __GameManager : Singleton<__GameManager>
{
	[SerializeField, ReadOnly(true)]
	protected int m_Width;
	[SerializeField, ReadOnly(true)]
	protected int m_Height;

	private event Action m_OnPlayModeEnter;
	private event Action m_OnPlayModeExit;

	#region 내부 프로퍼티
	#region 매니져
	protected ResourcesManager M_Resources => ResourcesManager.Instance;
	protected __EditManager M_Edit => __EditManager.Instance;

	protected StageManager M_Stage => StageManager.Instance;
	protected PlayerManager M_Player => PlayerManager.Instance;
	protected EnemyManager M_Enemy => EnemyManager.Instance;
	protected EnemyGizmoManager M_EnemyGizmo => EnemyGizmoManager.Instance;
	protected CoinManager M_Coin => CoinManager.Instance;
	protected TileManager M_Tile => TileManager.Instance;
	protected WallManager M_Wall => WallManager.Instance;
	protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
	protected GravityZoneManager M_GravityZone => GravityZoneManager.Instance;
	protected IceZoneManager M_IceZone => IceZoneManager.Instance;
	protected FloatingTextManager M_FloatingText => FloatingTextManager.Instance;
	#endregion
	#endregion
	#region 외부 프로퍼티
	public int width { get => m_Width; }
	public int height { get => m_Height; }

	public event Action onPlayModeEnter
	{
		add
		{
			lock (this)
			{
				m_OnPlayModeEnter += value;
			}
		}
		remove
		{
			lock (this)
			{
				m_OnPlayModeEnter -= value;
			}
		}
	}
	public event Action onPlayModeExit
	{
		add
		{
			lock (this)
			{
				m_OnPlayModeExit += value;
			}
		}
		remove
		{
			lock (this)
			{
				m_OnPlayModeExit -= value;
			}
		}
	}
	#endregion
	#region 내부 함수
	void __Initialize()
	{
		M_Edit.__Initialize();

		M_Stage.__Initialize();
		M_Player.__Initialize();
		M_Enemy.__Initialize();
		M_EnemyGizmo.__Initialize();
		M_Coin.__Initialize();
		M_Tile.__Initialize();
		M_Wall.__Initialize();
		M_SafetyZone.__Initialize();
		M_GravityZone.__Initialize();
		M_IceZone.__Initialize();
		M_FloatingText.__Initialize();
	}
	void __Finalize()
	{
		M_FloatingText.__Finalize();
		M_IceZone.__Finalize();
		M_GravityZone.__Finalize();
		M_SafetyZone.__Finalize();
		M_Wall.__Finalize();
		M_Tile.__Finalize();
		M_Coin.__Finalize();
		M_EnemyGizmo.__Finalize();
		M_Enemy.__Finalize();
		M_Player.__Finalize();
		M_Stage.__Finalize();

		M_Edit.__Finalize();
		M_Resources.__Finalize();
	}

	private IEnumerator DoActionWaitForSeconds(float t, Action action)
	{
		if (t > 0f)
		{
			yield return new WaitForSeconds(t);
		}

		action?.Invoke();
	}
	#endregion
	#region 외부 함수
	public void EnterPlayMode(float t = 0f)
	{
	 	StartCoroutine(DoActionWaitForSeconds(t, m_OnPlayModeEnter));
	}
	public void ExitPlayMode(float t = 0f)
	{
		StartCoroutine(DoActionWaitForSeconds(t, m_OnPlayModeExit));
	}
	#endregion

	#region 유니티 콜백 함수
	void Awake()
	{
		__Initialize();
	}
	void OnApplicationQuit()
	{
		__Finalize();
	}
	#endregion
}