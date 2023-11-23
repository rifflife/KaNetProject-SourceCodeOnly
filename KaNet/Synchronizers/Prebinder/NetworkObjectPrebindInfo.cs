using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

using KaNet;

namespace KaNet.Synchronizers.Prebinder
{
	public class NetworkObjectPrebindInfo
	{
		private Dictionary<string, InstanceMethodInfo> mRpcMethodInfoByName = new();
		private List<SyncVarInfo> mSyncVarInfos = new();
		private List<RpcCallerInfo> mRpcCallerInfos = new();

		public NetworkObjectPrebindInfo(Type type, bool ignoreBaseType = false)
		{
			// Rpc method name buffer
			List<string> rpcMethodNameBuffer = new List<string>();

			// Get Synchronizer, RpcCaller fields
			List<FieldInfo> fields = new List<FieldInfo>();

			var parentsType = type;

			while (true)
			{
				fields.AddRange(parentsType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance));

				if (ignoreBaseType)
				{
					break;
				}
				if (parentsType.BaseType == typeof(NetworkObject))
				{
					break;
				}

				if (parentsType.BaseType == typeof(object))
				{
					Ulog.LogError(UlogType.NetworkReflection, $"This type is not child of NetworkObject! Type : {type}");
					Debug.Assert(false);
					return;
				}

				parentsType = parentsType.BaseType;
			}

			foreach (var field in fields)
			{
				var attributes = field.GetCustomAttributes();
				string fieldName = field.Name;

				foreach (var attribute in attributes)
				{
					if (attribute is SyncVarAttribute)
					{
						var att = attribute as SyncVarAttribute;
						SyncVarInfo syncVarInfo = new(field.GetValue, fieldName, att.Type, att.Authority);
						mSyncVarInfos.Add(syncVarInfo);
						break;
					}
					else if (attribute is RpcCallAttribute)
					{
						var att = attribute as RpcCallAttribute;

						string methodName = "";

						if (fieldName.Length <= KaNetGlobal.RPC_PREFIX.Length)
						{
							throw new WrongRpcCallerName(KaNetGlobal.RPC_PREFIX.Length + 1);
						}

						if (fieldName.Substring(0, KaNetGlobal.RPC_PREFIX.Length) == KaNetGlobal.RPC_PREFIX)
						{
							methodName = fieldName.Substring(KaNetGlobal.RPC_PREFIX.Length);
						}
						else
						{
							throw new WrongRpcCallerName(fieldName);
						}

						RpcCallerInfo rpcCallerInfo = new(field.GetValue, methodName, att.Type, att.Authority);
						mRpcCallerInfos.Add(rpcCallerInfo);
						rpcMethodNameBuffer.TryAddUnique(methodName);
						break;
					}
				}
			}

			// Get methods for RpcCallers
			foreach (string methodName in rpcMethodNameBuffer)
			{
				try
				{
					var methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
					mRpcMethodInfoByName.TryAddUniqueByKey(methodName, new InstanceMethodInfo(methodInfo));
				}
				catch
				{
					throw new CannotFoundRpcMethod(type, methodName);
				}
			}
		}

		public void AddSynchronizersTo(object instance, List<Synchronizer> synchronizers)
		{
			foreach (var syncVarInfo in mSyncVarInfos)
			{
				try
				{
					synchronizers.Add(syncVarInfo.GetSyncVar(instance));
				}
				catch
				{
					throw new CannotFoundField(instance.GetType(), syncVarInfo.FieldName);
				}
			}
		}

		public void AddRpcCallersTo(object instance, List<RpcBase> rpcCallers)
		{
			foreach (var rpcCallerInfo in mRpcCallerInfos)
			{
				if (!mRpcMethodInfoByName.TryGetValue(rpcCallerInfo.RpcMethodName, out var methodInfo))
				{
					throw new CannotFoundRpcMethod(instance.GetType(), rpcCallerInfo.RpcMethodName);
				}

				object methodInstance = null;

				try
				{
					methodInstance = methodInfo.GetMethodInstance(instance);
				}
				catch
				{
					throw new CannotFoundRpcMethod(instance.GetType(), rpcCallerInfo.RpcMethodName);
				}

				try
				{
					rpcCallers.Add(rpcCallerInfo.GetRpcCaller(instance, methodInstance));
				}
				catch
				{
					throw new CannotFoundField
					(
						instance.GetType(),
						KaNetGlobal.RPC_PREFIX + rpcCallerInfo.RpcMethodName
					);
				}
			}
		}
	}
}
