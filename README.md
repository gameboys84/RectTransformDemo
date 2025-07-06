# RectTransformDemo
文档对应的实战部分

[RectTransform - 项目笔记 - Confluence](https://gameboys.atlassian.net/wiki/spaces/Note/pages/64946187/RectTransform)



# 实现需求

通用要求：屏幕为1920x1080，所有文本的外边框会有10个单位的空白padding区域

1. 实现一个文本提示框，要求提示框区域跟随文本内容变化，显示位置位于屏幕中间，垂直方向位于中间偏上30%的位置。
2. 实现一个图文混排的提示框，要求提示框区域跟随文本内容变化，位置随鼠标点击位置进行修正，不让文本框超出屏幕范围
3. 实现一个类似邮件列表的功能，要求屏幕上部固定高度区域为主标题区域，下面左边1/3区域固定保持为列表，下面右边2/3区域中的上部固定高度区域为邮件标题，下面是邮件内容，且内容支持滚动。

以上内容先用UGUI来实现，然后再用UIToolKit实现，并保持动画过渡效果。

所有功能优先保证UI的需求，其次再保证性能和优美。



# 测试场景说明

## UGUI_Tips

用UGUI实现的文本提示框

## UGUI_ClickTips

用UGUI实现的依据点击区域显示的提示

## UGUI_CMail

用UGUI实现的类似邮件列表的界面 



## TBD

后续会继续用UIToolKit来实现