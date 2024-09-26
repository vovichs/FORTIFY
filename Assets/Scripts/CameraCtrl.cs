using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraCtrl : MonoBehaviour
{
	public enum Mode
	{
		free,
		third,
		ground,
		mount,
		ortho
	}

	public static CameraCtrl inst;

	private BuilderSystem sys;

	public Mode mode;

	public static bool lvlCamMove;

	public bool screenshotMode;

	private bool rbMode;

	private float speed = 8f;

	private static int speedIndex = 2;

	public float[] speeds;

	public float[] shiftSpeeds;

	public Button[] speedButtons;

	public float lookMulti = 4f;

	private int yMax = 90;

	private int yMin = -90;

	private float turnRate = 10f;

	private static float xDeg = 0f;

	private static float yDeg = 0f;

	private static Vector3 startPos;

	private static Quaternion startRot;

	private static Quaternion startRotP;

	public Camera cam;

	public Camera[] cams;

	private Vector3 dir;

	public Rigidbody rb;

	public LayerMask jumpMask;

	private Vector3 camColCenter;

	public bool climb;

	private bool jump;

	private bool crouch;

	public Toggle groundTog;

	public Toggle thridPersonTog;

	public Toggle orthoToggle;

	public Slider orthoZoomSlider;

	public Toggle hideTerrainTog;

	public Toggle walkingToggle;

	public Toggle miniToggle;

	public RectTransform speedPanelRT;

	public CapsuleCollider camCol;

	public static GameObject jointChild;

	public MiniCopter miniCopter;

	public static avatar avatar;

	public CharacterJoint joint;

	public Transform tf;

	public static Transform parent;

	public Transform fireFrom;

	private Transform head;

	private bool Crouch
	{
		get
		{
			return crouch;
		}
		set
		{
			crouch = value;
			if (crouch)
			{
				rb.isKinematic = true;
				camCol.height = 0.3f;
				camCol.center = camColCenter + new Vector3(0f, 0.1f, 0f);
				rb.isKinematic = false;
			}
			else
			{
				camCol.height = 0.55f;
				camCol.center = camColCenter;
			}
		}
	}

	private void Awake()
	{
		inst = this;
	}

	private void Start()
	{
		tf = base.transform;
		sys = BuilderSystem.inst;
		parent = tf.parent;
		rb = parent.GetComponent<Rigidbody>();
		setRotation();
		speedButtons[speedIndex].onClick.Invoke();
		startPos = parent.position;
		startRotP = parent.rotation;
		startRot = tf.localRotation;
		fireFrom = tf.GetChild(0);
		camCol = parent.GetComponent<CapsuleCollider>();
		camColCenter = camCol.center;
		StartCoroutine(showSpeed(2f));
	}

	public void changeMode(int index)
	{
		Mode mode = this.mode;
		this.mode = (Mode)index;
		if (this.mode == mode)
		{
			this.mode = Mode.free;
		}
		if (infoSpotPanel.inst.infoPanel.activeSelf)
		{
			infoSpotPanel.inst.exit();
		}
		if (mode == Mode.ground)
		{
			groundMode();
		}
		if (mode == Mode.third)
		{
			thirdMode();
		}
		if (mode == Mode.mount)
		{
			miniToggle.SetIsOnWithoutNotify(value: false);
			miniCopter.dismount();
		}
		if (mode == Mode.ortho)
		{
			orthoMode();
		}
		if (this.mode == Mode.ground)
		{
			groundMode();
		}
		if (this.mode == Mode.third)
		{
			thirdMode();
		}
		if (this.mode == Mode.mount)
		{
			miniToggle.SetIsOnWithoutNotify(value: true);
			miniCopter.mount();
		}
		if (this.mode == Mode.ortho)
		{
			orthoMode();
		}
	}

	private void LateUpdate()
	{
		if (Input.GetButtonDown("Walk"))
		{
			changeMode(2);
		}
		else
		{
			if (mode == Mode.mount)
			{
				return;
			}
			if (Input.GetButton("CameraRotate"))
			{
				xDeg += UnityEngine.Input.GetAxis("Mouse X") * lookMulti;
				if (!Options.invertY)
				{
					yDeg -= UnityEngine.Input.GetAxis("Mouse Y") * lookMulti;
				}
				else
				{
					yDeg += UnityEngine.Input.GetAxis("Mouse Y") * lookMulti;
				}
				yDeg = ClampAngle(yDeg, yMin, yMax);
				if (!rbMode)
				{
					parent.rotation = Quaternion.Slerp(parent.rotation, Quaternion.Euler(0f, xDeg, 0f), Time.deltaTime * turnRate);
				}
				tf.localRotation = Quaternion.Slerp(tf.localRotation, Quaternion.Euler(yDeg, 0f, 0f), Time.deltaTime * turnRate);
			}
			speed = speeds[speedIndex];
			if (Input.GetButtonDown("Zoom") && !cam.orthographic)
			{
				zoom();
				return;
			}
			if (Input.GetButtonDown("speed1"))
			{
				speedButtons[0].onClick.Invoke();
				StartCoroutine(showSpeed(1f));
			}
			if (Input.GetButtonDown("speed2"))
			{
				speedButtons[1].onClick.Invoke();
				StartCoroutine(showSpeed(1f));
			}
			if (Input.GetButtonDown("speed3"))
			{
				speedButtons[2].onClick.Invoke();
				StartCoroutine(showSpeed(1f));
			}
			if (Input.GetButton("speedUp") && !rbMode)
			{
				speed = shiftSpeeds[speedIndex];
			}
			dir = default(Vector3);
			if (Input.GetButton("Left"))
			{
				dir.x -= 1f;
			}
			if (Input.GetButton("Right"))
			{
				dir.x += 1f;
			}
			if (mode == Mode.ground)
			{
				groundUpdate();
				return;
			}
			if (cam.orthographic)
			{
				orthoUpdate();
			}
			else
			{
				bool button = Input.GetButton("levelMoveTog");
				if ((lvlCamMove && !button) || (!lvlCamMove && button))
				{
					if (rbMode)
					{
						if (Input.GetButton("Forward"))
						{
							dir += tf.InverseTransformDirection(parent.forward) * 1f;
						}
						if (Input.GetButton("Back"))
						{
							dir += tf.InverseTransformDirection(parent.forward) * -1f;
						}
					}
					else
					{
						if (Input.GetButton("Forward"))
						{
							parent.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
						}
						if (Input.GetButton("Back"))
						{
							parent.Translate(Vector3.back * speed * Time.deltaTime, Space.Self);
						}
					}
				}
				else
				{
					if (Input.GetButton("Forward"))
					{
						dir.z += 1f;
					}
					if (Input.GetButton("Back"))
					{
						dir.z -= 1f;
					}
				}
				if (!rbMode && !RectSelection.isSelecting)
				{
					if (Input.GetButton("Up"))
					{
						parent.Translate(Vector3.up * speed * Time.deltaTime, Space.World);
					}
					if (Input.GetButton("Down"))
					{
						parent.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
					}
				}
			}
			if (!rbMode)
			{
				dir = dir * speed * Time.deltaTime;
				parent.Translate(dir, tf);
				return;
			}
			if (Input.GetButton("Up"))
			{
				dir += tf.InverseTransformDirection(Vector3.up) * 1f;
			}
			if (Input.GetButton("Down"))
			{
				dir += tf.InverseTransformDirection(Vector3.up) * -1f;
			}
			if (mode == Mode.third && (bool)head)
			{
				headTilt();
			}
		}
	}

	private void FixedUpdate()
	{
		if (mode == Mode.mount || !rbMode)
		{
			return;
		}
		Quaternion rot = Quaternion.Lerp(rb.rotation, Quaternion.Euler(0f, xDeg, 0f), Time.fixedDeltaTime * turnRate);
		rb.MoveRotation(rot);
		if (mode == Mode.ground)
		{
			Vector3 velocity = parent.TransformDirection(dir * speed);
			if (!climb)
			{
				velocity.y = rb.velocity.y;
			}
			rb.velocity = velocity;
		}
		else
		{
			rb.velocity = tf.TransformDirection(dir * speed);
		}
	}

	public void groundMode()
	{
		parent.GetComponent<CapsuleCollider>();
		walkingToggle.SetIsOnWithoutNotify(mode == Mode.ground);
		if (mode == Mode.ground)
		{
			groundTog.SetIsOnWithoutNotify(value: true);
			popUp.inst.message("not simulating rust movement");
			jointChild = (UnityEngine.Object.Instantiate(Resources.Load("game/emptyRigidbody"), tf.position, Quaternion.identity) as GameObject);
			joint.connectedBody = jointChild.GetComponent<Rigidbody>();
			rb.useGravity = true;
			rb.isKinematic = false;
			rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			rbMode = true;
			jump = false;
			return;
		}
		groundTog.SetIsOnWithoutNotify(value: false);
		camCol.height = 0.6f;
		rb.useGravity = false;
		rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
		rb.isKinematic = true;
		rbMode = false;
		climb = false;
		if ((bool)jointChild)
		{
			UnityEngine.Object.Destroy(jointChild);
		}
		joint.connectedBody = null;
	}

	private void groundUpdate()
	{
		speed = speeds[0];
		if (Input.GetButton("speedUp") && !crouch && !jump && !climb)
		{
			speed += 2f;
		}
		if (Input.GetButton("Forward"))
		{
			dir.z += 1f;
		}
		if (Input.GetButton("Back"))
		{
			dir.z -= 1f;
		}
		if (Input.GetButtonDown("Up"))
		{
			jump = true;
			float y = camCol.center.y - camCol.height * 0.5f;
			if (Physics.OverlapSphere(tf.position + new Vector3(0f, y, 0f), 0.05f, jumpMask).Length != 0)
			{
				rb.AddForce(Vector3.up * 17f, ForceMode.Impulse);
			}
		}
		else
		{
			jump = false;
		}
		if (!crouch && Input.GetButton("Down"))
		{
			Crouch = true;
		}
		if (crouch && Input.GetButtonUp("Down"))
		{
			Crouch = false;
		}
		if (climb && !jump)
		{
			if (Input.GetButton("Forward"))
			{
				dir.y += 1f;
			}
			if (Input.GetButton("Back"))
			{
				dir.y -= 1f;
			}
		}
	}

	public void orthoMode()
	{
		if (mode == Mode.ortho)
		{
			orthoToggle.SetIsOnWithoutNotify(value: true);
			RenderSettings.fog = false;
			Vector3 position = parent.transform.position;
			float y = Mathf.Clamp(position.y, 10f, 20f);
			parent.transform.position = new Vector3(position.x, y, position.z);
			cam.nearClipPlane = -35f;
			yMin = 20;
			cam.orthographic = true;
		}
		else
		{
			orthoToggle.SetIsOnWithoutNotify(value: false);
			RenderSettings.fog = true;
			cam.nearClipPlane = 0.05f;
			yMin = -90;
			cam.orthographic = false;
		}
		orthoToggle.transform.GetChild(2).gameObject.SetActive(mode == Mode.ortho);
		orthoToggle.transform.GetChild(3).gameObject.SetActive(mode == Mode.ortho);
	}

	private void orthoUpdate()
	{
		if (Input.GetButton("Forward"))
		{
			parent.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
		}
		if (Input.GetButton("Back"))
		{
			parent.Translate(Vector3.back * speed * Time.deltaTime, Space.Self);
		}
		float num = 0f;
		if (Input.GetButton("Up"))
		{
			num += 0.1f;
		}
		if (Input.GetButton("Down"))
		{
			num -= 0.1f;
		}
		if (num != 0f)
		{
			num = cam.orthographicSize + num;
			cam.orthographicSize = Mathf.Clamp(num, 1f, 12f);
			orthoZoomSlider.value = cam.orthographicSize;
		}
	}

	public void thirdMode()
	{
		cam.enabled = false;
		if (mode == Mode.third)
		{
			thridPersonTog.SetIsOnWithoutNotify(value: true);
			cams[1].gameObject.SetActive(value: true);
			cam = cams[1];
			enableAvatar(option: false);
			rbMode = true;
			rb.isKinematic = false;
		}
		else
		{
			thridPersonTog.SetIsOnWithoutNotify(value: false);
			cams[1].gameObject.SetActive(value: false);
			cam = cams[0];
			UnityEngine.Object.Destroy(jointChild);
			joint.connectedBody = null;
			head = null;
			rbMode = false;
			rb.isKinematic = true;
			fireFrom = tf.GetChild(0);
		}
		cam.enabled = true;
	}

	public void enableAvatar(bool option)
	{
		if (option && jointChild != null)
		{
			UnityEngine.Object.Destroy(jointChild);
		}
		jointChild = UnityEngine.Object.Instantiate(MGMT.inst.avatars3rd[Options.avatarIndex], tf.position - new Vector3(0f, 0.23f, 0f), Quaternion.identity);
		jointChild.transform.Rotate(0f, tf.eulerAngles.y, 0f);
		joint.connectedBody = jointChild.GetComponent<Rigidbody>();
		avatar = jointChild.GetComponent<avatar>();
		head = avatar.head;
		avatar.swapPlayerItems(Stability.inst.raidMode);
		fireFrom = avatar.cannon.transform;
	}

	private void headTilt()
	{
		if (Stability.inst.raidMode)
		{
			Vector3 forward = cam.ScreenToWorldPoint(new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 10f)) - fireFrom.position;
			Quaternion localRotation = Quaternion.Inverse(jointChild.transform.rotation) * Quaternion.LookRotation(forward);
			Vector3 eulerAngles = localRotation.eulerAngles;
			localRotation.eulerAngles = new Vector3(ClampHead(eulerAngles.x, -60f, 60f), ClampHead(eulerAngles.y, -20f, 20f), 0f);
			fireFrom.localRotation = localRotation;
		}
		Vector3 forward2 = cam.ScreenToWorldPoint(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 10f)) - head.position;
		Quaternion b = Quaternion.Inverse(parent.rotation) * Quaternion.LookRotation(forward2);
		b.eulerAngles = new Vector3(ClampHead(b.eulerAngles.x, -30f, 30f), ClampHead(b.eulerAngles.y, -40f, 40f), b.eulerAngles.z);
		b = Quaternion.Lerp(head.localRotation, b, 0.1f);
		head.localRotation = b;
	}

	public void setlookMulti(float val)
	{
		if (val == 0f)
		{
			lookMulti = 1f;
		}
		else
		{
			lookMulti = val / 10f * 6f + 1f;
		}
	}

	public static void resetTransform()
	{
		parent.position = startPos;
		parent.rotation = startRotP;
		BuilderSystem.inst._transform.localRotation = startRot;
		setRotation();
	}

	public static void setRotation()
	{
		xDeg = parent.rotation.eulerAngles.y;
		yDeg = parent.GetChild(0).localRotation.eulerAngles.x;
	}

	public static void lookAt(Vector3 pos)
	{
		Vector3 vector = pos - parent.position;
		vector.y = 0f;
		parent.rotation = Quaternion.LookRotation(vector, Vector3.up);
		vector = pos - parent.position;
		float x = Vector3.Angle(vector, parent.forward);
		parent.GetChild(0).localRotation = Quaternion.Euler(x, 0f, 0f);
	}

	public void speedSelect(int index)
	{
		speedIndex = index;
		speed = speeds[index];
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public void cursorLockMode()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void speedPanel(bool expand)
	{
		speedPanelRT.transform.GetChild(0).gameObject.SetActive(expand);
	}

	public void zoom()
	{
		if (Physics.Raycast(cam.ScreenPointToRay(UnityEngine.Input.mousePosition), out RaycastHit hitInfo, 250f) && hitInfo.transform.gameObject.layer != LayerMask.NameToLayer("camera"))
		{
			if (cam.orthographic)
			{
				parent.position = new Vector3(hitInfo.point.x, parent.position.y, hitInfo.point.z);
				return;
			}
			float num = Vector3.Distance(tf.position, hitInfo.point);
			lookAt(hitInfo.point);
			parent.position = Vector3.MoveTowards(parent.position, hitInfo.point, num - 5f);
			setRotation();
		}
	}

	public void hideTerrain()
	{
		if (sceneSettings.inst.terrain != null)
		{
			bool isActiveAndEnabled = sceneSettings.inst.terrain.isActiveAndEnabled;
			sceneSettings.inst.terrain.enabled = !isActiveAndEnabled;
			if ((bool)sceneSettings.inst.water)
			{
				sceneSettings.inst.water.SetActive(!isActiveAndEnabled);
			}
		}
	}

	private float ClampHead(float angle, float min, float max)
	{
		if (angle < 90f || angle > 270f)
		{
			if (angle > 180f)
			{
				angle -= 360f;
			}
			if (max > 180f)
			{
				max -= 360f;
			}
			if (min > 180f)
			{
				min -= 360f;
			}
		}
		angle = Mathf.Clamp(angle, min, max);
		if (angle < 0f)
		{
			angle += 360f;
		}
		return angle;
	}

	private IEnumerator showSpeed(float time)
	{
		speedPanel(expand: true);
		yield return new WaitForSeconds(time);
		speedPanel(expand: false);
	}
}
