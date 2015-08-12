namespace AldursLab.Deprec.Core.AppFramework.Operations
{
    public enum OperationStatus : byte
    {
        Unspecified = 0,
        NotStarted = 10,
        Running = 20,
        Succeeded = 30,
        Faulted = 40,
        Cancelled = 50
    }
}