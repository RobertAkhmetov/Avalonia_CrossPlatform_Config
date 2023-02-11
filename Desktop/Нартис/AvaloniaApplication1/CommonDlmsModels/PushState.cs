namespace CommonDlmsModels
{
    public enum PushState
    {
        Disable = 0,
        Idle = 1,
        PreparationOfMessages = 2,
        ReadyForShipment = 3,
        ReadyForResending = 4,
        Send = 5,
        WaitingExpirationOfTimeout = 6,
        SendOk = 7,
        SendError = 8
    }

}
