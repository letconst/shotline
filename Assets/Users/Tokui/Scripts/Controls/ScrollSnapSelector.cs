using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollSnapSelector : ScrollRect
{
	public int hIndex;
	public int vIndex;

	public int hPageNum = 3;
	public int vPageNum = 0;
	public float smoothness = 10f;
	public float scrollWeight = 0.1f;

	private Vector2 targetPosition;
	private float hPerPage;
	private float vPerPage;
	private bool dragging;
	private bool forcePositionUpdate = false;

	[SerializeField]
	private int hContent = 3;
	private int hContentMin = 1;

	[SerializeField]
	private int hPageMax = 4;
	private int hPageMin = 0;

	public void ScrollTo(int x, int y)
	{
		hIndex = x;
		vIndex = y;
		forcePositionUpdate = true;
	}

	protected override void Awake()
	{
		base.Awake();
		hPerPage = 1f / (float)(hPageNum - 1);
		vPerPage = 1f / (float)(hPageNum - 1);
	}

	protected override void Start()
	{
		base.Start();
		targetPosition = GetSnapPosition();
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		dragging = true;
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		UpdateIndex();
		targetPosition = GetSnapPosition();
		dragging = false;
	}

	void Update()
	{
		if (!dragging && normalizedPosition != targetPosition)
		{
			normalizedPosition = Vector2.Lerp(normalizedPosition, targetPosition, smoothness * Time.deltaTime);

			if (Vector2.Distance(normalizedPosition, targetPosition) < 0.009f)
			{
				normalizedPosition = targetPosition;
			}
		}

		if (forcePositionUpdate)
		{
			forcePositionUpdate = false;
			targetPosition = GetSnapPosition();
		}
	}

	void UpdateIndex()
	{
		float xPage, yPage = -1;

		if (horizontal && hPageNum > 0)
		{
			xPage = (normalizedPosition.x / hPerPage);
			float diff = xPage - (1 * hIndex);

			if (diff >= scrollWeight)
			{
				hIndex++;
			}
			else if (diff <= -scrollWeight)
			{
				hIndex--;
			}

			if(hIndex == hPageMin)
            {
				hIndex = hContent;
            }
			if(hIndex == hPageMax)
            {
				hIndex = hContentMin;
            }
		}

		if (vertical && vPageNum > 0)
		{
			yPage = normalizedPosition.y / vPerPage;
			float diff = yPage - (1 * vIndex);

			if (diff >= scrollWeight)
			{
				vIndex++;
			}
			else if (diff <= -scrollWeight)
			{
				vIndex--;
			}
		}
	}

	Vector2 GetSnapPosition()
	{
		return new Vector2(horizontal && hPageNum > 0 ? hIndex * hPerPage : normalizedPosition.x, vertical && vPageNum > 0 ? vIndex * vPerPage : normalizedPosition.y);
	}
}