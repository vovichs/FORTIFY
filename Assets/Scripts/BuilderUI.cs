using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuilderUI : MonoBehaviour
{
	public static BuilderUI inst;

	private BuilderSystem sys;

	private BuilderHideFloors hf;

	public int scene;

	public Button saveButton;

	public Toggle menuToggle;

	public GameObject menuPanel;

	public GameObject cameraControls;

	public GameObject buildPanel;

	public Canvas stbltCanvas;

	public Canvas mainCanvas;

	public Toggle editToggle;

	public Toggle saveSelectionTog;

	public Toggle modeToggle;

	public Toggle screenshotToggle;

	public Toggle hideToggle;

	public Toggle rectSelToggle;

	public Toggle mergeToggle;

	public InputField inputField;

	public GameObject quickPicker;

	public Toggle addCeilingTog;

	public Dropdown partFillOptions;

	public GameObject exitCanvas;

	public Toggle[] tierToggles;

	public GameObject guidePopUp;

	public Toggle snapTog;

	public Toggle[] includeToggles;

	public GameObject blockPanel;

	public Options options;

	public string[] lastUsed;

	private bool allowQuit;

	public bool mouseOverUI;

	private void Awake()
	{
		inst = this;
		sys = BuilderSystem.inst;
		hf = GetComponent<BuilderHideFloors>();
		if (BuilderSystem.saveSelection)
		{
			saveSelectionTog.isOn = true;
		}
		options.GetPrefs();
	}

	private void Update()
	{
		mouseOverUI = EventSystem.current.IsPointerOverGameObject();
		if (!Input.anyKey)
		{
			return;
		}
		if (!BuilderSystem.disableInput || CameraCtrl.inst.screenshotMode)
		{
			if (Input.GetButtonDown("Hide"))
			{
				hideToggle.isOn = !hideToggle.isOn;
			}
			if (sys.hideMode)
			{
				if (Input.GetButtonDown("floorUP"))
				{
					hf.HideFloors(1, send: true);
					return;
				}
				if (Input.GetButtonDown("floorDN"))
				{
					hf.HideFloors(-1, send: true);
					return;
				}
			}
		}
		if (Stability.inst.raidMode)
		{
			return;
		}
		if (Input.GetButtonDown("menu") && CameraCtrl.inst.mode != CameraCtrl.Mode.mount)
		{
			menuToggle.isOn = !menuToggle.isOn;
		}
		else
		{
			if (BuilderSystem.disableInput)
			{
				return;
			}
			if (!wiring.inst.on)
			{
				if (!BuilderSystem.editMode && Input.GetButtonDown("Swap"))
				{
					SwapSimilar(toggleParts.togGroupId);
				}
				if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad1))
				{
					SwapSimilar(1);
					return;
				}
				if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad2))
				{
					SwapSimilar(2);
					return;
				}
				if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad3))
				{
					SwapSimilar(3);
					return;
				}
				if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad4))
				{
					SwapSimilar(4);
					return;
				}
				if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad5))
				{
					SwapSimilar(5);
					return;
				}
				if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha6) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad6))
				{
					SwapSimilar(6);
					return;
				}
				if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha7) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad7))
				{
					SwapSimilar(7);
					return;
				}
				if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha8) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad8))
				{
					SwapSimilar(8);
					return;
				}
				if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha9) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad9))
				{
					SwapSimilar(9);
					return;
				}
				if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha0) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad0))
				{
					SwapSimilar(0);
					return;
				}
			}
			if (Input.GetButtonDown("Edit"))
			{
				editToggle.isOn = !editToggle.isOn;
				return;
			}
			if (Input.GetButtonDown("Mode"))
			{
				modeToggle.isOn = !modeToggle.isOn;
				return;
			}
			if (Input.GetButtonDown("Wiring") && DLC.DLC_owned)
			{
				wiring.inst.wireTog.isOn = !wiring.inst.wireTog.isOn;
				return;
			}
			if (Input.GetButtonDown("extend"))
			{
				extendArrow.inst.tog.isOn = !extendArrow.inst.tog.isOn;
				return;
			}
			if (BuilderSystem.editMode)
			{
				if (Input.GetButtonDown("RectSelect"))
				{
					rectSelToggle.isOn = !rectSelToggle.isOn;
				}
			}
			else if (Input.GetButtonDown("halfPlace"))
			{
				snapTog.isOn = !snapTog.isOn;
			}
			if (Input.GetButtonDown("tierSelect") && !mouseOverUI)
			{
				quickPicker.SetActive(value: true);
				quickPicker.transform.position = UnityEngine.Input.mousePosition;
			}
			if (Input.GetButtonDown("partPicker"))
			{
				if (!placePanelSwap.inst.pickTog.isOn)
				{
					placePanelSwap.inst.pickTog.isOn = true;
				}
				else
				{
					placePanelSwap.inst.pickTog.isOn = false;
				}
			}
		}
	}

	public void raidToggleUI()
	{
		Canvas component = menuPanel.GetComponent<Canvas>();
		component.enabled = !component.isActiveAndEnabled;
		if (menuToggle.isOn)
		{
			menuToggle.isOn = false;
		}
		menuToggle.gameObject.SetActive(!menuToggle.gameObject.activeSelf);
		mainCanvas.enabled = !mainCanvas.isActiveAndEnabled;
	}

	public void inputOver(bool enter)
	{
		if (enter)
		{
			inputField.ActivateInputField();
			inputField.MoveTextEnd(shift: false);
		}
		else
		{
			inputField.DeactivateInputField();
		}
		BuilderSystem.disableInput = enter;
		CameraCtrl.inst.enabled = !enter;
	}

	public void disableInput(bool state)
	{
		BuilderSystem.disableInput = state;
		CameraCtrl.inst.enabled = !state;
	}

	public void ReloadScene()
	{
		MGMT.inst.clearLists();
		if (BuilderSystem.multiplayer)
		{
			fileList.inst.changeSaveInfo("");
			sys.ClearScene();
			sys.SceneLoader(sys.sceneDropDn.value + 1, null);
			Multiplayer.sendResetCmd(sys.currentScene.buildIndex, 0uL);
			Multiplayer.syncObjList.Clear();
			CameraCtrl.resetTransform();
		}
		else
		{
			MGMT.inst.getDestroyObjs(clearScene: true);
			cupboardRange.inst.rangeList.Clear();
			PlayerPrefs.SetInt("Scene", sys.sceneDropDn.value);
			SceneManager.LoadScene(0);
		}
	}

	public void screenshotModeState()
	{
		if (!CameraCtrl.inst.screenshotMode)
		{
			CameraCtrl.inst.screenshotMode = true;
			stbltCanvas.enabled = false;
			mainCanvas.enabled = false;
			BuilderSystem.disableInput = true;
			RayPlace.noPlace = true;
			return;
		}
		CameraCtrl.inst.screenshotMode = false;
		stbltCanvas.enabled = true;
		if (!Stability.inst.raidMode)
		{
			mainCanvas.enabled = true;
		}
		BuilderSystem.disableInput = false;
		if (!BuilderSystem.editMode && !wiring.inst.on && !Stability.inst.raidMode)
		{
			RayPlace.noPlace = false;
		}
	}

	public void setHalfSnap()
	{
		PlacePartFoundation.snapMode = !PlacePartFoundation.snapMode;
		sys.StartCoroutine(sys.swapCheckState());
	}

	public void Undo()
	{
		MGMT.inst.Undo();
	}

	public void Redo()
	{
		MGMT.inst.Redo();
	}

	private IEnumerator delayedHidePanel(GameObject panel)
	{
		yield return new WaitForSeconds(1f);
		panel.transform.GetChild(0).gameObject.SetActive(value: false);
	}

	public void pickScene()
	{
		PlayerPrefs.SetInt("Scene", sys.sceneDropDn.value);
	}

	public void saveSelection()
	{
		if (saveSelectionTog.isOn)
		{
			BuilderSystem.saveSelection = true;
		}
		else
		{
			BuilderSystem.saveSelection = false;
		}
	}

	public void menu()
	{
		if (menuToggle.isOn)
		{
			menuPanel.SetActive(value: true);
			fileList.inst.loadList();
			cameraControls.SetActive(value: false);
		}
		else
		{
			menuPanel.SetActive(value: false);
			cameraControls.SetActive(value: true);
			inputOver(enter: false);
		}
	}

	public void swapActiveSelf(GameObject obj)
	{
		obj.SetActive(!obj.activeSelf);
	}

	public void rectSelectModeState()
	{
		sys.rectSelectMode = !sys.rectSelectMode;
		RectSelection.mouse3rectSelect = sys.rectSelectMode;
	}

	public void AlignEnabled()
	{
		GetComponent<AlignStructure>().enabled = true;
	}

	public void openLink(string URL)
	{
		Application.OpenURL(URL);
	}

	public void Exit(bool yes)
	{
		if (yes)
		{
			allowQuit = true;
			Application.Quit();
		}
		else
		{
			exitCanvas.SetActive(value: false);
		}
	}

	private void OnApplicationQuit()
	{
		if (SteamManager.Initialized && !allowQuit)
		{
			Application.CancelQuit();
			exitCanvas.SetActive(value: true);
		}
	}

	public void swapToggle(string partName)
	{
		if (BuilderSystem.editMode)
		{
			editToggle.isOn = false;
		}
		if (BuilderSystem.placeMode)
		{
			modeToggle.isOn = !modeToggle.isOn;
		}
		buildPanel.transform.Find(partName).gameObject.GetComponent<Toggle>().isOn = true;
	}

	private void SwapSimilar(int key)
	{
		string a = sys.BPinfo.name;
		if (a != lastUsed[key])
		{
			swapToggle(lastUsed[key]);
			return;
		}
		switch (key)
		{
		case 0:
			if (includeToggles[4].isOn)
			{
				a = IncludePartWith.inst.floorFrameTriPart;
			}
			if (a == "floor_frame_tri")
			{
				swapToggle("hatch_tri");
			}
			else if (a == "hatch_tri")
			{
				swapToggle("floor_tri_grill");
			}
			else if (a == "floor_tri_grill")
			{
				if (!includeToggles[4].isOn)
				{
					swapToggle("floor_frame_tri");
				}
				else
				{
					swapToggle("hatch_tri");
				}
			}
			break;
		case 1:
			if (a == "foundation")
			{
				swapToggle("foundation_tri");
			}
			else if (a == "foundation_tri")
			{
				swapToggle("foundation");
			}
			break;
		case 2:
			if (a == "floor")
			{
				swapToggle("floor_tri");
			}
			else if (a == "floor_tri")
			{
				swapToggle("floor");
			}
			break;
		case 3:
			if (a == "wall")
			{
				swapToggle("wall_half");
			}
			else if (a == "wall_half")
			{
				swapToggle("wall_low");
			}
			else if (a == "wall_low")
			{
				swapToggle("wall");
			}
			break;
		case 4:
			if (includeToggles[0].isOn)
			{
				a = IncludePartWith.inst.windowPart;
			}
			if (a == "wall_window")
			{
				swapToggle("window_bars_metal");
			}
			else if (a == "window_bars_metal")
			{
				swapToggle("window_glass_reinforced");
			}
			else if (a == "window_glass_reinforced")
			{
				swapToggle("window_bars_reinforced");
			}
			else if (a == "window_bars_reinforced")
			{
				swapToggle("shutters");
			}
			else if (a == "shutters")
			{
				swapToggle("embrasure_horizontal");
			}
			else if (a == "embrasure_horizontal")
			{
				swapToggle("embrasure_vertical");
			}
			else if (a == "embrasure_vertical")
			{
				if (!includeToggles[0].isOn)
				{
					swapToggle("wall_window");
				}
				else
				{
					swapToggle("window_bars_metal");
				}
			}
			break;
		case 5:
			if (includeToggles[1].isOn)
			{
				a = IncludePartWith.inst.doorPart;
			}
			if (a == "wall_doorway")
			{
				swapToggle("door_wood");
			}
			else if (a == "door_wood")
			{
				swapToggle("door_metal");
			}
			else if (a == "door_metal")
			{
				swapToggle("door_armor");
			}
			else if (a == "door_armor")
			{
				if (!includeToggles[1].isOn)
				{
					swapToggle("wall_doorway");
				}
				else
				{
					swapToggle("door_wood");
				}
			}
			break;
		case 6:
			if (a == "stairs_U")
			{
				swapToggle("stairs_L");
			}
			else if (a == "stairs_L")
			{
				swapToggle("stairs_spiral");
			}
			else if (a == "stairs_spiral")
			{
				swapToggle("stairs_spiral_tri");
			}
			else if (a == "stairs_spiral_tri")
			{
				swapToggle("foundation_steps");
			}
			else if (a == "foundation_steps")
			{
				swapToggle("ramp");
			}
			else if (a == "ramp")
			{
				swapToggle("stairs_U");
			}
			break;
		case 7:
			if (a == "roof")
			{
				swapToggle("roof_tri");
			}
			else if (a == "roof_tri")
			{
				swapToggle("roof");
			}
			break;
		case 8:
			if (includeToggles[2].isOn)
			{
				a = IncludePartWith.inst.wallFramePart;
			}
			if (a == "wall_frame")
			{
				swapToggle("wf_ddoor_wood");
			}
			else if (a == "wf_ddoor_wood")
			{
				swapToggle("wf_ddoor_metal");
			}
			else if (a == "wf_ddoor_metal")
			{
				swapToggle("wf_ddoor_armor");
			}
			else if (a == "wf_ddoor_armor")
			{
				swapToggle("shopfront_metal");
			}
			else if (a == "shopfront_metal")
			{
				swapToggle("shopfront");
			}
			else if (a == "shopfront")
			{
				swapToggle("cell_gate");
			}
			else if (a == "cell_gate")
			{
				swapToggle("cell");
			}
			else if (a == "cell")
			{
				swapToggle("garage_door");
			}
			else if (a == "garage_door")
			{
				swapToggle("netting");
			}
			else if (a == "netting")
			{
				swapToggle("fence_gate");
			}
			else if (a == "fence_gate")
			{
				swapToggle("fence");
			}
			else if (a == "fence")
			{
				if (!includeToggles[2].isOn)
				{
					swapToggle("wall_frame");
				}
				else
				{
					swapToggle("wf_ddoor_wood");
				}
			}
			break;
		case 9:
			if (includeToggles[3].isOn)
			{
				a = IncludePartWith.inst.floorFramePart;
			}
			if (a == "floor_frame")
			{
				swapToggle("ff_hatch");
			}
			else if (a == "ff_hatch")
			{
				swapToggle("floor_grill");
			}
			else if (a == "floor_grill")
			{
				if (!includeToggles[3].isOn)
				{
					swapToggle("floor_frame");
				}
				else
				{
					swapToggle("ff_hatch");
				}
			}
			break;
		}
	}
}
