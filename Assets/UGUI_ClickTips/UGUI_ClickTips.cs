using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UGUI_ClickTips : MonoBehaviour
{
    private TMP_Text text;
    private RectTransform rtText;
    private RectTransform rtBG;
    private RectTransform rtArrow;
    
    private bool isFixAnchor = true;
    private Vector2 offset = Vector2.zero;

    [Tooltip("距离屏幕边缘的距离，依次为左上右下")]
    [SerializeField] Vector4 padding = new Vector4(10f, 110f, 10f, 10f);
    
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Toggle toggleFixAnchor;
    [SerializeField] TMP_Text txtInfo;
    
    void Awake()
    {
        text = GetComponent<TMP_Text>();
        rtText = GetComponent<RectTransform>();
        rtBG = transform.Find("BG").GetComponent<RectTransform>();
        rtArrow = transform.Find("Arrow").GetComponent<RectTransform>();
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

        if (Input.GetMouseButtonDown(0))
        {
            Recalculate();
        }
        
        txtInfo.text = $"Mouse:{Input.mousePosition}, anchoredPos:{rtText.anchoredPosition}, offset:{offset}";
    }

    public void Recalculate()
    {
        // 屏幕点击坐标 
        var screenPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        
        // // 对于3D场景，其实是类似的，只是要先将世界坐标转为屏幕坐标
        // Physics.Raycast(Camera.main.ScreenPointToRay(screenPos), out RaycastHit hitInfo, 1000f, LayerMask.GetMask("MAP"));
        // var worldPos = hitInfo.point;
        // // 3D场景坐标转为UI坐标
        // var uiPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);
        
        var bgPadding = 10f * 2 * Vector2.one;
        
        // 以下都是基于 anchoredPosition 为0的情况下计算，也就是默认位置，如果后续大小不变的前提下，想要移动，只需要修改 anchoredPosition 即可
        rtText.anchoredPosition = Vector2.zero;
        rtBG.anchoredPosition = Vector2.zero;
        
        // 文字区域最大宽度
        var maxRectWidth = 600f; // Screen.width * 0.8f;
        var textSize = text.GetTextSize(maxRectWidth);
        var bgSize = textSize + bgPadding; // 10个像素padding
        
        // 点击位置坐标相对屏幕的 normalizedPosition， 且要保持锚点在点击位置
        var normalizedPosition = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        
        // 整个TIPS框的大小， 包括 文本 和 箭头
        var clickTipsRect = new Vector2(rtText.rect.width + rtArrow.rect.width, rtText.rect.height + rtArrow.rect.height);

        if (isFixAnchor)
        {
            // 固定锚点
            rtText.anchorMin = normalizedPosition;
            rtText.anchorMax = normalizedPosition;
            rtText.sizeDelta = textSize;
            
            rtBG.anchorMin = Vector2.one * 0.5f;
            rtBG.anchorMax = Vector2.one * 0.5f;
            rtBG.sizeDelta = bgSize;
        }
        else
        {
            // 锚区
            rtText.anchorMin = new Vector2(normalizedPosition.x - textSize.x / Screen.width * 0.5f, normalizedPosition.y - textSize.y / Screen.height * 0.5f);
            rtText.anchorMax = new Vector2(normalizedPosition.x + textSize.x / Screen.width * 0.5f, normalizedPosition.y + textSize.y / Screen.height * 0.5f);
            rtText.sizeDelta = Vector2.zero;

            rtBG.anchorMin = Vector2.zero;
            rtBG.anchorMax = Vector2.one;
            rtBG.sizeDelta = bgPadding;
        }
        
        DLog.Log($"width: {maxRectWidth}, textSize: {textSize}, rtText: {rtText.rect}, y: {rtText.position.y}");
        

        // 为保持箭头在点击位置，需要调整箭头、文字和背景的位置， 修改 anchoredPosition 不受Anchor区域的影响，只是修改Rect位置
        // 默认箭头朝上, 如果下部区域超出屏幕就换为箭头朝下
        if (screenPos.y - clickTipsRect.y - padding.w < 0)
        {
            // 箭头朝下
            rtText.anchoredPosition = new Vector2(0, clickTipsRect.y * 0.5f);
            rtArrow.anchoredPosition = new Vector2(0, -clickTipsRect.y * 0.5f);
            rtArrow.localRotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            // 箭头朝上
            rtArrow.localRotation = Quaternion.identity;
            rtText.anchoredPosition = new Vector2(0, -clickTipsRect.y * 0.5f);
            rtArrow.anchoredPosition = new Vector2(0, clickTipsRect.y * 0.5f);
        }
            
        // 如果左右区域超出屏幕， 就需要将文本限制在屏幕边缘，单独对箭头进行修正
        if (screenPos.x - clickTipsRect.x * 0.5f - padding.x < 0)
        {
            // 左侧超出，文本向右移动
            var left = screenPos.x - clickTipsRect.x * 0.5f - padding.x;
            var rightOffset = -left + padding.x - rtArrow.sizeDelta.x * 0.5f;
            rtText.anchoredPosition = new Vector2(rightOffset, rtText.anchoredPosition.y);
            rtArrow.anchoredPosition = new Vector2(-rightOffset, rtArrow.anchoredPosition.y);
        }
        else if (screenPos.x + clickTipsRect.x * 0.5f + padding.z > Screen.width)
        {
            // 右侧超出，文本向左移动
            var right = screenPos.x + clickTipsRect.x * 0.5f + padding.z - Screen.width;
            var leftOffset = -right - padding.z + rtArrow.sizeDelta.x * 0.5f;
            rtText.anchoredPosition = new Vector2(leftOffset, rtText.anchoredPosition.y);
            rtArrow.anchoredPosition = new Vector2(-leftOffset, rtArrow.anchoredPosition.y);
        }
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
