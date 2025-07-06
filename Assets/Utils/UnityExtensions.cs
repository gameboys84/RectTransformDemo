using TMPro;
using UnityEngine;

namespace Utils
{
    public static class UnityExtensions
    {
        /*
         * 获取文本的单字符宽度和单行高度
         * x:单个字符宽度，y:单行高度
         */
        public static Vector2 GetGlyphSize(this TMP_Text text)
        {
            var defaultOneLineSize = text.GetPreferredValues("a");
            var defaultTwoLineSize = text.GetPreferredValues("a a\na");
            var defaultSingleLineSize = defaultTwoLineSize - defaultOneLineSize; 
            return defaultSingleLineSize;
        }

        public static Vector2 GetTextSize(this TMP_Text text, float maxWidth, Vector2 glyphSize = default(Vector2))
        {
            var defaulGlyphSize = glyphSize == default ? text.GetGlyphSize() : glyphSize;
            
            // text.preferredHeight 在即使没超宽的情况下，计算出来的结果可能会比预期的更高，需要手动修正
            var textSize = new Vector2(text.preferredWidth, text.preferredHeight);
            var lines = text.text.Split('\n');
            var lineCount = lines.Length;
            
            // 依据行数和行高计算总高度， 如果混排的情况还要验证
            if (textSize.x > maxWidth)
            {
                foreach (var line in lines)
                {
                    var lineSize = text.GetPreferredValues(line, maxWidth, 0f);
                    // 单行超宽
                    if (lineSize.x > maxWidth)
                    {
                        var curLineCount = Mathf.CeilToInt((lineSize.x + defaulGlyphSize.x) / maxWidth);
                        lineCount += (curLineCount - 1);
                        DLog.Log($"Extra line: {curLineCount - 1}, lineSize: {lineSize}, maxWidth: {maxWidth}");
                    }
                }
            }

            textSize = new Vector2(Mathf.Min(textSize.x, maxWidth), lineCount * defaulGlyphSize.y);
            
            DLog.Log($"Max line: {lineCount}, Text: {textSize}, defaulGlyphSize: {defaulGlyphSize}");
            return textSize;
        }
    }
}