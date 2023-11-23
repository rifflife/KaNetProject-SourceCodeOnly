using System.Collections.Generic;
using System.Text;
using KaNet.Synchronizers;
using KaNet.Utils;

namespace KaNet.Session
{
	/// <summary>로비의 콜백 구조체입니다.</summary>
	public struct NetCallback : INetworkSerializable
	{
		public NetOperationType Operation;
		public NetOperationResult Result;
		public List<object> Arguments;

		public NetCallback
		(
			NetOperationType operation, 
			NetOperationResult result = NetOperationResult.None
		)
		{
			Operation = operation;
			Result = result;
			Arguments = null;
		}

		public void AddArgument(object argument)
		{
			Arguments.Add(argument);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append($"[LobbyOperationResult : {Result}]");

			foreach (var obj in Arguments)
			{
				sb.Append(obj.ToString());
			}

			return sb.ToString();
		}

		public int GetSyncDataSize() => 2;

		public void DeserializeFrom(in NetPacketReader reader)
		{
			Operation = (NetOperationType)reader.ReadUInt8();
			Result = (NetOperationResult)reader.ReadUInt8();
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			writer.WriteUInt8((byte)Operation);
			writer.WriteUInt8((byte)Result);
		}
	}
}
