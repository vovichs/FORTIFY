using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityStandardAssets.Water;

public class Options : MonoBehaviour
{
	public Toggle HeightLimitTog;

	public Toggle lvlCamTog;

	public Toggle InvertYTog;

	public Toggle cullAllTog;

	public Toggle usageTog;

	public Toggle msaaTog;

	public Slider SoundSlide;

	public Slider mouseSlide;

	public Slider ColorSlide;

	public Dropdown QualityDrop;

	public Text QualityText;

	public InputField FPSlimitInput;

	public Texture2D beardColors;

	public static int avatarIndex;

	public static bool invertY;

	private static bool cullAll;

	public static bool MSAA;

	public PostProcessLayer[] ppLayers;

	public PostProcessVolume ppVolume;

	public Toggle[] avatarTogs;

	private void OnDisable()
	{
		SavePrefs();
	}

	public void GetPrefs()
	{
		if (PlayerPrefs.HasKey("lvlCam"))
		{
			lvlCamTog.isOn = (PlayerPrefs.GetInt("lvlCam") == 1);
		}
		if (PlayerPrefs.HasKey("InvertY"))
		{
			InvertYTog.isOn = (PlayerPrefs.GetInt("InvertY") == 1);
		}
		if (cullAll)
		{
			cullAllTog.isOn = true;
		}
		else
		{
			cullDistances();
		}
		if (PlayerPrefs.HasKey("Sound"))
		{
			AudioListener.volume = PlayerPrefs.GetFloat("Sound");
			SoundSlide.value = AudioListener.volume;
		}
		else
		{
			AudioListener.volume = SoundSlide.value;
		}
		if (PlayerPrefs.HasKey("mouseLook"))
		{
			mouseSlide.value = PlayerPrefs.GetFloat("mouseLook");
			setLookSensitity();
		}
		if (PlayerPrefs.HasKey("MSAA"))
		{
			MSAA = (PlayerPrefs.GetInt("MSAA") == 1);
		}
		msaaTog.isOn = MSAA;
		if (PlayerPrefs.HasKey("FPSlimit"))
		{
			int num = PlayerPrefs.GetInt("FPSlimit");
			if (num > 144)
			{
				num = 144;
			}
			Application.targetFrameRate = num;
		}
		else
		{
			Application.targetFrameRate = 60;
		}
		FPSlimitInput.text = Application.targetFrameRate.ToString();
		if (DLC.DLC_owned)
		{
			if (PlayerPrefs.HasKey("av"))
			{
				avatarIndex = PlayerPrefs.GetInt("av");
			}
			avatarTogs[1].interactable = true;
		}
		avatarTogs[avatarIndex].isOn = true;
		QualityDrop.value = QualitySettings.GetQualityLevel();
		setAA();
		if (PlayerPrefs.HasKey("usageCalc"))
		{
			usageTog.isOn = (PlayerPrefs.GetInt("usageCalc") == 1);
		}
	}

	private void SavePrefs()
	{
		PlayerPrefs.SetInt("lvlCam", lvlCamTog.isOn ? 1 : 0);
		lvlCamMove();
		PlayerPrefs.SetInt("InvertY", InvertYTog.isOn ? 1 : 0);
		InvertY();
		PlayerPrefs.SetFloat("Sound", SoundSlide.value);
		AudioListener.volume = SoundSlide.value;
		PlayerPrefs.SetFloat("mouseLook", mouseSlide.value);
		PlayerPrefs.SetInt("MSAA", msaaTog.isOn ? 1 : 0);
		PlayerPrefs.SetInt("av", avatarIndex);
		PlayerPrefs.SetInt("usageCalc", usageTog.isOn ? 1 : 0);
	}

	public void setQuality()
	{
		QualitySettings.SetQualityLevel(QualityDrop.value);
		setAA();
		WaterBase.UpdateShader();
	}

