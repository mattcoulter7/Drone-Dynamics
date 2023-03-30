using UnityEngine;
using UnityEngine.UI;

public class SetContentHeight : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform contentRectTransform;
    public float padding = 0f;

    private void Update()
    {
        contentRectTransform.anchoredPosition = new Vector3(0, contentRectTransform.anchoredPosition.y, 0);
        if (contentRectTransform.childCount == 0) return;
        // Set the height of the content RectTransform
        Vector2 sizeDelta = contentRectTransform.sizeDelta;
        sizeDelta.y = GetTotalHeight() + padding;
        contentRectTransform.sizeDelta = sizeDelta;
    }

    private float GetTotalHeight()
    {
        float? minY = null;
        float? maxY = null;

        // Loop through all the contents and find the highest and lowest Y values
        foreach (RectTransform childRectTransform in contentRectTransform)
        {
            float top = childRectTransform.anchoredPosition.y;
            float bottom = childRectTransform.anchoredPosition.y + childRectTransform.sizeDelta.y;


            minY = minY.HasValue ? Mathf.Min(minY.Value, top) : top;
            maxY = maxY.HasValue ? Mathf.Max(maxY.Value, bottom) : bottom;
        }

        if (!minY.HasValue || !maxY.HasValue) return 0f;

        return Mathf.Abs(maxY.Value) + Mathf.Abs(minY.Value);
    }
}