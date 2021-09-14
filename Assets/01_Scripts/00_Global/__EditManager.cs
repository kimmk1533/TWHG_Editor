using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class __EditManager : Singleton<__EditManager>
{
    [Header("Canvas")]
    [SerializeField]
    protected Canvas m_Canvas_BG;
    [SerializeField]
    protected Canvas m_Canvas_UI;
    protected GraphicRaycaster m_Raycaster_BG;
    protected GraphicRaycaster m_Raycaster_UI;

    [Header("EditMode")]
    [SerializeField, ReadOnly]
    protected bool m_IsEdit;

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
    public Sprite PlayImage;
    public Sprite ResetImage;

    [Header("Public Option")]
    public GameObject m_PublicOptionPanel;
    protected GameObject m_CurrentOptionPanel;

    [Header("Enemy Option")]
    [SerializeField]
    protected GameObject m_EnemyOptionPanel;
    [SerializeField]
    protected Dropdown m_EnemyTypeDropdown;
    [SerializeField]
    protected GameObject m_EnemySpeedOption;
    public GameObject Edit_LinearOption;
    public GameObject Edit_CircularOption;
    public InputField input_EnemySpeed;
    public Dropdown dropdown_EnemyType;
    public Sprite AddImage;
    public Sprite DoneImage;
    public bool AddPoint;
    public Button button_AddPoint;
    List<string> enemyType_option;

    [Header("SafetyZone Option")]
    [SerializeField]
    protected GameObject m_SafetyZoneOptionPanel;
    [SerializeField]
    protected CheckBox m_SafetyZone_FinishCheckBox;

    [Header("Wall Option")]
    public GameObject m_WallOptionPanel;
    public Slider slider_red;
    public Slider slider_green;
    public Slider slider_blue;
    public InputField input_red;
    public InputField input_green;
    public InputField input_blue;

    #region 내부 프로퍼티
    #region 매니져
    protected ResourcesManager M_Resources => ResourcesManager.Instance;
    protected __GameManager M_Game => __GameManager.Instance;
    protected PlayerManager M_Player => PlayerManager.Instance;
    protected EnemyManager M_Enemy => EnemyManager.Instance;
    protected CoinManager M_Coin => CoinManager.Instance;
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    protected TileManager M_Tile => TileManager.Instance;
    protected StageManager M_Stage => StageManager.Instance;
    #endregion

    protected Vector3 SpawnPoint
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
            bool? isFocus = EventSystem.current.currentSelectedGameObject?.GetComponent<InputField>()?.isFocused;

            if (isFocus.HasValue)
                return isFocus.Value;

            return false;
        }
    }
    #endregion
    #region 외부 프로퍼티
    public bool isEditMode => m_IsEdit;

    #region SafetyZone
    public List<CheckBox.OptionData> safetyZoneFinishOptionList => m_SafetyZone_FinishCheckBox.options;
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
                        player.transform.position = SpawnPoint;
                        break;
                    case E_ObjectType.Enemy:
                        // 적 스폰
                        Enemy enemy = M_Enemy.SpawnEnemy();
                        // 위치 설정
                        enemy.transform.position = SpawnPoint;
                        break;
                    case E_ObjectType.Coin:
                        // 코인 스폰
                        Coin coin = M_Coin.SpawnCoin();
                        // 위치 설정
                        coin.transform.position = SpawnPoint;
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
            return;

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

    protected void SetSelectedUI(E_ObjectType type)
    {
        m_SelectedText.text = "Selected:" + "\n" + type.ToString();
        m_CurrentOptionPanel?.SetActive(false);

        switch (type)
        {
            case E_ObjectType.None:
                {
                    m_SelectedImage.sprite = null;
                    m_SelectedImage.color = Color.clear;
                    m_SelectedImageOutline.enabled = false;

                    m_CurrentOptionPanel = null;
                    break;
                }
            case E_ObjectType.Player:
                {
                    m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
                    m_SelectedImage.sprite = M_Resources.GetSprites("Player", "Player")[0];
                    m_SelectedImage.color = Color.white;
                    m_SelectedImageOutline.enabled = false;

                    m_CurrentOptionPanel = null;
                    break;
                }
            case E_ObjectType.Enemy:
                {
                    m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
                    m_SelectedImage.sprite = M_Resources.GetSprites("Enemy", "Enemy")[0];
                    m_SelectedImage.color = Color.white;
                    m_SelectedImageOutline.enabled = false;

                    m_CurrentOptionPanel = m_EnemyOptionPanel;
                    break;
                }
            case E_ObjectType.Coin:
                {
                    m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
                    m_SelectedImage.sprite = M_Resources.GetSprites("Coin", "Coin")[0];
                    m_SelectedImage.color = Color.white;
                    m_SelectedImageOutline.enabled = false;

                    m_CurrentOptionPanel = null;
                    break;
                }
            case E_ObjectType.Wall:
                {
                    m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f) - m_SelectedImageOutline.effectDistance * 2f;
                    m_SelectedImage.sprite = M_Resources.GetSprites("Tile", "Tile")[0];
                    m_SelectedImageOutline.enabled = true;

                    WallCollider wallCollider = m_ClickedObjectType?.GetGameObject().GetComponent<WallCollider>();
                    if (null == wallCollider)
                        m_SelectedImage.color = M_Game.m_WallColor;
                    else
                        m_SelectedImage.color = wallCollider.wall.tile.color;

                    slider_red.value = m_SelectedImage.color.r;
                    slider_green.value = m_SelectedImage.color.g;
                    slider_blue.value = m_SelectedImage.color.b;
                    ColorToText();

                    m_CurrentOptionPanel = m_WallOptionPanel;
                    break;
                }
            case E_ObjectType.SafetyZone:
                {
                    m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
                    m_SelectedImage.sprite = M_Resources.GetSprites("Tile", "Tile")[0];
                    m_SelectedImage.color = M_Game.m_SafetyZoneColor;
                    m_SelectedImageOutline.enabled = false;

                    m_CurrentOptionPanel = m_SafetyZoneOptionPanel;
                    break;
                }
            case E_ObjectType.Erase:
                {
                    m_SelectedImage.sprite = null;
                    m_SelectedImage.color = Color.clear;
                    m_SelectedImageOutline.enabled = false;

                    m_CurrentOptionPanel = null;
                    break;
                }
        }

        m_CurrentOptionPanel?.SetActive(true);
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
            SetSelectedType(E_ObjectType.SafetyZone);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetSelectedType(E_ObjectType.Wall);
        }
    }

    protected void UpdateClickedWallOption()
    {
        Wall wall = m_ClickedObjectType.GetGameObject().GetComponent<WallCollider>().wall;
        wall.tile.color = M_Game.m_WallColor;
    }
    #endregion
    #region 외부 함수
    public void __Initialize()
    {
        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;

        // 필드 초기화
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

        // Enemy
        AddPoint = false;
        dropdown_EnemyType.ClearOptions();
        enemyType_option = new List<string>();
        E_EnemyType type = E_EnemyType.Linear;
        for (int i = 0; i < (int)E_EnemyType.Max; ++i)
        {
            enemyType_option.Add(type.ToString());
            ++type;
        }
        dropdown_EnemyType.AddOptions(enemyType_option);

        // Wall
        slider_red.value = M_Game.m_WallColor.r;
        slider_green.value = M_Game.m_WallColor.g;
        slider_blue.value = M_Game.m_WallColor.b;
        m_SelectedImage.color = Color.clear;
        ColorToText();

        // SafetyZone
        // m_SafetyZone_FinishCheckBox.onValueChanged.AddListener(index => { M_SafetyZone.SelectFinishZone((int)(index.y - 1)); });
    }
    public void __Finalize()
    {

    }

    #region SafetyZone
    public void AddSafetyZoneOption(int value)
    {
        m_SafetyZone_FinishCheckBox.AddOption(value.ToString());
    }
    public void RemoveSafetyZoneOption(int value)
    {
        m_SafetyZone_FinishCheckBox.RemoveOption(value - 1);
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

    public void TestPlay()
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
    public void OnChangeEnemyType()
    {

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
        slider_red.value = float.Parse(input_red.text) / 255f;
        slider_green.value = float.Parse(input_green.text) / 255f;
        slider_blue.value = float.Parse(input_blue.text) / 255f;
    }
    public void ColorToText()
    {
        input_red.text = (slider_red.value * 255f).ToString();
        input_green.text = (slider_green.value * 255f).ToString();
        input_blue.text = (slider_blue.value * 255f).ToString();
    }
    public void UpdateWallOption()
    {
        M_Game.m_WallColor.r = slider_red.value;
        M_Game.m_WallColor.g = slider_green.value;
        M_Game.m_WallColor.b = slider_blue.value;

        m_SelectedImage.color = M_Game.m_WallColor;

        if (m_ClickedObjectType?.GetObjectType() == E_ObjectType.Wall)
        {
            UpdateClickedWallOption();
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
    public void PointButtonPressed()
    {
        if (!AddPoint)
        {
            button_AddPoint.image.sprite = DoneImage;
            AddPoint = true;
        }
        else
        {
            button_AddPoint.image.sprite = AddImage;
            AddPoint = false;
        }
    }
    #endregion
}
