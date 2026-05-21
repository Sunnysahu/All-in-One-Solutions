namespace Web_Hook.Services.Interfaces
{
    public interface ISignatureService
    {
        bool IsValid(string payload, string incomingSignature);
    }
}