	public void setAA()
	{
		QualityText.text = "";
		if (QualitySettings.GetQualityLevel() == 0)
		{
			PostProcessLayer[] array = ppLayers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			QualityText.text += "shadows off\n";
			QualityText.text += "AO off \n";
			msaaTog.isOn = false;
			MSAA = false;
			msaaTog.gameObject.SetActive(value: false);
			CameraCtrl.inst.cams[0].renderingPath = RenderingPath.Forward;
			CameraCtrl.inst.cams[1].renderingPath = RenderingPath.Forward;
		}
		else
		{
			msaaTog.gameObject.SetActive(value: true);
			MSAA = msaaTog.isOn;
			if (MSAA)
			{
				QualitySettings.antiAliasing = 4;
				CameraCtrl.inst.cams[0].renderingPath = RenderingPath.Forward;
				CameraCtrl.inst.cams[1].renderingPath = RenderingPath.Forward;
				AOmode(multiScale: false);
			}
			else
			{
				QualitySettings.antiAliasing = 0;
				CameraCtrl.inst.cams[0].renderingPath = RenderingPath.DeferredShading;
				CameraCtrl.inst.cams[1].renderingPath = RenderingPath.DeferredShading;
				AOmode(multiScale: true);
			}
			PostProcessLayer[] array = ppLayers;
			foreach (PostProcessLayer postProcessLayer in array)
			{
				postProcessLayer.enabled = true;
				if (MSAA)
				{
					postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
				}
				else
				{
					postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
				}
			}
			Text qualityText = QualityText;
			qualityText.text = qualityText.text + "shadow distance - " + QualitySettings.shadowDistance + "\n";
			qualityText = QualityText;
			qualityText.text = qualityText.text + "shadow quality - " + QualitySettings.shadowResolution + "\n";
		}
		if (MSAA)
		{
			QualityText.text += "MSAA x4";
		}
		else
		{
			QualityText.text += ppLayers[0].antialiasingMode;
		}
	}

	private void AOmode(bool multiScale)
	{
		if (ppVolume.profile.TryGetSettings(out AmbientOcclusion outSetting))
		{
			if (multiScale)
			{
				outSetting.intensity.value = 0.8f;
			}
			else
			{
				outSetting.intensity.value = 0.4f;
			}
		}
	}

	public void OnVolumeChanged()
	{
		AudioListener.volume = SoundSlide.value;
	}

	public void PickColor()
	{
		int y = Mathf.RoundToInt(128f * ColorSlide.value);
		MonoBehaviour.print(beardColors.GetPixel(0, y));
	}

	public void setAvatar(int index)
	{
		avatarIndex = index;
		if (CameraCtrl.inst.mode == CameraCtrl.Mode.third)
		{
			CameraCtrl.inst.enableAvatar(option: true);
		}
		if (BuilderSystem.multiplayer)
		{
			Multiplayer.inst.sendAvatar();
		}
	}

	public void setLookSensitity()
	{
		CameraCtrl.inst.setlookMulti(mouseSlide.value);
	}

	public void setFPSlimit()
	{
		if (FPSlimitInput.text == "")
		{
			FPSlimitInput.text = Application.targetFrameRate.ToString();
			return;
		}
		int num = int.Parse(FPSlimitInput.text);
		if (num < 10)
		{
			FPSlimitInput.text = "10";
			num = 10;
		}
		else if (num > 144)
		{
			FPSlimitInput.text = "144";
			num = 144;
		}
		PlayerPrefs.SetInt("FPSlimit", num);
		Application.targetFrameRate = num;
	}

	public void cullDistances()
	{
		float[] array = new float[32];
		int num = 100;
		if (cullAllTog.isOn)
		{
			cullAll = true;
			num = 50;
			array[0] = num;
			array[2] = num;
		}
		else
		{
			cullAll = false;
		}
		array[23] = Mathf.Min(num, 10);
		array[20] = Mathf.Min(num, 30);
		array[21] = Mathf.Min(num, 70);
		CameraCtrl.inst.cams[0].layerCullDistances = array;
		CameraCtrl.inst.cams[1].layerCullDistances = array;
	}

	public void lvlCamMove()
	{
		CameraCtrl.lvlCamMove = lvlCamTog.isOn;
	}

	public void InvertY()
	{
		invertY = InvertYTog.isOn;
	}

	public void usageSet()
	{
		Device.noUsage = !usageTog.isOn;
	}
}
