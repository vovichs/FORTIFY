using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class buttonSend : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public static GameObject deleteButton;

	public GameObject x;

	public Text text;

	public string path;

	public void SendLoadPath()
	{
		if (path != null)
		{
			if (fileList.inst.jsonToggle.isOn)
			{
				RustCopyPaste.inst.CopyPasteFile(path);
			}
			else if (BuilderUI.inst.mergeToggle.isOn)
			{
				fileList.inst.saveLoad.MergeFile(path);
			}
			else
			{
				fileList.inst.saveLoad.loadFileButton(path);
			}
		}
	}

	public void confirmDelete()
	{
		GameObject deleteConfirm = fileList.inst.deleteConfirm;
		if (path != null && !fileList.inst.deleteConfirm.activeSelf)
		{
			deleteConfirm.SetActive(value: true);
			deleteConfirm.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileNameWithoutExtension(path);
			deleteButton = base.gameObject;
		}
	}

	public void fileDelete()
	{
		File.Delete(deleteButton.GetComponent<buttonSend>().path);
		UnityEngine.Object.Destroy(deleteButton);
		deleteButton = null;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (x != null)
		{
			x.SetActive(value: true);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (x != null)
		{
			x.SetActive(value: false);
		}
	}
}
