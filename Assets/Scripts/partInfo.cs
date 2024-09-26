using UnityEngine;

[CreateAssetMenu(fileName = "partInfo", menuName = "ScriptableObjects/partInfo", order = 1)]
public class partInfo : ScriptableObject
{
	public enum Shop
	{
		bandit,
		outpost,
		fishing,
		both,
		craft
	}

	public string named;

	public int id;

	public bool favorite;

	public bool dontInclude;

	public bool defaultBP;

	public bool pickup;

	public int workbench;

	public float multiplier;

	public float angleLimit;

	public bool buildDeploy;

	[Header("")]
	public int researchCost;

	public int wood;

	public int stone;

	public int metal;

	public int hq_metal;

	[Header("")]
	public int gears;

	public int ladders;

	public int blades;

	public int tarp;

	public int fuel;

	public int cloth;

	public int sewkit;

	public int rope;

	public int sheetmetal;

	public int propane;

	public int techTrash;

	public int targetComp;

	public int CCTV;

	[Header("scrap info")]
	public int scrap;

	public Shop shop;

	[Header("")]
	public int storageSlots;

	public int waterStorage;

	[Header("")]
	public int HP;

	[Header("gen explo blunt bullet heat stab")]
	public float[] protection;

	[TextArea(15, 20)]
	public string description;

	[TextArea(15, 20)]
	public string rustDescription;
}
