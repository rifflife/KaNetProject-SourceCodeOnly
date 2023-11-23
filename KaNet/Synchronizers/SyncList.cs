using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using KaNet.Utils;
using Sirenix.OdinInspector;
using Steamworks.ServerList;
using UnityEngine;
using UnityEngine.Analytics;

namespace KaNet.Synchronizers
{
	/// <summary>직렬화 가능한 List입니다.</summary>
	/// <typeparam name="T">List 요소의 Type입니다.</typeparam>
	public struct NetList<T> : INetworkSerializable
		where T : INetworkSerializable
	{
		public List<T> DataList;

		public NetList(IList<T> data)
		{
			DataList = new List<T>(data);
		}

		public int GetSyncDataSize()
		{
			int syncSize = NetUInt16.SYNC_SIZE;

			if (DataList == null)
			{
				return syncSize;
			}

			foreach (var item in DataList)
			{
				syncSize += item.GetSyncDataSize();
			}
			return syncSize;
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			ushort count = DataList == null ? (ushort)0 : (ushort)DataList.Count;

			writer.WriteUInt16(count);

			if (DataList == null)
			{
				return;
			}

			foreach (var item in DataList)
			{
				item.SerializeTo(writer);
			}
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			DataList = new();
			var count = reader.ReadUInt16();
			for (int i = 0; i < count; i++)
			{
				T data = default(T);
				data.DeserializeFrom(reader);
				DataList.Add(data);
			}
		}

		//#region Operations
		//public T this[int index]
		//{
		//	get => DataList[index];
		//	set => DataList[index] = value;
		//}
		//public int Count => DataList.Count;
		//public bool IsReadOnly => throw new NotImplementedException();
		//public void Add(T value) => DataList.Add(value);
		//public void Clear() => DataList.Clear();
		//public bool Contains(T value ) => DataList.Contains(value);
		//public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
		//public IEnumerator GetEnumerator() => DataList.GetEnumerator();
		//public int IndexOf(T value) => DataList.IndexOf(value);
		//public void Insert(int index, T value) => DataList.Insert(index, value);
		//public void Remove(T value) => DataList.Remove(value);
		//public void RemoveAt(int index) => DataList.RemoveAt(index);
		//#endregion
	}

	public enum CollectionSyncType : byte
	{
		None = 0,
		Initialize,
		Operation,
	}

	public enum CollectionOperation : byte
	{
		None = 0,
		Clear,
		Add,
		Remove,
		Change,
	}

	/// <summary>1차원 컬랙션에 대해서 추가 제거를 제공합니다.</summary>
	/// <typeparam name="T">데이터 타입입니다.</typeparam>
	public struct NetCollectionSyncToke<T> : INetworkSerializable
		where T : INetworkSerializable
	{
		public NetUInt8<CollectionOperation> Operation;
		public T Data;
		public NetUInt16 Index;

		public NetCollectionSyncToke(CollectionOperation operation)
		{
			Operation = operation;
			Data = default(T);
			Index = 0;
		}

		public NetCollectionSyncToke(int changeIndex, T data)
		{
			Operation = CollectionOperation.Change;
			Index = (ushort)changeIndex;
			Data = data;
		}

		public NetCollectionSyncToke(T addData)
		{
			Operation = CollectionOperation.Add;
			Data = addData;
			Index = 0;
		}

		public NetCollectionSyncToke(int removeIndex)
		{
			Operation = CollectionOperation.Remove;
			Data = default(T);
			Index = (ushort)removeIndex;
		}

		public int GetSyncDataSize()
		{
			if (Operation == CollectionOperation.Add)
			{
				return Operation.GetSyncDataSize()
					+ Data.GetSyncDataSize();
			}
			else if (Operation == CollectionOperation.Remove)
			{
				return Operation.GetSyncDataSize()
					+ Index.GetSyncDataSize();
			}
			else if (Operation == CollectionOperation.Change)
			{
				return Operation.GetSyncDataSize()
					+ Index.GetSyncDataSize()
					+ Data.GetSyncDataSize();
			}

			return 0;
		}

		public void DeserializeFrom(in NetPacketReader reader)
		{
			Operation.DeserializeFrom(reader);
			if (Operation == CollectionOperation.Add)
			{
				Data.DeserializeFrom(reader);
			}
			else if (Operation == CollectionOperation.Remove)
			{
				Index.DeserializeFrom(reader);
			}
			else if (Operation == CollectionOperation.Change)
			{
				Index.DeserializeFrom(reader);
				Data.DeserializeFrom(reader);
			}
		}

		public void SerializeTo(in NetPacketWriter writer)
		{
			Operation.SerializeTo(writer);
			if (Operation == CollectionOperation.Add)
			{
				Data.SerializeTo(writer);
			}
			else if (Operation == CollectionOperation.Remove)
			{
				Index.SerializeTo(writer);
			}
			else if (Operation == CollectionOperation.Change)
			{
				Index.SerializeTo(writer);
				Data.SerializeTo(writer);
			}
		}
	}

