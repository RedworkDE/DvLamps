#if BepInEx
using BepInEx.Logging;

#endif

namespace RedworkDE.DvLamps
{
	/// <summary>
	/// Provides Logger for a type <typeparamref name="T"/> and static methods to access it directly
	/// </summary>
	public static class Logging<T>
	{
		// ReSharper disable once StaticMemberInGenericType
		public static readonly ManualLogSource Logger =
#if BepInEx
			BepInEx.Logging.Logger.CreateLogSource(typeof(T).Name);
#elif UMM
		new ManualLogSource(typeof(T).Name);
#endif


		public static void LogFatal(object data)
		{
			Logger.LogFatal(data);
		}

		public static void LogError(object data)
		{
			Logger.LogError(data);
		}

		public static void LogWarning(object data)
		{
			Logger.LogWarning(data);
		}

		public static void LogMessage(object data)
		{
			Logger.LogMessage(data);
		}

		public static void LogInfo(object data)
		{
			Logger.LogInfo(data);
		}

		public static void LogDebug(object data)
		{
			Logger.LogDebug(data);
		}
	}
}
