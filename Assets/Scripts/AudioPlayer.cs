using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
	public static AudioPlayer inst;

	private AudioSource AS;

	public AudioClip[] clips;

	public AudioClip crashSoft;

	public AudioClip crashHard;

	public AudioClip crashGroundSoft;

	public AudioClip crashGroundHard;

	public AudioClip doorClose;

	public AudioClip doorOpen;

	public AudioClip garage;

	private void Awake()
	{
		AS = GetComponent<AudioSource>();
		inst = this;
	}

	private void Start()
	{
		CrashSound.crashSoft = crashSoft;
		CrashSound.crashHard = crashHard;
		CrashSound.crashGroundSoft = crashGroundSoft;
		CrashSound.crashGroundHard = crashGroundHard;
	}

	public void playAtPoint(Vector3 pos, int index)
	{
		AS.pitch = 1f;
		AudioSource.PlayClipAtPoint(clips[index], pos);
	}

	public void playAtPoint(Vector3 pos, AudioClip clip, float vol)
	{
		AS.pitch = 1f;
		AudioSource.PlayClipAtPoint(clip, pos, vol);
	}

	public void crashAtPoint(Vector3 pos)
	{
		AS.pitch = UnityEngine.Random.Range(0.8f, 1.1f);
		AudioSource.PlayClipAtPoint(crashSoft, pos);
	}
}
