using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum E_SelectedType
{
    None,
    Player,
    Enemy,
    Coin,
    SafetyZone,
    Wall,
    Erase,
}

public class __EditManager : Singleton<__EditManager>
{
    [Header("Canvas")]
    public Canvas Canvas_BG;
    public Canvas Canvas_UI;

    [Header("EditMode")]
    public bool isEdit;

    [Header("Selected")]
    public E_SelectedType currentSelectedType;
    public Image currentSelectedImage;
    public Text currentSelectedText;

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
    public InputField input_red;
    public Slider slider_green;
    public InputField input_green;
    public Slider slider_blue;
    public InputField input_blue;

    // Managers
    GameObject PlayButton;

    __GameManager m_Game;
    PlayerManager m_Player;
    RoadManager m_Road;
    SafetyZoneManager m_SafetyZone;
    //

    protected void Awake()
    {
        isEdit = true;

        PlayButton = Canvas_UI.transform.Find("Public_UI").Find("Play").Find("Button").gameObject;

        m_Game = __GameManager.Instance;
        m_Player = PlayerManager.Instance;
        m_Road = RoadManager.Instance;
        m_SafetyZone = SafetyZoneManager.Instance;
    }

    public void TestPlay()
    {
        if (isEdit &&
            m_SafetyZone.Colliders.Count >= 2)
        {
            currentSelectedType = E_SelectedType.None;

            currentSelectedImage.color = Color.white * 0f;

            m_Game.EnterPlayMode();
        }
        else if (!isEdit)
        {
            m_Game.ExitPlayMode();
        }
    }

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

    public void __Initialize()
    {
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
        slider_red.value = m_Game.Wall.r;
        slider_green.value = m_Game.Wall.g;
        slider_blue.value = m_Game.Wall.b;
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

        switch (currentSelectedType)
        {
            case E_SelectedType.Enemy:
                ChangeEnemyType();
                Edit_Enemy.SetActive(true);
                break;
            case E_SelectedType.SafetyZone:
                UpdateSafetyZoneOption();
                Edit_SafetyZone.SetActive(true);
                break;
            case E_SelectedType.Wall:
                currentSelectedImage.color = m_Game.Wall;
                Edit_Wall.SetActive(true);
                break;
        }
    }
    // Enemy
    public void ChangeEnemyType()
    {
        m_Road.currentSelectedType = (E_EnemyType)enemyType_option.IndexOf(dropdown_EnemyType.captionText.text);

        if (m_Road.currentSelectedType == E_EnemyType.Linear ||
            m_Road.currentSelectedType == E_EnemyType.Linear_Repeat)
        {
            Edit_LinearOption.SetActive(true);
            Edit_CircularOption.SetActive(false);
        }
        else if (m_Road.currentSelectedType == E_EnemyType.Circular)
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

        m_SafetyZone.StartPoint = m_SafetyZone.Colliders[first];
        m_SafetyZone.EndPoint = m_SafetyZone.Colliders[last];

        m_Player.Player.transform.position = m_SafetyZone.StartPoint.GetCenter();
    }
    public void UpdateSafetyZoneOption()
    {
        safetyZone_option_first.Clear();
        safetyZone_option_last.Clear();

        for (int i = 0; i < m_SafetyZone.SafetyZones.Count; ++i)
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
        m_Game.Wall.r = slider_red.value;
        m_Game.Wall.g = slider_green.value;
        m_Game.Wall.b = slider_blue.value;
        currentSelectedImage.color = m_Game.Wall;
    }
}
