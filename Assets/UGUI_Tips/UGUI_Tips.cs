using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

// 实现一个文本提示框，要求提示框区域跟随文本内容变化，显示位置位于屏幕中间，垂直方向位于中间偏上30%的位置。
public class UGUI_Tips : MonoBehaviour
{
    private TMP_Text text;
    private RectTransform rtText;
    private RectTransform rtBG;
    private bool isFixAnchor = true;
    private Vector2 offset = Vector2.zero;

    [SerializeField] Vector2 normalizedPosition = new Vector2(0.5f, 0.7f);
    
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Toggle toggleFixAnchor;
    
    void Awake()
    {
        text = GetComponent<TMP_Text>();
        rtText = GetComponent<RectTransform>();
        rtBG = transform.Find("BG").GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
            offset += Vector2.up;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyUp(KeyCode.W))
            offset += Vector2.down;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            offset += Vector2.left;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
            offset += Vector2.right;

        if (offset != Vector2.zero)
        {
            rtText.anchoredPosition += offset * 1f; // 每次移动x个像素
        }
    }

    public void Recalculate()
    {
        offset = Vector2.zero;
        
        // 以下都是基于 anchoredPosition 为0的情况下计算，也就是默认位置，如果后续大小不变的前提下，想要移动，只需要修改 anchoredPosition 即可
        rtText.anchoredPosition = Vector2.zero;
        rtBG.anchoredPosition = Vector2.zero;
        
        // 文字区域最大为 80% 屏幕宽度
        var maxRectWidth = Screen.width * 0.8f;
        var textSize = text.GetTextSize(maxRectWidth);
        var bgSize = textSize + 10f * 2 * Vector2.one; // 10个像素padding

        if (isFixAnchor)
        {
            // 固定锚点, x,y都是距离, 锚点和选取会影响计算的复杂度
            // 对于文本父级窗口是全屏区域 Rect:[0,0,1920,1080], 锚点位于屏幕中心
            // 对于BG父级窗口是文本区域 Rect:[-rtText.x/2, -rtText.y/2, rtText.x, rtText.y]， 锚点位于文本区域中心
            
            #region anhoredPosition未设置在实际位置，而是设置在屏幕中心时
            // 相对屏幕中间偏移的距离
            // var screenOffset = new Vector2(Screen.width * (normalizedPosition.x - 0.5f), 
            //     Screen.height * (normalizedPosition.y - 0.5f));
            // 使用offset控制位置和区域
            // rtText.anchorMin = new Vector2(0.5f, 0.5f);
            // rtText.anchorMax = new Vector2(0.5f, 0.5f);
            // rtText.offsetMin = -0.5f * textSize + screenOffset;
            // rtText.offsetMax = 0.5f * textSize + screenOffset;
            #endregion

            
            #region anchor是相对屏幕区域而言，如果设置到对象实际的中心位置，会更简单
            // 使用 anchor Position 和 sizeDelta 控制位置和区域
            rtText.anchorMin = normalizedPosition;
            rtText.anchorMax = normalizedPosition;
            rtText.sizeDelta = textSize;
            #endregion

            
            #region BG是相对文本区域而言的，因此只需要设置锚点到文本的中心即可
            // 与text区域保持一致，并额外多10个像素作用padding
            rtBG.anchorMin = new Vector2(0.5f, 0.5f);
            rtBG.anchorMax = new Vector2(0.5f, 0.5f);
            
            // 向外扩展10个像素， 作为padding
            rtBG.offsetMin = -0.5f * bgSize;
            rtBG.offsetMax = 0.5f * bgSize;
            #endregion
        }
        else
        {
            // 锚框, x方向水平拉伸, y方向保持固定， 表示左上和左下的锚点重合，右上和右下的锚点重合
            
            // anchor值X不同，Y相同， 则X方向是比例,Y方向是距离， X方向取[0,1],表示锚点位于父级区域的最左和最右侧
            // 对于文本父级窗口是全屏区域 Rect:[0,0,1920,1080]
            // 对于BG父级窗口是文本区域 Rect:[-rtText.x/2, -rtText.y/2, rtText.x, rtText.y]
            
            // // 使用offset控制位置和区域
            // rtText.anchorMin = new Vector2(0f, 0.5f);
            // rtText.anchorMax = new Vector2(1f, 0.5f);
            // rtText.offsetMin = new Vector2((Screen.width - textSize.x) * 0.5f, -textSize.y * 0.5f) + screenOffset;
            // rtText.offsetMax = new Vector2((textSize.x - Screen.width) * 0.5f, textSize.y * 0.5f) + screenOffset;
            
            // // x方向拉伸，y方向距离保持固定
            // rtText.anchorMin = new Vector2(0.5f - textSize.x / Screen.width * 0.5f, normalizedPosition.y);
            // rtText.anchorMax = new Vector2(0.5f + textSize.x / Screen.width * 0.5f, normalizedPosition.y);
            // rtText.sizeDelta = new Vector2(0f, textSize.y);

            // x,y方向都拉伸，且保持和文本区域匹配,  使用 anchoredPosition 和 sizeDelta 控制位置和区域
            rtText.anchorMin = new Vector2(0.5f - textSize.x / Screen.width * 0.5f, normalizedPosition.y - textSize.y * 0.5f / Screen.height);
            rtText.anchorMax = new Vector2(0.5f + textSize.x / Screen.width * 0.5f, normalizedPosition.y + textSize.y * 0.5f / Screen.height);
            // 当锚点不重合时， sizeDelta 会同时修改pivot距不同锚框边的距离，可以先计算好锚框区域后，再设置sizeDelta
            rtText.sizeDelta = Vector2.zero;

            
            #region BG都是相对文本区域，无论是否固定锚点，都只取决于选择的各锚点位置

            // rtBG.anchorMin = new Vector2(0f, 0.5f);
            // rtBG.anchorMax = new Vector2(1f, 0.5f);
            // rtBG.offsetMin = new Vector2(-10f, -0.5f * bgSize.y); // rtBG的右点坐标 - 相应角的 anchor坐标
            // rtBG.offsetMax = new Vector2(10f, 0.5f * bgSize.y);

            rtBG.anchorMin = Vector2.zero;
            rtBG.anchorMax = Vector2.one;
            rtBG.sizeDelta = 10f * 2 * Vector2.one; // 向外扩展10个像素， 作为padding， 为负时向内缩

            #endregion
        }

        Debug.Log($"width: {maxRectWidth}, textSize: {textSize}, rtText: {rtText.rect}, y: {rtText.position.y}");
    }

    public void OnRefreshClicked()
    {
        text.text = inputField.text;
        Recalculate();
    }

    public void OnToggleFixAnchorClicked()
    {
        // 固定锚点
        if (isFixAnchor != toggleFixAnchor.isOn)
        {
            isFixAnchor = toggleFixAnchor.isOn;
            Recalculate();
        }
    }
}
