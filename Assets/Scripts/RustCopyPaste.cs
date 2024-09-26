using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RustCopyPaste : MonoBehaviour
{
	public class PartList
	{
		public string Name;

		public string Fname;

		public string Path;

		public int Id;

		public PartList(string name, string fname, string path, int id)
		{
			Name = name;
			Fname = fname;
			Path = path;
			Id = id;
		}
	}

	public class weapon
	{
		public int id;

		public string name;

		public int ammo;

		public weapon(int Id, string Name, int Ammo)
		{
			id = Id;
			name = Name;
			ammo = Ammo;
		}
	}

	public class Root
	{
		[JsonProperty(PropertyName = "default")]
		public Default Default;

		public List<Entities> entities = new List<Entities>();
	}

	public class Default
	{
		public sVector3 position;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public float rotationdiff;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public float rotationy;
	}

	public class Entities
	{
		[DefaultValue(null)]
		public List<Items> items;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		public int customColour;

		[DefaultValue(5)]
		public int grade = 5;

		[JsonProperty(PropertyName = "lock")]
		[DefaultValue(null)]
		public CodeLock codeLock;

		[JsonProperty(PropertyName = "IOEntity")]
		[DefaultValue(null)]
		public IOEntity ioEntity;

		[DefaultValue(null)]
		public Flags flags;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public sVector3 pos;

		public string prefabname;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public sVector3 rot;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public long skinid;
	}

	public class Flags
	{
		[DefaultValue(false)]
		public bool Open;
	}

	public class Items
	{
		public int amount = 1;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public int blueprintTarget;

		public string condition = "1000";

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public int dataInt;

		public int id;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public int position;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public long skinid;
	}

	public class CodeLock
	{
		public string code;

		public string prefabname;
	}

	public class IOEntity
	{
		[DefaultValue(-2)]
		public int branchAmount = -2;

		[DefaultValue(-2f)]
		public float timerLength = -2f;

		[DefaultValue(-2f)]
		public float targetNumber = -2f;

		[DefaultValue(-2)]
		public int frequency = -2;

		[JsonProperty(PropertyName = "inputs")]
		[DefaultValue(null)]
		public List<Inputs> inputs = new List<Inputs>();

		[JsonProperty(PropertyName = "oldID")]
		public int oldId;

		[JsonProperty(PropertyName = "outputs")]
		public List<Outputs> outputs = new List<Outputs>();
	}

	public class Inputs
	{
		[JsonProperty(PropertyName = "connectedID", DefaultValueHandling = DefaultValueHandling.Include)]
		public int id;

		[JsonProperty(PropertyName = "connectedToSlot", DefaultValueHandling = DefaultValueHandling.Include)]
		public int slot;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public string niceName = "";

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public int type;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public int wireColour;
	}

	public class Outputs
	{
		[JsonProperty(PropertyName = "connectedID", DefaultValueHandling = DefaultValueHandling.Include)]
		public int id;

		[JsonProperty(PropertyName = "connectedToSlot", DefaultValueHandling = DefaultValueHandling.Include)]
		public int slot;

		[JsonProperty(PropertyName = "linePoints")]
		[DefaultValue(null)]
		public List<fVector3> linePoints = new List<fVector3>();

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public string niceName = "";

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public int type;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public int wireColour;
	}

	public class sVector3
	{
		public string x;

		public string y;

		public string z;
	}

	public class fVector3
	{
		[DefaultValue(0)]
		public float x;

		[DefaultValue(0)]
		public float y;

		[DefaultValue(0)]
		public float z;
	}

	public class Output
	{
		public io io;

		public int slot;

		public int id;

		public int color;

		public Vector3[] points;

		public Output(io _io, int _id, int _slot, int _color, Vector3[] _points)
		{
			io = _io;
			id = _id;
			slot = _slot;
			color = _color;
			points = _points;
		}
	}

	public class Input
	{
		public Device dev;

		public int id;

		public Input(Device _dev, int _id)
		{
			dev = _dev;
			id = _id;
		}
	}

	public static RustCopyPaste inst;

	public GameObject wirePrefab;

	public GameObject centerPrefab;

	public static GameObject center;

	private BuilderSystem sys;

	private float highY;

	public Image centerIcon;

	public static bool getCenter;

	public Toggle wireOffset;

	private int connectId;

	public List<weapon> weaponsList = new List<weapon>();

	private int woodSkinId;

	private int stoneSkinId;

	private int metalSkinId;

	private int containerColorId;

	public Transform colorPicker;

	public Image containerColor;

	public static List<PartList> partList = new List<PartList>();

	private int[] ammoList = new int[3]
	{
		785728077,
		-727717969,
		-1211166256
	};

	private void Awake()
	{
		inst = this;
		sys = BuilderSystem.inst;
		getCenter = false;
		setPartList();
		setWeaponList();
	}

	public string CopyPasteExport(string path, bool returns)
	{
		float num = 100f;
		connectId = 1;
		Root root = new Root();
		if (center == null)
		{
			Transform transform = null;
			foreach (BuilderPart bp in sys.bpList)
			{
				Transform transform2 = bp._transform;
				if (bp.found && transform2.position.y < num)
				{
					num = transform2.position.y;
					transform = transform2;
				}
			}
			if (transform != null)
			{
				SetCenter(transform.position);
			}
			else
			{
				SetCenter(sys.bpList[0].transform.position);
			}
			popUp.inst.message("random center created, use Set Center");
		}
		Transform transform3 = center.transform;
		num = transform3.position.y;
		Default @default = new Default();
		sVector3 sVector = new sVector3();
		sVector.x = "0";
		sVector.z = "0";
		sVector.z = "0";
		sVector.y = num.ToString();
		@default.position = sVector;
		root.Default = @default;
		for (int i = 0; i < sys.bpList.Count; i++)
		{
			BuilderPart builderPart = sys.bpList[i];
			GameObject obj = builderPart.gameObject;
			Transform transform4 = builderPart._transform;
			Entities entities = new Entities();
			sVector3 sVector2 = new sVector3();
			sVector3 sVector3 = new sVector3();
			CodeLock codeLock = new CodeLock();
			int num2 = partList.FindIndex((PartList item) => item.Fname == obj.name);
			if (num2 == -1)
			{
				continue;
			}
			entities.prefabname = partList[num2].Path + partList[num2].Name + ".prefab";
			entities.grade = builderPart.tier;
			Vector3 position = transform4.position;
			Quaternion rotation = transform4.rotation;
			if ((bool)builderPart.block)
			{
				if (builderPart.tier == 1)
				{
					entities.skinid = woodSkinId;
				}
				else if (builderPart.tier == 2)
				{
					entities.skinid = stoneSkinId;
				}
				else if (builderPart.tier == 3)
				{
					entities.skinid = metalSkinId;
					if (metalSkinId == 10221)
					{
						entities.customColour = containerColorId;
					}
				}
				if ((bool)builderPart.origin && !builderPart.found)
				{
					transform4.Translate(builderPart.origin.position - transform4.position, Space.World);
					transform4.rotation = builderPart.origin.rotation;
				}
				else
				{
					if (!builderPart.block.stairs)
					{
						transform4.Translate(Vector3.up * -0.0333f);
					}
					if (builderPart.block.pillar)
					{
						transform4.Translate(Vector3.forward * 0.5f);
					}
					if (builderPart.block.steps || builderPart.block.ramp)
					{
						transform4.Translate(Vector3.right * -1f);
					}
					else if (builderPart.block.stairs && !builderPart.tri)
					{
						transform4.Translate(Vector3.right * 0.5f);
						transform4.Rotate(Vector3.up * -90f);
					}
					if ((builderPart.block.floor || builderPart.block.found) && !builderPart.tri)
					{
						transform4.Translate(Vector3.right * -0.5f);
					}
					if (builderPart.block.found)
					{
						transform4.Translate(Vector3.up * 0.5f);
					}
					if (builderPart.tri && !builderPart.block.stairs && !builderPart.block.roof)
					{
						transform4.Rotate(Vector3.up * -90f);
					}
					if (builderPart.block.roof)
					{
						transform4.Rotate(Vector3.up * 90f);
						if (builderPart.tri)
						{
							transform4.Translate(Vector3.forward * -0.2889f);
						}
						else
						{
							transform4.Translate(Vector3.forward * -0.5f);
						}
					}
				}
			}
			else
			{
				entities.items = new List<Items>();
				if ((bool)builderPart.deploy.lockEnt)
				{
					string code = builderPart.deploy.lockEnt.code;
					if (code != "")
					{
						if (code.Length == 4)
						{
							codeLock.code = code;
						}
						else
						{
							codeLock.code = "";
						}
						codeLock.prefabname = "assets/prefabs/locks/keypad/lock.code.prefab";
						entities.codeLock = codeLock;
					}
				}
				if ((bool)builderPart.door)
				{
					Flags flags = new Flags();
					flags.Open = builderPart.door.open;
					entities.flags = flags;
				}
				if ((bool)builderPart.door || builderPart.deploy.frameWall || builderPart.deploy.windowPart)
				{
					transform4.Translate(Vector3.up * -0.0333f);
				}
				if (builderPart.deploy.copypaste != 0)
				{
					if (builderPart.deploy.copypaste == Deploy.copyPaste.up90)
					{
						transform4.Rotate(Vector3.up * 90f);
					}
					else if (builderPart.deploy.copypaste == Deploy.copyPaste.flip)
					{
						transform4.Rotate(Vector3.up * 180f);
					}
					else if (builderPart.deploy.copypaste == Deploy.copyPaste.r90)
					{
						transform4.Rotate(Vector3.right * -90f);
					}
					else if (builderPart.deploy.copypaste == Deploy.copyPaste.origin)
					{
						transform4.Translate(builderPart.deploy.origin.position - transform4.position, Space.World);
						transform4.rotation = builderPart.deploy.origin.rotation;
					}
				}
			}
			Vector3 eulerAngles = transform4.eulerAngles;
			Vector3 vector = transform3.InverseTransformPoint(transform4.position);
			sVector2.x = (vector.x * 3f).ToString();
			sVector2.y = ((vector.y - 0.5f) * 3f).ToString();
			sVector2.z = (vector.z * 3f).ToString();
			entities.pos = sVector2;
			sVector3.x = (eulerAngles.x * ((float)Math.PI / 180f)).ToString();
			sVector3.y = (eulerAngles.y * ((float)Math.PI / 180f)).ToString();
			sVector3.z = (eulerAngles.z * ((float)Math.PI / 180f)).ToString();
			entities.rot = sVector3;
			if ((bool)builderPart.device)
			{
				IOEntity iOEntity = new IOEntity();
				Device device = builderPart.device;
				if (device.id == 0)
				{
					device.id = connectId;
					connectId++;
				}
				iOEntity.oldId = device.id;
				if (device is timerDevice)
				{
					iOEntity.timerLength = (device as timerDevice).time;
				}
				else if (device is branch)
				{
					iOEntity.branchAmount = device.value;
				}
				else if (device is counter)
				{
					if (device.value > 0)
					{
						iOEntity.targetNumber = device.value;
					}
				}
				else if (device is RF_broadcaster || device is RF_receiver)
				{
					iOEntity.frequency = device.value;
				}
				else if (device is autoTurret && device.value != 0)
				{
					Items weapon = new Items();
					weapon.id = device.value;
					entities.items.Add(weapon);
					int index = weaponsList.FindIndex((weapon w) => w.id == weapon.id);
					int ammo = weaponsList[index].ammo;
					if (ammo > -1)
					{
						Items items = new Items();
						items.id = ammoList[ammo];
						items.amount = 256;
						items.position = 1;
						entities.items.Add(items);
					}
				}
				for (int j = 0; j < device.outputTo.Length; j++)
				{
					io io = device.outputTo[j];
					Outputs outputs = new Outputs();
					outputs.type = io.type;
					if (io.connectedTo != null)
					{
						outputs.slot = io.connectedTo.index;
						if (outputs.slot == -1)
						{
							outputs.slot = 0;
						}
						if (outputs.slot == 2 && io.connectedTo.dev is elevator)
						{
							outputs.slot = 0;
							if (io.connectedTo.id == 0)
							{
								io.connectedTo.id = connectId;
								connectId++;
							}
							outputs.id = io.connectedTo.id;
						}
						else
						{
							if (io.connectedTo.dev.id == 0)
							{
								io.connectedTo.dev.id = connectId;
								connectId++;
							}
							outputs.id = io.connectedTo.dev.id;
						}
						if (io.wire != null)
						{
							if (outputs.type == 0)
							{
								outputs.wireColour = colorIndexSwap(io.wire.colorIndex, import: false);
							}
							else if (outputs.type == 4)
							{
								outputs.wireColour = io.wire.colorIndex;
							}
							LineRenderer lr = io.wire.lr;
							Vector3[] array = new Vector3[lr.positionCount];
							lr.GetPositions(array);
							Vector3[] array2 = array;
							foreach (Vector3 position2 in array2)
							{
								Vector3 vector2 = transform4.InverseTransformPoint(position2);
								vector2.x *= 3f;
								vector2.y *= 3f;
								vector2.z *= 3f;
								Vector3 point = vector2 - transform3.position;
								point = transform3.rotation * point;
								vector2 = point + transform3.position;
								fVector3 fVector = new fVector3();
								fVector.x = vector2.x;
								fVector.y = vector2.y;
								fVector.z = vector2.z;
								outputs.linePoints.Add(fVector);
							}
						}
					}
					else
					{
						outputs.id = 0;
						outputs.slot = 0;
					}
					if (io.dev is waterPurifier)
					{
						string prefabName = "assets/prefabs/deployable/playerioents/poweredwaterpurifier/poweredwaterpurifier.storage.prefab";
						if (!(io.dev as waterPurifier).poweredType)
						{
							prefabName = "assets/prefabs/deployable/waterpurifier/waterstorage.prefab";
						}
						root.entities.Add(ioEntitySeperate(entities, io, null, outputs, prefabName));
					}
					else
					{
						iOEntity.outputs.Add(outputs);
					}
				}
				for (int l = 0; l < device.inputFrom.Length; l++)
				{
					io io2 = device.inputFrom[l];
					Inputs inputs = new Inputs();
					inputs.type = io2.type;
					if (io2.connectedTo != null)
					{
						if (io2.connectedTo.dev is waterPurifier)
						{
							if (io2.connectedTo.dev.id == 0)
							{
								io2.connectedTo.id = connectId;
								connectId++;
							}
							inputs.id = io2.connectedTo.id;
						}
						else
						{
							if (io2.connectedTo.dev.id == 0)
							{
								io2.connectedTo.dev.id = connectId;
								connectId++;
							}
							inputs.id = io2.connectedTo.dev.id;
						}
						inputs.slot = io2.index;
						if (inputs.slot == -1)
						{
							inputs.slot = 0;
						}
					}
					else
					{
						inputs.id = 0;
						inputs.slot = 0;
					}
					if (l == 2 && device is elevator && (device as elevator).top)
					{
						root.entities.Add(ioEntitySeperate(entities, io2, inputs, null, "assets/prefabs/deployable/elevator/elevatorioentity.prefab"));
						root.entities.Add(ioEntitySeperate(entities, io2, null, null, "assets/prefabs/deployable/elevator/elevator_lift.prefab"));
					}
					else
					{
						iOEntity.inputs.Add(inputs);
					}
				}
				entities.ioEntity = iOEntity;
			}
			root.entities.Add(entities);
			transform4.position = position;
			transform4.rotation = rotation;
		}
		foreach (BuilderPart bp2 in sys.bpList)
		{
			if ((bool)bp2.device)
			{
				bp2.device.id = 0;
				if (bp2.device is elevator)
				{
					bp2.device.inputFrom[2].id = 0;
				}
				else if (bp2.device is waterPurifier)
				{
					bp2.device.outputTo[0].id = 0;
				}
			}
		}
		string result = "";
		if (returns)
		{
			return JsonConvert.SerializeObject(root, Formatting.None, new JsonSerializerSettings
			{
				DefaultValueHandling = DefaultValueHandling.Ignore
			});
		}
		using (StreamWriter streamWriter = File.CreateText(path))
		{
			if (streamWriter == null)
			{
				popUp.inst.message("invalid characters in name?");
				return result;
			}
			JsonSerializer jsonSerializer = new JsonSerializer();
			jsonSerializer.Formatting = Formatting.Indented;
			jsonSerializer.DefaultValueHandling = DefaultValueHandling.Ignore;
			jsonSerializer.Serialize(streamWriter, root);
			popUp.inst.message("json saved");
			return result;
		}
	}

	private Entities ioEntitySeperate(Entities refStruct, io io, Inputs input, Outputs output, string prefabName)
	{
		Entities entities = new Entities();
		entities.prefabname = prefabName;
		if (input != null)
		{
			IOEntity iOEntity = new IOEntity();
			if (io.id == 0)
			{
				io.id = connectId;
				connectId++;
			}
			iOEntity.oldId = io.id;
			iOEntity.inputs.Add(input);
			entities.ioEntity = iOEntity;
		}
		if (output != null)
		{
			IOEntity iOEntity2 = new IOEntity();
			if (io.id == 0)
			{
				io.id = connectId;
				connectId++;
			}
			iOEntity2.oldId = io.id;
			iOEntity2.outputs.Add(output);
			entities.ioEntity = iOEntity2;
		}
		entities.pos = refStruct.pos;
		entities.rot = refStruct.rot;
		return entities;
	}

	public void CopyPasteFile(string path)
	{
		if (!File.Exists(path))
		{
			popUp.inst.message("file not found");
			return;
		}
		Root root = new Root();
		using (StreamReader reader = File.OpenText(path))
		{
			JsonTextReader reader2 = new JsonTextReader(reader);
			JsonSerializer jsonSerializer = new JsonSerializer();
			try
			{
				root = jsonSerializer.Deserialize<Root>(reader2);
			}
			catch
			{
				popUp.inst.message("json deserialize error");
				return;
			}
		}
		CopyPasteImport(root);
		fileList.inst.changeSaveInfo(path.Replace(".json", ""));
	}

	public void CopyPasteString(string _string)
	{
		Root root = new Root();
		JsonTextReader reader = new JsonTextReader(new StringReader(_string));
		JsonSerializer jsonSerializer = new JsonSerializer();
		try
		{
			root = jsonSerializer.Deserialize<Root>(reader);
		}
		catch
		{
			popUp.inst.message("json deserialize error");
			return;
		}
		CopyPasteImport(root);
	}

	public void CopyPasteImport(Root root)
	{
		Symmetry.inst.symTog.isOn = false;
		Symmetry.inst.centerSet = false;
		GameObject gameObject = null;
		sys.ClearScene();
		sys.SceneLoader(2, null);
		center = UnityEngine.Object.Instantiate(centerPrefab, Vector3.zero, Quaternion.identity);
		List<Output> list = new List<Output>();
		List<Input> list2 = new List<Input>();
		List<Device> list3 = new List<Device>();
		float num2 = 0f;
		GameObject gameObject2 = null;
		string rustPrefab = "";
		Vector3 vector = default(Vector3);
		Vector3 euler = default(Vector3);
		int num;
		for (int i = 0; i < root.entities.Count; i++)
		{
			Entities entities = root.entities[i];
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(entities.prefabname);
			if (fileNameWithoutExtension != rustPrefab)
			{
				rustPrefab = fileNameWithoutExtension;
				int num3 = partList.FindIndex((PartList item) => item.Name == rustPrefab);
				if (num3 < 0)
				{
					continue;
				}
				gameObject2 = MGMT.inst.prefabList[partList[num3].Id];
			}
			if (gameObject2 == null)
			{
				continue;
			}
			vector.x = Convert.ToSingle(entities.pos.x) * 0.33333f;
			vector.y = Convert.ToSingle(entities.pos.y) * 0.33333f + 0.5f;
			vector.z = Convert.ToSingle(entities.pos.z) * 0.33333f;
			if (vector.y > highY)
			{
				highY = vector.y;
			}
			euler.x = Convert.ToSingle(entities.rot.x) * 57.29578f;
			euler.y = Convert.ToSingle(entities.rot.y) * 57.29578f;
			euler.z = Convert.ToSingle(entities.rot.z) * 57.29578f;
			GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject2, vector, Quaternion.Euler(euler));
			gameObject3.name = gameObject2.name;
			BuilderPart component = gameObject3.GetComponent<BuilderPart>();
			Transform transform = component._transform;
			component.level = 0f;
			if ((bool)component.block)
			{
				if (entities.grade != 5)
				{
					component.tier = entities.grade;
					component.rend[0].sharedMaterial = sys.tierMats[component.tier];
				}
				if ((bool)component.origin && !component.found)
				{
					transform.rotation = component.origin.rotation;
					transform.Translate(transform.position - component.origin.position, Space.World);
				}
				else
				{
					if (!component.block.stairs)
					{
						transform.Translate(Vector3.up * 0.0333f);
					}
					if (component.block.pillar)
					{
						transform.Translate(Vector3.forward * -0.5f);
					}
					if ((component.block.floor || component.block.found) && !component.tri)
					{
						transform.Translate(Vector3.right * 0.5f);
					}
					if (component.block.steps || component.block.ramp)
					{
						transform.Translate(Vector3.right * 1f);
					}
					else if (component.block.stairs && !component.tri)
					{
						transform.Rotate(Vector3.up * 90f);
						transform.Translate(Vector3.right * -0.5f);
					}
					if (component.found)
					{
						transform.Translate(Vector3.down * 0.5f);
						if (transform.position.y < num2)
						{
							num2 = transform.position.y;
						}
						if (gameObject == null)
						{
							gameObject = gameObject3;
						}
					}
					if (component.tri && !component.block.stairs && !component.block.roof)
					{
						transform.Rotate(Vector3.up * 90f);
					}
					if (component.block.roof)
					{
						if (component.tri)
						{
							transform.Translate(Vector3.forward * 0.2889f);
						}
						else
						{
							transform.Translate(Vector3.forward * 0.5f);
						}
						transform.Rotate(Vector3.up * -90f);
					}
				}
			}
			else
			{
				if (entities.ioEntity != null)
				{
					Device device = component.device;
					if ((bool)device)
					{
						IOEntity ioEntity = entities.ioEntity;
						try
						{
							if (device is branch)
							{
								device.setValue(ioEntity.branchAmount, send: false);
							}
							else if (device is timerDevice)
							{
								device.setValue(ioEntity.timerLength, send: false);
							}
							else if (device is counter)
							{
								device.setValue((int)ioEntity.targetNumber, send: false);
							}
							else if (device is RF_broadcaster || device is RF_receiver)
							{
								device.setValue(ioEntity.frequency, send: false);
							}
							if (device is autoTurret && entities.items != null)
							{
								foreach (Items item in entities.items)
								{
									num = item.id;
									if (num != 0 && weaponsList.FindIndex((weapon w) => w.id == num) > 0)
									{
										device.setValue(num, send: false);
										break;
									}
								}
							}
						}
						catch
						{
						}
						if (ioEntity.inputs != null)
						{
							foreach (Inputs input in ioEntity.inputs)
							{
								if (input.id > 0)
								{
									list2.Add(new Input(device, ioEntity.oldId));
									break;
								}
							}
						}
						if (ioEntity.outputs != null)
						{
							bool flag = false;
							int num4 = -1;
							foreach (Outputs output2 in ioEntity.outputs)
							{
								num4++;
								if (output2.id != 0)
								{
									flag = true;
									if (num4 != device.outputTo.Length)
									{
										io io = device.outputTo[num4];
										int count = output2.linePoints.Count;
										if (count >= 1)
										{
											Vector3 zero = Vector3.zero;
											Vector3[] array = new Vector3[count];
											for (int j = 0; j < output2.linePoints.Count; j++)
											{
												Vector3 vector2 = default(Vector3);
												vector2.x = output2.linePoints[j].x * 0.3333f;
												vector2.y = output2.linePoints[j].y * 0.3333f;
												vector2.z = output2.linePoints[j].z * 0.3333f;
												if (wireOffset.isOn)
												{
													vector2.y += 0.5f;
													array[j] = vector2;
												}
												else
												{
													vector2 += transform.position;
													Vector3 point = vector2 - transform.position;
													point = transform.rotation * point;
													array[j] = point + transform.position;
												}
											}
											list.Add(new Output(io, output2.id, output2.slot, output2.wireColour, array));
										}
									}
								}
							}
							if (flag && device.powerSource)
							{
								list3.Add(device);
							}
						}
						if (device is ioCopyPaste)
						{
							if ((device as ioCopyPaste).elevator)
							{
								if (device.inputFrom[0].connectedTo == null)
								{
									UnityEngine.Object.Destroy(device.gameObject);
								}
							}
							else if (device.outputTo[0].connectedTo == null)
							{
								UnityEngine.Object.Destroy(device.gameObject);
							}
							continue;
						}
					}
				}
				if ((bool)component.door)
				{
					Flags flags = entities.flags;
					if (flags != null && !flags.Open)
					{
						StartCoroutine(component.changeDoorMeshState(state: false, audio: false, send: false));
					}
				}
				if (component.deploy.lockEnt != null && entities.codeLock != null)
				{
					locks.inst.addCodelock(component, entities.codeLock.code);
				}
				if ((bool)component.door || component.deploy.frameWall || component.deploy.windowPart)
				{
					transform.Translate(Vector3.up * 0.0333f);
				}
				if (component.deploy.copypaste != 0)
				{
					if (component.deploy.copypaste == Deploy.copyPaste.origin)
					{
						Quaternion lhs = transform.rotation * Quaternion.Inverse(component.deploy.origin.rotation);
						transform.rotation = lhs * transform.rotation;
						transform.position += transform.position - component.deploy.origin.position;
					}
					else if (component.deploy.copypaste == Deploy.copyPaste.up90)
					{
						transform.Rotate(Vector3.up * -90f);
					}
					else if (component.deploy.copypaste == Deploy.copyPaste.flip)
					{
						transform.Rotate(Vector3.up * 180f);
					}
					else if (component.deploy.copypaste == Deploy.copyPaste.r90)
					{
						transform.Rotate(Vector3.right * 90f);
					}
				}
			}
			sys.PlacedSetup(component, send: false, notLoaded: false, sound: false, useCodeTog: false);
		}
		sys.checkAllConditionals(clearNeighbors: false);
		if (num2 < -0.4f)
		{
			popUp.inst.message("applying underground fix");
			Vector3 vector3 = new Vector3(0f, Mathf.Abs(num2 - -0.4f), 0f);
			foreach (BuilderPart bp in sys.bpList)
			{
				bp._transform.position += vector3;
			}
			foreach (Output item2 in list)
			{
				for (int k = 0; k < item2.points.Length; k++)
				{
					item2.points[k] += vector3;
				}
			}
		}
		if (highY > 10000f)
		{
			popUp.inst.message("ERROR: too far from origin");
			sys.ClearScene();
			return;
		}
		CameraCtrl.parent.position = new Vector3(10f, highY + 5f, 10f);
		if (gameObject != null)
		{
			CameraCtrl.lookAt(gameObject.transform.position);
			CameraCtrl.setRotation();
		}
		GetComponent<SetLevels>().setLevels();
		if (BuilderSystem.multiplayer)
		{
			Multiplayer.sendResetCmd(sys.currentScene.buildIndex, 0uL);
			Multiplayer.inst.sendScene(sendAll: true);
		}
		foreach (Output output in list)
		{
			if (output.io.dev is ioCopyPaste)
			{
				io parentOutputIO = (output.io.dev as ioCopyPaste).getParentOutputIO();
				UnityEngine.Object.Destroy(output.io.gameObject);
				if (!(parentOutputIO != null))
				{
					MonoBehaviour.print("IO connect error");
					continue;
				}
				output.io = parentOutputIO;
			}
			int num5 = list2.FindIndex((Input item) => item.id == output.id);
			if (num5 == -1)
			{
				MonoBehaviour.print(output.io.dev.name + " missing connection");
			}
			else
			{
				Device dev = list2[num5].dev;
				io io2 = null;
				if (dev is ioCopyPaste)
				{
					io parentInputIO = (dev as ioCopyPaste).getParentInputIO();
					UnityEngine.Object.Destroy(dev.gameObject);
					if (!(parentInputIO != null))
					{
						MonoBehaviour.print("IO connect error");
						continue;
					}
					io2 = parentInputIO;
				}
				else
				{
					if (dev.inputFrom.Length <= output.slot)
					{
						continue;
					}
					io2 = dev.inputFrom[output.slot];
				}
				if (output.io.type == io2.type)
				{
					int color = output.color;
					if (output.io.type == 0)
					{
						color = colorIndexSwap(output.color, import: true);
					}
					else if (output.io.type == 1)
					{
						color = 6;
					}
					wiring.inst.wiredConnect(output.io, io2, output.points, color, powerThru: false, send: false);
				}
			}
		}
		foreach (Device item3 in list3)
		{
			item3.newPowerThru();
		}
	}

	private Vector3 NormalizePosition(Vector3 InitialPos, Vector3 CurrentPos, float diffRot)
	{
		Vector3 vector = CurrentPos - InitialPos;
		float x = vector.x * (float)Math.Cos(0f - diffRot) + vector.z * (float)Math.Sin(0f - diffRot);
		float z = vector.z * (float)Math.Cos(0f - diffRot) - vector.x * (float)Math.Sin(0f - diffRot);
		vector.x = x;
		vector.z = z;
		return vector;
	}

	public void SetCenter(Vector3 pos)
	{
		getCenter = false;
		if (center != null)
		{
			UnityEngine.Object.Destroy(center);
		}
		if (Physics.Raycast(pos + new Vector3(0f, 10f, 0f), Vector3.down, out RaycastHit hitInfo, 100f, 256))
		{
			pos = hitInfo.point;
		}
		center = UnityEngine.Object.Instantiate(centerPrefab, pos, Quaternion.identity);
		centerIcon.color = Color.white;
	}

	public void getCenterState()
	{
		getCenter = true;
		popUp.inst.message("click part to set center");
	}

	private float roundUP(float val)
	{
		val = Mathf.Round(val * 1000f) / 1000f;
		return val;
	}

	private void addElevatorLift()
	{
	}

	public void JSONtoggle()
	{
		if (fileList.inst.jsonToggle.isOn)
		{
			if (center != null)
			{
				center.SetActive(value: true);
				centerIcon.color = Color.white;
			}
			else
			{
				centerIcon.color = Color.grey;
			}
			BuilderUI.inst.mergeToggle.interactable = false;
		}
		else
		{
			if (center != null)
			{
				center.SetActive(value: false);
			}
			BuilderUI.inst.mergeToggle.interactable = true;
		}
		fileList.inst.loadList();
	}

	public void setWoodSkinId(int index)
	{
		switch (index)
		{
		case 0:
			woodSkinId = 0;
			break;
		case 1:
			woodSkinId = 10232;
			break;
		case 2:
			woodSkinId = 2;
			break;
		}
	}

	public void setStoneSkinId(int index)
	{
		switch (index)
		{
		case 0:
			stoneSkinId = 0;
			break;
		case 1:
			stoneSkinId = 10220;
			break;
		case 2:
			stoneSkinId = 10223;
			break;
		case 3:
			stoneSkinId = 10225;
			break;
		}
	}

	public void setMetalSkinId(int index)
	{
		switch (index)
		{
		case 0:
			metalSkinId = 0;
			break;
		case 1:
			metalSkinId = 10221;
			break;
		}
	}

	public void setMetalSkinColor(int id)
	{
		containerColorId = id;
		containerColor.color = colorPicker.Find(id.ToString()).GetComponent<Image>().color;
		colorPicker.gameObject.SetActive(value: false);
	}

	private void setPartList()
	{
		if (partList.Count > 0)
		{
			return;
		}
		partList = new List<PartList>();
		partList.Add(new PartList("waterpurifier.deployed", "water_purifier", "assets/prefabs/deployable/waterpurifier/", 0));
		partList.Add(new PartList("pillar", "pillar", "assets/prefabs/building core/pillar/", 0));
		partList.Add(new PartList("wall", "wall", "assets/prefabs/building core/wall/", 0));
		partList.Add(new PartList("wall.half", "wall_half", "assets/prefabs/building core/wall.half/", 0));
		partList.Add(new PartList("wall.low", "wall_low", "assets/prefabs/building core/wall.low/", 0));
		partList.Add(new PartList("wall.window", "wall_window", "assets/prefabs/building core/wall.window/", 0));
		partList.Add(new PartList("wall.doorway", "wall_doorway", "assets/prefabs/building core/wall.doorway/", 0));
		partList.Add(new PartList("wall.frame", "wall_frame", "assets/prefabs/building core/wall.frame/", 0));
		partList.Add(new PartList("floor", "floor", "assets/prefabs/building core/floor/", 0));
		partList.Add(new PartList("floor.frame", "floor_frame", "assets/prefabs/building core/floor.frame/", 0));
		partList.Add(new PartList("floor.triangle.frame", "floor_frame_tri", "assets/prefabs/building core/floor.triangle.frame/", 0));
		partList.Add(new PartList("floor.triangle", "floor_tri", "assets/prefabs/building core/floor.triangle/", 0));
		partList.Add(new PartList("foundation", "foundation", "assets/prefabs/building core/foundation/", 0));
		partList.Add(new PartList("foundation.triangle", "foundation_tri", "assets/prefabs/building core/foundation.triangle/", 0));
		partList.Add(new PartList("foundation.steps", "foundation_steps", "assets/prefabs/building core/foundation.steps/", 0));
		partList.Add(new PartList("ramp", "ramp", "assets/prefabs/building core/ramp/", 0));
		partList.Add(new PartList("roof", "roof", "assets/prefabs/building core/roof/", 0));
		partList.Add(new PartList("roof.triangle", "roof_tri", "assets/prefabs/building core/roof.triangle/", 0));
		partList.Add(new PartList("block.stair.lshape", "stairs_L", "assets/prefabs/building core/stairs.l/", 0));
		partList.Add(new PartList("block.stair.ushape", "stairs_U", "assets/prefabs/building core/stairs.u/", 0));
		partList.Add(new PartList("block.stair.spiral", "stairs_spiral", "assets/prefabs/building core/stairs.spiral/", 0));
		partList.Add(new PartList("block.stair.spiral.triangle", "stairs_spiral_tri", "assets/prefabs/building core/stairs.spiral.triangle/", 0));
		partList.Add(new PartList("door.double.hinged.metal", "wf_ddoor_metal", "assets/prefabs/building/door.double.hinged/", 0));
		partList.Add(new PartList("door.double.hinged.toptier", "wf_ddoor_armor", "assets/prefabs/building/door.double.hinged/", 0));
		partList.Add(new PartList("door.double.hinged.wood", "wf_ddoor_wood", "assets/prefabs/building/door.double.hinged/", 0));
		partList.Add(new PartList("door.hinged.metal", "door_metal", "assets/prefabs/building/door.hinged/", 0));
		partList.Add(new PartList("door.hinged.toptier", "door_armor", "assets/prefabs/building/door.hinged/", 0));
		partList.Add(new PartList("door.hinged.wood", "door_wood", "assets/prefabs/building/door.hinged/", 0));
		partList.Add(new PartList("shutter.metal.embrasure.a", "embrasure_horizontal", "assets/prefabs/building/wall.window.embrasure/", 0));
		partList.Add(new PartList("shutter.metal.embrasure.b", "embrasure_vertical", "assets/prefabs/building/wall.window.embrasure/", 0));
		partList.Add(new PartList("shutter.wood.a", "shutters", "assets/prefabs/building/wall.window.shutter/", 0));
		partList.Add(new PartList("floor.grill", "floor_grill", "assets/prefabs/building/floor.grill/", 0));
		partList.Add(new PartList("floor.triangle.grill", "floor_tri_grill", "assets/prefabs/building/floor.triangle.grill/", 0));
		partList.Add(new PartList("floor.ladder.hatch", "ff_hatch", "assets/prefabs/building/floor.ladder.hatch/", 0));
		partList.Add(new PartList("floor.triangle.ladder.hatch", "hatch_tri", "assets/prefabs/building/floor.triangle.ladder.hatch/", 0));
		partList.Add(new PartList("wall.frame.cell", "cell", "assets/prefabs/building/wall.frame.cell/", 0));
		partList.Add(new PartList("wall.frame.cell.gate", "cell_gate", "assets/prefabs/building/wall.frame.cell/", 0));
		partList.Add(new PartList("wall.frame.fence", "fence", "assets/prefabs/building/wall.frame.fence/", 0));
		partList.Add(new PartList("wall.frame.fence.gate", "fence_gate", "assets/prefabs/building/wall.frame.fence/", 0));
		partList.Add(new PartList("wall.frame.garagedoor", "garage_door", "assets/prefabs/building/wall.frame.garagedoor/", 0));
		partList.Add(new PartList("wall.frame.netting", "netting", "assets/prefabs/building/wall.frame.netting/", 0));
		partList.Add(new PartList("wall.frame.shopfront", "shopfront", "assets/prefabs/building/wall.frame.shopfront/", 0));
		partList.Add(new PartList("wall.frame.shopfront.metal", "shopfront_metal", "assets/prefabs/building/wall.frame.shopfront/", 0));
		partList.Add(new PartList("wall.window.bars.metal", "window_bars_metal", "assets/prefabs/building/wall.window.bars/", 0));
		partList.Add(new PartList("wall.window.bars.toptier", "window_bars_reinforced", "assets/prefabs/building/wall.window.bars/", 0));
		partList.Add(new PartList("wall.window.bars.wood", "window_bars_wood", "assets/prefabs/building/wall.window.bars/", 0));
		partList.Add(new PartList("wall.window.glass.reinforced", "window_glass_reinforced", "assets/prefabs/building/wall.window.reinforcedglass/", 0));
		partList.Add(new PartList("autoturret_deployed", "sentry", "assets/prefabs/npc/autoturret/", 0));
		partList.Add(new PartList("barricade.concrete", "barricade_concrete", "assets/prefabs/deployable/barricades/", 0));
		partList.Add(new PartList("barricade.metal", "barricade", "assets/prefabs/deployable/barricades/", 0));
		partList.Add(new PartList("barricade.sandbags", "barricade_sandbags", "assets/prefabs/deployable/barricades/", 0));
		partList.Add(new PartList("barricade.stone", "barricade_stone", "assets/prefabs/deployable/barricades/", 0));
		partList.Add(new PartList("barricade.wood", "barricade_wood", "assets/prefabs/deployable/barricades/", 0));
		partList.Add(new PartList("barricade.woodwire", "barricade_woodwire", "assets/prefabs/deployable/barricades/", 0));
		partList.Add(new PartList("bbq.deployed", "BBQ", "assets/prefabs/deployable/bbq/", 0));
		partList.Add(new PartList("beartrap", "beartrap", "assets/prefabs/deployable/bear trap/", 0));
		partList.Add(new PartList("bed_deployed", "bed", "assets/prefabs/deployable/bed/", 0));
		partList.Add(new PartList("box.wooden.large", "box_large", "assets/prefabs/deployable/large wood storage/", 0));
		partList.Add(new PartList("woodbox_deployed", "box", "assets/prefabs/deployable/woodenbox/", 0));
		partList.Add(new PartList("campfire", "campfire", "assets/prefabs/deployable/campfire/", 0));
		partList.Add(new PartList("ceilinglight.deployed", "ceiling_light", "assets/prefabs/deployable/ceiling light/", 0));
		partList.Add(new PartList("chair.deployed", "chair", "assets/prefabs/deployable/chair/", 0));
		partList.Add(new PartList("cupboard.tool.deployed", "cupboard", "assets/prefabs/deployable/tool cupboard/", 0));
		partList.Add(new PartList("dropbox.deployed", "dropbox", "assets/prefabs/deployable/dropbox/", 0));
		partList.Add(new PartList("electric.windmill.small", "wind_turbine", "assets/prefabs/deployable/windmill/", 0));
		partList.Add(new PartList("fireplace.deployed", "fireplace", "assets/prefabs/deployable/fireplace/", 0));
		partList.Add(new PartList("flameturret.deployed", "flame_turret", "assets/prefabs/npc/flame turret/", 0));
		partList.Add(new PartList("fridge.deployed", "fridge", "assets/prefabs/deployable/fridge/", 0));
		partList.Add(new PartList("furnace", "furnace", "assets/prefabs/deployable/furnace/", 0));
		partList.Add(new PartList("furnace.large", "furnace_large", "assets/prefabs/deployable/furnace.large/", 0));
		partList.Add(new PartList("gates.external.high.stone", "gate_stone", "assets/prefabs/building/gates.external.high/gates.external.high.stone/", 0));
		partList.Add(new PartList("gates.external.high.wood", "gate_wood", "assets/prefabs/building/gates.external.high/gates.external.high.wood/", 0));
		partList.Add(new PartList("wall.external.high.stone", "wall_ext", "assets/prefabs/building/wall.external.high.stone/", 0));
		partList.Add(new PartList("wall.external.high.wood", "wall_ext_wood", "assets/prefabs/building/wall.external.high.wood/", 0));
		partList.Add(new PartList("graveyardfence", "graveyard_fence", "assets/prefabs/misc/halloween/graveyard_fence/", 0));
		partList.Add(new PartList("guntrap.deployed", "shotgun_trap", "assets/prefabs/deployable/single shot trap/", 0));
		partList.Add(new PartList("jackolantern.happy", "jack_o_lantern", "assets/prefabs/deployable/jack o lantern/", 0));
		partList.Add(new PartList("carvable.pumpkin", "carvable_pumpkin", "assets/prefabs/misc/halloween/carvablepumpkin/", 0));
		partList.Add(new PartList("ladder.wooden.wall", "ladder_wood", "assets/prefabs/building/ladder.wall.wood/", 0));
		partList.Add(new PartList("landmine", "landmine", "assets/prefabs/deployable/landmine/", 0));
		partList.Add(new PartList("lantern.deployed", "lantern", "assets/prefabs/deployable/lantern/", 0));
		partList.Add(new PartList("locker.deployed", "locker", "assets/prefabs/deployable/locker/", 0));
		partList.Add(new PartList("mailbox.deployed", "mailbox", "assets/prefabs/deployable/mailbox/", 0));
		partList.Add(new PartList("minicopter.entity", "minicopter", "assets/content/vehicles/minicopter/", 0));
		partList.Add(new PartList("planter.large.deployed", "planter", "assets/prefabs/deployable/planters/", 0));
		partList.Add(new PartList("planter.small.deployed", "planter_small", "assets/prefabs/deployable/planters/", 0));
		partList.Add(new PartList("reactivetarget_deployed", "target", "assets/prefabs/deployable/reactive target/", 0));
		partList.Add(new PartList("recycler_static", "recycler", "assets/bundled/prefabs/static/", 0));
		partList.Add(new PartList("refinery_small_deployed", "refinery", "assets/prefabs/deployable/oil refinery/", 0));
		partList.Add(new PartList("repairbench_deployed", "repair_bench", "assets/prefabs/deployable/repair bench/", 0));
		partList.Add(new PartList("mixingtable.deployed", "mixing_table", "assets/prefabs/deployable/mixingtable/", 0));
		partList.Add(new PartList("researchtable_deployed", "research_table", "assets/prefabs/deployable/research table/", 0));
		partList.Add(new PartList("sam_site_turret_deployed", "sam_site", "assets/prefabs/npc/sam_site_turret/", 0));
		partList.Add(new PartList("rowboat", "rowboat", "assets/content/vehicles/boats/rowboat/", 0));
		partList.Add(new PartList("rug.bear.deployed", "rug_bear", "assets/prefabs/deployable/rug/", 0));
		partList.Add(new PartList("rug.deployed", "rug", "assets/prefabs/deployable/rug/", 0));
		partList.Add(new PartList("searchlight.deployed", "searchlight", "assets/prefabs/deployable/search light/", 0));
		partList.Add(new PartList("sedantest.entity", "sedan", "assets/content/vehicles/sedan_a/", 0));
		partList.Add(new PartList("shelves", "shelves", "assets/prefabs/deployable/shelves/", 0));
		partList.Add(new PartList("sign.hanging", "sign_hanging", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.hanging.banner.large", "banner_hanging", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.hanging.ornate", "sign_hanging_ornate", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.huge.wood", "sign_huge", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.large.wood", "sign_large", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.medium.wood", "sign_medium", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.pictureframe.landscape", "sign_pictureframe_landscape", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.pictureframe.portrait", "sign_pictureframe_portrait", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.pictureframe.tall", "sign_pictureframe_tall", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.pictureframe.xl", "sign_pictureframe_xl", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.pictureframe.xxl", "sign_pictureframe_xxl", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.pole.banner.large", "banner_pole", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.post.double", "sign_post_double", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.post.single", "sign_post_single", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.post.town", "sign_post_town", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.post.town.roof", "sign_post_town_roof", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.small.wood", "sign_small", "assets/prefabs/deployable/signs/", 0));
		partList.Add(new PartList("sign.neon.xl", "sign_neon_large", "assets/prefabs/misc/xmas/neon_sign/", 0));
		partList.Add(new PartList("sign.neon.125x215", "sign_neon_medium", "assets/prefabs/misc/xmas/neon_sign/", 0));
		partList.Add(new PartList("sign.neon.125x125", "sign_neon_small", "assets/prefabs/misc/xmas/neon_sign/", 0));
		partList.Add(new PartList("sign.neon.125x215.animated", "sign_neon_medium_animated", "assets/prefabs/misc/xmas/neon_sign/", 0));
		partList.Add(new PartList("sign.neon.xl.animated", "sign_neon_large_animated", "assets/prefabs/misc/xmas/neon_sign/", 0));
		partList.Add(new PartList("sleepingbag_leather_deployed", "sleeping_bag", "assets/prefabs/deployable/sleeping bag/", 0));
		partList.Add(new PartList("snowman.deployed", "snowman", "assets/prefabs/misc/xmas/snowman/", 0));
		partList.Add(new PartList("solarpanel.large.deployed", "solar_panel_large", "assets/prefabs/deployable/playerioents/generators/solar_panels_roof/", 0));
		partList.Add(new PartList("spinner.wheel.deployed", "spinner_wheel", "assets/prefabs/deployable/spinner_wheel/", 0));
		partList.Add(new PartList("table.deployed", "table", "assets/prefabs/deployable/table/", 0));
		partList.Add(new PartList("tunalight.deployed", "tunacan_lamp", "assets/prefabs/deployable/tuna can wall lamp/", 0));
		partList.Add(new PartList("vendingmachine.deployed", "vending_machine", "assets/prefabs/deployable/vendingmachine/", 0));
		partList.Add(new PartList("watchtower.wood", "watchtower_wood", "assets/prefabs/building/watchtower.wood/", 0));
		partList.Add(new PartList("water_catcher_large", "water_catcher_large", "assets/prefabs/deployable/water catcher/", 0));
		partList.Add(new PartList("water_catcher_small", "water_catcher", "assets/prefabs/deployable/water catcher/", 0));
		partList.Add(new PartList("waterbarrel", "water_barrel", "assets/prefabs/deployable/liquidbarrel/", 0));
		partList.Add(new PartList("workbench1.deployed", "workbench_tier1", "assets/prefabs/deployable/tier 1 workbench/", 0));
		partList.Add(new PartList("workbench2.deployed", "workbench_tier2", "assets/prefabs/deployable/tier 2 workbench/", 0));
		partList.Add(new PartList("workbench3.deployed", "workbench_tier3", "assets/prefabs/deployable/tier 3 workbench/", 0));
		partList.Add(new PartList("xmas.lightstring.deployed", "xmas_lights", "assets/prefabs/misc/xmas/christmas_lights/", 0));
		partList.Add(new PartList("xmas_tree_a.deployed", "xmas_tree", "assets/prefabs/misc/xmas/xmastree/", 0));
		partList.Add(new PartList("rhib", "RHIB", "assets/content/vehicles/boats/rhib/", 0));
		partList.Add(new PartList("chippyarcademachine", "chippy_arcade", "assets/prefabs/misc/chippy arcade/", 0));
		partList.Add(new PartList("hitchtrough.deployed", "hitch_trough", "assets/prefabs/deployable/hitch & trough/", 0));
		partList.Add(new PartList("counter", "counter", "assets/prefabs/deployable/playerioents/counter/", 0));
		partList.Add(new PartList("doorcontroller.deployed", "door_controller", "assets/prefabs/deployable/playerioents/doormanipulators/", 0));
		partList.Add(new PartList("andswitch.entity", "AND_switch", "assets/prefabs/deployable/playerioents/gates/andswitch/", 0));
		partList.Add(new PartList("electrical.blocker.deployed", "blocker", "assets/prefabs/deployable/playerioents/gates/blocker/", 0));
		partList.Add(new PartList("electrical.branch.deployed", "branch", "assets/prefabs/deployable/playerioents/gates/branch/", 0));
		partList.Add(new PartList("electrical.combiner.deployed", "root_combiner", "assets/prefabs/deployable/playerioents/gates/combiner/", 0));
		partList.Add(new PartList("electrical.memorycell.deployed", "memory_cell", "assets/prefabs/deployable/playerioents/gates/dflipflop/", 0));
		partList.Add(new PartList("orswitch.entity", "OR_switch", "assets/prefabs/deployable/playerioents/gates/orswitch/", 0));
		partList.Add(new PartList("xorswitch.entity", "XOR_switch", "assets/prefabs/deployable/playerioents/gates/xorswitch/", 0));
		partList.Add(new PartList("switch", "switch", "assets/prefabs/deployable/playerioents/simpleswitch/", 0));
		partList.Add(new PartList("splitter", "splitter", "assets/prefabs/deployable/playerioents/splitter/", 0));
		partList.Add(new PartList("timer", "timer", "assets/prefabs/deployable/playerioents/timers/", 0));
		partList.Add(new PartList("pressurepad.deployed", "pressure_pad", "assets/prefabs/deployable/playerioents/detectors/pressurepad/", 0));
		partList.Add(new PartList("laserdetector", "laser_detector", "assets/prefabs/deployable/playerioents/detectors/laserdetector/", 0));
		partList.Add(new PartList("electrical.random.switch.deployed", "RAND_switch", "assets/prefabs/deployable/playerioents/gates/randswitch/", 0));
		partList.Add(new PartList("smallrechargablebattery.deployed", "battery_small", "assets/prefabs/deployable/playerioents/batteries/", 0));
		partList.Add(new PartList("medium.rechargable.battery.deployed", "battery_medium", "assets/prefabs/deployable/playerioents/batteries/medium/", 0));
		partList.Add(new PartList("large.rechargable.battery.deployed", "battery_large", "assets/prefabs/deployable/playerioents/batteries/large/", 0));
		partList.Add(new PartList("rfbroadcaster", "RF_broadcaster", "assets/prefabs/deployable/playerioents/gates/rfbroadcaster/", 0));
		partList.Add(new PartList("rfreceiver", "RF_receiver", "assets/prefabs/deployable/playerioents/gates/rfreceiver/", 0));
		partList.Add(new PartList("audioalarm", "audio_alarm", "assets/prefabs/deployable/playerioents/alarms/", 0));
		partList.Add(new PartList("hbhfsensor.deployed", "HBHF_sensor", "assets/prefabs/deployable/playerioents/detectors/hbhfsensor/", 0));
		partList.Add(new PartList("electric.sirenlight.deployed", "siren_light", "assets/prefabs/deployable/playerioents/lights/sirenlight/", 0));
		partList.Add(new PartList("electric.flasherlight.deployed", "flasher_light", "assets/prefabs/deployable/playerioents/lights/flasherlight/", 0));
		partList.Add(new PartList("generator.small", "generator_small", "assets/prefabs/deployable/playerioents/generators/", 0));
		partList.Add(new PartList("simplelight", "simple_light", "assets/prefabs/deployable/playerioents/lights/", 0));
		partList.Add(new PartList("small_fuel_generator.deployed", "fuel_generator_small", "assets/prefabs/deployable/playerioents/generators/fuel generator/", 0));
		partList.Add(new PartList("scraptransporthelicopter", "transport_helicopter", "assets/content/vehicles/scrap heli carrier/", 0));
		partList.Add(new PartList("attackhelicopter.entity", "attack_helicopter", "assets/content/vehicles/attackhelicopter/", 0));
		partList.Add(new PartList("teslacoil.deployed", "tesla_coil", "assets/prefabs/deployable/playerioents/teslacoil/", 0));
		partList.Add(new PartList("mining_quarry", "quarry", "assets/prefabs/deployable/quarry/", 0));
		partList.Add(new PartList("piano.deployed", "piano", "assets/prefabs/instruments/piano/", 0));
		partList.Add(new PartList("drumkit.deployed", "drumkit", "assets/prefabs/instruments/drumkit/", 0));
		partList.Add(new PartList("xylophone.deployed", "xylophone", "assets/prefabs/instruments/xylophone/", 0));
		partList.Add(new PartList("stocking_large_deployed", "stocking_super", "assets/prefabs/misc/xmas/stockings/", 0));
		partList.Add(new PartList("stocking_small_deployed", "stocking_small", "assets/prefabs/misc/xmas/stockings/", 0));
		partList.Add(new PartList("giantcandycane.deployed", "candycane_giant", "assets/prefabs/misc/xmas/giant_candy_cane/", 0));
		partList.Add(new PartList("pookie_deployed", "pookie", "assets/prefabs/misc/xmas/pookie/", 0));
		partList.Add(new PartList("igniter.deployed", "igniter", "assets/prefabs/deployable/playerioents/igniter/", 0));
		partList.Add(new PartList("coffinstorage", "coffin", "assets/prefabs/misc/halloween/coffin/", 0));
		partList.Add(new PartList("computerstation.deployed", "computer_station", "assets/prefabs/deployable/computerstation/", 0));
		partList.Add(new PartList("cctv_deployed", "CCTV_camera", "assets/prefabs/deployable/cctvcamera/", 0));
		partList.Add(new PartList("ptzsecuritycamera", "PTZ_CCTV_camera", "assets/prefabs/deployable/ptz security camera/", 0));
		partList.Add(new PartList("composter", "composter", "assets/prefabs/deployable/composter/", 0));
		partList.Add(new PartList("electric.sprinkler.deployed", "sprinkler", "assets/prefabs/deployable/playerioents/sprinkler/", 0));
		partList.Add(new PartList("electrical.heater", "electric_heater", "assets/prefabs/deployable/playerioents/electricheater/", 0));
		partList.Add(new PartList("poweredwaterpurifier.deployed", "powered_water_purifier", "assets/prefabs/deployable/playerioents/poweredwaterpurifier/", 0));
		partList.Add(new PartList("poweredwaterpurifier.storage", "pwp_storage", "assets/prefabs/deployable/playerioents/poweredwaterpurifier/", 0));
		partList.Add(new PartList("waterstorage", "wp_storage", "assets/prefabs/deployable/waterpurifier/", 0));
		partList.Add(new PartList("fogmachine", "fogger", "assets/content/props/fog machine/", 0));
		partList.Add(new PartList("strobelight", "strobe_light", "assets/content/props/strobe light/", 0));
		partList.Add(new PartList("survivalfishtrap.deployed", "fish_trap", "assets/prefabs/deployable/survivalfishtrap/", 0));
		partList.Add(new PartList("smartalarm", "smart_alarm", "assets/prefabs/deployable/playerioents/app/smartalarm/", 0));
		partList.Add(new PartList("smartswitch", "smart_switch", "assets/prefabs/deployable/playerioents/app/smartswitch/", 0));
		partList.Add(new PartList("storagemonitor.deployed", "storage_monitor", "assets/prefabs/deployable/playerioents/app/storagemonitor/", 0));
		partList.Add(new PartList("button", "red_button", "assets/prefabs/deployable/playerioents/button/", 0));
		partList.Add(new PartList("electrical.modularcarlift.deployed", "modular_car_lift", "assets/prefabs/deployable/modular car lift/", 0));
		partList.Add(new PartList("2module_car_spawned.entity", "modular_car_small", "assets/content/vehicles/modularcar/", 0));
		partList.Add(new PartList("3module_car_spawned.entity", "modular_car_med", "assets/content/vehicles/modularcar/", 0));
		partList.Add(new PartList("4module_car_spawned.entity", "modular_car_large", "assets/content/vehicles/modularcar/", 0));
		partList.Add(new PartList("water.pump.deployed", "water_pump", "assets/prefabs/deployable/playerioents/waterpump/", 0));
		partList.Add(new PartList("fluidswitch", "fluid_switch_pump", "assets/prefabs/deployable/playerioents/fluidswitch/", 0));
		partList.Add(new PartList("elevator", "elevator", "assets/prefabs/deployable/elevator/", 0));
		partList.Add(new PartList("elevatorioentity", "elevatorCheck", "assets/prefabs/deployable/elevator/", 0));
		partList.Add(new PartList("cursedcauldron.deployed", "cursed_cauldron", "assets/prefabs/misc/halloween/cursed_cauldron/", 0));
		partList.Add(new PartList("scarecrow.deployed", "scarecrow", "assets/prefabs/misc/halloween/scarecrow/", 0));
		partList.Add(new PartList("abovegroundpool.deployed", "pool_above_ground", "assets/prefabs/misc/summer_dlc/abovegroundpool/", 0));
		partList.Add(new PartList("paddlingpool.deployed", "pool_paddling", "assets/prefabs/misc/summer_dlc/paddling_pool/", 0));
		partList.Add(new PartList("telephone.deployed", "telephone", "assets/prefabs/voiceaudio/telephone/", 0));
		partList.Add(new PartList("sofa.deployed", "sofa", "assets/prefabs/deployable/sofa/", 0));
		partList.Add(new PartList("hobobarrel.deployed", "hobo_barrel", "assets/prefabs/misc/twitch/hobobarrel/", 0));
		partList.Add(new PartList("chineselantern.deployed", "chinese_lantern", "assets/prefabs/misc/chinesenewyear/chineselantern/", 0));
		partList.Add(new PartList("slotmachine", "slot_machine", "assets/prefabs/misc/casino/slotmachine/", 0));
		partList.Add(new PartList("barricade.cover.wood_double", "barricade_wood_cover", "assets/prefabs/deployable/barricades/", 0));
		partList.Add(new PartList("beachtowel.deployed", "beach_towel", "assets/prefabs/misc/summer_dlc/beach_towel/", 0));
		partList.Add(new PartList("beachchair.deployed", "beach_chair", "assets/prefabs/misc/summer_dlc/beach_chair/", 0));
		partList.Add(new PartList("beachparasol.deployed", "beach_parasol", "assets/prefabs/misc/summer_dlc/beach_chair/", 0));
		partList.Add(new PartList("beachtable.deployed", "beach_table", "assets/prefabs/misc/summer_dlc/beach_chair/", 0));
		partList.Add(new PartList("ball.entity", "giant_ball", "assets/content/vehicles/ball/", 0));
		partList.Add(new PartList("phonebooth.static", "phone_booth", "assets/bundled/prefabs/autospawn/phonebooth/", 0));
		partList.Add(new PartList("fluidsplitter", "fluid_splitter", "assets/prefabs/deployable/playerioents/fluidsplitter/", 0));
		partList.Add(new PartList("fluid.combiner.deployed", "fluid_combiner", "assets/prefabs/deployable/playerioents/fluidcombiner/", 0));
		partList.Add(new PartList("submarineduo.entity", "submarine_duo", "assets/content/vehicles/submarine/", 0));
		partList.Add(new PartList("submarinesolo.entity", "submarine_solo", "assets/content/vehicles/submarine/", 0));
		partList.Add(new PartList("boombox.deployed", "boom_box", "assets/prefabs/voiceaudio/boombox/", 0));
		partList.Add(new PartList("connectedspeaker.deployed", "connected_speaker", "assets/prefabs/voiceaudio/hornspeaker/", 0));
		partList.Add(new PartList("discoball.deployed", "disco_ball", "assets/prefabs/voiceaudio/discoball/", 0));
		partList.Add(new PartList("discofloor.deployed", "disco_floor", "assets/prefabs/voiceaudio/discofloor/", 0));
		partList.Add(new PartList("laserlight.deployed", "laser_light", "assets/prefabs/voiceaudio/laserlight/", 0));
		partList.Add(new PartList("skull_fire_pit", "skull_fire_pit", "assets/prefabs/misc/halloween/skull_fire_pit/", 0));
		partList.Add(new PartList("skullspikes.deployed", "skull_spikes", "assets/prefabs/misc/halloween/skull spikes/", 0));
		partList.Add(new PartList("largecandleset", "candle_set_large", "assets/prefabs/misc/halloween/candles/", 0));
		partList.Add(new PartList("smallcandleset", "candle_set_small", "assets/prefabs/misc/halloween/candles/", 0));
		partList.Add(new PartList("frankensteintable.deployed", "franken_table", "assets/prefabs/deployable/frankensteintable/", 0));
		partList.Add(new PartList("wall.external.high.ice", "wall_ext_ice", "assets/prefabs/misc/xmas/icewalls/", 0));
		partList.Add(new PartList("icewall", "ice_wall_short", "assets/prefabs/misc/xmas/icewalls/", 0));
		partList.Add(new PartList("newyeargong.deployed", "new_year_gong", "assets/prefabs/misc/chinesenewyear/newyeargong/", 0));
		partList.Add(new PartList("industrial.wall.lamp.deployed", "industrial_wall_light", "assets/prefabs/misc/permstore/industriallight/", 0));
		partList.Add(new PartList("industrial.wall.lamp.green.deployed", "industrial_wall_light_green", "assets/prefabs/misc/permstore/industriallight/", 0));
		partList.Add(new PartList("industrial.wall.lamp.red.deployed", "industrial_wall_light_red", "assets/prefabs/misc/permstore/industriallight/", 0));
		partList.Add(new PartList("tomahasnowmobile", "snowmobile_tomaha", "assets/content/vehicles/snowmobiles/", 0));
		partList.Add(new PartList("soundlight.deployed", "sound_light", "assets/prefabs/voiceaudio/soundlight/", 0));
		partList.Add(new PartList("mining.pumpjack", "pumpjack", "assets/prefabs/deployable/oil jack/", 0));
		partList.Add(new PartList("gravestone.stone.deployed", "gravestone", "assets/prefabs/misc/halloween/deployablegravestone/", 0));
		partList.Add(new PartList("gravestone.wood.deployed", "wooden_cross", "assets/prefabs/misc/halloween/deployablegravestone/", 0));
		partList.Add(new PartList("doorgarland.deployed", "garland_doorway", "assets/prefabs/misc/xmas/doorgarland/", 0));
		partList.Add(new PartList("windowgarland.deployed", "garland_window", "assets/prefabs/misc/xmas/windowgarland/", 0));
		partList.Add(new PartList("double_doorgarland.deployed", "garland_frame", "assets/prefabs/misc/xmas/double_doorgarland/", 0));
		partList.Add(new PartList("electricfurnace.deployed", "electric_furnace", "assets/prefabs/deployable/playerioents/electricfurnace/", 0));
		partList.Add(new PartList("industrialconveyor.deployed", "industrial_conveyor", "assets/prefabs/deployable/playerioents/industrialconveyor/", 0));
		partList.Add(new PartList("storageadaptor.deployed", "storage_adaptor", "assets/prefabs/deployable/playerioents/industrialadaptors/", 0));
		partList.Add(new PartList("industrialcrafter.deployed", "industrial_crafter", "assets/prefabs/deployable/playerioents/industrialcrafter/", 0));
		partList.Add(new PartList("industrialsplitter.deployed", "industrial_splitter", "assets/prefabs/deployable/playerioents/industrialsplitter/", 0));
		partList.Add(new PartList("industrialcombiner.deployed", "industrial_combiner", "assets/prefabs/deployable/playerioents/industrialcombiner/", 0));
		partList.Add(new PartList("drone.deployed", "drone", "assets/prefabs/deployable/drone/", 0));
		partList.Add(new PartList("cardreader", "card_reader", "assets/prefabs/io/electric/switches/", 0));
		partList.Add(new PartList("tugboat", "tugboat", "assets/content/vehicles/boats/tugboat/", 0));
		partList.Add(new PartList("storage_barrel_a", "storage_barrel_vertical", "assets/prefabs/misc/decor_dlc/storagebarrel/", 0));
		partList.Add(new PartList("storage_barrel_b", "storage_barrel_vertical", "assets/prefabs/misc/decor_dlc/storagebarrel/", 0));
		partList.Add(new PartList("storage_barrel_c", "storage_barrel_horizontal", "assets/prefabs/misc/decor_dlc/storagebarrel/", 0));
		partList.Add(new PartList("weaponrack_wide.deployed", "weapon_rack_wide", "assets/prefabs/deployable/weaponracks/", 0));
		partList.Add(new PartList("weaponrack_tall.deployed", "weapon_rack_tall", "assets/prefabs/deployable/weaponracks/", 0));
		partList.Add(new PartList("weaponrack_horizontal.deployed", "weapon_rack_horizontal", "assets/prefabs/deployable/weaponracks/", 0));
		partList.Add(new PartList("weaponrack_stand.deployed", "weapon_rack_stand", "assets/prefabs/deployable/weaponracks/", 0));
		partList.Add(new PartList("electric.seismicsensor.deployed", "seismic_sensor", "assets/prefabs/deployable/playerioents/seismicsensor/", 0));
		partList.Add(new PartList("motorbike", "motorbike", "assets/content/vehicles/bikes/", 0));
		partList.Add(new PartList("electric.digitalclock.deployed", "digital_clock", "assets/prefabs/deployable/playerioents/digitalclock/", 0));
		partList.Add(new PartList("mortarblue", "mortar_blue", "assets/prefabs/deployable/fireworks/", 0));
		partList.Add(new PartList("mortarchampagne", "mortar_champagne", "assets/prefabs/deployable/fireworks/", 0));
		partList.Add(new PartList("mortargreen", "mortar_green", "assets/prefabs/deployable/fireworks/", 0));
		partList.Add(new PartList("mortarorange", "mortar_orange", "assets/prefabs/deployable/fireworks/", 0));
		partList.Add(new PartList("mortarred", "mortar_red", "assets/prefabs/deployable/fireworks/", 0));
		partList.Add(new PartList("mortarviolet", "mortar_violet", "assets/prefabs/deployable/fireworks/", 0));
		for (int i = 0; i < partList.Count; i++)
		{
			string fname = partList[i].Fname;
			for (int j = 0; j < MGMT.inst.prefabList.Length; j++)
			{
				if (fname == MGMT.inst.prefabList[j].name)
				{
					partList[i].Id = j;
					break;
				}
			}
		}
	}

	public string GetWeaponNameFromID(int num)
	{
		weapon weapon = weaponsList.Single((weapon w) => w.id == num);
		if (weapon != null)
		{
			return weapon.name;
		}
		return "";
	}

	public void setWeaponList()
	{
		if (weaponsList.Count <= 0)
		{
			weaponsList.Add(new weapon(0, "empty", -1));
			weaponsList.Add(new weapon(1545779598, "assault rifle", 2));
			weaponsList.Add(new weapon(-2069578888, "M249", 2));
			weaponsList.Add(new weapon(-1812555177, "LR-300", 2));
			weaponsList.Add(new weapon(-778367295, "L96 rifle", 2));
			weaponsList.Add(new weapon(1588298435, "bolt action rifle", 2));
			weaponsList.Add(new weapon(-904863145, "semi-automatic rifle", 2));
			weaponsList.Add(new weapon(28201841, "M39 rifle", 2));
			weaponsList.Add(new weapon(-1758372725, "thompson", 0));
			weaponsList.Add(new weapon(1796682209, "custom smg", 0));
			weaponsList.Add(new weapon(1318558775, "MP5", 0));
			weaponsList.Add(new weapon(1373971859, "python", 0));
			weaponsList.Add(new weapon(818877484, "semi-automatic pistol", 0));
			weaponsList.Add(new weapon(-852563019, "M92 pistol", 0));
			weaponsList.Add(new weapon(649912614, "revolver", 0));
			weaponsList.Add(new weapon(-41440462, "spas-12 shotgun", 1));
			weaponsList.Add(new weapon(795371088, "pump shotgun", 1));
			weaponsList.Add(new weapon(-765183617, "double barrel shotgun", 1));
			weaponsList.Add(new weapon(-1367281941, "waterpipe shotgun", 1));
			weaponsList.Add(new weapon(-75944661, "eoka", 1));
			weaponsList.Add(new weapon(273172220, "trumpet", -1));
		}
	}

	public int colorIndexSwap(int index, bool import)
	{
		if (import)
		{
			switch (index)
			{
			case 0:
				return 0;
			case 1:
				return 5;
			case 2:
				return 2;
			case 3:
				return 1;
			case 4:
				return 3;
			case 5:
				return 9;
			case 6:
				return 10;
			case 7:
				return 4;
			case 8:
				return 7;
			case 9:
				return 8;
			}
		}
		else
		{
			switch (index)
			{
			case 0:
				return 0;
			case 5:
				return 1;
			case 2:
				return 2;
			case 1:
				return 3;
			case 3:
				return 4;
			case 9:
				return 5;
			case 10:
				return 6;
			case 4:
				return 7;
			case 7:
				return 8;
			case 8:
				return 9;
			}
		}
		return 0;
	}
}
