using UnityEngine;
using UnityEngine.UI;

public static class GoHelper
{
    public static Vector3 GetUIToWorldScale(GameObject uiElement)
    {
        return GetUIToWorldScale(uiElement.GetComponent<Image>());
    }

    public static Vector3 GetUIToWorldScale(Image uiElement)
    {
        /// <summary>
        /// 计算在指定深度，1 像素对应的世界单位大小
        /// </summary>
        float GetWorldUnitsPerPixel(Camera cam, float depth)
        {
            if (cam.orthographic)
            {
                // 正交摄像机：世界高度 = orthographicSize * 2
                return (cam.orthographicSize * 2f) / Screen.height;
            }
            else
            {
                // 透视摄像机：根据深度和 FOV 计算
                float fovRad = cam.fieldOfView * Mathf.Deg2Rad;
                float heightAtDepth = 2f * depth * Mathf.Tan(fovRad * 0.5f);
                return heightAtDepth / Screen.height;
            }
        }

        // 3. 获取 UI 元素的实际屏幕尺寸（考虑 Canvas Scaler）
        Vector2 uiSize = uiElement.rectTransform.sizeDelta * uiElement.canvas.scaleFactor;

        // 4. 计算世界单位中“1 像素”的大小
        float worldUnitsPerPixel = GetWorldUnitsPerPixel(Camera.main, uiElement.canvas.planeDistance);

        // 5. 计算 Sprite 应该缩放多少才能匹配 UI 尺寸
        Vector2 spriteOriginalSize = uiElement.sprite.bounds.size;
        Vector3 targetScale = Vector3.one;
        targetScale.x = (uiSize.x * worldUnitsPerPixel) / spriteOriginalSize.x;
        targetScale.y = (uiSize.y * worldUnitsPerPixel) / spriteOriginalSize.y;
        return targetScale;
    }
}