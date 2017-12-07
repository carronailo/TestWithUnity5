
public static class SystemParameters
{
	//public static Logger.ELogLevel logLevel = Logger.ELogLevel.全部;
	public static bool debugCore = false;
	public static bool forceInternal = true;
	public static bool loadShader = true;
	public static bool reuseSFX = true;
	public static bool useFixedUpdate = false;
	public static bool useSDKLogin = false;
	public static string loginVerifyURL = string.Empty;
	public static string tokenRequestURL = string.Empty;
	public static bool useSDKPay = false;
	public static string orderRequestURL = string.Empty;
	public static string payCallbackURL = string.Empty;

	public static bool disableUIResourceLoader = false;
	public static bool disableAudioResourceLoader = false;
	public static bool disableSFXResourceLoader = false;
	public static bool disableCharacterResourceLoader = false;
	public static bool disableMapResourceLoader = false;
    public static bool disablePanelResourceLoader = false;

    public static bool alreadyLogin = false;
    public static bool disableLogoutButton = false;
}
