public static class Constants
{
    // ��ȭâ�� ����
    public enum DialogueType { ROOM, FOLLOW, FOLLOW_ANGRY }
    public static int ToInt(this DialogueType dialogueType)
    {
        // ���̾�α� Ÿ�Կ� ���� �ٸ� ���� ��ȯ
        switch (dialogueType)
        {
            case DialogueType.ROOM: return 0;
            case DialogueType.FOLLOW: return 1;
            case DialogueType.FOLLOW_ANGRY: return 2;
            default: return 0;
        }
    }




    // -------------- ���� ���� ������ -------------- //
    public enum FirstFollowObject
    {
        // ����, ����, ������, ��, ����ī��, ī��, ������, ��ȣ��, ����, 1�� �ʰ���, 2�� �ʰ���
        Villa, Bread, Con, Bar, Izakawa, Cat, Cafe, Receipt, Light, Coffee, Shop_1, Shop_2,

        // ������, �Ĵ�, Ĭ���� ��, ȭ��ǰ ����, ������, Ŭ��, ����, ȭȯ, ȭ�� ���
        Construction, Omerice, Cocktail, MiracleYoung, MusicBar, Club, Beer, Wreath, Angry
    }

    public static string EventID(this FirstFollowObject objectName)
    {
        // ���� ������Ʈ�� ���� event id ��ȯ
        switch (objectName)
        {
            // ù��° ���
            case FirstFollowObject.Villa:   return "Event_019";
            case FirstFollowObject.Bread:   return "Event_020";
            case FirstFollowObject.Con:     return "Event_021";
            case FirstFollowObject.Bar:     return "Event_022";
            case FirstFollowObject.Izakawa: return "Event_023";
            case FirstFollowObject.Cat:     return "Event_024";
            case FirstFollowObject.Cafe:    return "Event_025";
            case FirstFollowObject.Receipt: return "Event_026";
            case FirstFollowObject.Light:   return "Event_027";
            case FirstFollowObject.Coffee:  return "Event_028";
            case FirstFollowObject.Shop_1: return "Event_029";
            case FirstFollowObject.Shop_2:   return "Event_030";

            // �ι�° ���
            case FirstFollowObject.Construction: return "Event_031";
            case FirstFollowObject.Omerice: return "Event_032";
            case FirstFollowObject.Cocktail: return "Event_033";
            case FirstFollowObject.MiracleYoung: return "Event_034";
            case FirstFollowObject.MusicBar: return "Event_035";
            case FirstFollowObject.Club: return "Event_036";
            case FirstFollowObject.Beer: return "Event_037";
            case FirstFollowObject.Wreath: return "Event_038";
            case FirstFollowObject.Angry: return "Event_039";

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

            case FirstFollowObject.Coffee: return "LatteClcik";
            case FirstFollowObject.Shop_1: return "1FClothesClick";
            case FirstFollowObject.Shop_2: return "2FClothesClick";
            case FirstFollowObject.Construction: return "ConstructionClick";
            case FirstFollowObject.Omerice: return "RestaurantClick";
            case FirstFollowObject.Cocktail: return "CocktailBarClick";
            case FirstFollowObject.MiracleYoung: return "CosemeticClick";
            case FirstFollowObject.MusicBar: return "MusicBarClick";
            case FirstFollowObject.Club: return "ClubCliclk";
            case FirstFollowObject.Beer: return "BeerClick";
            default: return null;
        }
    }
}
