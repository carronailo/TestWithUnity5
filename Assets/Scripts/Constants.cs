using UnityEngine;

public static class Constants
{
#if UNITY_EDITOR
	public static string externalResourcePath = Application.dataPath + "/../AssetBundles/";
	public static string externalResourcePath_Editor = Application.dataPath + "/../AssetBundles_Editor/";
#else
	public static string externalResourcePath = Application.persistentDataPath + "/";
#endif

	public static string internalResourcePath = Application.streamingAssetsPath + "/";

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
	public const string urlFileHeader = "file:///";
#else
	public const string urlFileHeader = "file://";
#endif

	public const int networkCommunicateBufferUnitSize = 2048;

	//public const float connectionCheckInterval = 3f;
	//public const float connectionRequestTimeout = 10f;

	// 
	public const long heartBeatReturnTimeout = 10000;    // 心跳接收间隔极限时间，超出则认为心跳丢失
	public const int heartBeatLostLimit = 6;		// 连续多少条心跳超时，认为是网络连接已经断开
	public const long heartBeatInterval = 5000;		// 心跳发送间隔，此为最小间隔，因为心跳按顺序发送，如果上一条心跳没有返回也没有超时，那么会一直等待（也可以推导出心跳最大间隔=max(发送间隔,心跳超时)）

	public const int reconnectMaxRetry = 2;    // 重连尝试次数
	public const long reconnectRetryTimeout = 10000;    // 重连超时时间

	//public const float defaultUp = 0f;
	public const float StandStillMass = 1000f;
	public const float StandardMass = 1f;
	public const float HugeDrag = 99999f;
	public const float randomRunRadius = 8f;
	public const float hitStallingSeconds = 0.05f;
	public const float minimumDashDistance = 4f;
	public const float dashStoppingDistance = 1f;
	//public const float runningUnitTime = 0.5f;
	//public const float attackRushingUnitTime = 0.15f;
	//public const float attackRushingDistanceThreshold = 6f;
	//public const float attackRushingStoppingDistance = 2f;
	//public const float dashingUnitTime = 0.33f;
	//public const float dodgingUnitTime = 1f;
	public const float joystickMessageRepeatTime = 0f;
	public const float buttonPressToHoldThresholdTime = 0.01f;
	public const float aiPositioningRepeatTime = 0.5f;
	public const float aiStateCheckLoopTime = 0f;
	public const float skillComboTimeWindow = 5f;       // 技能Combo衔接的时间窗口，默认3秒
														//public const float MinAngle = 20f;
	public static Vector2 groundBoundaryX = new Vector2(-10f, 50f);
	public static Vector2 groundBoundaryZ = new Vector2(-25f, 250f);
	public const float touchButtonHoldMessageInterval = 0.01f;
	public const float minDistanceBetweenCharacters = 1.5f;
	public const float distanceEpsilon = 0.01f;
	public const int knockbackFrameCount = 20;
	public const float maxKnockbackDistancePerFrame = 0.66f;
	public const float defaultAutoLockOnDistance = 10f;
	public const float comboTimeLimit = 5f;
	public const float floatTouchDownThreshold = 0.1f;          // 浮空过程中使用了定帧，floatTouchDownThreshold决定了当角色离地面多高时开始就要取消定帧恢复正常的动作
	public const float resumeAutoCombatDelay = 3f;
	public static Vector3 ignoreYAxis = new Vector3(1, 0, 1);
	public const float mpRecoverInterval = 5f;      // 5秒回蓝一次
	public const float spiritRegenerateInterval = 5f;   // 5秒增加灵魂一次
	public const int maximumSpirit = 100;   // 一次战斗内的灵魂存储上限
	public const float minDistanceToLockedTarget = 0.5f;
	public const float submitComboInterval = 1f;
	public const float submitPlayerHPInterval = 1f;

	public static float controllerDirectionThresholdForIdle = 0.0001f;
	public static float controllerDirectionThresholdForWalk = 0.3001f;
	//public static float controllerDirectionThresholdForRun = 0.6f;

	public static float checkMonsterDistanceOnBattleFieldInterval = 0.5f;

	public static float chargeGaugeDisplayDelay = 0.3f;
	public static float guideGaugeDisplayDelay = 0.3f;

	public static Color beHitColor = new Color(1f, 1f, 1f, 0.5f);
	public static Color noBeHitColor = new Color(0f, 0f, 0f, 0.5f);

	public const string terrainLayerName = "Terrain";
	public static int terrainLayer = LayerMask.NameToLayer(terrainLayerName);
	public const string blocksLayerName = "Block";
	public static int blocksLayer = LayerMask.NameToLayer(blocksLayerName);
	public const string weaponLayerName = "Weapon";
	public static int weaponLayer = LayerMask.NameToLayer(weaponLayerName);
	public const string bodyLayerName = "Body";
	public static int bodyLayer = LayerMask.NameToLayer(bodyLayerName);
	public const string characterLayerName = "Character";
	public static int characterLayer = LayerMask.NameToLayer(characterLayerName);
	public const string characterIgnorePhysicsLayerName = "CharacterIgnorePhysics";
	public static int characterIgnorePhysicsLayer = LayerMask.NameToLayer(characterIgnorePhysicsLayerName);
	public const string distortionLayerName = "DistortionSFX";
	public static int distortionLayer = LayerMask.NameToLayer(distortionLayerName);
	public const string sfxLayerName = "TransparentFX";
	public static int sfxLayer = LayerMask.NameToLayer(sfxLayerName);
    public const string uiLayerName = "UI";
    public static int uiLayer = LayerMask.NameToLayer(uiLayerName);

