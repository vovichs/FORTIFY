using UnityEngine;

public class RectSelection : MonoBehaviour
{
	public static bool mouse3rectSelect;

	public static bool isSelecting;

	private static Texture2D _whiteTexture;

	private Vector3 mousePosition1;

	private BuilderSystem cs;

	private Bounds viewportBounds;

	private Rect rect;

	private static Texture2D WhiteTexture
	{
		get
		{
			_whiteTexture = new Texture2D(1, 1);
			_whiteTexture.SetPixel(0, 0, Color.white);
			_whiteTexture.Apply();
			return _whiteTexture;
		}
	}

	private void Start()
	{
		cs = BuilderSystem.inst;
		mouse3rectSelect = false;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(2))
		{
			mouse3rectSelect = true;
		}
		if (!cs.rectSelectMode && !mouse3rectSelect)
		{
			return;
		}
		mouse3rectSelect = true;
		if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2)) && cs.copyParent == null && !AlignStructure.inst.on && !BuilderUI.inst.mouseOverUI)
		{
			isSelecting = true;
			mousePosition1 = UnityEngine.Input.mousePosition;
		}
		if ((!Input.GetMouseButtonUp(0) && !Input.GetMouseButtonUp(2)) || !isSelecting)
		{
			return;
		}
		if (rect.size.x > 0f && rect.size.y > 0f)
		{
			viewportBounds = GetViewportBounds(mousePosition1, UnityEngine.Input.mousePosition);
			bool key = UnityEngine.Input.GetKey(KeyCode.LeftAlt);
			bool key2 = UnityEngine.Input.GetKey(KeyCode.LeftControl);
			if (!key && !key2)
			{
				cs.objList.Clear();
			}
			foreach (BuilderPart bp in cs.bpList)
			{
				if (!(bp == null) && !bp.hidden)
				{
					if (key2)
					{
						if (IsWithinSelectionBounds(bp) && !cs.objList.Contains(bp.gameObject))
						{
							cs.objList.Add(bp.gameObject);
							cs.changeSelection(bp, add: true);
						}
					}
					else if (key)
					{
						if (IsWithinSelectionBounds(bp))
						{
							cs.objList.Remove(bp.gameObject);
							cs.changeSelection(bp, add: false);
						}
					}
					else if (IsWithinSelectionBounds(bp))
					{
						cs.objList.Add(bp.gameObject);
						cs.changeSelection(bp, add: true);
					}
					else
					{
						cs.changeSelection(bp, add: false);
					}
				}
			}
		}
		isSelecting = false;
		mouse3rectSelect = false;
	}

	public bool IsWithinSelectionBounds(BuilderPart bp)
	{
		if (!isSelecting)
		{
			return false;
		}
		return viewportBounds.Contains(CameraCtrl.inst.cam.WorldToViewportPoint(bp.rend[0].bounds.center));
	}

	private void OnGUI()
	{
		if (isSelecting)
		{
			rect = GetScreenRect(mousePosition1, UnityEngine.Input.mousePosition);
			DrawScreenRectBorder(rect, 2f, new Color(1f, 1f, 1f));
		}
	}

	private static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
	{
		screenPosition1.y = (float)Screen.height - screenPosition1.y;
		screenPosition2.y = (float)Screen.height - screenPosition2.y;
		Vector3 vector = Vector3.Min(screenPosition1, screenPosition2);
		Vector3 vector2 = Vector3.Max(screenPosition1, screenPosition2);
		return Rect.MinMaxRect(vector.x, vector.y, vector2.x, vector2.y);
	}

	private Bounds GetViewportBounds(Vector3 screenPosition1, Vector3 screenPosition2)
	{
		Camera cam = CameraCtrl.inst.cam;
		Vector3 lhs = cam.ScreenToViewportPoint(screenPosition1);
		Vector3 rhs = cam.ScreenToViewportPoint(screenPosition2);
		Vector3 min = Vector3.Min(lhs, rhs);
		Vector3 max = Vector3.Max(lhs, rhs);
		min.z = cam.nearClipPlane;
		max.z = cam.farClipPlane;
		Bounds result = default(Bounds);
		result.SetMinMax(min, max);
		return result;
	}

	private static void DrawScreenRect(Rect rect, Color color)
	{
		GUI.color = color;
		GUI.DrawTexture(rect, WhiteTexture);
		GUI.color = Color.white;
	}

	private static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
	{
		DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
		DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
		DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
		DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
	}

	private void OnDisable()
	{
		mouse3rectSelect = false;
	}
}
