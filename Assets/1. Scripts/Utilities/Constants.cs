public static class Constants
{
    // 대화창의 종류
    public enum DialogueType { ROOM, FOLLOW }
    public static int ToInt(this DialogueType dialogueType)
    {
        // 다이얼로그 타입에 따라 다른 숫자 반환
        switch (dialogueType)
        {
            case DialogueType.ROOM: return 0;
            case DialogueType.FOLLOW: return 1;
            default: return 0;
        }
    }




    // -------------- 미행 관련 변수들 -------------- //
    public enum FirstFollowObject
    {
        // 첫번째 배경 (빌라, 빵집, 편의점, 바, 이자카야, 카페, 영수증, 신호등)
        Villa, Bread, Con, Bar, Izakawa, Cat, Cafe, Receipt, Light

        // 두번째 배경
    }

    public static string EventID(this FirstFollowObject objectName)
    {
        // 미행 오브젝트에 따라 event id 반환
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
        // 미행 오브젝트에 따라 변수 이름 반환
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
