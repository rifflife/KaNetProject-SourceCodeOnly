using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace KaNet.Synchronizers.Prebinder
{
	public class InstanceMethodInfo
	{
		private MethodInfo mMethodInfo;
		private Type mDeclareType;

		public InstanceMethodInfo(MethodInfo methodInfo)
		{
			mMethodInfo = methodInfo;

			var argsList = new List<Type>();
			foreach (var arg in mMethodInfo.GetParameters())
			{
				argsList.Add(arg.ParameterType);
			}

			mDeclareType = Expression.GetActionType(argsList.ToArray());
		}

		public object GetMethodInstance(object instance)
		{
			return Delegate.CreateDelegate(mDeclareType, instance, mMethodInfo);
		}
	}
}
