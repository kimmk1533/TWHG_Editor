using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class __EditManager : Singleton<__EditManager>
{
    [Header("Canvas")]
    public Canvas Canvas_BG;
    public Canvas Canvas_UI;
    public GraphicRaycaster m_Raycaster_BG;

    [Header("EditMode")]
    public bool isEdit;

    [Header("Selected")]
    public E_ObjectType CurrentSelectedType;
    public Image CurrentSelectedImage;
    public Text CurrentSelectedText;

    [Header("PlayButton")]
    public Sprite PlayImage;
    public Sprite ResetImage;

    [Header("Public Option")]
    public GameObject Edit_Public;

    [Header("Enemy Option")]
    public GameObject Edit_Enemy;
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
    public GameObject Edit_SafetyZone;
    public Dropdown dropdown_first;
    public Dropdown dropdown_last;
    List<string> safetyZone_option_first;
    List<string> safetyZone_option_last;

    [Header("Wall Option")]
    public GameObject Edit_Wall;
    public Slider slider_red;
    public Slider slider_green;
    public Slider slider_blue;
    public InputField input_red;
    public InputField input_green;
    public InputField input_blue;

    GameObject PlayButton;

    #region 내부 프로퍼티
    #region 매니져
    protected __GameManager M_Game => __GameManager.Instance;
    protected PlayerManager M_Player => PlayerManager.Instance;
    protected EnemyRoadManager M_Road => EnemyRoadManager.Instance;
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    protected CoinManager M_Coin => CoinManager.Instance;
    protected TileManager M_Tile => TileManager.Instance;
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
    protected bool IsLeft => Input.GetMouseButton(0);
    protected bool IsRight => Input.GetMouseButton(1);
    protected bool IsLeftDown => Input.GetMouseButtonDown(0);
    protected bool IsRightDown => Input.GetMouseButtonDown(1);
    #endregion
    #region 외부 프로퍼티
    public bool IsEditMode => isEdit;
    #endregion
    #region 내부 함수
    #endregion
    #region 유니티 콜백 함수
    protected void Awake()
    {
        isEdit = true;

        PlayButton = Canvas_UI.transform.Find("Public_UI").Find("Play").Find("Button").gameObject;
    }
    private void Update()
    {
        if (isEdit && IsMouseDown)
        {
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
            #endregion

            if (results.Count <= 0)
            {
                Debug.Log("레이 오류");
                return;
            }

            GameObject obj = obj_hit.transform?.gameObject;
            GameObject ui_obj = results[0].gameObject;

            #region Click Process

            if (IsLeftDown)
            {
                switch (CurrentSelectedType)
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
            else if (IsRightDown)
            {
                obj?.GetComponent<IEraserable>()?.Erase();
            }
            #endregion

            #region Drag Process
            // 레이를 맞은 오브젝트가 있으면 동작
            if (results.Count > 0)
            {
                if (IsLeft)
                {
                    switch (CurrentSelectedType)
                    {
                        case E_ObjectType.Wall:
                            M_Tile.Draw(ui_obj, E_TileType.Wall);
                            break;
                        case E_ObjectType.SafetyZone:
                            M_Tile.Draw(ui_obj, E_TileType.SafetyZone);
                            obj?.GetComponent<WallCollider>()?.Erase();
                            break;
                        case E_ObjectType.Erase:
                            M_Tile.Draw(ui_obj, E_TileType.None);
                            break;
                    }
                }
                else if (IsRight)
                {
                    M_Tile.Draw(ui_obj, E_TileType.None);
                    obj?.GetComponent<IEraserable>()?.Erase();
                }
            }
            #endregion
        }
    }
    #endregion

    #region 외부 함수
    public void __Initialize()
    {
        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;

        // 레이캐스터 설정
        if (null == m_Raycaster_BG)
        {
            if (null == Canvas_BG)
            {
                Debug.LogError("EditManager: Null Canvas");
                return;
            }

            m_Raycaster_BG = Canvas_BG.GetComponent<GraphicRaycaster>();
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

        // SafetyZone
        safetyZone_option_first = new List<string>();
        safetyZone_option_last = new List<string>();

        // Wall
        slider_red.value = M_Game.m_Wall.r;
        slider_green.value = M_Game.m_Wall.g;
        slider_blue.value = M_Game.m_Wall.b;
        ColorToText();
    }
    public void __Finalize()
    {

    }

    public void UpdateOption()
    {
        //Edit_Public.SetActive(true); // 미구현
        Edit_Public.SetActive(false);
        Edit_Enemy.SetActive(false);
        Edit_SafetyZone.SetActive(false);
        Edit_Wall.SetActive(false);

        switch (CurrentSelectedType)
        {
            case E_ObjectType.Enemy:
                ChangeEnemyType();
                Edit_Enemy.SetActive(true);
                break;
            case E_ObjectType.SafetyZone:
                UpdateSafetyZoneOption();
                Edit_SafetyZone.SetActive(true);
                break;
            case E_ObjectType.Wall:
                CurrentSelectedImage.color = M_Game.m_Wall;
                Edit_Wall.SetActive(true);
                break;
        }
    }
    #endregion

    #region 이벤트 함수
    public void OnPlayEnter()
    {
        PlayButton.GetComponent<Image>().sprite = ResetImage;

        Canvas_UI.transform.Find("Game_UI").gameObject.SetActive(true);
        Canvas_UI.transform.Find("Edit_UI").gameObject.SetActive(false);

        isEdit = false;
    }
    public void OnPlayExit()
    {
        isEdit = true;

        Canvas_UI.transform.Find("Game_UI").gameObject.SetActive(false);
        Canvas_UI.transform.Find("Edit_UI").gameObject.SetActive(true);

        PlayButton.GetComponent<Image>().sprite = PlayImage;
    }

    public void TestPlay()
    {
        if (isEdit &&
            M_SafetyZone.m_ColliderList.Count >= 2)
        {
            CurrentSelectedType = E_ObjectType.None;

            CurrentSelectedImage.color = Color.white * 0f;

            M_Game.EnterPlayMode();
        }
        else if (!isEdit)
        {
            M_Game.ExitPlayMode();
        }
    }
    #endregion

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
    // SafetyZone
    public void UpdateSafetyZone()
    {
        int first = dropdown_first.value;
        int last = dropdown_last.value;

        M_SafetyZone.m_StartPoint = M_SafetyZone.m_ColliderList[first];
        M_SafetyZone.m_EndPoint = M_SafetyZone.m_ColliderList[last];

        M_Player.SpawnPlayer(M_SafetyZone.m_StartPoint.GetCenter());
    }
    public void UpdateSafetyZoneOption()
    {
        safetyZone_option_first.Clear();
        safetyZone_option_last.Clear();

        for (int i = 0; i < M_SafetyZone.m_SafetyZoneList.Count; ++i)
        {
            safetyZone_option_first.Add("index(" + (i + 1).ToString() + ")");
            safetyZone_option_last.Add("index(" + (i + 1).ToString() + ")");
        }

        dropdown_first.AddOptions(safetyZone_option_first);
        dropdown_last.AddOptions(safetyZone_option_last);
    }

    // Wall
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
        M_Game.m_Wall.r = slider_red.value;
        M_Game.m_Wall.g = slider_green.value;
        M_Game.m_Wall.b = slider_blue.value;
        CurrentSelectedImage.color = M_Game.m_Wall;
    }
}
