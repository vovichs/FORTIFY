using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class fileList : MonoBehaviour
{
	public static fileList inst;

	public Transform listParent;

	public GameObject buttonPrefab;

	public GameObject folderPrefab;

	public ToggleGroup folderGroup;

	private DirectoryInfo saves;

	private List<DirectoryInfo> dirs;

	public List<folderButton> folders;

	public Toggle dateToggle;

	public static bool dateSort;

	public Toggle jsonToggle;

	public GameObject deleteConfirm;

	public SaveLoad saveLoad;

	public static string saveFolder;

	public Text storedText;

	public string storedPath = "";

	private string savesPath = "";

	private void Awake()
	{
		inst = this;
		if (Application.platform == RuntimePlatform.OSXPlayer)
		{
			savesPath = (Application.dataPath + "/Saves/").Replace("\\", "/");
		}
		else
		{
			savesPath = (Application.dataPath + "\\Saves\\").Replace("/", "\\");
		}
		if (dateSort)
		{
			dateToggle.SetIsOnWithoutNotify(value: true);
		}
		saves = new DirectoryInfo(savesPath);
		dirs = saves.GetDirectories().ToList();
		dirs.Insert(0, saves);
	}

	public void loadList()
	{
		foreach (Transform item in listParent)
		{
			if (item.tag != "folder")
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		List<DirectoryInfo> list = new List<DirectoryInfo>(saves.GetDirectories());
		string @string = PlayerPrefs.GetString("extFolder");
		if (@string != null && Directory.Exists(@string))
		{
			list.Add(new DirectoryInfo(@string));
		}
		bool flag = false;
		if (dirs.Count != list.Count + 1 || folders.Count == 0)
		{
			flag = true;
		}
		else
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Name != dirs[i + 1].Name)
				{
					flag = true;
					break;
				}
			}
		}
		string searchPattern = "*.txt";
		if (jsonToggle.isOn)
		{
			searchPattern = "*.json";
		}
		FileInfo[] array = null;
		if (flag)
		{
			dirs = new List<DirectoryInfo>(list);
			dirs.Insert(0, saves);
			foreach (folderButton folder in folders)
			{
				UnityEngine.Object.Destroy(folder.gameObject);
			}
			folders.Clear();
			bool flag2 = false;
			for (int j = 0; j < dirs.Count; j++)
			{
				if ((dirs[j].Attributes & FileAttributes.Hidden) == (FileAttributes)0)
				{
					folderButton component = UnityEngine.Object.Instantiate(folderPrefab).GetComponent<folderButton>();
					if (j > 0 && dirs[j].Parent.Name != "Saves")
					{
						component.removeButt.SetActive(value: true);
					}
					component.minimized = true;
					component.Text.text = dirs[j].Name;
					component.path = dirs[j].FullName;
					component.tog.group = folderGroup;
					if (dirs[j].Name == saveFolder)
					{
						component.tog.isOn = true;
						flag2 = true;
					}
					component._transform.SetParent(listParent, worldPositionStays: false);
					folders.Add(component);
				}
			}
			if (!flag2)
			{
				folders[0].tog.isOn = true;
			}
			if (dirs.Count == 1)
			{
				folders[0].minimized = false;
			}
		}
		foreach (folderButton folder2 in folders)
		{
			folder2._transform.SetAsLastSibling();
			DirectoryInfo directoryInfo = new DirectoryInfo(folder2.path);
			array = null;
			if (!dateToggle.isOn)
			{
				array = directoryInfo.GetFiles(searchPattern);
				dateSort = false;
			}
			else
			{
				array = (from p in directoryInfo.GetFiles(searchPattern)
					orderby p.CreationTime descending
					select p).ToArray();
				dateSort = true;
			}
			folder2.fileButtons.Clear();
			FileInfo[] array2 = array;
			foreach (FileInfo fileInfo in array2)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(buttonPrefab, listParent);
				buttonSend component2 = gameObject.GetComponent<buttonSend>();
				component2.text.text = "  " + Path.GetFileNameWithoutExtension(fileInfo.Name);
				component2.path = fileInfo.FullName;
				folder2.fileButtons.Add(gameObject);
				if (folder2.minimized)
				{
					gameObject.SetActive(value: false);
				}
			}
		}
	}

	public void addFolder()
	{
		string text = BuilderUI.inst.inputField.text;
		if (text == "")
		{
			popUp.inst.message("type in name for new folder");
			return;
		}
		string path = saves.FullName + text;
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
			loadList();
		}
	}

	public void addExternalFolder()
	{
		string text = BuilderUI.inst.inputField.text;
		if (text == "")
		{
			popUp.inst.message("paste path in filenname field below");
			return;
		}
		text = text.Replace("\"", "");
		if (!Directory.Exists(text))
		{
			popUp.inst.message("invalid directory path");
			popUp.inst.message("paste folder path");
			return;
		}
		popUp.inst.message("external folder added");
		popUp.inst.message("limited to one");
		PlayerPrefs.SetString("extFolder", text);
		loadList();
	}

	public void changeSaveInfo(string path)
	{
		if (path == "")
		{
			storedPath = path;
			storedText.text = "untitled";
			BuilderUI.inst.saveButton.interactable = false;
			BuilderUI.inst.inputField.text = "";
		}
		else
		{
			storedText.text = Path.GetFileNameWithoutExtension(path);
			storedPath = Path.ChangeExtension(path, null);
			BuilderUI.inst.saveButton.interactable = true;
			BuilderUI.inst.inputField.text = storedText.text;
		}
	}

	public void changeFolder(string folderPath)
	{
		if (Directory.Exists(folderPath))
		{
			saveLoad.folderPath = folderPath;
			saveFolder = Path.GetFileNameWithoutExtension(folderPath);
		}
	}

	public void OpenFolder()
	{
		if (Application.platform == RuntimePlatform.OSXPlayer)
		{
			string text = savesPath;
			if (!text.StartsWith("\""))
			{
				text = "\"" + text;
			}
			if (!text.EndsWith("\""))
			{
				text += "\"";
			}
			Process.Start("open", text);
		}
		else
		{
			Process.Start("file://" + savesPath);
		}
	}
}
