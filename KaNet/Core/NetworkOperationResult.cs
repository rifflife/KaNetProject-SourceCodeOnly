namespace KaNet.Core
{
    public enum NetworkOperationResult
    {
        NONE = 0,
        SUCCESS = 1,

        #region SOCKET

        // Initialize Result
        SOCKET_BIND_ERROR,
        SOCKET_WRONG_IP,
        SOCKET_WRONG_PORT,

        // Operating Result
        SOCKET_AREADY_STARTED,
        SOCKET_NOT_STARTED,
        SOCKET_DISPOSE_ERROR,

        #endregion
    }
}
