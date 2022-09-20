using UnityEngine;

public static class RectTransformExtensions
{
    public static bool Overlaps(this RectTransform rectTransform1, RectTransform rectTransform2)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform1.GetWorldCorners(corners);
        Rect rec = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);

        rectTransform2.GetWorldCorners(corners);
        Rect rec2 = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);

        return rec.Overlaps(rec2);
    }
}
