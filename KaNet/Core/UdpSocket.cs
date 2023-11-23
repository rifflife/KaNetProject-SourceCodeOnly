using KaNet.Extensions;
using KaNet.Utils;
using System;
using System.Net;
using System.Net.Sockets;
using Utils;
using Utils.Analytics;

namespace KaNet.Core
{
    public class UdpSocket : IDisposable
    {
        public bool IsStarted { get; private set; } = false;
        public int MaxMTU { get; private set; } = KaNetGlobal.DEFAULT_MTU;
        public int LocalPort { get; private set; } = 0;

        public event Action OnDisconnected;
        public event Action OnSended;
        public event Action<EndPoint, NetBuffer> OnReceived;
        public event Action<SocketError> OnError;

        private Socket mSocket;

        private SocketAsyncEventArgs mRecvEventArg;
        private SocketAsyncEventArgs mSendEventArg;
        private NetBuffer mReceiveBuffer;

        private NumericAccumulator mSendStatistics;
        private NumericAccumulator mRecvStatistics;

        public ulong TotalSend => mSendStatistics.TotalValue;
        public ulong SendPerSecond => mSendStatistics.PerSecond;
        public ulong TotalRecv => mRecvStatistics.TotalValue;
        public ulong RecvPerSecond => mSendStatistics.PerSecond;

        /// <summary>UDP 소켓을 시작합니다.</summary>
        /// <param name="port">현재 로컬에 할당할 Port번호 입니다.</param>
        /// <returns>수행 결과입니다.</returns>
        public NetworkOperationResult Start(int port, int maxMTU = KaNetGlobal.DEFAULT_MTU)
        {
            if (IsStarted)
            {
                return NetworkOperationResult.SOCKET_AREADY_STARTED;
            }

            mSendStatistics = new NumericAccumulator("UDP Send");
            mRecvStatistics = new NumericAccumulator("UDP Recv");

            mSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);

            // Setup socket options
            mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, false);

            if (mSocket.AddressFamily == AddressFamily.InterNetworkV6)
            {
                mSocket.DualMode = true;
            }

            // Allocate send/receive buffer as much as OS limits
            mSocket.SetSendBufferToOsLimit(Numeric.KiB, 100);
            mSocket.SetRecvBufferToOsLimit(Numeric.KiB, 100);

            // Try bind socket
            try
            {
                mSocket.Bind(new IPEndPoint(IPAddress.Any, port));

                var localEndPoint = mSocket.LocalEndPoint as IPEndPoint;
                LocalPort = localEndPoint.Port;
            }
            catch (Exception e)
            {
                mSocket.Close();
                Ulog.LogError(UlogType.UdpSocket, e);
                return NetworkOperationResult.SOCKET_BIND_ERROR;
            }


            // Setup event arguments
            mRecvEventArg = new SocketAsyncEventArgs();
            mRecvEventArg.RemoteEndPoint = mSocket.LocalEndPoint;
            mRecvEventArg.Completed += onAsyncOperationCompleted;

            mSendEventArg = new SocketAsyncEventArgs();
            mSendEventArg.Completed += onAsyncOperationCompleted;

            // Setup buffer
            MaxMTU = maxMTU;
            mReceiveBuffer = new NetBuffer(MaxMTU * 2);

            IsStarted = true;

            // Start to receive
            tryReceive();

