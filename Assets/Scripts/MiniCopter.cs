using UnityEngine;
using UnityEngine.UI;

public class MiniCopter : MonoBehaviour
{
	public class HelicopterInputState
	{
		public float throttle;

		public float roll;

		public float yaw;

		public float pitch;

		public bool groundControl;

		public void Reset()
		{
			throttle = 0f;
			roll = 0f;
			yaw = 0f;
			pitch = 0f;
			groundControl = false;
		}
	}

	public static MiniCopter inst;

	public Rigidbody rb;

	public Transform _transform;

	public Transform mountTF;

	public Transform com;

	public Transform fireFrom;

	public WheelCollider[] wheels;

	public Animator anim;

	public AudioSource AS;

	public AudioClip[] crashSounds;

	public LayerMask mask;

	public Toggle miniTog;

	public bool flying = true;

	public float engineThrustMax = 1000f;

	public Vector3 torqueScale;

	private float liftDotMax = 0.5f;

	private float currentThrottle;

	private float avgThrust;

	private float scaledGravity = -3.27f;

	private float altForceDotMin = 0.85f;

	private float liftFraction = 0.25f;

	private float thrustLerpSpeed = 4f;

	private float hoverForceScale = 0.81f;

	public HelicopterInputState currentInputState = new HelicopterInputState();

	public float lastPlayerInputTime;

	public Vector3 damageTorque;

	public float nextDamageTime;

	public float pendingImpactDamage;

	private float cachedPitch;

	private float cachedYaw;

	private float cachedRoll;

	private bool mouseLook;

	private Transform camT;

	private float slerpTime = 0.3f;

	private float curSlerpTime;

	private float xDeg;

	private float yDeg;

	private void Awake()
	{
		inst = this;
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = com.localPosition;
	}

	private void Update()
	{
		if (Input.GetButtonDown("menu") || UnityEngine.Input.GetKeyDown(KeyCode.Space))
		{
			miniTog.isOn = false;
			return;
		}
		mouseLook = Input.GetButton("copterLook");
		PilotInput();
		if (mouseLook)
		{
			curSlerpTime = 0f;
			float num = 2f;
			xDeg += UnityEngine.Input.GetAxis("Mouse X") * num;
			xDeg = ClampAngle(xDeg, 120f);
			if (!Options.invertY)
			{
				yDeg -= UnityEngine.Input.GetAxis("Mouse Y") * num;
			}
			else
			{
				yDeg += UnityEngine.Input.GetAxis("Mouse Y") * num;
			}
			yDeg = ClampAngle(yDeg, 80f);
			camT.localRotation = Quaternion.Slerp(camT.localRotation, Quaternion.Euler(yDeg, xDeg, 0f), Time.fixedDeltaTime * 10f);
		}
		else if (curSlerpTime < slerpTime)
		{
			xDeg = 0f;
			yDeg = 0f;
			curSlerpTime += Time.deltaTime;
			float t = curSlerpTime / slerpTime;
			camT.localRotation = Quaternion.Slerp(camT.localRotation, Quaternion.Euler(0f, 0f, 0f), t);
		}
	}