    public const string playerTag = "Player";
	public const string aiTag = "AI";
	public const string uiCameraTag = "UICamera";
	public const string specialCameraTag = "SpecialCamera";
	public const string miniMapCameraTag = "MiniMapCamera";
	public const string splashScreenTag = "SplashScreen";
	public const string cameraAnchorTag = "CameraAnchor";
	public const string extraEffectTag = "ExtraEffect";
	public const string reusableEffectTag = "ReusableEffect";
	public const string specialCombatTargetTag = "SpecialCombatTarget";
	public const string sfxBaseComponentTag = "SFXBaseComponent";
	public const string sfxExtraComponentTag = "SFXExtraComponent";

	public const string playerCameraAnchorBoneName = "Bone006";

	public const string miniMapTexturePrefix = "MiniMap";

	//public static int characterEquipmentSlotsCount = EquipInstanceCtrl.BODY_PART_NUM + EquipInstanceCtrl.BODY_JEWELRY_NUM;

	public static int arenaRecordSubmitSizeLimit = 1024;

	public static string p2pGameType = "ChingLemon3DARPG";
	public static int p2pNetworkConnectionLimit = 32;

	public const int EquipMaxQua = 7;
	public const int SKILL_TOTAL_NUM = 4;

	//以下放在配置表中
	//public const int SpiritPerFeather = 1;
	//public const int FeatherId = 104523;

	//面板的不同层级下的panel根据Depth值排序，每个增加100, 
	public const int PanelAddRenderDepth = 100;
	public const int MaxPanelRenderDepth = 10000;

	//每层面板的数值
	public const int BackgroudRenderDepth = 4000;
	public const int ModuleRenderDepth = 5000;
	public const int FloatingRenderDepth = 6000;
	public const int DialogRenderDepth = 7000;
	public const int GuildRenderDepth = 8000;
	public const int AlertRenderDepth = 9000;

	public const int spiritLessClusterAmount = 3;
	public const int spiritModerateClusterAmount = 5;
	public const int spiritGrandClusterAmount = 10;
	public const float minionSpiritMultiply = 1f;
	public const float eliteSpiritMultiply = 5f;
	public const float bossSpiritMultiply = 10f;

	//各个场景可以打开的最大面板数
	public const int MainCity_Max_UseOpen_Count = 20;
	public const int BattleField_Max_UseOpen_Count = 20;
	public const int Arena_Max_UseOpen_Count = 20;
	public const int Moba_Max_UseOpen_Count = 20;
	public const int TeamPVP_Max_UseOpen_Count = 20;
	public const int Fishing_Max_UseOpen_Count = 20;
	public const int FightingJam_Max_UseOpen_Count = 20;
	public const int Dafault_Max_UseOpen_Count = 5;
	public const int Endless_Max_UseOpen_Count = 20;

	public const int copyPathSlotMax = 12;      // 副本选择界面，副本按钮排列路径上可以放置副本按钮的槽位最大数量
	public const int maxSaoDangTimes = 10;
	public const string worldMapAtlas = "BattlePrepare";
	public const string worldMapCopyPass = "已通关";
	public const string worldMapStarAtlas = "Star";
	public const string worldMapStarSlotSprite = "星_底(副本用)";
	public const string worldMapStarSprite = "星_金(副本用)";

	public const int reckoningItemDisplayMax = 8;   // 结算面板显示奖励物品时，最多显示多少个物品格子（必定显示，不管有无物品）
	public const string reckoningItemEmptyAtlas = "Temp";
	public const string reckoningItemEmptySprite = "背包_空";

	public const int bagItemPerPage = 25;       // 背包一页显示的物品数量，主要用来判断当现实的物品不足一页时自动补空格
    public const int bagItemPerLine = 5;        // 背包一行显示的物品数量

	public const int factionItemPerPage = 4;
	public const string dismissFactionPassword = "解散公会";

	public const int maxChatMessagePerChannel = 50;

	public const int maxMailKeepDays = 3;   // 邮件最多保存的天数
	public const long maxMailKeepMillis = 3 * 24 * 60 * 60 * 1000;

	public const bool trueBool = true;
	public const bool falseBool = false;

	public const int zeroInt = 0;
	public const int oneInt = 1;
	public const int twoInt = 2;

	public const float zeroFloat = 0f;
	public const float zeroDotEightFloat = 0.8f;
	public const float oneFloat = 1.0f;
	public const float oneDotTowFloat = 1.2f;
	public const float twoFloat = 2f;
	public const float hundredFloat = 100f;

}
