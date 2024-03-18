using UnityEngine;
using UnityEngine.UI;

public class SuperButton : Button
{
    protected override void Start()
    {
        base.Start();

        // ��ư�� Transition�� Color Tint�� None ���·� ����
        ChangeButtonColorTint(Color.white);
    }

    // ��ư�� Color Tint�� �����ϴ� �Լ�
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