	[Serializable]
	public class SyncList<T> : Synchronizer, IEnumerable<T>
		where T : struct, INetworkSerializable
	{
		public event Action OnDeserialized;
		public override event Action OnChanged;
		private List<NetCollectionSyncToke<T>> mOperationBuffer = new();

		public SyncList() {}

		public SyncList(IList<T> list)
		{
			mDataList.AddRange(list);
		}

		public override bool IsDirty { get; protected set; }

		[ShowInInspector] private readonly List<T> mDataList = new();
		[ShowInInspector] public NetUInt16 Count => (ushort)mDataList.Count;

		public IReadOnlyList<T> DataList => mDataList;

		public T this[int index]
		{
			get
			{
				return mDataList[index];
			}
			set
			{
				mDataList[index] = value;
				mOperationBuffer.Add(new(index, value));
				IsDirty = true;
				this.OnChanged?.Invoke();
			}
		}

		public void Add(T data)
		{
			// 에코된 패킷으로 인해 중복 추가가 되는 문제가 있음
			mDataList.Add(data);
			mOperationBuffer.Add(new(data));
			IsDirty = true;
			this.OnChanged?.Invoke();
		}

		public void Remove(Predicate<T> find)
		{
			Remove(mDataList.FindIndex(0, mDataList.Count, find));
		}

		public void Remove(int index)
		{
			if (index < 0 || index >= mDataList.Count)
			{
				return;
			}

			mDataList.RemoveAt(index);
			mOperationBuffer.Add(new(index));
			IsDirty = true;
			this.OnChanged?.Invoke();
		}

		public bool TryFind(Predicate<T> predicate, out T data)
		{
			for (int i = 0; i < mDataList.Count; i++)
			{
				if (predicate(mDataList[i]))
				{
					data = mDataList[i];
					return true;
				}
			}

			data = default(T);
			return false;
		}

		public override void ResetDeserializeEvent()
		{
			OnDeserialized = null;
		}

		public override void ResetOnDataChangeEvent()
		{
			OnChanged = null;
		}

		protected override int getSyncDataSize()
		{
			int size = 0;

			foreach (var b in mOperationBuffer)
			{
				size += b.GetSyncDataSize();
			}

			return size;
		}

		public override void OnSeralized()
		{
			mOperationBuffer.Clear();
			IsDirty = false;
		}

		public override void DeserializeFrom(in NetPacketReader reader)
		{
			NetUInt8<CollectionSyncType> syncType = new(reader);
			NetUInt16 count = new(reader);

			if (syncType == CollectionSyncType.Initialize)
			{
				mDataList.Clear();

				for (int i = 0; i < count; i++)
				{
					T data = new();
					data.DeserializeFrom(reader);
					mDataList.Add(data);
				}
			}
			else if (syncType == CollectionSyncType.Operation)
			{
				for (int i = 0; i < count; i++)
				{
					NetCollectionSyncToke<T> token = new();
					token.DeserializeFrom(reader);

					switch (token.Operation.GetEnum())
					{
						case CollectionOperation.Clear:
							mDataList.Clear();
							break;

						case CollectionOperation.Add:
							mDataList.Add(token.Data);
							break;

						case CollectionOperation.Remove:
							mDataList.RemoveAt(token.Index);
							break;

						case CollectionOperation.Change:
							mDataList[token.Index] = token.Data;
							break;

						default:
							break;
					}
				}
			}

			OnDeserialized?.Invoke();
		}

		public override void IgnoreDeserialize(in NetPacketReader reader)
		{
			NetUInt8<CollectionSyncType> syncType = new(reader);
			NetUInt16 count = new(reader);

			if (syncType == CollectionSyncType.Initialize)
			{
				for (int i = 0; i < count; i++)
				{
					T data = new();
					data.DeserializeFrom(reader);
				}
			}
			else if (syncType == CollectionSyncType.Operation)
			{
				for (int i = 0; i < count; i++)
				{
					NetCollectionSyncToke<T> token = new();
					token.DeserializeFrom(reader);
				}
			}
		}

		protected override void internalSerializeChangedPartTo(in NetPacketWriter writer)
		{
			NetUInt8<CollectionSyncType> syncType = CollectionSyncType.Operation;
			NetUInt16 count = (ushort)mOperationBuffer.Count;

			syncType.SerializeTo(writer);
			count.SerializeTo(writer);

			foreach (var operation in mOperationBuffer)
			{
				operation.SerializeTo(writer);
			}
		}

		protected override int getEntireDataSize()
		{
			int dataSize = 1 + 2; // Header, Count

			for (int i = 0; i < mDataList.Count; i++)
			{
				dataSize += mDataList[i].GetSyncDataSize();
			}

			return dataSize;
		}

		protected override void internalSerializeEntirelyTo(in NetPacketWriter writer)
		{
			NetUInt8<CollectionSyncType> syncType = CollectionSyncType.Initialize;
			NetUInt16 count = (ushort)mDataList.Count;

			syncType.SerializeTo(writer);
			count.SerializeTo(writer);

			foreach (T data in mDataList)
			{
				data.SerializeTo(writer);
			}
		}

		public override string ToString()
		{
			return $"List<{typeof(T).Name}> Count : {Count}";
		}

		public IEnumerator<T> GetEnumerator()
		{
			return mDataList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return mDataList.GetEnumerator();
		}
	}
}
