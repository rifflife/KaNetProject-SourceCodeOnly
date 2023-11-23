namespace KaNet.Core
{
	public enum PacketHeaderType : byte
    {
        NONE = 0,

        // Connection 
        REQ_CONNECT = 1,
        ACK_CALLBACK = 2,
        REQ_DISCONNECT = 3,

        // Heartbeat
        REQ_HEARTBEAT = 4,
        //ACK_HEARTBEAT = 5,

        // Synchronization
        REQ_OBJ_SYN = 20,
        SYN_OBJ_LIFE = 23,
        SYN_OBJ_FIELD = 25,
        SYN_OBJ_RPC = 26,
    }
}
