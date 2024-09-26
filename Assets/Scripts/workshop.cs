using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class workshop : MonoBehaviour
{
	public class workshopItem
	{
		public PublishedFileId_t id;

		public UGCHandle_t handle;

		public string title;

		public workshopItem(PublishedFileId_t Id, UGCHandle_t Handle, string Title)
		{
			id = Id;
			title = Title;
		}
	}

	public static workshop inst;

	private CallResult<RemoteStoragePublishFileResult_t> PublishFileResult;

	private CallResult<RemoteStorageGetPublishedFileDetailsResult_t> GetPublishedFileDetailsResult;

	private CallResult<RemoteStorageDownloadUGCResult_t> DownloadUGCResult;

	private CallResult<RemoteStorageUpdatePublishedFileResult_t> UpdatePublishedFileResult;

	private CallResult<SteamUGCQueryCompleted_t> OnSteamUGCQueryCompletedCallResult;

	private CallResult<SteamUGCRequestUGCDetailsResult_t> OnSteamUGCRequestUGCDetailsResultCallResult;

	private CallResult<RemoteStorageUnsubscribePublishedFileResult_t> UnsubscribePublishedFileResult;

	private static CSteamID userId;

	private PublishedFileId_t publishedFileID;

	public buttonWorkshop seletedItem;

	public static List<workshopItem> subscribedList = new List<workshopItem>();

	public static List<workshopItem> publishedList = new List<workshopItem>();

	public static bool itemDetailsCheck;

	public bool wait;

	public uint itemsLeft;

	private bool showPublished;

	private string tempFileName;

	public GameObject openItemPageButt;

	public GameObject refreshBlock;

	public Toggle workshopToggle;

	public Toggle infoToggle;

	public Dropdown tagDD;

	public Toggle[] tagTogs;

	public Image[] subpubImages;

	public Button publishBut;

	public Text titleText;

	public Text tagsLabel;

	public Canvas canvas;

	public Toggle prevUpdateTog;

	public GameObject screenArea;

	private Texture2D screenshot;

	public RawImage previewImg;

	private string previewName;

	private string lastWorkshopTitle = "";

	public Text tagDisplay;

	private List<string> tagList;

	private string[] tagArr;

	public GameObject fileParent;

	public GameObject buttonPrefabSub;

	public GameObject buttonPrefabUpdate;

	public GameObject endFiller;

	public GameObject confirmPanel;

	private void Awake()
	{
		inst = this;
		publishBut.interactable = true;
		tagList = new List<string>();
		tagList.Add("Building 4.0");
	}

	private void Start()
	{
		if (SteamManager.Initialized)
		{
			workshopToggle.interactable = true;
			PublishFileResult = CallResult<RemoteStoragePublishFileResult_t>.Create(OnPublishFileResult);
			DownloadUGCResult = CallResult<RemoteStorageDownloadUGCResult_t>.Create(OnDownloadUGCResult);
			OnSteamUGCQueryCompletedCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(OnSteamUGCQueryCompleted);
			GetPublishedFileDetailsResult = CallResult<RemoteStorageGetPublishedFileDetailsResult_t>.Create(OnGetPublishedFileDetailsResult);
			UnsubscribePublishedFileResult = CallResult<RemoteStorageUnsubscribePublishedFileResult_t>.Create(OnUnsubscribePublishedFileResult);
			UpdatePublishedFileResult = CallResult<RemoteStorageUpdatePublishedFileResult_t>.Create(OnUpdatePublishedFileResult);
			if (!itemDetailsCheck)
			{
				itemDetailsCheck = true;
				userId = SteamUser.GetSteamID();
				StartCoroutine(GetWorkshopItemLists());
			}
			else
			{
				showPublishedState(showPublished);
			}
		}
	}

	public void refreshList()
	{
		foreach (Transform item in fileParent.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		publishedList.Clear();
		subscribedList.Clear();
		StartCoroutine(GetWorkshopItemLists());
	}

	public void showPublishedState(bool published)
	{
		showPublished = published;
		foreach (Transform item in fileParent.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		if (showPublished)
		{
			foreach (workshopItem published2 in publishedList)
			{
				addButton(published2);
			}
		}
		else
		{
			foreach (workshopItem subscribed in subscribedList)
			{
				addButton(subscribed);
			}
		}
	}

	private IEnumerator GetWorkshopItemLists()
	{
		itemsLeft = 1000u;
		uint page2 = 0u;
		while (itemsLeft != 0)
		{
			wait = true;
			page2++;
			GetSubscribed(page2);
			yield return new WaitUntil(() => !wait);
		}
		itemsLeft = 1000u;
		page2 = 0u;
		while (itemsLeft != 0)
		{
			wait = true;
			page2++;
			GetPublished(page2);
			yield return new WaitUntil(() => !wait);
		}
	}

	private void GetSubscribed(uint pageNum)
	{
		SteamAPICall_t hAPICall = SteamUGC.SendQueryUGCRequest(SteamUGC.CreateQueryUserUGCRequest(userId.GetAccountID(), EUserUGCList.k_EUserUGCList_Subscribed, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, EUserUGCListSortOrder.k_EUserUGCListSortOrder_SubscriptionDateDesc, AppId_t.Invalid, (AppId_t)505040u, pageNum));
		OnSteamUGCQueryCompletedCallResult.Set(hAPICall);
	}

	private void GetPublished(uint pageNum)
	{
		SteamAPICall_t hAPICall = SteamUGC.SendQueryUGCRequest(SteamUGC.CreateQueryUserUGCRequest(userId.GetAccountID(), EUserUGCList.k_EUserUGCList_Published, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderAsc, AppId_t.Invalid, (AppId_t)505040u, pageNum));
		OnSteamUGCQueryCompletedCallResult.Set(hAPICall);
	}

	private void OnSteamUGCQueryCompleted(SteamUGCQueryCompleted_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_eResult == EResult.k_EResultOK)
		{
			if (itemsLeft == 1000)
			{
				itemsLeft = pCallback.m_unTotalMatchingResults - pCallback.m_unNumResultsReturned;
			}
			else
			{
				itemsLeft -= pCallback.m_unNumResultsReturned;
			}
			for (int i = 0; i < pCallback.m_unNumResultsReturned; i++)
			{
				if (!SteamUGC.GetQueryUGCResult(pCallback.m_handle, (uint)i, out SteamUGCDetails_t pDetails))
				{
					continue;
				}
				workshopItem item = new workshopItem(pDetails.m_nPublishedFileId, pDetails.m_hFile, pDetails.m_rgchTitle);
				if ((CSteamID)pDetails.m_ulSteamIDOwner == userId)
				{
					publishedList.Add(item);
					if (showPublished)
					{
						addButton(item);
					}
				}
				else
				{
					subscribedList.Add(item);
					if (!showPublished)
					{
						addButton(item);
					}
				}
			}
		}
		SteamUGC.ReleaseQueryUGCRequest(pCallback.m_handle);
		wait = false;
	}

	private void addButton(workshopItem item)
	{
		GameObject gameObject = null;
		gameObject = ((!showPublished) ? UnityEngine.Object.Instantiate(buttonPrefabSub) : UnityEngine.Object.Instantiate(buttonPrefabUpdate));
		buttonWorkshop component = gameObject.GetComponent<buttonWorkshop>();
		component.title.text = item.title;
		component.item = item;
		gameObject.transform.SetParent(fileParent.transform, worldPositionStays: false);
	}

	public void downloadContent(PublishedFileId_t id)
	{
		popUp.inst.message("downloading");
		SteamAPICall_t publishedFileDetails = SteamRemoteStorage.GetPublishedFileDetails(id, 0u);
		GetPublishedFileDetailsResult.Set(publishedFileDetails);
	}

	private void OnGetPublishedFileDetailsResult(RemoteStorageGetPublishedFileDetailsResult_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_eResult == EResult.k_EResultOK)
		{
			SteamAPICall_t hAPICall = SteamRemoteStorage.UGCDownload(pCallback.m_hFile, 0u);
			DownloadUGCResult.Set(hAPICall);
		}
		else
		{
			popUp.inst.message("item details error");
		}
	}

	private void OnDownloadUGCResult(RemoteStorageDownloadUGCResult_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_eResult != EResult.k_EResultOK)
		{
			popUp.inst.message("item download error");
			popUp.inst.message(pCallback.m_eResult.ToString());
			return;
		}
		byte[] array = new byte[pCallback.m_nSizeInBytes];
		int count = SteamRemoteStorage.UGCRead(pCallback.m_hFile, array, pCallback.m_nSizeInBytes, 0u, EUGCReadAction.k_EUGCRead_Close);
		string @string = Encoding.UTF8.GetString(array, 0, count);
		canvas.GetComponent<SaveLoad>().WorkshopLoad(@string);
		fileList.inst.changeSaveInfo("");
		BuilderUI.inst.inputField.text = pCallback.m_pchFileName.Replace(".txt", "");
	}

	public void publishContent()
	{
		tagArr = tagList.ToArray();
		if (tagArr.Length == 0)
		{
			tagArr = null;
		}
		if (!screenshot)
		{
			popUp.inst.message("get preview first");
			return;
		}
		if (titleText.text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
		{
			popUp.inst.message("invalid character in name");
			return;
		}
		if (lastWorkshopTitle == titleText.text)
		{
			popUp.inst.message("duplicate workshop title");
			return;
		}
		publishBut.interactable = false;
		SaveToWorkshop(titleText.text + ".txt", canvas.GetComponent<SaveLoad>().Save(infoToggle.isOn), titleText.text, "", tagArr);
	}

	public void SaveToWorkshop(string fileName, string fileData, string workshopTitle, string workshopDescription, string[] tags)
	{
		tempFileName = fileName;
		if (SteamRemoteStorage.FileExists(fileName))
		{
			popUp.inst.message("file already exists with that name");
			publishBut.interactable = true;
			return;
		}
		byte[] array = File.ReadAllBytes(Application.dataPath + "/workshop.png");
		SteamRemoteStorage.FileWrite("workshop.png", array, array.Length);
		if (!UploadFile(fileName, fileData))
		{
			popUp.inst.message("upload failed");
			publishBut.interactable = true;
		}
		else
		{
			UploadToWorkshop(fileName, workshopTitle, workshopDescription, tags);
		}
	}

	private bool UploadFile(string fileName, string fileData)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(fileData)];
		Encoding.UTF8.GetBytes(fileData, 0, fileData.Length, array, 0);
		return SteamRemoteStorage.FileWrite(fileName, array, array.Length);
	}

	private void UploadToWorkshop(string fileName, string workshopTitle, string workshopDescription, string[] tags)
	{
		previewName = "workshop.png";
		if (!File.Exists(Application.dataPath + "/workshop.png"))
		{
			previewName = null;
		}
		lastWorkshopTitle = workshopTitle;
		SteamAPICall_t hAPICall = SteamRemoteStorage.PublishWorkshopFile(fileName, previewName, SteamUtils.GetAppID(), workshopTitle, workshopDescription, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic, tags, EWorkshopFileType.k_EWorkshopFileTypeFirst);
		PublishFileResult.Set(hAPICall);
	}

	private void OnPublishFileResult(RemoteStoragePublishFileResult_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_eResult == EResult.k_EResultOK)
		{
			popUp.inst.message("published - takes a minute to appear");
			publishedFileID = pCallback.m_nPublishedFileId;
			openItemPageButt.SetActive(value: true);
			openItemPageButt.GetComponent<buttonWorkshop>().item = new workshopItem(publishedFileID, default(UGCHandle_t), "");
			titleText.text = "";
		}
		else
		{
			popUp.inst.message("unable to publish");
			lastWorkshopTitle = "";
			popUp.inst.message(pCallback.m_eResult.ToString());
		}
		publishBut.interactable = true;
		SteamRemoteStorage.FileDelete(tempFileName);
		if (SteamRemoteStorage.FileExists(previewName))
		{
			SteamRemoteStorage.FileDelete(previewName);
		}
	}

	public void Unsubscribe(PublishedFileId_t id)
	{
		SteamAPICall_t hAPICall = SteamRemoteStorage.UnsubscribePublishedFile(id);
		UnsubscribePublishedFileResult.Set(hAPICall);
	}

	private void OnUnsubscribePublishedFileResult(RemoteStorageUnsubscribePublishedFileResult_t pCallback, bool bIOFailure)
	{
	}

	public void UpdatePublished()
	{
		tempFileName = seletedItem.title.text;
		string text = canvas.GetComponent<SaveLoad>().Save(infoToggle.isOn);
		byte[] array = new byte[Encoding.UTF8.GetByteCount(text)];
		Encoding.UTF8.GetBytes(text, 0, text.Length, array, 0);
		if (!SteamRemoteStorage.FileWrite(tempFileName, array, array.Length))
		{
			popUp.inst.message("upload failed");
			return;
		}
		PublishedFileUpdateHandle_t updateHandle = SteamRemoteStorage.CreatePublishedFileUpdateRequest(seletedItem.item.id);
		SteamRemoteStorage.UpdatePublishedFileFile(updateHandle, tempFileName);
		if (prevUpdateTog.isOn)
		{
			if (!File.Exists(Application.dataPath + "/workshop.png"))
			{
				popUp.inst.message("preview image missing");
			}
			else
			{
				byte[] array2 = File.ReadAllBytes(Application.dataPath + "/workshop.png");
				SteamRemoteStorage.FileWrite("workshop.png", array2, array2.Length);
				SteamRemoteStorage.UpdatePublishedFilePreviewFile(updateHandle, "workshop.png");
			}
		}
		SteamAPICall_t hAPICall = SteamRemoteStorage.CommitPublishedFileUpdate(updateHandle);
		UpdatePublishedFileResult.Set(hAPICall);
	}

	private void OnUpdatePublishedFileResult(RemoteStorageUpdatePublishedFileResult_t pCallback, bool bIOFailure)
	{
		popUp.inst.message("update complete");
		SteamRemoteStorage.FileDelete(tempFileName);
		confirmPanel.SetActive(value: false);
	}

	private void addAdditionalPreviewFiles(UGCUpdateHandle_t handle)
	{
		string pszPreviewFile = "localfile";
		SteamUGC.AddItemPreviewFile(handle, pszPreviewFile, EItemPreviewType.k_EItemPreviewType_Image);
	}

	public void getPreview()
	{
		StartCoroutine(areaScreenshot());
	}

	private IEnumerator areaScreenshot()
	{
		yield return new WaitForEndOfFrame();
		Rect rect = screenArea.GetComponent<RectTransform>().rect;
		int width = Mathf.RoundToInt(rect.width * canvas.scaleFactor);
		int height = Mathf.RoundToInt(rect.height * canvas.scaleFactor);
		if (screenshot != null)
		{
			UnityEngine.Object.Destroy(screenshot);
		}
		screenshot = new Texture2D(width, height, TextureFormat.RGB24, mipChain: false);
		screenshot.ReadPixels(GetScreenCoordinates(screenArea.GetComponent<RectTransform>()), 0, 0);
		screenshot.Apply();
		byte[] bytes = screenshot.EncodeToPNG();
		previewImg.enabled = true;
		previewImg.texture = screenshot;
		File.WriteAllBytes(Application.dataPath + "/workshop.png", bytes);
	}

	public Rect GetScreenCoordinates(RectTransform UIelement)
	{
		Vector3[] array = new Vector3[4];
		UIelement.GetWorldCorners(array);
		return new Rect(array[0].x, array[0].y, array[2].x - array[0].x, array[2].y - array[0].y);
	}

	public void addTag()
	{
		if (tagDD.value != 0)
		{
			string text = tagDD.captionText.text;
			if (!tagList.Contains(text))
			{
				tagList.Add(text);
				Text text2 = tagDisplay;
				text2.text = text2.text + text + ", ";
			}
		}
	}

	public void updateTags()
	{
		tagList.Clear();
		string text = "Tags: ";
		for (int i = 0; i < tagTogs.Length; i++)
		{
			if (tagTogs[i].isOn)
			{
				text = text + tagTogs[i].name + ", ";
				tagList.Add(tagTogs[i].name);
			}
			else if (i == 0)
			{
				tagList.Add("Building 4.0");
			}
		}
		tagDisplay.text = text;
	}

	public void clearTags()
	{
		Toggle[] array = tagTogs;
		foreach (Toggle toggle in array)
		{
			if (toggle.isOn)
			{
				toggle.isOn = false;
			}
		}
	}

	public void openWorkshopPage()
	{
		string str = publishedFileID.ToString();
		Application.OpenURL("http://steamcommunity.com/sharedfiles/filedetails/?id=" + str);
	}

	public void workshopPage()
	{
		SteamFriends.ActivateGameOverlayToWebPage("http://steamcommunity.com/app/505040/workshop/");
	}
}
