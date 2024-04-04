public static class Constants
{
    // ��ȭâ�� ����
    public enum DialogueType { ROOM, FOLLOW }
    public static int ToInt(this DialogueType dialogueType)
    {
        // ���̾�α� Ÿ�Կ� ���� �ٸ� ���� ��ȯ
        switch (dialogueType)
        {
            case DialogueType.ROOM: return 0;
            case DialogueType.FOLLOW: return 1;
            default: return 0;
        }
    }




    // -------------- ���� ���� ������ -------------- //
    public enum FirstFollowObject
    {
        // ù��° ��� (����, ����, ������, ��, ����ī��, ī��, ������, ��ȣ��)
        Villa, Bread, Con, Bar, Izakawa, Cat, Cafe, Receipt, Light

        // �ι�° ���
    }

    public static string EventID(this FirstFollowObject objectName)
    {
        // ���� ������Ʈ�� ���� event id ��ȯ
        switch (objectName)
        {
            case FirstFollowObject.Villa:   return "Event_019";
            case FirstFollowObject.Bread:   return "Event_020";
            case FirstFollowObject.Con:     return "Event_021";
            case FirstFollowObject.Bar:     return "Event_022";
            case FirstFollowObject.Izakawa: return "Event_023";
            case FirstFollowObject.Cat:     return "Event_024";
            case FirstFollowObject.Cafe:    return "Event_025";
            case FirstFollowObject.Receipt: return "Event_026";
            case FirstFollowObject.Light:   return "Event_027";
            default: return null;
        }
    }

    public static string ClickVariable(this FirstFollowObject objectName)
    {
        // ���� ������Ʈ�� ���� ���� �̸� ��ȯ
        switch (objectName)
        {
            case FirstFollowObject.Villa:   return "VillaClick";
            case FirstFollowObject.Bread:   return "BreadClick";
            case FirstFollowObject.Con:     return "ConClick";
            case FirstFollowObject.Bar:     return "BarClick";
            case FirstFollowObject.Izakawa: return "IzakawaClick";
            case FirstFollowObject.Cat:     return "CatClick";
            case FirstFollowObject.Cafe:    return "CafeClick";
            case FirstFollowObject.Receipt: return "ReceiptClick";
            case FirstFollowObject.Light:   return "LightClick";
            default: return null;
        }
    }
}
