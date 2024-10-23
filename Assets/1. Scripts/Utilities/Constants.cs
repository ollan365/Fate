public static class Constants
{
    // 씬의 종류
    public enum SceneType { START, ROOM_1, FOLLOW_1, ROOM_2, FOLLOW_2, ENDING }
    public static int ToInt(this SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.START: return 0;
            case SceneType.ROOM_1: return 1;
            case SceneType.FOLLOW_1: return 2;
            case SceneType.ROOM_2: return 3;
            case SceneType.FOLLOW_2: return 4;
            case SceneType.ENDING: return 5;
            default: return 0;
        }
    }
    public static SceneType ToEnum(this int sceneType)
    {
        switch (sceneType)
        {
            case 0: return SceneType.START;
            case 1: return SceneType.ROOM_1;
            case 2: return SceneType.FOLLOW_1;
            case 3: return SceneType.ROOM_2;
            case 4: return SceneType.FOLLOW_2;
            case 5: return SceneType.ENDING;
            default: return 0;
        }
    }

    // 대화창의 종류
    public enum DialogueType { ROOM, ROOM_THINKING, FOLLOW, FOLLOW_THINKING, FOLLOW_EXTRA, CENTER }
    public static int ToInt(this DialogueType dialogueType)
    {
        // 다이얼로그 타입에 따라 다른 숫자 반환
        switch (dialogueType)
        {
            case DialogueType.ROOM: return 0;
            case DialogueType.ROOM_THINKING: return 1;
            case DialogueType.FOLLOW: return 2;
            case DialogueType.FOLLOW_THINKING: return 3;
            case DialogueType.FOLLOW_EXTRA: return 4;
            case DialogueType.CENTER: return 5;
            default: return 0;
        }
    }

    public enum EndingType { NONE, BAD_A, BAD_B, TRUE, HIDDEN }

    public enum SoundType { BGM, LOOP, SOUND_EFFECT }
    // 사운드 종류
    // 1. 배경음
    public const int
        BGM_STOP = -1,
        BGM_OPENING = 0,
        BGM_ROOM1 = 1,
        BGM_FOLLOW1 = 2,
        BGM_MINIGAME = 3,
        BGM_FINISHGAME = 4,
        BGM_ROOM2 = 5,
        BGM_BAD = 6, 
        BGM_TRUE = 7, 
        BGM_HIDDEN = 8;

    // 2. 방탈출 오브젝트들
    public const int
        Sound_CarpetOpen = 0, 
        Sound_CarpetClose = 1, 
        Sound_ClosetOpen = 2, 
        Sound_ClosetClose = 3,
        Sound_StorageOpen = 4,
        Sound_StorageClose = 5,
        Sound_Key = 6,
        Sound_LockerKeyMovement = 7,
        Sound_LockerUnlock = 9,
        Sound_DiaryUnlock = 10,
        Sound_LaptopBoot = 11,
        Sound_Poster = 12,
        Sound_ChairMovement = 13,
        Sound_GrabPaper = 23,
        Sound_TincaseOpen = 24, 
        Sound_SewingBoxBall = 25, 
        Sound_SewingBoxOpen = 26,
        Sound_HeartPop = 38;

    // 3. 미행
    public const int
        Sound_Building = 15, 
        Sound_FollowSpecialObject = 16, 
        Sound_Cat = 17, 
        Sound_TurnAround = 18, 
        Sound_FollowEnd = 19;

    // 4. 루프 (반복 되어야 하는 것)
    public const int
        Sound_FootStep_Fate = 0, 
        Sound_FootStep_Accidy = 1,
        Sound_ClockMovement = 2, 
        Sound_TincaseScroll = 3;

    // 5. 그 외
    public const int
        Sound_Typing = -1, 
        Sound_Click = 20, 
        Sound_Correct = 21,
        Sound_Wrong = 22;

    public const int
        Sound_AccidentSound = 27,
        Sound_Run2_1 = 28,
        Sound_9_1_LockerKeyMovement = 29,
        Sound_CarCrash_new = 30,
        Sound_RunStairs1_2 = 31,
        Sound_Door_open_metal = 32,
        Sound_Unlock_doorslap_ = 33,
        Sound_Wall_crash_002 = 34,
        Sound_City_people_talking = 35,
        Sound_Wavecrash__ = 36,
        Sound_Truck_honk_1 = 37;

    // 시계 부분

    // -------------- 미행 관련 변수들 -------------- //
    public enum FollowObjectName
    {
        // ========== 첫번째 미행 ========== //
        // 빌라, 빵집, 편의점, 바, 이자카야, 카페, 영수증, 신호등, 음료, 1층 옷가게, 2층 옷가게
        Villa, Bread, Con, Bar, Izakawa, Cat, Cafe, Receipt, Light, Coffee, Shop_1, Shop_2,
        // 공사장, 식당, 칵테일 바, 화장품 가게, 뮤직바, 클럽, 술집, 화환
        Construction, Omerice, Cocktail, MiracleYoung, MusicBar, Club, Beer, Wreath,


        // ========== 두번째 미행 ========== //
        Villa2,
        Bar2,
        Bread2,
        BeerA,
        Izakawa2,
        Cafe2,
        Clothes_3F,
        Clothes_2F,
        DrinkA,
        CocktailBar2,
        Restaurant2,
        DrinkB,
        MusicBar2,
        Cosmetic2,
        DrinkC,
        BeerB,
        Club2,
        Light2, 
        Con2, 
        Clothes_1F,

        // ========== 기타 등등 ========== //
        Extra
    }

    public enum FollowExtra
    {
        None, 
        Angry, 
        Employee, 
        RunAway_1, 
        RunAway_2, 
        Police, 
        Smoker_1, 
        Smoker_2, 
        Clubber_1, 
        Clubber_2
    }
    public static string EventID(this FollowObjectName objectName)
    {
        // 미행 오브젝트에 따라 event id 반환
        switch (objectName)
        {
            // ===== 첫번째 미행 ===== //
            // 첫번째 배경
            case FollowObjectName.Villa:   return "EventFollowVilla";
            case FollowObjectName.Bread:   return "EventFollowBread";
            case FollowObjectName.Con:     return "EventFollowConvenienceStore";
            case FollowObjectName.Bar:     return "EventFollowBar";
            case FollowObjectName.Izakawa: return "EventFollowIzakaya";
            case FollowObjectName.Cat:     return "EventFollowCat";
            case FollowObjectName.Cafe:    return "EventFollowCafe";
            case FollowObjectName.Receipt: return "EventFollowCafeReceipt";
            case FollowObjectName.Light:   return "EventFollowTrafficLight";
            case FollowObjectName.Coffee:  return "EventFollowCafeLatte";
            case FollowObjectName.Shop_1: return "EventFollow1FClothingStore";
            case FollowObjectName.Shop_2:   return "EventFollow2FClothingStore";

            // 두번째 배경
            case FollowObjectName.Construction: return "EventFollowConstructionSite";
            case FollowObjectName.Omerice: return "EventFollowRestaurant";
            case FollowObjectName.Cocktail: return "EventFollowCocktailBar";
            case FollowObjectName.MiracleYoung: return "EventFollowDrugstore";
            case FollowObjectName.MusicBar: return "EventFollowMusicBar";
            case FollowObjectName.Club: return "EventFollowClub";
            case FollowObjectName.Beer: return "EventFollowPub";
            case FollowObjectName.Wreath: return "EventFollowWreath";
            case FollowObjectName.Light2: return "EventFollow2TrafficLight";
            case FollowObjectName.Con2: return "EventFollow2Con";
            case FollowObjectName.Clothes_1F: return "EventFollow21FClothingStore";



            // ===== 두번째 미행 ===== //
            // 첫번째 배경
            case FollowObjectName.Villa2: return "EventFollow2Villa";
            case FollowObjectName.Bar2: return "EventFollow2Bar";
            case FollowObjectName.Bread2: return "EventFollow2Bread";
            case FollowObjectName.BeerA: return "EventFollow2BeerA";
            case FollowObjectName.Izakawa2: return "EventFollow2Izakawa";
            case FollowObjectName.Cafe2: return "EventFollow2Cafe";
            case FollowObjectName.Clothes_3F: return "EventFollow23FClothes";
            case FollowObjectName.Clothes_2F: return "EventFollow22FClothes";
            case FollowObjectName.DrinkA: return "EventFollow2DrinkA";
            case FollowObjectName.CocktailBar2: return "EventFollow2CocktailBar";
            case FollowObjectName.Restaurant2: return "EventFollow2Restaurant";
            case FollowObjectName.DrinkB: return "EventFollow2DrinkB";
            case FollowObjectName.MusicBar2: return "EventFollow2MusicBar";
            case FollowObjectName.Cosmetic2: return "EventFollow2Cosemetic";
            case FollowObjectName.DrinkC: return "EventFollow2DrinkC";
            case FollowObjectName.BeerB: return "EventFollow2BeerB";
            case FollowObjectName.Club2: return "EventFollow2Club";
            default: return null;
        }
    }

    public static string EventID(this FollowExtra extraName)
    {
        switch (extraName)
        {
            case FollowExtra.Angry: return "EventFollowAngryPerson";

            case FollowExtra.Employee: return "EventFollow2Solicitation";
            case FollowExtra.RunAway_1: return "EventFollow2Teenage";
            case FollowExtra.RunAway_2: return "EventFollow2Teenage";
            case FollowExtra.Police: return "EventFollow2Police";
            case FollowExtra.Smoker_1: return "EventFollow2Cigarette";
            case FollowExtra.Smoker_2: return "EventFollow2Cigarette";
            case FollowExtra.Clubber_1: return "EventFollow2Guard";
            case FollowExtra.Clubber_2: return "EventFollow2Guard";

            default: return null;
        }
    }

    public static string ClickVariable(this FollowObjectName objectName)
    {
        // 미행 오브젝트에 따라 변수 이름 반환
        switch (objectName)
        {
            // ===== 첫번째 미행 ===== //
            case FollowObjectName.Villa:   return "VillaClick";
            case FollowObjectName.Bread:   return "BreadClick";
            case FollowObjectName.Con:     return "ConClick";
            case FollowObjectName.Bar:     return "BarClick";
            case FollowObjectName.Izakawa: return "IzakawaClick";
            case FollowObjectName.Cat:     return "CatClick";
            case FollowObjectName.Cafe:    return "CafeClick";
            case FollowObjectName.Receipt: return "ReceiptClick";
            case FollowObjectName.Light:   return "LightClick";

            case FollowObjectName.Coffee: return "LatteClcik";
            case FollowObjectName.Shop_1: return "1FClothesClick";
            case FollowObjectName.Shop_2: return "2FClothesClick";
            case FollowObjectName.Construction: return "ConstructionClick";
            case FollowObjectName.Omerice: return "RestaurantClick";
            case FollowObjectName.Cocktail: return "CocktailBarClick";
            case FollowObjectName.MiracleYoung: return "CosemeticClick";
            case FollowObjectName.MusicBar: return "MusicBarClick";
            case FollowObjectName.Club: return "ClubCliclk";
            case FollowObjectName.Beer: return "BeerClick";
            case FollowObjectName.Wreath: return "WreathClick";

            // ===== 두번째 미행 ===== //
            case FollowObjectName.Villa2: return "VillaClick2";
            case FollowObjectName.Bar2: return "BarClick2";
            case FollowObjectName.Bread2: return "BreadClick2";
            case FollowObjectName.BeerA: return "BeerAClick2";
            case FollowObjectName.Izakawa2: return "IzakawaClick2";
            case FollowObjectName.Cafe2: return "CafeClick2";
            case FollowObjectName.Clothes_3F: return "3FClothesClick2";
            case FollowObjectName.Clothes_2F: return "2FClothesClick2";
            case FollowObjectName.DrinkA: return "DrinkAClick2";
            case FollowObjectName.CocktailBar2: return "CocktailBarClick2";
            case FollowObjectName.Restaurant2: return "RestaurantClick2";
            case FollowObjectName.DrinkB: return "DrinkBClick2";
            case FollowObjectName.MusicBar2: return "MusicBarClick2";
            case FollowObjectName.Cosmetic2: return "CosemeticClick2";
            case FollowObjectName.DrinkC: return "DrinkCClick2";
            case FollowObjectName.BeerB: return "BeerBClick2";
            case FollowObjectName.Club2: return "ClubClick2";
            case FollowObjectName.Light2: return "LightClick2";
            case FollowObjectName.Con2: return "ConClick2";
            case FollowObjectName.Clothes_1F: return "1FClothesClick2";
            default: return null;
        }
    }

    public static string ClickVariable(this FollowExtra extraName)
    {
        switch (extraName)
        {
            case FollowExtra.Angry: return "AngryClick";

            case FollowExtra.Employee: return "SolicitationClick2";
            case FollowExtra.RunAway_1: return "TeenageClick2";
            case FollowExtra.RunAway_2: return "TeenageClick2";
            case FollowExtra.Police: return "PoliceClick2";
            case FollowExtra.Smoker_1: return "CigaretteClick2";
            case FollowExtra.Smoker_2: return "CigaretteClick2";
            case FollowExtra.Clubber_1: return "GuardClick2";
            case FollowExtra.Clubber_2: return "GuardClick2";
            default: return null;
        }
    }
}
