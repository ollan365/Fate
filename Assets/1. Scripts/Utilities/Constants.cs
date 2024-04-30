public static class Constants
{
    // 대화창의 종류
    public enum DialogueType { ROOM, ROOM_THINKING, FOLLOW, FOLLOW_THINKING, FOLLOW_ANGRY, FOLLOW_TUTORIAL }
    public static int ToInt(this DialogueType dialogueType)
    {
        // 다이얼로그 타입에 따라 다른 숫자 반환
        switch (dialogueType)
        {
            case DialogueType.ROOM: return 0;
            case DialogueType.ROOM_THINKING: return 1;
            case DialogueType.FOLLOW: return 2;
            case DialogueType.FOLLOW_THINKING: return 3;
            case DialogueType.FOLLOW_ANGRY: return 4;
            case DialogueType.FOLLOW_TUTORIAL: return 5;
            default: return 0;
        }
    }

    public enum SoundType { BGM, LOOP, SOUND_EFFECT }
    // 사운드 종류
    // 1. 배경음
    public const int
        BGM_ROOM = 0, BGM_FOLLOW = 1, BGM_MINIGAME = 2;

    // 2. 방탈출 오브젝트들
    public const int
        Sound_CarpetOpen = 0, Sound_CarpetClose = 1, Sound_ClosetOpen = 2, Sound_ClosetClose = 3,
        Sound_StorageOpen = 4, Sound_StorageClose = 5, Sound_ClockMovement = 6, Sound_LockerKeyMovement = 7, Sound_LockerUnlock = 9,
        Sound_DiaryUnlock = 10, Sound_LaptopBoot = 11, Sound_Poster = 12, Sound_ChairMovement = 13;

    // 3. 미행
    public const int
        Sound_Building = 15, Sound_FollowSpecialObject = 16, Sound_Cat = 17;

    // 4. 루프 (반복 되어야 하는 것)
    public const int
        Sound_FootStep_Accidy = 0, Sound_FootStep_Fate = 1;

    // 5. 그 외
    public const int
        Sound_Typing = -1, Sound_Click = 18, Sound_Correct = 19, Sound_Wrong = 20;
        

    // 시계 부분



    // -------------- 미행 관련 변수들 -------------- //
    public enum FirstFollowObject
    {
        // 빌라, 빵집, 편의점, 바, 이자카야, 카페, 영수증, 신호등, 음료, 1층 옷가게, 2층 옷가게
        Villa, Bread, Con, Bar, Izakawa, Cat, Cafe, Receipt, Light, Coffee, Shop_1, Shop_2,

        // 공사장, 식당, 칵테일 바, 화장품 가게, 뮤직바, 클럽, 술집, 화환, 화난 사람
        Construction, Omerice, Cocktail, MiracleYoung, MusicBar, Club, Beer, Wreath, Angry
    }

    public static string EventID(this FirstFollowObject objectName)
    {
        // 미행 오브젝트에 따라 event id 반환
        switch (objectName)
        {
            // 첫번째 배경
            case FirstFollowObject.Villa:   return "EventFollowVilla";
            case FirstFollowObject.Bread:   return "EventFollowBread";
            case FirstFollowObject.Con:     return "EventFollowConvenienceStore";
            case FirstFollowObject.Bar:     return "EventFollowBar";
            case FirstFollowObject.Izakawa: return "EventFollowIzakaya";
            case FirstFollowObject.Cat:     return "EventFollowCat";
            case FirstFollowObject.Cafe:    return "EventFollowCafe";
            case FirstFollowObject.Receipt: return "EventFollowCafeReceipt";
            case FirstFollowObject.Light:   return "EventFollowTrafficLight";
            case FirstFollowObject.Coffee:  return "EventFollowCafeLatte";
            case FirstFollowObject.Shop_1: return "EventFollow1FClothingStore";
            case FirstFollowObject.Shop_2:   return "EventFollow2FClothingStore";

            // 두번째 배경
            case FirstFollowObject.Construction: return "EventFollowConstructionSite";
            case FirstFollowObject.Omerice: return "EventFollowRestaurant";
            case FirstFollowObject.Cocktail: return " EventFollowCocktailBar";
            case FirstFollowObject.MiracleYoung: return "EventFollowDrugstore";
            case FirstFollowObject.MusicBar: return "EventFollowMusicBar";
            case FirstFollowObject.Club: return "EventFollowClub";
            case FirstFollowObject.Beer: return "EventFollowPub";
            case FirstFollowObject.Wreath: return "EventFollowWreath";
            case FirstFollowObject.Angry: return "EventFollowAngryPerson";

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
            case FirstFollowObject.Wreath: return "WreathClick";
            case FirstFollowObject.Angry: return "AngryClick";
            default: return null;
        }
    }
}
