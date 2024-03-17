using UnityEngine;
using UnityEngine.UI;

public class SuperButton : Button
{
    protected override void Start()
    {
        base.Start();

        // 버튼의 Transition의 Color Tint를 None 상태로 변경
        ChangeButtonColorTint(Color.white);
    }

    // 버튼의 Color Tint를 변경하는 함수
    protected void ChangeButtonColorTint(Color color)
    {
        ColorBlock colorBlock = colors;
        colorBlock.normalColor = color;
        colorBlock.highlightedColor = color;
        colorBlock.pressedColor = color;
        colorBlock.disabledColor = color;
        colors = colorBlock;
    }
}