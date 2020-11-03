using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class AutoSetAnchor : Editor
{
    [MenuItem("Auto/AutoSetAnchor")]
    private static void autoSetAnchor1()
    {
        var rect = Selection.activeTransform.GetComponent<RectTransform>();
        var parentRect = rect.parent.GetComponent<RectTransform>();
        Vector2 left_bottom = new Vector2(rect.offsetMin.x/parentRect.rect.width, rect.offsetMin.y / parentRect.rect.height);
        Vector2 right_up = new Vector2(rect.offsetMax.x / parentRect.rect.width, rect.offsetMax.y / parentRect.rect.height);
        rect.anchorMax += right_up;
        rect.anchorMin += left_bottom;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
    [MenuItem("Auto/AutoSetAnchor",true)]
    private static bool autoSetAnchor2()
    {
        return Selection.activeTransform.parent!=null&&
            Selection.activeTransform.GetComponent<Canvas>()==null&& 
            Selection.activeTransform.GetComponent<RectTransform>() != null;
    }
}
