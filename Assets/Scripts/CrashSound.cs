using UnityEngine;

public class CrashSound : MonoBehaviour
{
	public static int count;

	public static AudioClip crashSoft;

	public static AudioClip crashHard;

	public static AudioClip crashGroundSoft;

	public static AudioClip crashGroundHard;

	private AudioSource source;

	private float lowPitchRange = 0.75f;

	private float highPitchRange = 1.25f;

	private float velToVol = 0.2f;

	private float velocityClipSplit = 10f;

	private void Awake()
	{
		count++;
		source = base.gameObject.AddComponent<AudioSource>();
		source.volume = 0.9f;
		source.spatialBlend = 1f;
		source.pitch = UnityEngine.Random.Range(lowPitchRange, highPitchRange);
		source.PlayOneShot(crashSoft, 1f);
	}

	private void OnCollisionEnter(Collision col)
	{
		if (source.isPlaying)
		{
			return;
		}
		float magnitude = col.relativeVelocity.magnitude;
		float volumeScale = magnitude * velToVol;
		if (col.gameObject.layer == 8)
		{
			if (magnitude < velocityClipSplit)
			{
				source.PlayOneShot(crashGroundSoft, volumeScale);
			}
			else
			{
				source.PlayOneShot(crashGroundHard, volumeScale);
			}
		}
		else if (magnitude < velocityClipSplit)
		{
			source.PlayOneShot(crashSoft, volumeScale);
		}
		else
		{
			source.PlayOneShot(crashHard, volumeScale);
		}
	}

	private void OnDestroy()
	{
		count--;
	}
}