	private static float ClampAngle(float angle, float limit)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, 0f - limit, limit);
	}

	private void FixedUpdate()
	{
		if (Time.time > lastPlayerInputTime + 0.5f)
		{
			SetDefaultInputState();
		}
		MovementUpdate();
	}

	[ContextMenu("mount")]
	public void mount()
	{
		if (!camT)
		{
			camT = CameraCtrl.inst.transform;
		}
		flying = true;
		mouseLook = false;
		engineThrustMax = 1000f;
		if (BuilderSystem.multiplayer)
		{
			Multiplayer.inst.lobbyMemberDataChange("mount", "1");
		}
		if (BuilderSystem.editMode)
		{
			BuilderUI.inst.editToggle.isOn = false;
		}
		if (BuilderUI.inst.menuToggle.isOn)
		{
			BuilderUI.inst.menuToggle.isOn = false;
		}
		if (!Stability.inst.raidMode)
		{
			BuilderUI.inst.screenshotModeState();
			RayPlace.noPlace = true;
		}
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		CameraCtrl.inst.cam.fieldOfView = 70f;
		Transform parent = camT.parent;
		Vector3 position = parent.position;
		float y = 0.1f;
		if (Physics.Raycast(position, Vector3.down, out RaycastHit hitInfo, 300f, mask))
		{
			y = hitInfo.point.y + 0.1f;
		}
		position.y = y;
		parent.position = position;
		base.gameObject.SetActive(value: true);
		_transform.SetPositionAndRotation(parent.position, parent.rotation);
		BuilderSystem.inst._transform.localRotation = Quaternion.identity;
		parent.GetComponent<Rigidbody>().detectCollisions = false;
		parent.parent = _transform;
		parent.SetPositionAndRotation(mountTF.position, mountTF.rotation);
		CameraCtrl.inst.fireFrom = fireFrom;
		base.enabled = true;
		popUp.inst.message("press escape or space to dismount");
		popUp.inst.message("left alt for mouse look");
	}

	[ContextMenu("dismount")]
	public void dismount()
	{
		flying = false;
		mouseLook = false;
		if (BuilderSystem.multiplayer)
		{
			Multiplayer.inst.lobbyMemberDataChange("mount", "0");
		}
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		if (!Stability.inst.raidMode)
		{
			BuilderUI.inst.screenshotModeState();
			RayPlace.noPlace = false;
		}
		Transform parent = BuilderSystem.inst._transform.parent;
		base.enabled = false;
		parent.parent = null;
		parent.rotation = Quaternion.Euler(0f, parent.rotation.y, 0f);
		camT.localRotation = Quaternion.Euler(camT.localRotation.x, 0f, 0f);
		CameraCtrl.inst.fireFrom = camT.GetChild(0);
		base.gameObject.SetActive(value: false);
		CameraCtrl.inst.cam.fieldOfView = 65f;
		CameraCtrl.inst.changeMode(0);
		parent.GetComponent<Rigidbody>().detectCollisions = true;
	}

	public void PilotInput()
	{
		currentInputState.Reset();
		currentInputState.throttle = (Input.GetButton("Forward") ? 1f : 0f);
		currentInputState.throttle -= ((Input.GetButton("Back") || UnityEngine.Input.GetKey(KeyCode.LeftControl)) ? 1f : 0f);
		if (!mouseLook)
		{
			currentInputState.pitch = UnityEngine.Input.GetAxis("Mouse Y");
			if (Options.invertY)
			{
				currentInputState.pitch = 0f - currentInputState.pitch;
			}
			currentInputState.roll = 0f - UnityEngine.Input.GetAxis("Mouse X");
		}
		currentInputState.yaw = (Input.GetButton("Right") ? 1f : 0f);
		currentInputState.yaw -= (Input.GetButton("Left") ? 1f : 0f);
		currentInputState.pitch = Mathf.Clamp(currentInputState.pitch, -1f, 1f);
		currentInputState.roll = Mathf.Clamp(currentInputState.roll, -1f, 1f);
		lastPlayerInputTime = Time.time;
		currentInputState.groundControl = UnityEngine.Input.GetKey(KeyCode.LeftControl);
		if (currentInputState.groundControl)
		{
			currentInputState.roll = 0f;
			currentInputState.throttle = (Input.GetButton("Forward") ? 1f : 0f);
			currentInputState.throttle -= (Input.GetButton("Back") ? 1f : 0f);
		}
		cachedRoll = currentInputState.roll;
		cachedYaw = currentInputState.yaw;
		cachedPitch = currentInputState.pitch;
	}

	public void MovementUpdate()
	{
		bool flag = Grounded();
		if (flag)
		{
			ApplyForceAtWheels();
		}
		if (!currentInputState.groundControl || !flag)
		{
			HelicopterInputState helicopterInputState = currentInputState;
			currentThrottle = Mathf.Lerp(currentThrottle, helicopterInputState.throttle, 2f * Time.fixedDeltaTime);
			currentThrottle = Mathf.Clamp(currentThrottle, -0.8f, 1f);
			if (helicopterInputState.pitch != 0f || helicopterInputState.roll != 0f || helicopterInputState.yaw != 0f)
			{
				rb.AddRelativeTorque(new Vector3(helicopterInputState.pitch * torqueScale.x, helicopterInputState.yaw * torqueScale.y, helicopterInputState.roll * torqueScale.z), ForceMode.Force);
			}
			if (damageTorque != Vector3.zero)
			{
				rb.AddRelativeTorque(new Vector3(damageTorque.x, damageTorque.y, damageTorque.z), ForceMode.Force);
			}
			avgThrust = Mathf.Lerp(avgThrust, engineThrustMax * currentThrottle, Time.fixedDeltaTime * thrustLerpSpeed);
			float value = Mathf.Clamp01(Vector3.Dot(_transform.up, Vector3.up));
			float d = Mathf.InverseLerp(liftDotMax, 1f, value);
			float d2 = 1f - Mathf.InverseLerp(altForceDotMin, 1f, value);
			Vector3 force = Vector3.up * engineThrustMax * liftFraction * currentThrottle * d;
			Vector3 force2 = (_transform.up - Vector3.up).normalized * engineThrustMax * currentThrottle * d2;
			if (flying)
			{
				float d3 = rb.mass * (0f - scaledGravity);
				rb.AddForce(_transform.up * d3 * d * hoverForceScale, ForceMode.Force);
			}
			rb.AddForce(force, ForceMode.Force);
			rb.AddForce(force2, ForceMode.Force);
		}
		rb.AddForce(Vector3.up * rb.mass * scaledGravity);
	}

	public void SetDefaultInputState()
	{
		currentInputState.Reset();
		cachedRoll = 0f;
		cachedYaw = 0f;
		cachedPitch = 0f;
		if (!Grounded())
		{
			float num = Vector3.Dot(Vector3.up, _transform.right);
			float num2 = Vector3.Dot(Vector3.up, _transform.forward);
			currentInputState.roll = ((num < 0f) ? 1f : 0f);
			currentInputState.roll -= ((num > 0f) ? 1f : 0f);
			if (num2 < 0f)
			{
				currentInputState.pitch = -1f;
			}
			else if (num2 > 0f)
			{
				currentInputState.pitch = 1f;
			}
		}
	}

	public bool Grounded()
	{
		if (!wheels[1].isGrounded)
		{
			return false;
		}
		return wheels[2].isGrounded;
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (Time.time < nextDamageTime)
		{
			return;
		}
		float magnitude = collision.relativeVelocity.magnitude;
		float num = Mathf.InverseLerp(5f, 30f, magnitude);
		if (num > 0f)
		{
			pendingImpactDamage += Mathf.Max(num, 0.15f);
			if (Vector3.Dot(_transform.up, Vector3.up) < 0.5f)
			{
				pendingImpactDamage *= 5f;
			}
			Vector3 force = collision.GetContact(0).normal * (1f + 3f * num);
			ContactPoint contact = collision.GetContact(0);
			rb.AddForceAtPosition(force, contact.point, ForceMode.VelocityChange);
			nextDamageTime = Time.time + 0.333f;
			float volumeScale = collision.relativeVelocity.magnitude * 0.35f;
			int num2 = UnityEngine.Random.Range(0, 3);
			AS.PlayOneShot(crashSounds[num2], volumeScale);
		}
	}

	private void ApplyForceAtWheels()
	{
		float num = 16.666f;
		float num2 = 0f;
		float num3 = 0f;
		if (!currentInputState.groundControl)
		{
			num = 20f;
			num3 = 0f;
			num2 = 0f;
		}
		else
		{
			num = ((currentInputState.throttle == 0f) ? 16.666f : 0f);
			num2 = currentInputState.throttle;
			num3 = currentInputState.yaw;
		}
		num2 *= 1f;
		ApplyWheelForce(wheels[0], num2, num, num3);
		ApplyWheelForce(wheels[1], num2, num, 0f);
		ApplyWheelForce(wheels[2], num2, num, 0f);
	}

	public void ApplyWheelForce(WheelCollider wheel, float gasScale, float brakeScale, float turning)
	{
		if (wheel.isGrounded)
		{
			float num = gasScale * 50f;
			float num2 = brakeScale * 250f;
			float num3 = 45f * turning;
			if (!Mathf.Approximately(wheel.motorTorque, num))
			{
				wheel.motorTorque = num;
			}
			if (!Mathf.Approximately(wheel.brakeTorque, num2))
			{
				wheel.brakeTorque = num2;
			}
			if (!Mathf.Approximately(wheel.steerAngle, num3))
			{
				wheel.steerAngle = num3;
			}
		}
	}
}
