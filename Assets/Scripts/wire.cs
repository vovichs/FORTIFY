using TMPro;
using UnityEngine;

public class wire : MonoBehaviour
{
	public float lvl;

	public LineRenderer lr;

	public bool lockGlow;

	public io output;

	public io input;

	public int colorIndex;

	public GameObject powerDisplay;

	public TextMeshProUGUI powerText;

	protected int labelPower = -1;

	public bool skip;

	private void OnDestroy()
	{
		BuilderSystem.wireList.Remove(this);
	}

	public virtual void setMat(int ind, bool circuit)
	{
		colorIndex = ind;
		if (wiring.inst.wireFlow)
		{
			lr.sharedMaterial = wiring.inst.wireMatColorsFlow[ind];
		}
		else
		{
			lr.sharedMaterial = wiring.inst.wireMatColors[ind];
		}
		if (circuit)
		{
			circuitColor(ind);
		}
	}

	public virtual void highlightMat(bool state)
	{
		if (lockGlow && !state)
		{
			return;
		}
		if (state)
		{
			if (wiring.inst.wireFlow)
			{
				lr.sharedMaterial = wiring.inst.wireMatGlowFlow;
			}
			else
			{
				lr.sharedMaterial = wiring.inst.wireMatGlow;
			}
		}
		else if (!wiring.inst.wireFlow)
		{
			lr.sharedMaterial = wiring.inst.wireMatColors[colorIndex];
		}
		else
		{
			lr.sharedMaterial = wiring.inst.wireMatColorsFlow[colorIndex];
		}
	}

	public virtual void showWire(bool state)
	{
		if (wiring.inst.labelsOn)
		{
			powerDisplay.SetActive(state);
		}
		lr.enabled = state;
	}

	public void circuitColor(int ind)
	{
		if (input == null || input.dev == null)
		{
			return;
		}
		int type = input.type;
		for (int i = 0; i < input.dev.outputTo.Length; i++)
		{
			io io = input.dev.outputTo[i];
			if (io != null && io.type == type && io.connectedTo != null && io.wire.colorIndex != ind)
			{
				io.wire.setMat(ind, circuit: true);
			}
		}
	}

	public void updateLabel(int power)
	{
		if (power != labelPower)
		{
			labelPower = power;
			powerText.text = power.ToString();
		}
	}

	public void getMidpoint(Vector3[] points)
	{
		float num = 0f;
		float[] array = new float[points.Length - 1];
		for (int i = 1; i < points.Length; i++)
		{
			num += (array[i - 1] = (points[i - 1] - points[i]).magnitude);
		}
		Transform transform = powerDisplay.transform;
		float num2 = 0f;
		float num3 = num * 0.5f;
		for (int j = 0; j < array.Length; j++)
		{
			num2 += array[j];
			if (num2 >= num3)
			{
				Vector3 position = Vector3.Lerp(points[j + 1], points[j], (num2 - num3) / array[j]);
				transform.localPosition = base.transform.InverseTransformPoint(position);
				break;
			}
		}
		transform.rotation = Quaternion.identity;
	}
}
