using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class __EditManager : Singleton<__EditManager>
{
    [Header("Canvas")]
    [SerializeField]
    protected Canvas Canvas_BG;
    [SerializeField]
    protected Canvas Canvas_UI;
    protected GraphicRaycaster m_Raycaster_BG;
    protected GraphicRaycaster m_Raycaster_UI;

    [Header("EditMode")]
    [SerializeField, ReadOnly]
    protected bool m_IsEdit;

    [Header("Selected")]
    public E_ObjectType m_SelectedType;
    public Image m_SelectedImage;
    public Text m_SelectedText;
    protected Outline m_SelectedImageOutline;

    [Header("PlayButton")]
    public Button PlayButton;
    public Sprite PlayImage;
    public Sprite ResetImage;

    [Header("Public Option")]
    public GameObject m_PublicOptionPanel;
    protected GameObject m_CurrentOptionPanel;

    [Header("Enemy Option")]
    public GameObject m_EnemyOptionPanel;
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
    protected EnemyRoadManager M_Road => EnemyRoadManager.Instance;
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    protected CoinManager M_Coin => CoinManager.Instance;
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

    #region Wall
    protected bool IsInputFieldFocus => input_red.isFocused || input_green.isFocused || input_blue.isFocused || input_EnemySpeed.isFocused;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public bool isEditMode => m_IsEdit;
    public bool isPlayMode => !m_IsEdit;

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
                        M_Player.SpawnPlayer(SpawnPoint);
                        break;
                    case E_ObjectType.Enemy:
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
        else if (IsRightDown)
        {
            obj?.GetComponent<IEraserable>()?.Erase();
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
                    obj?.GetComponent<IEraserable>()?.Erase();
                    M_Tile.Draw(ui_obj, E_TileType.None);
                    break;
            }
        }
        else if (IsRight)
        {
            obj?.GetComponent<IEraserable>()?.Erase();
            M_Tile.Draw(ui_obj, E_TileType.None);
        }
        #endregion
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
    #endregion
    #region 외부 함수
    public void __Initialize()
    {
        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;

        m_IsEdit = true;

        // 레이캐스터 설정
        if (null == m_Raycaster_BG)
        {
            m_Raycaster_BG = Canvas_BG.GetComponent<GraphicRaycaster>();
        }
        if (null == m_Raycaster_UI)
        {
            m_Raycaster_UI = Canvas_UI.GetComponent<GraphicRaycaster>();
        }

        if (null == m_SelectedImageOutline)
        {
            m_SelectedImageOutline = m_SelectedImage.GetComponent<Outline>();
        }

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
        m_SelectedText.text = "Selected:" + "\n" + type.ToString();
        m_CurrentOptionPanel?.SetActive(false);

        switch (type)
        {
            case E_ObjectType.None:
                {
                    m_SelectedType = E_ObjectType.None;
                    m_SelectedImage.sprite = null;
                    m_SelectedImage.color = Color.clear;
                    m_SelectedImageOutline.enabled = false;

                    m_CurrentOptionPanel = null;
                    break;
                }
            case E_ObjectType.Player:
                {
                    m_SelectedType = E_ObjectType.Player;
                    m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
                    m_SelectedImage.sprite = M_Resources.GetSprites("Player", "Player")[0];
                    m_SelectedImage.color = Color.white;
                    m_SelectedImageOutline.enabled = false;

                    m_CurrentOptionPanel = null;
                    break;
                }
            case E_ObjectType.Enemy:
                {
                    m_SelectedType = E_ObjectType.Enemy;
                    m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
                    m_SelectedImage.sprite = M_Resources.GetSprites("Enemy", "Enemy")[0];
                    m_SelectedImage.color = Color.white;
                    m_SelectedImageOutline.enabled = false;

                    m_CurrentOptionPanel = m_EnemyOptionPanel;
                    break;
                }
            case E_ObjectType.Coin:
                {
                    m_SelectedType = E_ObjectType.Coin;
                    m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
                    m_SelectedImage.sprite = M_Resources.GetSprites("Coin", "Coin")[0];
                    m_SelectedImage.color = Color.white;
                    m_SelectedImageOutline.enabled = false;

                    m_CurrentOptionPanel = null;
                    break;
                }
            case E_ObjectType.Wall:
                {
                    m_SelectedType = E_ObjectType.Wall;
                    m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f) - m_SelectedImageOutline.effectDistance * 2f;
                    m_SelectedImage.sprite = M_Resources.GetSprites("Tile", "Tile")[0];
                    m_SelectedImage.color = M_Game.m_WallColor;
                    m_SelectedImageOutline.enabled = true;

                    m_CurrentOptionPanel = m_WallOptionPanel;
                    break;
                }
            case E_ObjectType.SafetyZone:
                {
                    m_SelectedType = E_ObjectType.SafetyZone;
                    m_SelectedImage.rectTransform.sizeDelta = new Vector2(100f, 100f);
                    m_SelectedImage.sprite = M_Resources.GetSprites("Tile", "Tile")[0];
                    m_SelectedImage.color = M_Game.m_SafetyZoneColor;
                    m_SelectedImageOutline.enabled = false;

                    m_CurrentOptionPanel = m_SafetyZoneOptionPanel;
                    break;
                }
            case E_ObjectType.Erase:
                {
                    m_SelectedType = E_ObjectType.Erase;
                    m_SelectedImage.sprite = null;
                    m_SelectedImage.color = Color.clear;
                    m_SelectedImageOutline.enabled = false;

                    m_CurrentOptionPanel = null;
                    break;
                }
        }

        m_CurrentOptionPanel?.SetActive(true);
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

        Canvas_UI.transform.Find("Game_UI").gameObject.SetActive(true);
        Canvas_UI.transform.Find("Edit_UI").gameObject.SetActive(false);

        m_IsEdit = false;
    }
    public void OnPlayExit()
    {
        m_IsEdit = true;

        Canvas_UI.transform.Find("Game_UI").gameObject.SetActive(false);
        Canvas_UI.transform.Find("Edit_UI").gameObject.SetActive(true);

        PlayButton.image.sprite = PlayImage;
    }

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
        input_red.text = (slider_red.value * 255).ToString();
        input_green.text = (slider_green.value * 255f).ToString();
        input_blue.text = (slider_blue.value * 255f).ToString();
    }
    public void UpdateWallOption()
    {
        M_Game.m_WallColor.r = slider_red.value;
        M_Game.m_WallColor.g = slider_green.value;
        M_Game.m_WallColor.b = slider_blue.value;

        m_SelectedImage.color = M_Game.m_WallColor;
    }
    #endregion

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
            #endregion

            #region 키보드
            if (!IsInputFieldFocus)
            {
                ChangeSelectedType();
            }
            #endregion
        }
    }
    #endregion

    #region 기존 함수
    // Enemy
    public void ChangeEnemyType()
    {
        M_Road.m_CurrentSelectedType = (E_EnemyType)enemyType_option.IndexOf(dropdown_EnemyType.captionText.text);

        if (M_Road.m_CurrentSelectedType == E_EnemyType.Linear ||
            M_Road.m_CurrentSelectedType == E_EnemyType.Linear_Repeat)
        {
            Edit_LinearOption.SetActive(true);
            Edit_CircularOption.SetActive(false);
        }
        else if (M_Road.m_CurrentSelectedType == E_EnemyType.Circular)
        {
            Edit_LinearOption.SetActive(false);
            Edit_CircularOption.SetActive(true);
        }
    }
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
