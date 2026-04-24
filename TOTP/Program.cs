using OtpNet;
using QRCoder;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();

// Here ONE SERECT KEY PER CLIENT PER USER [!!! Encrypted at Rests]

byte[] secretKey = KeyGeneration.GenerateRandomKey();
string base32SecretKey = Base32Encoding.ToString(secretKey);

const string issuer = "OtpAuthDemo";
const string user = "sunny1234@gmail.com";

app.MapGet("otp/qrcode", () =>
{
    string escapedIssuer = Uri.EscapeDataString(issuer);
    string escapedUser = Uri.EscapeDataString(user);
    string otpUri = $"""otpauth://totp/{escapedIssuer}:{escapedUser}?secret={base32SecretKey}&issuer={escapedIssuer}&digits=6&period=30""";

    using var qrGenerator = new QRCodeGenerator();
    using var qrCodeData = qrGenerator.CreateQrCode(otpUri, QRCodeGenerator.ECCLevel.Q);
    using var qrCode = new PngByteQRCode(qrCodeData);
    byte[] qrCodeImage = qrCode.GetGraphic(4);

    return Results.File(qrCodeImage, MediaTypeNames.Image.Png);
});

app.MapPost("otp/validate", (ValidateOtpRequest request) =>
{
    var totp = new Totp(secretKey);

    var isValid = totp.VerifyTotp(
        request.Code,
        out var timeStepMatched,
        VerificationWindow.RfcSpecifiedNetworkDelay);

    //var isValid = totp.VerifyTotp(
    //    request.Code,
    //    out var timeStepMatched,
    //   new VerificationWindow(previous: 0, future: 0)); // TOP is valid for Before and After 30 Seconds, P-0 and F-0 means strict 30 sec.

    Console.WriteLine("Stamp : " + timeStepMatched);
    return Results.Ok(new
    {
        isValid
    });
});

app.Run();


internal record ValidateOtpRequest(string Code)
{

}