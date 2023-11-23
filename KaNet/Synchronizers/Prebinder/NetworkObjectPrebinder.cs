using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using Utils;

public static class NetworkObjectPrebinder
{
	private static NetworkObjectPrebindInfo mBasePrebinderByType;
	private static Dictionary<Type, NetworkObjectPrebindInfo> mPrebinderByType = new();
	private static Action<object, object> mSyncFieldsSetter;
	private static Action<object, object> mSyncRPCsSetter;

	public static void InitializeByProcessHandler(Type projectRepresentType)
	{
		// Set NetworkObject field setter actions
		Type baseType = typeof(NetworkObject);

		mSyncFieldsSetter = baseType.GetField("mSyncFields", BindingFlags.Instance | BindingFlags.NonPublic).SetValue;
		mSyncRPCsSetter = baseType.GetField("mSyncRPCs", BindingFlags.Instance | BindingFlags.NonPublic).SetValue;

		// Make network object prebind infomation by definded types
		mBasePrebinderByType = new NetworkObjectPrebindInfo(typeof(NetworkObject), true);

		List<Type> networkObjectTypes = new();

		networkObjectTypes.AddRange(getTypesFromAssembly(projectRepresentType, baseType));
		networkObjectTypes.AddRange(getTypesFromAssembly(typeof(NetworkObject), baseType));

		foreach (var type in networkObjectTypes)
		{
			NetworkObjectPrebindInfo info = new NetworkObjectPrebindInfo(type);
			mPrebinderByType.Add(type, info);
		}

		IEnumerable<Type> getTypesFromAssembly(Type typeInAssembly, Type type)
		{
			var assembly = Assembly.GetAssembly(typeInAssembly);
			var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type));
			return types;
		}
	}

	public static void PrebindByReflection(Type type, object networkObject)
	{
		if (!mPrebinderByType.TryGetValue(type, out var prebinderInfo))
		{
			Ulog.LogError(UlogType.NetworkReflection, $"PrebindByReflection Error! There is no such type : {type}");
			return;
		}

		List<Synchronizer> synchronizers = new List<Synchronizer>();
		List<RpcBase> rpcCallers = new List<RpcBase>();

		mBasePrebinderByType.AddSynchronizersTo(networkObject, synchronizers);
		mBasePrebinderByType.AddRpcCallersTo(networkObject, rpcCallers);

		prebinderInfo.AddSynchronizersTo(networkObject, synchronizers);
		prebinderInfo.AddRpcCallersTo(networkObject, rpcCallers);

		mSyncFieldsSetter(networkObject, synchronizers);
		mSyncRPCsSetter(networkObject, rpcCallers);
	}
}
