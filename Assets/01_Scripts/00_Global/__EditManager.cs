using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class __EditManager : Singleton<__EditManager>
{
    [Header("EditMode")]
    [SerializeField, ReadOnly]
    protected bool m_IsEdit;

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
    protected IObjectType m_ClickedObjectType;
    protected List<IObjectType> m_CurrentClickedObjectList;
    protected List<IObjectType> m_LastClickedObjectList;

    [Header("PlayButton")]
    public Button PlayButton;

    [Space(10)]
    public Sprite PlayImage;
    public Sprite ResetImage;

    #region 옵션
    protected GameObject m_Current_Panel_Option;

    #region 공용 옵션
    [Header("Public Option")]
    [SerializeField]
    protected GameObject m_Public_Panel_Option;
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
    #region 안전지역 옵션
    [Header("SafetyZone Option")]
    [SerializeField]
    protected GameObject m_SafetyZone_Panel_Option;
    [SerializeField]
    protected CheckBox m_SafetyZone_CheckBox_FinishZone;
    #endregion
    #endregion

    #region 내부 프로퍼티
    #region 매니져
    protected ResourcesManager M_Resources => ResourcesManager.Instance;
    protected __GameManager M_Game => __GameManager.Instance;
    protected PlayerManager M_Player => PlayerManager.Instance;
    protected EnemyManager M_Enemy => EnemyManager.Instance;
    protected EnemyGizmoManager M_EnemyGizmo => EnemyGizmoManager.Instance;
    protected CoinManager M_Coin => CoinManager.Instance;
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    protected TileManager M_Tile => TileManager.Instance;
    protected StageManager M_Stage => StageManager.Instance;
    #endregion

    protected Vector3 spawnPoint
    {
        get
        {
            float distance = 5f;
            Vector3 position = Input.mousePosition;
            position.z = distance;
            return Camera.main.ScreenToWorldPoint(position);
        }
    }
    protected bool IsMouseDown => IsLeft || IsRight;
    protected bool IsLeft => Input.GetMouseButton((int)E_InputButton.Left);
    protected bool IsRight => Input.GetMouseButton((int)E_InputButton.Right);
    protected bool IsLeftDown => Input.GetMouseButtonDown((int)E_InputButton.Left);
    protected bool IsRightDown => Input.GetMouseButtonDown((int)E_InputButton.Right);
    protected bool IsLeftUp => Input.GetMouseButtonUp((int)E_InputButton.Left);
    protected bool IsRightUp => Input.GetMouseButtonUp((int)E_InputButton.Right);

    protected bool isInputFieldFocus
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
    protected E_EnemyType enemyType => (E_EnemyType)m_Enemy_Dropdown_Type.value - 1;
    protected float enemySpeed
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
    #endregion
    #region 외부 프로퍼티
    public bool isEditMode => m_IsEdit;

    #region SafetyZone
    public List<CheckBox.OptionData> safetyZoneFinishOptionList => m_SafetyZone_CheckBox_FinishZone.options;
    #endregion
    #endregion
    #region 내부 함수
    protected void DrawInEditMode()
    {
        if (IsPointerOverUIObject())
            return;

        #region Object Raycast
        // 레이캐스트 설정
        Ray obj_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 레이캐스트
        RaycastHit2D obj_hit = Physics2D.Raycast(obj_ray.origin, obj_ray.direction);
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

        GameObject obj = obj_hit.transform?.gameObject;
        GameObject ui_obj = results[0].gameObject;

        #region Erase
        if (IsRight)
        {
            if (null != m_ClickedObjectType&&
                obj?.GetComponent<IObjectType>() == m_ClickedObjectType)
            {
                SetSelectedUI(E_ObjectType.None);

                m_ClickedObjectType = null;
            }

            obj?.GetComponent<IEraserable>()?.Erase();
        }
        #endregion

        #region Click Process
        if (IsLeftDown)
        {
            if (null == obj?.GetComponent<WallCollider>())
            {
                switch (m_SelectedType)
                {
                    case E_ObjectType.Player:
                        // 플레이어 스폰
                        Player player = M_Player.SpawnPlayer();
                        // 위치 설정
                        player.transform.position = spawnPoint;
                        break;
                    case E_ObjectType.Enemy:
                        // 적 스폰
                        Enemy enemy = M_Enemy.SpawnEnemy();
                        // 위치 설정
                        enemy.transform.position = spawnPoint;
                        // 적 타입 설정
                        enemy.type = enemyType;
                        // 적 이동 속도 설정
                        enemy.speed = enemySpeed;
                        break;
                    case E_ObjectType.Coin:
                        // 코인 스폰
                        Coin coin = M_Coin.SpawnCoin();
                        // 위치 설정
                        coin.transform.position = spawnPoint;
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
                    obj?.GetComponent<SafetyZoneCollider>()?.Erase();
                    M_Tile.Draw(ui_obj, E_TileType.Wall);
                    break;
                case E_ObjectType.SafetyZone:
                    obj?.GetComponent<WallCollider>()?.Erase();
                    M_Tile.Draw(ui_obj, E_TileType.SafetyZone);
                    break;
                case E_ObjectType.Erase:
                    M_Tile.Draw(ui_obj, E_TileType.None);
                    break;
            }
        }
        else if (IsRight)
        {
            M_Tile.Draw(ui_obj, E_TileType.None);
        }
        #endregion
    }
    protected void SelectInEditMode()
    {
        if (IsPointerOverUIObject())
            return;

        if (m_SelectedType != E_ObjectType.None)
            return;

        Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int layerMask = 0;
        for (E_ObjectType i = E_ObjectType.Player; i < E_ObjectType.Erase; ++i)
        {
            layerMask += 1 << LayerMask.NameToLayer(i.ToString());
        }

        Collider2D[] colliders = Physics2D.OverlapPointAll(origin, layerMask);

        if (colliders.Length <= 0)
        {
            m_ClickedObjectType = null;
            return;
        }

        m_LastClickedObjectList.Clear();
        m_LastClickedObjectList.AddRange(m_CurrentClickedObjectList);

        m_CurrentClickedObjectList.Clear();
        foreach (var item in colliders)
        {
            IObjectType objectType = item.GetComponent<IObjectType>();
            if (null == objectType)
                continue;

            m_CurrentClickedObjectList.Add(objectType);
        }

        int currentCount = m_CurrentClickedObjectList.Count;
        int lastCount = m_LastClickedObjectList.Count;

        if (lastCount <= 0)
        {
            m_ClickIndex = 1;
            m_ClickedObjectType = m_CurrentClickedObjectList[0];
            SetSelectedUI(m_ClickedObjectType.GetObjectType());
            return;
        }

        if (m_ClickIndex >= currentCount)
            m_ClickIndex = 0;

        for (int i = 0; i < currentCount; ++i)
        {
            if (m_CurrentClickedObjectList[i] == m_LastClickedObjectList[i])
            {
                if (m_ClickIndex <= i)
                {
                    m_ClickIndex = i + 1;
                    m_ClickedObjectType = m_CurrentClickedObjectList[i];
                    SetSelectedUI(m_ClickedObjectType.GetObjectType());
                    break;
                }
            }
            else
            {
                m_ClickIndex = 1;
                m_ClickedObjectType = m_CurrentClickedObjectList[0];
                SetSelectedUI(m_ClickedObjectType.GetObjectType());
                break;
            }
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
    }
    protected void SetSelectedUI(E_ObjectType type)
    {
        m_SelectedText.text = "Selected:" + "\n" + type.ToString();
        m_Current_Panel_Option?.SetActive(false);
        M_EnemyGizmo.SetActiveAllGizmo(false);

        if (m_SelectedType != E_ObjectType.None)
        {
            if (m_ClickedObjectType?.GetObjectType() == type)
            {
                m_ClickedObjectType = null;
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
                    break;
                }
            case E_ObjectType.Player:
                {
                    m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
                    m_SelectedImage.sprite = M_Resources.GetSprites("Player", "Player")[0];
                    m_SelectedImage.color = Color.white;
                    m_SelectedImageOutline.enabled = false;

                    m_Current_Panel_Option = null;
                    break;
                }
            case E_ObjectType.Enemy:
                {
                    m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
                    m_SelectedImage.sprite = M_Resources.GetSprites("Enemy", "Enemy")[0];
                    m_SelectedImage.color = Color.white;
                    m_SelectedImageOutline.enabled = false;

                    m_Current_Panel_Option = m_Enemy_Panel_Option;

                    if (m_ClickedObjectType?.GetObjectType() == E_ObjectType.Enemy)
                    {
                        Enemy enemy = m_ClickedObjectType.GetGameObject().GetComponent<EnemyCollider>().enemy;

                        m_Enemy_Dropdown_Type.value = (int)enemy.type + 1;

                        m_Enemy_Panel_Speed.SetActive(false);
                        if (enemy.type != E_EnemyType.None)
                        {
                            m_Enemy_Panel_Speed.SetActive(true);
                            m_Enemy_InputField_Speed.text = enemy.speed.ToString();
                        }

                        ShowEnemyWayPointList(enemy);
                    }
                    else
                    {
                        m_Enemy_Panel_WayPoint.SetActive(false);
                    }
                    break;
                }
            case E_ObjectType.Coin:
                {
                    m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
                    m_SelectedImage.sprite = M_Resources.GetSprites("Coin", "Coin")[0];
                    m_SelectedImage.color = Color.white;
                    m_SelectedImageOutline.enabled = false;

                    m_Current_Panel_Option = null;
                    break;
                }
            case E_ObjectType.Wall:
                {
                    m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f) - m_SelectedImageOutline.effectDistance * 2f;
                    m_SelectedImage.sprite = M_Resources.GetSprites("Tile", "Tile")[0];
                    m_SelectedImageOutline.enabled = true;

                    WallCollider wallCollider = m_ClickedObjectType?.GetGameObject().GetComponent<WallCollider>();
                    if (null == wallCollider)
                        m_SelectedImage.color = M_Game.wallColor;
                    else
                        m_SelectedImage.color = wallCollider.wall.tile.color;

                    m_WallColor_Slider_Red.value = m_SelectedImage.color.r;
                    m_WallColor_Slider_Green.value = m_SelectedImage.color.g;
                    m_WallColor_Slider_Blue.value = m_SelectedImage.color.b;
                    ColorToText();

                    m_Current_Panel_Option = m_Wall_Panel_Option;
                    break;
                }
            case E_ObjectType.SafetyZone:
                {
                    m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
                    m_SelectedImage.sprite = M_Resources.GetSprites("Tile", "Tile")[0];
                    m_SelectedImage.color = M_Game.safetyZoneColor;
                    m_SelectedImageOutline.enabled = false;

                    m_Current_Panel_Option = m_SafetyZone_Panel_Option;
                    break;
                }
            case E_ObjectType.Erase:
                {
                    m_SelectedImage.sprite = null;
                    m_SelectedImage.color = Color.clear;
                    m_SelectedImageOutline.enabled = false;

                    m_Current_Panel_Option = null;
                    break;
                }
        }

        m_Current_Panel_Option?.SetActive(true);
    }

    protected void UpdateClickedWallColor()
    {
        Wall wall = m_ClickedObjectType.GetGameObject().GetComponent<WallCollider>().wall;
        wall.tile.color = M_Game.wallColor;
    }

    #region Enemy
    protected void ShowEnemyWayPointList(Enemy enemy)
    {
        M_EnemyGizmo.SetActiveAllGizmo(false);
        m_Enemy_Panel_WayPoint.SetActive(false);

        if (enemy.type == E_EnemyType.Linear ||
            enemy.type == E_EnemyType.LinearRepeat)
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
    }
    #endregion
    #endregion
    #region 외부 함수
    public void __Initialize()
    {
        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;

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
            m_CurrentClickedObjectList = new List<IObjectType>();
        }
        if (null == m_LastClickedObjectList)
        {
            m_LastClickedObjectList = new List<IObjectType>();
        }

        float unit = m_Canvas_BG.referencePixelsPerUnit;
        Vector2 size = new Vector2(M_Game.width, M_Game.height) * unit;
        m_Canvas_BG.GetComponent<RectTransform>().sizeDelta = size;

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

        // Wall
        m_WallColor_Slider_Red.value = M_Game.wallColor.r;
        m_WallColor_Slider_Green.value = M_Game.wallColor.g;
        m_WallColor_Slider_Blue.value = M_Game.wallColor.b;
        m_SelectedImage.color = Color.clear;
        ColorToText();

        // SafetyZone
        // m_SafetyZone_FinishCheckBox.onValueChanged.AddListener(index => { M_SafetyZone.SelectFinishZone((int)(index.y - 1)); });

        m_Enemy_Panel_Option.SetActive(false);
        m_Enemy_Panel_Speed.SetActive(false);
        m_Enemy_Panel_WayPoint.SetActive(false);
        m_Wall_Panel_Option.SetActive(false);
        m_SafetyZone_Panel_Option.SetActive(false);
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
        SetSelectedUI(type);
    }
    public bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(null);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster_UI.Raycast(eventData, results);

        return results.Count > 0;
    }
    #endregion
    #endregion
    #region 이벤트 함수
    public void OnPlayEnter()
    {
        PlayButton.image.sprite = ResetImage;

        m_Canvas_UI.transform.Find("Game_UI").gameObject.SetActive(true);
        m_Canvas_UI.transform.Find("Edit_UI").gameObject.SetActive(false);

        m_IsEdit = false;
    }
    public void OnPlayExit()
    {
        m_IsEdit = true;

        m_Canvas_UI.transform.Find("Game_UI").gameObject.SetActive(false);
        m_Canvas_UI.transform.Find("Edit_UI").gameObject.SetActive(true);

        PlayButton.image.sprite = PlayImage;
    }

    public void OnTestPlay()
    {
        if (isEditMode)/* &&
            M_SafetyZone.m_ColliderList.Count >= 2)*/
        {
            m_SelectedType = E_ObjectType.None;

            m_SelectedImage.color = Color.white * 0f;

            M_Game.EnterPlayMode();
        }
        else if (!isEditMode)
        {
            M_Game.ExitPlayMode();
        }
    }

    #region Enemy
    public void OnEnemyChangeType()
    {
        m_Enemy_Panel_Speed.SetActive(false);
        if (enemyType != E_EnemyType.None)
        {
            m_Enemy_Panel_Speed.SetActive(true);
            OnEnemyClampSpeed();
        }

        if (m_ClickedObjectType?.GetObjectType() != E_ObjectType.Enemy)
            return;

        Enemy enemy = m_ClickedObjectType.GetGameObject().GetComponent<EnemyCollider>().enemy;
        enemy.type = enemyType;

        ShowEnemyWayPointList(enemy);
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

        if (m_ClickedObjectType?.GetObjectType() != E_ObjectType.Enemy)
            return;

        Enemy enemy = m_ClickedObjectType.GetGameObject().GetComponent<EnemyCollider>().enemy;
        enemy.speed = value;

        m_Enemy_InputField_Speed.text = enemy.speed.ToString();
    }
    public void OnEnemyAddPointButtonPressed()
    {
        if (m_ClickedObjectType?.GetObjectType() != E_ObjectType.Enemy)
            return;

        Enemy enemy = m_ClickedObjectType.GetGameObject().GetComponent<EnemyCollider>().enemy;
        enemy.AddWayPoint();
    }
    public void OnEnemyRemovePointButtonPressed()
    {
        if (m_ClickedObjectType?.GetObjectType() != E_ObjectType.Enemy)
            return;

        Enemy enemy = m_ClickedObjectType.GetGameObject().GetComponent<EnemyCollider>().enemy;
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
        if (m_ClickedObjectType?.GetObjectType() != E_ObjectType.Enemy)
            return;

        Enemy enemy = m_ClickedObjectType.GetGameObject().GetComponent<EnemyCollider>().enemy;

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
        m_SelectedImage.color = M_Game.wallColor = color;

        if (m_ClickedObjectType?.GetObjectType() == E_ObjectType.Wall)
        {
            UpdateClickedWallColor();
        }
    }
    #endregion
    #endregion
    #region 유니티 콜백 함수
    private void Update()
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
        }
    }
    #endregion

    #region 기존 함수
    // Enemy
    //public void ChangeEnemyType()
    //{
    //    M_Road.m_CurrentSelectedType = (E_EnemyType)enemyType_option.IndexOf(dropdown_EnemyType.captionText.text);

    //    if (M_Road.m_CurrentSelectedType == E_EnemyType.Linear ||
    //        M_Road.m_CurrentSelectedType == E_EnemyType.LinearRepeat)
    //    {
    //        Edit_LinearOption.SetActive(true);
    //        Edit_CircularOption.SetActive(false);
    //    }
    //    else if (M_Road.m_CurrentSelectedType == E_EnemyType.Circular)
    //    {
    //        Edit_LinearOption.SetActive(false);
    //        Edit_CircularOption.SetActive(true);
    //    }
    //}
    #endregion
}
