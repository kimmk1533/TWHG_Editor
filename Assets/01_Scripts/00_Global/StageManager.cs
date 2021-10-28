using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageManager : Singleton<StageManager>
{
	protected E_TileType[,] m_Stage;

	[SerializeField]
	protected bool m_CanSave;

	[SerializeField, ReadOnly(true)]
	protected List<GameObject> m_SaveManagerList;
	protected List<ISaveHandler> m_SaveList;
	protected List<ILoadHandler> m_LoadList;

	protected E_Type m_Type;

	protected string m_StageName;
	[SerializeField]
	protected GameObject m_SaveLoadPanel;
	[SerializeField]
	protected InputField m_StageNameInputField;
	[SerializeField]
	protected Button m_SavePanelButton;
	[SerializeField]
	protected Button m_LoadPanelButton;
	[SerializeField]
	protected Button m_ApplyButton;
	[SerializeField]
	protected TextMeshProUGUI m_Text;

	#region 내부 프로퍼티
	#region 매니저
	protected __GameManager M_Game => __GameManager.Instance;
	protected __EditManager M_Edit => __EditManager.Instance;

	protected PlayerManager M_Player => PlayerManager.Instance;
	protected EnemyManager M_Enemy => EnemyManager.Instance;
	protected CoinManager M_Coin => CoinManager.Instance;
	protected WallManager M_Wall => WallManager.Instance;
	protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
	protected TileManager M_Tile => TileManager.Instance;
	protected FloatingTextManager M_FloatingText => FloatingTextManager.Instance;
	#endregion

	protected int width { get => M_Game.width; }
	protected int height { get => M_Game.height; }

	protected bool saveButtonActive
	{
		get => m_SavePanelButton.gameObject.activeSelf;
		set => m_SavePanelButton.gameObject.SetActive(value);
	}
	protected bool loadButtonActive
	{
		get => m_LoadPanelButton.gameObject.activeSelf;
		set => m_LoadPanelButton.gameObject.SetActive(value);
	}
	#endregion
	#region 외부 프로퍼티
	public E_TileType[,] stage { get => m_Stage; }
	public string stageName { get => m_StageName; set => m_StageName = value; }
	public bool canSave
	{
		get => m_CanSave;
		set
		{
			m_CanSave = value;
			panelActive = false;

			Color color = m_SavePanelButton.image.color;
			if (!m_CanSave)
			{
				color.a = 0.5f;
				m_SavePanelButton.image.color = color;
			}
			else
			{
				color.a = 1f;
				m_SavePanelButton.image.color = color;
			}
		}
	}
	public bool panelActive { get => m_SaveLoadPanel.activeSelf; set => m_SaveLoadPanel.SetActive(value); }
	#endregion
	#region 내부 함수
	protected void ResetStage()
	{
		for (int y = 0; y < height; ++y)
		{
			for (int x = 0; x < width; ++x)
			{
				m_Stage[y, x] = E_TileType.None;
			}
		}
	}
	#endregion
	#region 외부 함수
	public void __Initialize()
	{
		M_Game.onPlayModeEnter += OnEnterPlayMode;
		M_Game.onPlayModeExit += OnExitPlayMode;

		m_Stage = new E_TileType[height, width];

		ResetStage();

		if (null == m_SaveList)
		{
			m_SaveList = new List<ISaveHandler>();
		}
		if (null == m_LoadList)
		{
			m_LoadList = new List<ILoadHandler>();
		}

		foreach (var item in m_SaveManagerList)
		{
			m_SaveList.Add(item.GetComponent<ISaveHandler>());
			m_LoadList.Add(item.GetComponent<ILoadHandler>());
		}

		m_CanSave = false;
		panelActive = false;

		Color color = m_SavePanelButton.image.color;
		color.a = 0.5f;
		m_SavePanelButton.image.color = color;
		saveButtonActive = true;
		loadButtonActive = true;

		m_StageNameInputField.onEndEdit.AddListener(item =>
		{
			m_StageName = item;
		});

		m_SavePanelButton.onClick.AddListener(() =>
		{
			if (!m_CanSave)
			{
				M_FloatingText.SpawnFloatingText("클리어 후에 저장이 가능합니다.", new Vector2(0.5f, 0.5f));
				return;
			}

			m_Type = E_Type.Save;
			m_Text.text = "Save";
			panelActive = true;

			m_StageNameInputField.Select();
			m_StageNameInputField.ActivateInputField();
		});
		m_LoadPanelButton.onClick.AddListener(() =>
		{
			m_Type = E_Type.Load;
			m_Text.text = "Load";
			panelActive = true;

			m_StageNameInputField.Select();
			m_StageNameInputField.ActivateInputField();
		});
		m_ApplyButton.onClick.AddListener(() =>
		{
			switch (m_Type)
			{
				case E_Type.Save:
					SaveStage(stageName);
					break;
				case E_Type.Load:
					LoadStage(stageName);
					break;
			}

			panelActive = false;
		});
	}
	public void __Finalize()
	{
		ResetStage();
	}

	public void SaveStage(string stageName)
	{
		string path = Path.Combine(Application.dataPath, "Stage");
		string file = stageName;
		if (null == file)
		{
			M_FloatingText.SpawnFloatingText("저장 실패");
			M_FloatingText.SpawnFloatingText("스테이지 이름을 입력해주세요", 0.5f);
			return;
		}
		if (!file.EndsWith(".std"))
		{
			file += ".std";
		}
		string filepath = Path.Combine(path, file);

		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		XmlWriter writer = XmlWriter.Create(filepath, settings);

		try
		{
			// 루트 시작
			writer.WriteStartElement("root");

			//// 주석
			//writer.WriteComment("버젼");
			//// 버젼 시작
			//writer.WriteStartElement("Game Version");
			//// 버젼 입력
			//writer.WriteValue(m_Version);
			//// 버젼 끝
			//writer.WriteEndElement();

			foreach (var item in m_SaveList)
			{
				item?.Save(writer);
			}

			// 루트 끝
			writer.WriteEndElement();

			writer.Close();

			File.WriteAllText(filepath, Encrypt.EncryptData(filepath));

			M_FloatingText.SpawnFloatingText("저장 완료");
		}
		catch (Exception e)
		{
			M_FloatingText.SpawnFloatingText("저장 실패");
			M_FloatingText.SpawnFloatingText(e.Message, 0.5f);
		}
		finally
		{
			writer.Close();
			writer.Dispose();
		}
	}
	public void LoadStage(string stageName)
	{
		string path = Path.Combine(Application.dataPath, "Stage", stageName + ".std");

		if (!File.Exists(path))
		{
			M_FloatingText.SpawnFloatingText("불러오기 실패");
			M_FloatingText.SpawnFloatingText("파일이 존재하지 않습니다.", 0.5f);
			return;
		}

		StringReader xml = new StringReader(Decrypt.DecryptData(path));
		XmlReader reader = XmlReader.Create(xml);

		try
		{
			foreach (var item in m_LoadList)
			{
				item?.Load(reader);
			}

			M_FloatingText.SpawnFloatingText("불러오기 완료");
		}
		catch (Exception e)
		{
			M_FloatingText.SpawnFloatingText("불러오기 실패");
			M_FloatingText.SpawnFloatingText(e.Message, 0.5f);
		}
		finally
		{
			reader.Close();
			reader.Dispose();
		}
	}
	#endregion
	#region 이벤트 함수
	public void OnEnterPlayMode()
	{
		panelActive = false;
		saveButtonActive = false;
		loadButtonActive = false;
	}
	public void OnExitPlayMode()
	{
		saveButtonActive = true;
		loadButtonActive = true;
	}
	#endregion

	protected enum E_Type
	{
		None = -1,

		Save,
		Load
	}
}
