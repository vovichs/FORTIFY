using UnityEngine;

public class Car : MonoBehaviour
{
	private float scaled = 0.05f;

	public Transform _transform;

	public Rigidbody rB;

	[Header("Car Settings")]
	public float wheelRadius;

	public float steeringAngle;

	public float maxSteeringAngle;

	public float maxSpeed;

	[Header("Wheels")]
	public float brakeTorque;

	[Header("Wheels")]
	public float handBrakeTorque;

	public bool braking;

	public WheelCollider[] wheels;

	public GameObject[] fWheels;

	public GameObject[] fWheelsmesh;

	public GameObject[] rWheels;

	public GameObject[] rWheelsmesh;

	[Header("CarProperties")]
	public bool lightsOn;

	private bool slow;

	private bool rocketOn;

	private float rForce = 10000f;

	[Header("Rocket")]
	public ParticleSystem ps;

	public AudioSource AS;

	private void Start()
	{
		rB.AddForce(_transform.forward * 400000f);
		if (CameraCtrl.inst.mode != CameraCtrl.Mode.third)
		{
			scaled = 0.25f;
		}
	}

	private void FixedUpdate()
	{
		if (_transform.localScale.x < 1f)
		{
			scaled += 0.05f;
			_transform.localScale = new Vector3(scaled, scaled, scaled);
		}
		if (rocketOn)
		{
			rB.AddForce(_transform.forward * rForce);
			rForce -= 50f;
			if (rForce <= 0f)
			{
				rocketOn = false;
				ps.Stop();
				AS.Stop();
			}
		}
		else
		{
			move();
		}
	}

	private void LateUpdate()
	{
		if (rB.velocity.magnitude < 0.1f)
		{
			if (slow)
			{
				GetComponent<collisionExplode>().Explode();
				base.enabled = false;
			}
			slow = true;
		}
	}

	private void move()
	{
		for (int i = 0; i < 2; i++)
		{
			wheels[i].motorTorque = 40f;
		}
	}

	public void rocket()
	{
		if (!rocketOn)
		{
			rocketOn = true;
			_transform.GetChild(0).gameObject.SetActive(value: true);
			ps.Play();
			AS.Play();
		}
	}

	private void turning(float _steeringAngle)
	{
		float num = rB.velocity.magnitude * 3.6f;
		if (_steeringAngle < 0f)
		{
			steeringAngle += num / 11f;
			if (steeringAngle > 0f)
			{
				steeringAngle = 0f;
			}
		}
		else
		{
			steeringAngle -= num / 2f;
			if (steeringAngle < 0f)
			{
				steeringAngle = 0f;
			}
		}
		for (int i = 0; i < fWheels.Length; i++)
		{
			fWheels[i].GetComponent<WheelCollider>().steerAngle = _steeringAngle;
			float x = fWheelsmesh[i].transform.localEulerAngles.x;
			float z = fWheelsmesh[i].transform.localEulerAngles.z;
			float y = fWheels[i].GetComponent<WheelCollider>().steerAngle - z;
			fWheelsmesh[i].transform.localEulerAngles = new Vector3(x, y, z);
		}
	}

	private void rotateWheels()
	{
		for (int i = 0; i < rWheels.Length; i++)
		{
			rWheelsmesh[i].transform.Rotate(rWheels[i].GetComponent<WheelCollider>().rpm / 60f * 360f * Time.fixedDeltaTime, 0f, 0f);
			fWheelsmesh[i].transform.Rotate(fWheels[i].GetComponent<WheelCollider>().rpm / 60f * 360f * Time.fixedDeltaTime, 0f, 0f);
		}
	}
}