            return NetworkOperationResult.SUCCESS;
        }

        /// <summary>UDP 소캣을 종료합니다.</summary>
        public NetworkOperationResult Stop()
        {
            try
            {
                tryDispose();
                return NetworkOperationResult.SUCCESS;
            }
            catch (Exception e)
            {
                Ulog.LogError(UlogType.UdpSocket, e);
                return NetworkOperationResult.SOCKET_DISPOSE_ERROR;
            }
        }

        #region Sender

        public void SendTo(byte[] data, EndPoint sendTo)
        {
            trySend(new NetBuffer(data), sendTo);
        }

        public void SendTo(NetBuffer data, EndPoint sendTo)
        {
            trySend(data, sendTo);
        }

        #endregion

        #region Async Operations

        /// <summary>소캣 비동기 Operation Callback입니다.</summary>
        private void onAsyncOperationCompleted(object sender, SocketAsyncEventArgs e)
        {
            var lastOperation = e.LastOperation;

            if (lastOperation == SocketAsyncOperation.ReceiveFrom)
            {
                onReceiveOperationCompleted(e);
                return;
            }
            else if (lastOperation == SocketAsyncOperation.SendTo)
            {
                onSendOperationCompleted(e);
                return;
            }

            Ulog.LogError(UlogType.UdpSocket, "Wrong async operation detected!");
        }

        private void trySend(NetBuffer data, EndPoint sendTo)
        {
            if (!IsStarted)
            {
                Ulog.LogError(UlogType.UdpSocket, $"trySend Error! Socket already disposed!");
                return;
            }

            if (data.IsEmpty())
            {
                Ulog.LogWarning(UlogType.UdpSocket, $"You're trying to send empty data!");
                return;
            }

            try
            {
                mSendEventArg.RemoteEndPoint = sendTo;
                mSendEventArg.SetBuffer(data.BufferData);

                if (!mSocket.SendToAsync(mSendEventArg))
                {
                    // If SendToAsync operation completed synchronously.
                    //Logger.LogStackFrame(LogType.UDP_SOCKET, "SendToAsync 함수가 동기로 실행되었습니다.");
                    //throw new Exception("SendToAsync 함수가 동기로 실행되었습니다.");

                    onSendOperationCompleted(mSendEventArg);
                }
            }
            catch (ObjectDisposedException)
            {
                Ulog.LogError(UlogType.UdpSocket, $"trySend Error! Socket already disposed!");
            }
            catch (Exception e)
            {
                Ulog.LogError(UlogType.UdpSocket, $"trySend Error! Socket error occur : {e}");
                //trySend(data, sendTo);
            }
        }

        private void onSendOperationCompleted(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                OnError?.Invoke(e.SocketError);
                return;
            }

            int sendLength = e.BytesTransferred;
            mSendStatistics.Accumulate(sendLength + KaNetGlobal.UDP_HEADER_SIZE);

            OnSended?.Invoke();
        }

        private void tryReceive()
        {
            try
            {
                mRecvEventArg.SetBuffer(mReceiveBuffer.RawBufferData, 0, mReceiveBuffer.Capacity);

                if (!mSocket.ReceiveFromAsync(mRecvEventArg))
                {
                    onReceiveOperationCompleted(mRecvEventArg);
                }
            }
            catch (ObjectDisposedException)
            {
                Ulog.LogError(UlogType.UdpSocket, $"tryReceive Error! Socket already disposed!");
            }
            catch (Exception e)
            {
                Ulog.LogError(UlogType.UdpSocket, $"tryReceive Error! Socket error occur : {e}");
                //tryReceive();
            }
        }

        private void onReceiveOperationCompleted(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                OnError?.Invoke(e.SocketError);
                return;
            }

            int receivedCount = e.BytesTransferred;
            var fromEndpoint = e.RemoteEndPoint;

            mReceiveBuffer.ForceSetSize(receivedCount);
            NetBuffer curReceiveData = new NetBuffer(mReceiveBuffer);

            OnReceived?.Invoke(fromEndpoint, curReceiveData);

            tryReceive();
        }

        #endregion

        public void Dispose()
        {
            tryDispose();
        }

        public void tryDispose()
        {
            if (IsStarted)
            {
                IsStarted = false;

                // Release socket
                mSocket?.Close();
                mSocket?.Dispose();

                // Release arguments
                mRecvEventArg?.Dispose();
                mSendEventArg?.Dispose();
            }
        }
    }
}