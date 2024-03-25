namespace Core;

public enum ProcessState
{
    Received = 0,
    Incomplete = 1,
    Processed = 2,
    TombstoneReceived = 3,
    TombstoneProcessed = 4
}