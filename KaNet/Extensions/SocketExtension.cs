using KaNet.Utils;
using System.Net.Sockets;

namespace KaNet.Extensions
{
    public static class SocketExtension
    {
        /// <summary>소켓의 수신 버퍼 크기를 OS의 한계치만큼 할당 받습니다. 기본적으로 1 KiB씩 증가시킵니다. 기본 최대 할당 크기 = 100 MB</summary>
        public static void SetRecvBufferToOsLimit(this Socket socket, int stepSize = Numeric.KiB, int attemptCount = Numeric.KiB * 100)
        {
            for (int i = 0; i < attemptCount; ++i)
            {
                try
                {
                    socket.ReceiveBufferSize += stepSize;
                }
                catch
                {
                    break;
                }
            }
        }

        /// <summary>소켓의 송신 버퍼 크기를 OS의 한계치만큼 할당 받습니다. 기본적으로 1 KiB씩 증가시킵니다. 기본 최대 할당 크기 = 100 MB</summary>
        public static void SetSendBufferToOsLimit(this Socket socket, int stepSize = Numeric.KiB, int attemptCount = Numeric.KiB * 100)
        {
            for (int i = 0; i < attemptCount; ++i)
            {
                try
                {
                    socket.SendBufferSize += stepSize;
                }
                catch
                {
                    break;
                }
            }
        }
    }
}
