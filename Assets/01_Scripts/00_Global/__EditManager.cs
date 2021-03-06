using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class __EditManager : Singleton<__EditManager>
{
	[SerializeField]
	protected bool m_IsDebugMode = false;

	[Header("Cursor")]
	[SerializeField, ReadOnly]
	protected E_ObjectType m_Cursor_Type;

	[SerializeField]
	protected Texture2D m_Cursor_Default;
	[SerializeField]
	protected Texture2D m_Cursor_Erase;

	[SerializeField]
	protected Texture2D m_Cursor_Player;
	[SerializeField]
	protected Texture2D m_Cursor_Enemy;
	[SerializeField]
	protected Texture2D m_Cursor_Coin;
	[SerializeField]
	protected Texture2D m_Cursor_Wall;
	[SerializeField]
	protected Texture2D m_Cursor_SafetyZone;
	[SerializeField]
	protected Texture2D m_Cursor_GravityZone;
	[SerializeField]
	protected Texture2D m_Cursor_IceZone;

	[Header("EditMode")]
	[SerializeField, ReadOnly]
	protected bool m_IsEdit;
	[SerializeField]
	protected GameObject m_Edit_Panel;

	[Header("Canvas")]
	[SerializeField]
	protected Canvas m_Canvas_BG;
	[SerializeField]
	protected Canvas m_Canvas_UI;
	protected GraphicRaycaster m_Raycaster_BG;
	protected GraphicRaycaster m_Raycaster_UI;

	[Header("Selected")]
	[SerializeField]
	protected E_ObjectType m_SelectedType;
	[SerializeField]
	protected Image m_SelectedImage;
	[SerializeField]
	protected Text m_SelectedText;
	[SerializeField]
	protected Outline m_SelectedImageOutline;
	protected int m_ClickIndex;
	protected int m_ClickedObjectSortingLayerID;
	protected IClickerableObject m_ClickedObject;
	protected List<IClickerableObject> m_CurrentClickedObjectList;
	protected List<IClickerableObject> m_LastClickedObjectList;

	[Header("PlayButton")]
	[SerializeField]
	protected Button m_PlayButton;

	[Space(10)]
	[SerializeField]
	protected Sprite m_PlayImage;
	[SerializeField]
	protected Sprite m_ResetImage;

	#region 옵션
	protected GameObject m_Current_Panel_Option;
	protected int m_MyObjectsLayerMask;

	#region 선택한 오브젝트 옵션
	[Header("Selected Object Option")]
	[SerializeField]
	protected GameObject m_SelectedObject_Panel_Option;
	[SerializeField]
	protected InputField m_SelectedObject_InputField_XPos;
	[SerializeField]
	protected InputField m_SelectedObject_InputField_YPos;

	[SerializeField, ReadOnly]
	protected bool m_IsClickUI;
	#endregion
	#region 적 옵션
	[Header("Enemy Option")]
	[SerializeField]
	protected GameObject m_Enemy_Panel_Option;
	[SerializeField]
	protected Dropdown m_Enemy_Dropdown_Type;
	[SerializeField]
	protected GameObject m_Enemy_Panel_Speed;
	[SerializeField]
	protected InputField m_Enemy_InputField_Speed;
	[SerializeField]
	protected GameObject m_Enemy_Panel_WayPoint;
	[SerializeField]
	protected ScrollInputFieldList m_Enemy_Scroll_WayPointList;
	[SerializeField]
	protected GameObject m_Enemy_Panel_Center;
	[SerializeField]
	protected InputField m_Enemy_InputField_CenterX;
	[SerializeField]
	protected InputField m_Enemy_InputField_CenterY;
	#endregion
	#region 벽 옵션
	[Header("Wall Option")]
	[SerializeField]
	protected GameObject m_Wall_Panel_Option;
	[SerializeField]
	protected InputField m_WallColor_InputField_Red;
	[SerializeField]
	protected Slider m_WallColor_Slider_Red;
	[SerializeField]
	protected InputField m_WallColor_InputField_Green;
	[SerializeField]
	protected Slider m_WallColor_Slider_Green;
	[SerializeField]
	protected InputField m_WallColor_InputField_Blue;
	[SerializeField]
	protected Slider m_WallColor_Slider_Blue;
	#endregion
	#region 안전구역 옵션
	[Header("SafetyZone Option")]
	[SerializeField]
	protected GameObject m_SafetyZone_Panel_Option;
	[SerializeField]
	protected CheckBox m_SafetyZone_CheckBox_FinishZone;
	#endregion
	#region 중력구역 옵션
	[Header("GravityZone Option")]
	[SerializeField]
	protected GameObject m_GravityZone_Panel_Option;
	[SerializeField]
	protected InputField m_GravityZone_InputField_GravityX;
	[SerializeField]
	protected InputField m_GravityZone_InputField_GravityY;
	#endregion
	#region 얼음구역 옵션
	[Header("IceZone Option")]
	[SerializeField]
	protected GameObject m_IceZone_Panel_Option;
	[SerializeField]
	protected InputField m_IceZone_InputField_Drag;
	#endregion
	#endregion

	#region 내부 프로퍼티
	#region 매니져
	protected ResourcesManager M_Resources => ResourcesManager.Instance;
	protected __GameManager M_Game => __GameManager.Instance;
	protected StageManager M_Stage => StageManager.Instance;
	protected FloatingTextManager M_FloatingText => FloatingTextManager.Instance;
	protected UndoRedoManager M_UndoRedo => UndoRedoManager.Instance;

	protected PlayerManager M_Player => PlayerManager.Instance;
	protected EnemyManager M_Enemy => EnemyManager.Instance;
	protected EnemyGizmoManager M_EnemyGizmo => EnemyGizmoManager.Instance;
	protected CoinManager M_Coin => CoinManager.Instance;
	protected TileManager M_Tile => TileManager.Instance;
	protected WallManager M_Wall => WallManager.Instance;
	protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
	protected GravityZoneManager M_GravityZone => GravityZoneManager.Instance;
	protected IceZoneManager M_IceZone => IceZoneManager.Instance;
	#endregion

	protected Vector3 spawnPoint_Obj
	{
		get
		{
			Vector3 mousePos = Input.mousePosition;
			Vector3 pos = (Vector2)Camera.main.ScreenToWorldPoint(mousePos);
			return pos;
		}
	}
	protected bool IsMouseDown => IsLeft || IsRight;
	protected bool IsLeft => Input.GetMouseButton((int)E_InputButton.Left);
	protected bool IsRight => Input.GetMouseButton((int)E_InputButton.Right);
	protected bool IsLeftDown => Input.GetMouseButtonDown((int)E_InputButton.Left);
	protected bool IsRightDown => Input.GetMouseButtonDown((int)E_InputButton.Right);
	protected bool IsLeftUp => Input.GetMouseButtonUp((int)E_InputButton.Left);
	protected bool IsRightUp => Input.GetMouseButtonUp((int)E_InputButton.Right);

	protected E_EnemyType enemy_Type => (E_EnemyType)m_Enemy_Dropdown_Type.value - 1;
	protected float enemy_Speed
	{
		get
		{
			float speed;
			if (!float.TryParse(m_Enemy_InputField_Speed.text, out speed))
			{
				speed = 0.01f;
			}
			return speed;
		}
	}
	protected Color wall_Color
	{
		get
		{
			Color color;
			color.r = m_WallColor_Slider_Red.value;
			color.g = m_WallColor_Slider_Green.value;
			color.b = m_WallColor_Slider_Blue.value;
			color.a = 1f;
			return color;
		}
	}
	protected Vector2 gravityZone_Gravity
	{
		get
		{
			Vector2 gravity;
			if (!float.TryParse(m_GravityZone_InputField_GravityX.text, out gravity.x))
			{
				gravity.x = MyPhysics.Physics2D.gravity.x;
			}
			if (!float.TryParse(m_GravityZone_InputField_GravityY.text, out gravity.y))
			{
				gravity.y = MyPhysics.Physics2D.gravity.y;
			}
			return gravity;
		}
	}
	protected float iceZone_Drag
	{
		get
		{
			float drag;
			if (!float.TryParse(m_IceZone_InputField_Drag.text, out drag))
			{
				drag = 0f;
			}
			return drag;
		}
	}
	#endregion
	#region 외부 프로퍼티
	public bool isEditMode => m_IsEdit;
	public bool isPlayMode => !m_IsEdit;

	public bool isInputFieldFocus
	{
		get
		{
			if (null == EventSystem.current.currentSelectedGameObject)
				return false;

			bool? isFocus = EventSystem.current.currentSelectedGameObject.GetComponent<InputField>()?.isFocused;

			if (isFocus.HasValue)
				return isFocus.Value;

			return false;
		}
	}
	public CheckBox safetyZoneFinishZone { get => m_SafetyZone_CheckBox_FinishZone; }
	#endregion
	#region 내부 함수
	protected void DrawPlayer()
	{
		UndoRedoArgs args = new UndoRedoArgs();

		#region Undo Variable
		// 이전 스폰 여부
		bool spawn = M_Player.playerActive;
		// 이전 위치
		Vector3 lastPos = M_Player.playerPos;

		// 이전 저장 가능 여부
		bool canSave = M_Stage.canSave;
		#endregion

		// 플레이어 스폰
		Player player = M_Player.SpawnPlayer();
		// 위치 설정
		player.transform.position = spawnPoint_Obj;

		M_Stage.canSave = false;

		#region Redo Variable
		// 현재 위치
		Vector3 curPos = spawnPoint_Obj;
		#endregion

		#region Undo Redo
		args.undo += () =>
		{
			player.gameObject.SetActive(spawn);
			player.transform.position = lastPos;

			M_Stage.canSave = canSave;
		};
		args.redo += () =>
		{
			player.gameObject.SetActive(true);
			player.transform.position = curPos;

			M_Stage.canSave = false;
		};

		M_UndoRedo.AddUndoRedoArgs(args);
		#endregion
	}
	protected void DrawEnemy()
	{
		UndoRedoArgs args = new UndoRedoArgs();

		#region Undo Variable
		// 이전 저장 가능 여부
		bool canSave = M_Stage.canSave;
		#endregion

		// 적 스폰
		Enemy enemy = M_Enemy.SpawnEnemy();
		// 위치 설정
		enemy.transform.position = spawnPoint_Obj;
		// 적 타입 설정
		enemy.type = enemy_Type;
		// 적 이동 속도 설정
		enemy.speed = enemy_Speed;

		M_Stage.canSave = false;

		args.objectValue = enemy.gameObject;

		#region Redo Variable
		// 현재 위치
		Vector3 curPos = spawnPoint_Obj;
		// 현재 적 타입
		E_EnemyType curType = enemy_Type;
		// 현재 적 이동 속도
		float curSpeed = enemy_Speed;
		#endregion

		#region Undo Redo
		args.undo += () =>
		{
			args.objectValue.GetComponent<Enemy>().EraseObject();

			M_Stage.canSave = canSave;
		};
		args.redo += () =>
		{
			// 적 스폰
			Enemy enemy = M_Enemy.SpawnEnemy();
			// 위치 설정
			enemy.transform.position = curPos;
			// 적 타입 설정
			enemy.type = curType;
			// 적 이동 속도 설정
			enemy.speed = curSpeed;

			M_Stage.canSave = false;

			args.objectValue = enemy.gameObject;
		};

		M_UndoRedo.AddUndoRedoArgs(args);
		#endregion
	}
	protected void DrawCoin()
	{
		UndoRedoArgs args = new UndoRedoArgs();

		#region Undo Variable
		// 이전 저장 가능 여부
		bool canSave = M_Stage.canSave;
		#endregion

		// 코인 스폰
		Coin coin = M_Coin.SpawnCoin();
		// 위치 설정
		coin.transform.position = spawnPoint_Obj;

		M_Stage.canSave = false;

		args.objectValue = coin.gameObject;

		#region Redo Variable
		// 현재 위치
		Vector3 curPos = spawnPoint_Obj;
		#endregion

		#region Undo Redo
		args.undo += () =>
		{
			args.objectValue.GetComponent<Coin>().EraseObject();

			M_Stage.canSave = canSave;
		};
		args.redo += () =>
		{
			// 코인 스폰
			Coin coin = M_Coin.SpawnCoin();
			// 위치 설정
			coin.transform.position = curPos;

			M_Stage.canSave = false;

			args.objectValue = coin.gameObject;
		};

		M_UndoRedo.AddUndoRedoArgs(args);
		#endregion
	}
	protected void Erase(GameObject obj, GameObject tile_obj)
	{
		if (null != m_ClickedObject &&
			obj?.GetComponent<IClickerableObject>() == m_ClickedObject)
		{
			Renderer renderer = m_ClickedObject?.GetRenderer();
			if (null != renderer)
			{
				renderer.sortingLayerID = m_ClickedObjectSortingLayerID;
			}

			m_ClickedObject = null;

			SetSelectedUI(E_ObjectType.Erase);
		}

		IEraserableObject eraserableObject = obj?.GetComponent<IEraserableObject>();
		if (null != eraserableObject)
		{
			eraserableObject.EraseObject();

			M_Stage.canSave = false;
		}
		IEraserableTile eraserableTile = obj?.GetComponent<IEraserableTile>();
		if (null != eraserableTile)
		{
			eraserableTile.EraseTile();

			M_Stage.canSave = false;
		}

		Tile tile = tile_obj.GetComponent<Tile>();

		if (null != tile)
		{
			tile.SetType(E_TileType.None);

			M_Stage.canSave = false;
		}
	}
	protected void DrawInEditMode()
	{
		if (IsPointerOverUIObject())
		{
			if (IsLeftDown)
			{
				m_IsClickUI = true;
			}
			return;
		}

		#region Object Raycast
		// 클릭한 위치
		Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		// 클릭한 위치에 존재하던 오브젝트
		MyPhysics.Collider2D obj = MyPhysics.Physics2D.OverlapPoint(point);
		#endregion
		#region UI Raycast
		// 레이캐스트 설정
		PointerEventData eventData = new PointerEventData(null);
		eventData.position = Input.mousePosition;
		List<RaycastResult> results = new List<RaycastResult>();

		// 레이캐스트
		m_Raycaster_BG.Raycast(eventData, results);

		if (results.Count <= 0)
			return;
		#endregion

		GameObject tile_obj = results[0].gameObject;

		#region Erase
		if (IsRight ||
			(IsLeft && m_SelectedType == E_ObjectType.Erase))
		{
			Erase(obj?.gameObject, tile_obj);
		}
		#endregion

		if (m_SelectedType == E_ObjectType.None)
			return;

		#region Click Process
		if (IsLeftDown)
		{
			m_IsClickUI = false;

			if (null == obj?.GetComponent<Wall>())
			{
				switch (m_SelectedType)
				{
					case E_ObjectType.Player:
						DrawPlayer();
						break;
					case E_ObjectType.Enemy:
						DrawEnemy();
						break;
					case E_ObjectType.Coin:
						DrawCoin();
						break;
				}
			}
		}
		#endregion
		#region Drag Process
		if (IsLeft)
		{
			switch (m_SelectedType)
			{
				case E_ObjectType.Wall:
					{
						Tile tile = tile_obj.GetComponent<Tile>();

						if (null != tile)
						{
							if (tile.SetType(E_TileType.Wall))
							{
								obj?.GetComponent<IEraserableTile>()?.EraseTile(E_ObjectType.Wall);

								Vector3 spawnPoint = (Vector2)tile.transform.position;

								// 스폰
								Wall wall = M_Wall.SpawnWall();
								// 위치 설정
								wall.transform.position = spawnPoint;
								// 초기화
								wall.__Initialize(tile);

								M_Stage.canSave = false;
							}
							else
							{
								tile.color = wall_Color;
							}
						}
					}
					break;
				case E_ObjectType.SafetyZone:
					{
						Tile tile = tile_obj.GetComponent<Tile>();

						if (null != tile)
						{
							if (tile.SetType(E_TileType.SafetyZone))
							{
								obj?.GetComponent<IEraserableTile>()?.EraseTile(E_ObjectType.SafetyZone);

								Vector3 spawnPoint = (Vector2)tile.transform.position;

								// 스폰
								SafetyZone safetyZone = M_SafetyZone.SpawnSafetyZone();
								// 위치 설정
								safetyZone.transform.position = spawnPoint;
								// 초기화
								safetyZone.__Initialize(tile);

								M_Stage.canSave = false;
							}
						}
					}
					break;
				case E_ObjectType.GravityZone:
					{
						Tile tile = tile_obj.GetComponent<Tile>();

						if (null != tile)
						{
							if (tile.SetType(E_TileType.GravityZone))
							{
								obj?.GetComponent<IEraserableTile>()?.EraseTile(E_ObjectType.GravityZone);

								Vector3 spawnPoint = (Vector2)tile.transform.position;

								// 스폰
								GravityZone gravityZone = M_GravityZone.SpawnGravityZone();
								// 위치 설정
								gravityZone.transform.position = spawnPoint;
								// 초기화
								gravityZone.__Initialize(tile);
								// 중력 설정
								gravityZone.gravity = gravityZone_Gravity;

								M_Stage.canSave = false;
							}
						}
					}
					break;
				case E_ObjectType.IceZone:
					{
						Tile tile = tile_obj.GetComponent<Tile>();

						if (null != tile)
						{
							if (tile.SetType(E_TileType.IceZone))
							{
								obj?.GetComponent<IEraserableTile>()?.EraseTile(E_ObjectType.IceZone);

								Vector3 spawnPoint = (Vector2)tile.transform.position;

								// 스폰
								IceZone iceZone = M_IceZone.SpawnIceZone();
								// 위치 설정
								iceZone.transform.position = spawnPoint;
								// 초기화
								iceZone.__Initialize(tile);
								// 저항 설정
								iceZone.friction = iceZone_Drag;

								M_Stage.canSave = false;
							}
						}
					}
					break;
			}
		}
		#endregion
	}
	protected void SelectInEditMode()
	{
		#region 예외 처리
		if (m_IsClickUI)
		{
			m_IsClickUI = false;
			return;
		}

		if (m_SelectedType != E_ObjectType.None)
			return;

		if (IsPointerOverUIObject())
			return;
		#endregion

		Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		var colliders = MyPhysics.Physics2D.OverlapPointAll(origin, m_MyObjectsLayerMask);

		#region 추후 인덱스 대신 Peek, Pop, Push로 변경
		//#region 빈 곳 클릭
		//if (colliders.Length <= 0)
		//{
		//    SpriteRenderer renderer = m_ClickedObject?.GetSpriteRenderer();
		//    if (null != renderer)
		//    {
		//        renderer.sortingLayerID = m_ClickedObjectSortingLayerID;
		//    }

		//    m_ClickedObject = null;

		//    SetSelectedUI(m_SelectedType);
		//    return;
		//}
		//#endregion

		//#region 리스트 세팅
		//if (m_LastClickedObjectList.Count > 0)
		//{
		//    m_LastClickedObjectList.Clear();
		//}
		//m_LastClickedObjectList.AddRange(m_CurrentClickedObjectList);

		//if (m_CurrentClickedObjectList.Count > 0)
		//{
		//    m_CurrentClickedObjectList.Clear();
		//}
		//foreach (var item in colliders)
		//{
		//    IClickedObject clickedObject = item.GetComponent<IClickedObject>();
		//    if (null == clickedObject)
		//        continue;

		//    m_CurrentClickedObjectList.Add(clickedObject);
		//}
		//#endregion

		//int currentCount = m_CurrentClickedObjectList.Count;
		//int lastCount = m_LastClickedObjectList.Count;

		//if (lastCount <= 0)
		//{
		//    SpriteRenderer renderer = m_ClickedObject?.GetSpriteRenderer();
		//    if (null != renderer)
		//    {
		//        renderer.sortingLayerID = m_ClickedObjectSortingLayerID;
		//    }

		//    m_ClickIndex = 1;
		//    m_ClickedObject = m_CurrentClickedObjectList[0];

		//    renderer = m_ClickedObject?.GetSpriteRenderer();
		//    if (null != renderer)
		//    {
		//        m_ClickedObjectSortingLayerID = renderer.sortingLayerID;
		//        renderer.sortingLayerID = SortingLayer.NameToID("Selected");
		//    }

		//    SetSelectedUI(m_ClickedObject.GetObjectType());
		//    return;
		//}
		#endregion

		#region 기존 코드
		#region 빈 곳 클릭
		if (colliders.Length <= 0)
		{
			Renderer renderer = m_ClickedObject?.GetRenderer();
			if (null != renderer)
			{
				renderer.sortingLayerID = m_ClickedObjectSortingLayerID;
			}

			m_ClickedObject = null;

			SetSelectedUI(m_SelectedType);
			return;
		}
		#endregion

		#region 리스트 세팅
		m_LastClickedObjectList.Clear();
		m_LastClickedObjectList.AddRange(m_CurrentClickedObjectList);

		m_CurrentClickedObjectList.Clear();
		foreach (var item in colliders)
		{
			IClickerableObject clickedObject = item.GetComponent<IClickerableObject>();
			if (null == clickedObject)
				continue;

			m_CurrentClickedObjectList.Add(clickedObject);
		}
		#endregion

		int currentCount = m_CurrentClickedObjectList.Count;
		int lastCount = m_LastClickedObjectList.Count;

		if (lastCount <= 0)
		{
			Renderer renderer = m_ClickedObject?.GetRenderer();
			if (null != renderer)
			{
				renderer.sortingLayerID = m_ClickedObjectSortingLayerID;
			}

			m_ClickIndex = 1;
			m_ClickedObject = m_CurrentClickedObjectList[0];

			renderer = m_ClickedObject?.GetRenderer();
			if (null != renderer)
			{
				m_ClickedObjectSortingLayerID = renderer.sortingLayerID;
				renderer.sortingLayerID = SortingLayer.NameToID("Selected");
			}

			SetSelectedUI(m_ClickedObject.GetObjectType());
			return;
		}

		if (m_ClickIndex >= currentCount)
			m_ClickIndex = 0;

		for (int i = 0; i < currentCount; ++i)
		{
			if (i < m_LastClickedObjectList.Count &&
				m_CurrentClickedObjectList[i] == m_LastClickedObjectList[i])
			{
				if (m_ClickIndex <= i)
				{
					Renderer renderer = m_ClickedObject?.GetRenderer();
					if (null != renderer)
					{
						renderer.sortingLayerID = m_ClickedObjectSortingLayerID;
					}

					m_ClickIndex = i + 1;
					m_ClickedObject = m_CurrentClickedObjectList[i];

					renderer = m_ClickedObject?.GetRenderer();
					if (null != renderer)
					{
						m_ClickedObjectSortingLayerID = renderer.sortingLayerID;
						renderer.sortingLayerID = SortingLayer.NameToID("Selected");
					}

					SetSelectedUI(m_ClickedObject.GetObjectType());
					break;
				}
			}
			else
			{
				Renderer renderer = m_ClickedObject?.GetRenderer();
				if (null != renderer)
				{
					renderer.sortingLayerID = m_ClickedObjectSortingLayerID;
				}

				m_ClickIndex = 1;
				m_ClickedObject = m_CurrentClickedObjectList[0];

				renderer = m_ClickedObject?.GetRenderer();
				if (null != renderer)
				{
					m_ClickedObjectSortingLayerID = renderer.sortingLayerID;
					renderer.sortingLayerID = SortingLayer.NameToID("Selected");
				}

				SetSelectedUI(m_ClickedObject.GetObjectType());
				break;
			}
		}
		#endregion
	}

	protected void ChangeCursor()
	{
		if (IsPointerOverUIObject())
		{
			SetCursorImage(E_ObjectType.None);
			return;
		}

		if (IsRight)
		{
			SetCursorImage(E_ObjectType.Erase);
			return;
		}

		if (m_Cursor_Type != m_SelectedType)
		{
			SetCursorImage(m_SelectedType);
		}
	}
	protected void SetCursorImage(E_ObjectType type)
	{
		m_Cursor_Type = type;

		switch (type)
		{
			case E_ObjectType.None:
				Cursor.SetCursor(m_Cursor_Default, Vector2.up, CursorMode.ForceSoftware);
				break;
			case E_ObjectType.Erase:
				Cursor.SetCursor(m_Cursor_Erase, new Vector2(m_Cursor_Erase.width, m_Cursor_Erase.height) * 0.5f, CursorMode.ForceSoftware);
				break;

			case E_ObjectType.Player:
				Cursor.SetCursor(m_Cursor_Player, new Vector2(m_Cursor_Player.width, m_Cursor_Player.height) * 0.5f, CursorMode.ForceSoftware);
				break;
			case E_ObjectType.Enemy:
				Cursor.SetCursor(m_Cursor_Enemy, new Vector2(m_Cursor_Enemy.width, m_Cursor_Enemy.height) * 0.5f, CursorMode.ForceSoftware);
				break;
			case E_ObjectType.Coin:
				Cursor.SetCursor(m_Cursor_Coin, new Vector2(m_Cursor_Coin.width, m_Cursor_Coin.height) * 0.5f, CursorMode.ForceSoftware);
				break;
			case E_ObjectType.Wall:
				Cursor.SetCursor(m_Cursor_Wall, new Vector2(m_Cursor_Wall.width, m_Cursor_Wall.height) * 0.5f, CursorMode.ForceSoftware);
				break;
			case E_ObjectType.SafetyZone:
				Cursor.SetCursor(m_Cursor_SafetyZone, new Vector2(m_Cursor_SafetyZone.width, m_Cursor_SafetyZone.height) * 0.5f, CursorMode.ForceSoftware);
				break;
			case E_ObjectType.GravityZone:
				Cursor.SetCursor(m_Cursor_GravityZone, new Vector2(m_Cursor_GravityZone.width, m_Cursor_GravityZone.height) * 0.5f, CursorMode.ForceSoftware);
				break;
			case E_ObjectType.IceZone:
				Cursor.SetCursor(m_Cursor_IceZone, new Vector2(m_Cursor_IceZone.width, m_Cursor_IceZone.height) * 0.5f, CursorMode.ForceSoftware);
				break;
		}
	}

	protected void ChangeSelectedType()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SetSelectedType(E_ObjectType.None);
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SetSelectedType(E_ObjectType.Player);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SetSelectedType(E_ObjectType.Enemy);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			SetSelectedType(E_ObjectType.Coin);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			SetSelectedType(E_ObjectType.Wall);
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			SetSelectedType(E_ObjectType.SafetyZone);
		}
		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			SetSelectedType(E_ObjectType.GravityZone);
		}
		if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			SetSelectedType(E_ObjectType.IceZone);
		}
	}
	protected void SetSelectedUI(E_ObjectType type)
	{
		m_SelectedText.text = "Selected:" + "\n" + type.ToString();
		m_Current_Panel_Option?.SetActive(false);
		M_EnemyGizmo.SetActiveAllGizmo(false);
		m_SelectedObject_Panel_Option.SetActive(false);

		if (null != m_ClickedObject)
		{
			if (null != m_ClickedObject.GetGameObject().GetComponent<IEraserableObject>())
			{
				Vector2 pos = m_ClickedObject.GetGameObject().transform.position;
				m_SelectedObject_InputField_XPos.text = pos.x.ToString();
				m_SelectedObject_InputField_YPos.text = pos.y.ToString();
				m_SelectedObject_Panel_Option.SetActive(true);
			}
		}

		switch (type)
		{
			case E_ObjectType.None:
				{
					m_SelectedImage.sprite = null;
					m_SelectedImage.color = Color.clear;
					m_SelectedImageOutline.enabled = false;

					m_Current_Panel_Option = null;
				}
				break;
			case E_ObjectType.Erase:
				{
					m_SelectedImage.sprite = null;
					m_SelectedImage.color = Color.clear;
					m_SelectedImageOutline.enabled = false;

					m_Current_Panel_Option = null;
				}
				break;

			case E_ObjectType.Player:
				{
					m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
					m_SelectedImage.sprite = M_Resources.GetSprites("Player", "Player")[0];
					m_SelectedImage.color = Color.white;
					m_SelectedImageOutline.enabled = false;

					m_Current_Panel_Option = null;
				}
				break;
			case E_ObjectType.Enemy:
				{
					m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
					m_SelectedImage.sprite = M_Resources.GetSprites("Enemy", "Enemy")[0];
					m_SelectedImage.color = Color.white;
					m_SelectedImageOutline.enabled = false;

					m_Current_Panel_Option = m_Enemy_Panel_Option;

					if (m_ClickedObject?.GetObjectType() == E_ObjectType.Enemy)
					{
						Enemy enemy = m_ClickedObject.GetGameObject().GetComponent<Enemy>();

						m_Enemy_Dropdown_Type.value = (int)enemy.type + 1;

						m_Enemy_Panel_Speed.SetActive(false);
						m_Enemy_InputField_Speed.text = "";
						if (enemy.type != E_EnemyType.None)
						{
							m_Enemy_Panel_Speed.SetActive(true);
							m_Enemy_InputField_Speed.text = enemy.speed.ToString();
						}

						ShowEnemyGizmo(enemy);
					}
					else
					{
						m_Enemy_Panel_WayPoint.SetActive(false);
						m_Enemy_Panel_Center.SetActive(false);
					}
				}
				break;
			case E_ObjectType.Coin:
				{
					m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
					m_SelectedImage.sprite = M_Resources.GetSprites("Coin", "Coin")[0];
					m_SelectedImage.color = Color.white;
					m_SelectedImageOutline.enabled = false;

					m_Current_Panel_Option = null;
				}
				break;
			case E_ObjectType.Wall:
				{
					m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f) - m_SelectedImageOutline.effectDistance * 2f;
					m_SelectedImage.sprite = M_Resources.GetSprites("Tile", "Tile")[0];
					m_SelectedImageOutline.enabled = true;

					m_Current_Panel_Option = m_Wall_Panel_Option;

					Color color = m_SelectedImage.color = M_Tile.wallColor;
					if (m_ClickedObject?.GetObjectType() == E_ObjectType.Wall)
					{
						Wall wall = m_ClickedObject.GetComponent<Wall>();

						if (null != wall)
							color = m_SelectedImage.color = wall.tile.color;

						m_WallColor_Slider_Red.value = color.r;
						m_WallColor_Slider_Green.value = color.g;
						m_WallColor_Slider_Blue.value = color.b;
						ColorToText();
					}
				}
				break;
			case E_ObjectType.SafetyZone:
				{
					m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f) - m_SelectedImageOutline.effectDistance * 2f;
					m_SelectedImage.sprite = M_Resources.GetSprites("Tile", "Tile")[0];
					m_SelectedImage.color = M_Tile.safetyZoneColor;
					m_SelectedImageOutline.enabled = true;

					m_Current_Panel_Option = m_SafetyZone_Panel_Option;
				}
				break;
			case E_ObjectType.GravityZone:
				{
					m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f) - m_SelectedImageOutline.effectDistance * 2f;
					m_SelectedImage.sprite = M_Resources.GetSprites("Tile", "Tile")[0];
					m_SelectedImage.color = M_Tile.gravityZoneColor;
					m_SelectedImageOutline.enabled = true;

					m_Current_Panel_Option = m_GravityZone_Panel_Option;

					if (m_ClickedObject?.GetObjectType() == E_ObjectType.GravityZone)
					{
						GravityZone gravityZone = m_ClickedObject.GetComponent<GravityZone>();

						m_GravityZone_InputField_GravityX.text = gravityZone.gravity.x.ToString();
						m_GravityZone_InputField_GravityY.text = gravityZone.gravity.y.ToString();
					}
				}
				break;
			case E_ObjectType.IceZone:
				{
					m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f) - m_SelectedImageOutline.effectDistance * 2f;
					m_SelectedImage.sprite = M_Resources.GetSprites("Tile", "Tile")[0];
					m_SelectedImage.color = M_Tile.iceZoneColor;
					m_SelectedImageOutline.enabled = true;

					m_Current_Panel_Option = m_IceZone_Panel_Option;

					if (m_ClickedObject?.GetObjectType() == E_ObjectType.IceZone)
					{
						IceZone iceZone = m_ClickedObject.GetComponent<IceZone>();

						m_IceZone_InputField_Drag.text = iceZone.friction.ToString();
					}
				}
				break;
		}

		m_Current_Panel_Option?.SetActive(true);
	}

	protected void UpdateClickedWallColor()
	{
		Wall wall = m_ClickedObject.GetComponent<Wall>();

		if (null == wall)
			return;

		wall.tile.color = M_Tile.wallColor;
	}
	#region Enemy
	protected void ShowEnemyGizmo(Enemy enemy)
	{
		M_EnemyGizmo.SetActiveAllGizmo(false);
		m_Enemy_Panel_WayPoint.SetActive(false);
		m_Enemy_Panel_Center.SetActive(false);
		enemy.center.gameObject.SetActive(false);

		switch (enemy.type)
		{
			case E_EnemyType.Linear:
			case E_EnemyType.LinearRepeat:
				{
					m_Enemy_Scroll_WayPointList.ClearOption();

					for (int i = 0; i < enemy.wayPointList.Count; ++i)
					{
						enemy.wayPointList[i].gameObject.SetActive(true);

						m_Enemy_Scroll_WayPointList.AddOption((enemy.wayPointList[i].index + 1).ToString());

						Vector2 pos = enemy.wayPointList[i].transform.position;
						m_Enemy_Scroll_WayPointList.items[i].SetInputFieldValue(0, pos.x.ToString());
						m_Enemy_Scroll_WayPointList.items[i].SetInputFieldValue(1, pos.y.ToString());
					}

					OnEnemyClampWayPointIndex();
					m_Enemy_Panel_WayPoint.SetActive(true);
				}
				break;
			case E_EnemyType.Circular:
				{
					enemy.center.gameObject.SetActive(true);
					m_Enemy_Panel_Center.SetActive(true);

					m_Enemy_InputField_CenterX.text = enemy.center.transform.position.x.ToString();
					m_Enemy_InputField_CenterY.text = enemy.center.transform.position.y.ToString();
				}
				break;
		}
	}
	#endregion
	#endregion
	#region 외부 함수
	public void __Initialize()
	{
		#region 이벤트 링크
		M_Game.onPlayModeEnter += OnPlayModeEnter;
		M_Game.onPlayModeExit += OnPlayModeExit;
		#endregion

		// 멤버 초기화
		m_IsEdit = true;
		if (null == m_Raycaster_BG)
		{
			m_Raycaster_BG = m_Canvas_BG.GetComponent<GraphicRaycaster>();
		}
		if (null == m_Raycaster_UI)
		{
			m_Raycaster_UI = m_Canvas_UI.GetComponent<GraphicRaycaster>();
		}

		if (null == m_SelectedImageOutline)
		{
			m_SelectedImageOutline = m_SelectedImage.GetComponent<Outline>();
		}
		if (null == m_CurrentClickedObjectList)
		{
			m_CurrentClickedObjectList = new List<IClickerableObject>();
		}
		if (null == m_LastClickedObjectList)
		{
			m_LastClickedObjectList = new List<IClickerableObject>();
		}

		float unit = m_Canvas_BG.referencePixelsPerUnit;
		Vector2 size = new Vector2(M_Game.width, M_Game.height) * unit;
		m_Canvas_BG.GetComponent<RectTransform>().sizeDelta = size;

		m_IsClickUI = false;

		m_MyObjectsLayerMask = 0;
		for (E_ObjectType i = E_ObjectType.Player; i < E_ObjectType.Max; ++i)
		{
			m_MyObjectsLayerMask += 1 << LayerMask.NameToLayer(i.ToString());
		}

		SetCursorImage(E_ObjectType.None);

		#region Option
		#region Enemy
		#region Type
		m_Enemy_Dropdown_Type.ClearOptions();
		List<string> enemyType_option = new List<string>();
		for (E_EnemyType i = E_EnemyType.None; i < E_EnemyType.Max; ++i)
		{
			enemyType_option.Add(i.ToString());
		}
		m_Enemy_Dropdown_Type.AddOptions(enemyType_option);
		#endregion
		#region Waypoint
		m_Enemy_Scroll_WayPointList.onAddButtonClicked.AddListener(() =>
		{
			OnEnemyAddPointButtonPressed();
		});
		m_Enemy_Scroll_WayPointList.onAddButtonClicked.AddListener(() =>
		{
			OnEnemyClampWayPointIndex();
		});
		m_Enemy_Scroll_WayPointList.onRemoveButtonClicked.AddListener(() =>
		{
			OnEnemyRemovePointButtonPressed();
		});
		m_Enemy_Scroll_WayPointList.onRemoveButtonClicked.AddListener(() =>
		{
			OnEnemyClampWayPointIndex();
		});

		m_Enemy_Scroll_WayPointList.onItemValueChanged += (item) => OnEnemyChangeWayPoint();
		m_Enemy_Scroll_WayPointList.onSelectedIndexEndEdit.AddListener(item =>
		{
			OnEnemyClampWayPointIndex();
		});
		#endregion
		#endregion
		#region Wall
		m_WallColor_Slider_Red.value = M_Tile.wallColor.r;
		m_WallColor_Slider_Green.value = M_Tile.wallColor.g;
		m_WallColor_Slider_Blue.value = M_Tile.wallColor.b;
		m_SelectedImage.color = Color.clear;
		ColorToText();
		#endregion
		#region SafetyZone
		m_SafetyZone_CheckBox_FinishZone.onValueChanged.AddListener(index =>
		{
			M_SafetyZone.ToggleFinishZone((int)(index - 1));
		});
		#endregion
		#region GravityZone
		m_GravityZone_InputField_GravityX.onEndEdit.AddListener(item =>
		{
			OnGravityZoneChangeGravity();
		});
		m_GravityZone_InputField_GravityY.onEndEdit.AddListener(item =>
		{
			OnGravityZoneChangeGravity();
		});
		#endregion
		#region IceZone
		m_IceZone_InputField_Drag.onEndEdit.AddListener(item =>
		{
			OnIceZoneChangeDrag();
		});
		#endregion

		#region Option Panel
		m_SelectedObject_Panel_Option.SetActive(false);

		m_Enemy_Panel_Option.SetActive(false);
		m_Enemy_Panel_Speed.SetActive(false);
		m_Enemy_Panel_WayPoint.SetActive(false);
		m_Enemy_Panel_Center.SetActive(false);

		m_Wall_Panel_Option.SetActive(false);

		m_SafetyZone_Panel_Option.SetActive(false);

		m_GravityZone_Panel_Option.SetActive(false);

		m_IceZone_Panel_Option.SetActive(false);
		#endregion 
		#endregion

		#region Stage Save Event Link
		m_SelectedObject_InputField_XPos.onEndEdit.AddListener(item =>
		{
			M_Stage.canSave = false;
		});
		m_SelectedObject_InputField_YPos.onEndEdit.AddListener(item =>
		{
			M_Stage.canSave = false;
		});

		m_Enemy_Dropdown_Type.onValueChanged.AddListener(item =>
		{
			if (M_Stage.canSave &&
				m_ClickedObject?.GetObjectType() == E_ObjectType.Enemy)
			{
				M_Stage.canSave = false;
			}
		});
		m_Enemy_InputField_Speed.onEndEdit.AddListener(item =>
		{
			if (M_Stage.canSave &&
				m_ClickedObject?.GetObjectType() == E_ObjectType.Enemy)
			{
				M_Stage.canSave = false;
			}
		});
		m_Enemy_Scroll_WayPointList.onItemValueChanged += (item) =>
		{
			if (M_Stage.canSave &&
				m_ClickedObject?.GetObjectType() == E_ObjectType.Enemy)
			{
				M_Stage.canSave = false;
			}
		};
		m_Enemy_Scroll_WayPointList.onAddButtonClicked.AddListener(() =>
		{
			if (M_Stage.canSave &&
				m_ClickedObject?.GetObjectType() == E_ObjectType.Enemy)
			{
				M_Stage.canSave = false;
			}
		});
		m_Enemy_Scroll_WayPointList.onRemoveButtonClicked.AddListener(() =>
		{
			if (M_Stage.canSave &&
				m_ClickedObject?.GetObjectType() == E_ObjectType.Enemy)
			{
				M_Stage.canSave = false;
			}
		});
		m_Enemy_InputField_CenterX.onEndEdit.AddListener(item =>
		{
			if (M_Stage.canSave &&
				m_ClickedObject?.GetObjectType() == E_ObjectType.Enemy)
			{
				M_Stage.canSave = false;
			}
		});
		m_Enemy_InputField_CenterY.onEndEdit.AddListener(item =>
		{
			if (M_Stage.canSave &&
				m_ClickedObject?.GetObjectType() == E_ObjectType.Enemy)
			{
				M_Stage.canSave = false;
			}
		});

		m_SafetyZone_CheckBox_FinishZone.onValueChanged.AddListener(item =>
		{
			if (M_Stage.canSave &&
				m_ClickedObject?.GetObjectType() == E_ObjectType.SafetyZone)
			{
				M_Stage.canSave = false;
			}
		});

		m_GravityZone_InputField_GravityX.onEndEdit.AddListener(item =>
		{
			if (M_Stage.canSave &&
				m_ClickedObject?.GetObjectType() == E_ObjectType.GravityZone)
			{
				M_Stage.canSave = false;
			}
		});
		m_GravityZone_InputField_GravityY.onEndEdit.AddListener(item =>
		{
			if (M_Stage.canSave &&
				m_ClickedObject?.GetObjectType() == E_ObjectType.GravityZone)
			{
				M_Stage.canSave = false;
			}
		});

		m_IceZone_InputField_Drag.onEndEdit.AddListener(item =>
		{
			if (M_Stage.canSave &&
				m_ClickedObject?.GetObjectType() == E_ObjectType.IceZone)
			{
				M_Stage.canSave = false;
			}
		});
		#endregion
	}
	public void __Finalize()
	{

	}

	#region SafetyZone
	public void AddSafetyZoneOption(int value)
	{
		m_SafetyZone_CheckBox_FinishZone.AddOption(value.ToString());
	}
	public void RemoveSafetyZoneOption(int value)
	{
		m_SafetyZone_CheckBox_FinishZone.RemoveOption(value - 1);
	}
	#endregion
	#region UI
	public void SetSelectedType(E_ObjectType type)
	{
		m_SelectedType = type;

		Renderer renderer = m_ClickedObject?.GetRenderer();
		if (null != renderer)
		{
			renderer.sortingLayerID = m_ClickedObjectSortingLayerID;
		}
		m_ClickedObject = null;
		SetSelectedUI(type);
		SetCursorImage(type);

		M_Stage.panelActive = false;
	}
	public bool IsPointerOverUIObject()
	{
		PointerEventData eventData = new PointerEventData(null);
		eventData.position = Input.mousePosition;

		List<RaycastResult> results = new List<RaycastResult>();
		m_Raycaster_UI?.Raycast(eventData, results);

		return results.Count > 0;
	}
	#endregion
	#endregion
	#region 이벤트 함수
	private void OnPlayModeEnter()
	{
		m_PlayButton.image.sprite = m_ResetImage;

		m_Edit_Panel.SetActive(false);

		m_IsEdit = false;

		SetCursorImage(E_ObjectType.None);
	}
	private void OnPlayModeExit()
	{
		m_IsEdit = true;

		m_Edit_Panel.SetActive(true);

		m_PlayButton.image.sprite = m_PlayImage;
	}

	public void OnPlayButtonClicked()
	{
		if (isEditMode)
		{
			if (!m_IsDebugMode)
			{
				if (!M_Player.playerActive)
				{
					M_FloatingText.SpawnFloatingText("플레이어는 배치된 상태여야 합니다", new Vector3(0.5f, 0.3f));
					return;
				}
				if (M_SafetyZone.finishZoneCount <= 0)
				{
					M_FloatingText.SpawnFloatingText("완료 구역이 최소 1개 이상 있어야 합니다", new Vector3(0.5f, 0.3f));
					return;
				}
			}

			m_SelectedType = E_ObjectType.None;

			m_SelectedImage.color = Color.white * 0f;

			M_Game.EnterPlayMode();
		}
		else if (!isEditMode)
		{
			M_Game.ExitPlayMode();
		}
	}

	#region SelectedObject
	public void OnSelectedObjectChangePosition()
	{
		if (null == m_ClickedObject)
			return;

		Transform clickedTransform = m_ClickedObject.GetGameObject().transform;

		string strX = m_SelectedObject_InputField_XPos.text;
		string strY = m_SelectedObject_InputField_YPos.text;

		float x, y, z;

		if (!float.TryParse(strX, out x))
		{
			x = 0f;
		}
		if (!float.TryParse(strY, out y))
		{
			y = 0f;
		}
		z = clickedTransform.position.z;

		clickedTransform.position = new Vector3(x, y, z);
	}
	#endregion
	#region Enemy
	public void OnEnemyChangeType()
	{
		m_Enemy_Panel_Speed.SetActive(false);
		if (enemy_Type != E_EnemyType.None)
		{
			m_Enemy_Panel_Speed.SetActive(true);
		}

		if (m_ClickedObject?.GetObjectType() != E_ObjectType.Enemy)
			return;

		Enemy enemy = m_ClickedObject.GetGameObject().GetComponent<Enemy>();
		enemy.type = enemy_Type;
		m_Enemy_InputField_Speed.text = enemy.speed.ToString();

		ShowEnemyGizmo(enemy);
	}
	public void OnEnemyClampSpeed()
	{
		if (m_Enemy_InputField_Speed.text == "")
		{
			m_Enemy_InputField_Speed.text = "0.01f";
			return;
		}

		float value = float.Parse(m_Enemy_InputField_Speed.text);
		value = Mathf.Clamp(value, 0.01f, 100f);

		if (m_ClickedObject?.GetObjectType() != E_ObjectType.Enemy)
			return;

		Enemy enemy = m_ClickedObject.GetGameObject().GetComponent<Enemy>();
		enemy.speed = value;

		m_Enemy_InputField_Speed.text = enemy.speed.ToString();
	}
	public void OnEnemyAddPointButtonPressed()
	{
		if (m_ClickedObject?.GetObjectType() != E_ObjectType.Enemy)
			return;

		Enemy enemy = m_ClickedObject.GetGameObject().GetComponent<Enemy>();
		enemy.AddWayPoint();
	}
	public void OnEnemyRemovePointButtonPressed()
	{
		if (m_ClickedObject?.GetObjectType() != E_ObjectType.Enemy)
			return;

		Enemy enemy = m_ClickedObject.GetGameObject().GetComponent<Enemy>();
		enemy.RemoveWayPoint(m_Enemy_Scroll_WayPointList.selectedIndex);
	}
	public void OnEnemyClampWayPointIndex()
	{
		InputField inputField = m_Enemy_Scroll_WayPointList.selectedIndexInputField;

		if (m_Enemy_Scroll_WayPointList.Count <= 0)
		{
			inputField.text = "";
			return;
		}
		else if (inputField.text == "")
		{
			inputField.text = (m_Enemy_Scroll_WayPointList.selectedIndex + 1).ToString();
			return;
		}

		int value = int.Parse(inputField.text);
		value = Mathf.Clamp(value, 0, m_Enemy_Scroll_WayPointList.Count);
		inputField.text = value.ToString();

		OnEnemySelectWayPointIndex();
	}
	public void OnEnemySelectWayPointIndex()
	{
		InputField inputField = m_Enemy_Scroll_WayPointList.selectedIndexInputField;

		if (inputField.text == "")
		{
			OnEnemyClampWayPointIndex();
			return;
		}

		m_Enemy_Scroll_WayPointList.selectedIndex = int.Parse(inputField.text) - 1;
	}
	public void OnEnemyChangeWayPoint()
	{
		if (m_ClickedObject?.GetObjectType() != E_ObjectType.Enemy)
			return;

		Enemy enemy = m_ClickedObject.GetGameObject().GetComponent<Enemy>();

		int index = m_Enemy_Scroll_WayPointList.selectedIndex;
		ScrollInputFieldListItem scrollInputFieldItem = m_Enemy_Scroll_WayPointList.GetInputFieldItem(index);
		if (null == scrollInputFieldItem)
		{
			Debug.LogError("아이템 Null");
			return;
		}

		string strX = scrollInputFieldItem.GetInputFieldValue(0);
		string strY = scrollInputFieldItem.GetInputFieldValue(1);

		float x, y;

		if (!float.TryParse(strX, out x))
		{
			x = 0;
		}
		if (!float.TryParse(strY, out y))
		{
			y = 0;
		}

		float z = enemy.wayPointList[index].transform.position.z;

		Vector3 wayPointPos = new Vector3(x, y, z);
		enemy.wayPointList[index].transform.position = wayPointPos;
	}
	public void OnEnemyChangeCenterPosition()
	{
		if (m_ClickedObject?.GetObjectType() != E_ObjectType.Enemy)
			return;

		EnemyGizmo center = m_ClickedObject.GetGameObject().GetComponent<Enemy>().center;

		string strX = m_Enemy_InputField_CenterX.text;
		string strY = m_Enemy_InputField_CenterY.text;

		float x, y;
		if (!float.TryParse(strX, out x))
		{
			x = 0;
		}
		if (!float.TryParse(strY, out y))
		{
			y = 0;
		}
		float z = center.transform.position.z;

		center.transform.position = new Vector3(x, y, z);
	}
	#endregion
	#region Wall
	public void ClampColorOption(InputField input)
	{
		float value = float.Parse(input.text);

		if (value < 0f)
			value = 0f;
		if (value > 255f)
			value = 255f;

		input.text = value.ToString();
	}
	public void TextToColor()
	{
		m_WallColor_Slider_Red.value = float.Parse(m_WallColor_InputField_Red.text) / 255f;
		m_WallColor_Slider_Green.value = float.Parse(m_WallColor_InputField_Green.text) / 255f;
		m_WallColor_Slider_Blue.value = float.Parse(m_WallColor_InputField_Blue.text) / 255f;
	}
	public void ColorToText()
	{
		m_WallColor_InputField_Red.text = (m_WallColor_Slider_Red.value * 255f).ToString();
		m_WallColor_InputField_Green.text = (m_WallColor_Slider_Green.value * 255f).ToString();
		m_WallColor_InputField_Blue.text = (m_WallColor_Slider_Blue.value * 255f).ToString();
	}
	public void OnUpdateWallColor()
	{
		Color color = Color.white;
		color.r = m_WallColor_Slider_Red.value;
		color.g = m_WallColor_Slider_Green.value;
		color.b = m_WallColor_Slider_Blue.value;
		m_SelectedImage.color = M_Tile.wallColor = color;

		if (m_ClickedObject?.GetObjectType() == E_ObjectType.Wall)
		{
			UpdateClickedWallColor();
		}
	}
	#endregion
	#region GravityZone
	public void OnGravityZoneChangeGravity()
	{
		if (m_ClickedObject?.GetObjectType() != E_ObjectType.GravityZone)
			return;

		GravityZone gravityZone = m_ClickedObject.GetGameObject().GetComponent<GravityZone>();
		gravityZone.gravity = gravityZone_Gravity;
	}
	#endregion
	#region IceZone
	public void OnIceZoneChangeDrag()
	{
		if (m_ClickedObject?.GetObjectType() != E_ObjectType.IceZone)
			return;

		IceZone iceZone = m_ClickedObject.GetGameObject().GetComponent<IceZone>();
		float drag;
		if (!float.TryParse(m_IceZone_InputField_Drag.text, out drag))
		{
			drag = 0f;
		}
		iceZone.friction = drag;
	}
	#endregion
	#endregion
	#region 유니티 콜백 함수
	protected void Update()
	{
		if (m_IsEdit)
		{
			#region 마우스
			if (IsMouseDown)
			{
				DrawInEditMode();
			}
			if (IsLeftUp)
			{
				SelectInEditMode();
			}
			#endregion

			#region 키보드
			if (!isInputFieldFocus)
			{
				ChangeSelectedType();
			}
			#endregion

			ChangeCursor();
		}
	}
	#endregion
}