using UnityEngine;
using UnityEngine.UI;

public class dayNightCtrl : MonoBehaviour
{
	public static dayNightCtrl inst;

	public BuilderSystem cs;

	public Light sun;

	public Transform sunParent;

	public Slider slider;

	public Gradient fogColor;

	public Gradient lightColor;

	public float latt = 11f;

	public AnimationCurve ambientIntensity;

	private float sunMaxIntensity = 0.8f;

	public static bool night;

	private float currentTime;

	private float wait;

	public Toggle playTog;

	public Image playIcon;

	public Image pauseIcon;

	private void Awake()
	{
		inst = this;
		cs = BuilderSystem.inst;
		night = false;
	}

	private void Update()
	{
		currentTime += Time.deltaTime / 360f;
		if (currentTime >= 1f)
		{
			currentTime = 0f;
		}
		wait += currentTime;
		if (wait >= 0.05f)
		{
			slider.value = currentTime;
			UpdateTime();
			wait = 0f;
		}
	}

	public void playToggle()
	{
		if (base.enabled)
		{
			playIcon.gameObject.SetActive(value: true);
			pauseIcon.gameObject.SetActive(value: false);
			playTog.targetGraphic = playIcon;
			base.enabled = false;
		}
		else
		{
			currentTime = slider.value;
			playIcon.gameObject.SetActive(value: false);
			pauseIcon.gameObject.SetActive(value: true);
			playTog.targetGraphic = pauseIcon;
			base.enabled = true;
		}
	}

	public void updateWaterBrightness()
	{
		if (sceneSettings.inst != null && sceneSettings.inst.waterMat != null)
		{
			sceneSettings.inst.waterMat.SetFloat("_Brightness", ambientIntensity.Evaluate(slider.value));
		}
	}

	public void UpdateTime()
	{
		float value = slider.value;
		sunParent.localRotation = Quaternion.Euler(value * 360f - 180f, 0f, latt);
		RenderSettings.fogColor = fogColor.Evaluate(value);
		sun.color = lightColor.Evaluate(value);
		updateWaterBrightness();
		float num = 1f;
		if (value <= 0.23f || value >= 0.75f)
		{
			num = 0f;
			if (!night)
			{
				dayNightChange(state: true);
			}
		}
		else
		{
			if (night)
			{
				dayNightChange(state: false);
			}
			if (value <= 0.25f)
			{
				num = Mathf.Clamp01((value - 0.23f) * 50f);
			}
			else if (value >= 0.73f)
			{
				num = Mathf.Clamp01(1f - (value - 0.73f) * 50f);
			}
		}
		sun.intensity = sunMaxIntensity * num;
		RenderSettings.ambientIntensity = ambientIntensity.Evaluate(value);
	}

	public void dayNightChange(bool state)
	{
		night = state;
		foreach (BuilderPart bp in cs.bpList)
		{
			if ((bool)bp.deploy)
			{
				if ((bool)bp.deploy.lc && (bool)bp.deploy.lc._light)
				{
					bp.deploy.lc._light.enabled = state;
				}
				if (bp.device is powerSolar)
				{
					bp.device.newPowerThru();
				}
			}
		}
	}
}
