using Microsoft.AspNetCore.Identity;
using PsychAppointments_API.Auth;
using PsychAppointments_API.Service.Factories;

namespace PsychAppointmentsTests;

public class PasswordHasherTest
{
    private IHasherFactory _hasherFactory;
    private AccessUtilities _hasher;

    [SetUp]
    public void Setup()
    {
        _hasherFactory = new HasherFactory();
        _hasher = new AccessUtilities(_hasherFactory);
    }

    [Test]
    public void HashPasswordTest()
    {
        string password = "1234";
        string email = "abcd@asdf.com";
        string password2 = "1234";
        string email2 = "abcd@asdf.com";
        string hashed = _hasher.HashPassword(password, email);
        string hashed2 = _hasher.HashPassword(password2, email2);

        Assert.That(hashed != hashed2);
    }

    [Test]
    [Repeat(25)]
    public void SaltGenratorTest()
    {
        string email = "horvathv01@gmail.com";
        string email2 = "horvathv01@gmail.com";
        string salt1 = _hasher.GetSalt(email);
        string salt2 = _hasher.GetSalt(email2);
        
        Assert.That(salt1, Is.EqualTo(salt2));
    }

    [Test]
    [Repeat(100)]
    public void TryAuthorizeTest()
    {
        string password = "1234";
        string email = "abcd@asdf.com";
        string hashed = _hasher.HashPassword(password, email);
        var verificationResult = _hasher.Authenticate(email, hashed, password);
        
        Assert.That(verificationResult, Is.EqualTo(PasswordVerificationResult.Success));

    }
}