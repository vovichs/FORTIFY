using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class buttonWorkshop : MonoBehaviour
{
	public workshop.workshopItem item;

	public Text title;

	private RectTransform rt;

	private bool over;

	public bool sub;

	public void downloadID()
	{
		workshop.inst.downloadContent(item.id);
	}

	public void openWorkshopPage()
	{
		Application.OpenURL("http://steamcommunity.com/sharedfiles/filedetails/?id=" + item.id.ToString());
	}

	public void unsubscribe()
	{
		workshop.inst.Unsubscribe(item.id);
		workshop.subscribedList.RemoveAt(base.transform.GetSiblingIndex());
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void ConfirmPanelOpen()
	{
		workshop.inst.confirmPanel.SetActive(value: true);
		workshop.inst.seletedItem = this;
	}

	public void ConfirmPanelClose()
	{
		workshop.inst.confirmPanel.SetActive(value: false);
	}

	public void mouseOver(bool _over)
	{
		over = _over;
		if (rt == null)
		{
			rt = GetComponent<RectTransform>();
		}
		Vector2 sizeDelta = rt.sizeDelta;
		if (over)
		{
			rt.sizeDelta = new Vector2(sizeDelta.x, sizeDelta.y * 2f);
			base.transform.GetChild(0).gameObject.SetActive(value: true);
		}
		else
		{
			rt.sizeDelta = new Vector2(sizeDelta.x, sizeDelta.y / 2f);
			base.transform.GetChild(0).gameObject.SetActive(value: false);
		}
	}

	[ContextMenu("previewTest")]
	private void addAdditionalPreviewFiles()
	{
		UGCUpdateHandle_t uGCUpdateHandle_t = SteamUGC.StartItemUpdate((AppId_t)505040u, item.id);
		MonoBehaviour.print(uGCUpdateHandle_t);
		string pszPreviewFile = Application.dataPath + "/workshop.png";
		MonoBehaviour.print(SteamUGC.AddItemPreviewFile(uGCUpdateHandle_t, pszPreviewFile, EItemPreviewType.k_EItemPreviewType_Image));
		SteamUGC.SubmitItemUpdate(uGCUpdateHandle_t, null);
	}

	public void OnDisable()
	{
		if (over)
		{
			mouseOver(_over: false);
		}
	}
}
