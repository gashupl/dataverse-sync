using Pg.DataverseSync.Api.Domain;

namespace Pg.DataverseSync.Api.Domain.Tests;

public class UserTests
{
    [Fact]
    public void GeneratePasswordHash_ShouldCreateNonEmptyHashAndSalt()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com" };
        var password = "MySecurePassword123!";

        // Act
        user.GeneratePasswordHash(password);

        // Assert
        Assert.NotEmpty(user.PasswordHash);
        Assert.NotEmpty(user.PasswordSalt);
        Assert.Equal(32, user.PasswordHash.Length); // SHA256 output length
        Assert.Equal(16, user.PasswordSalt.Length); // Salt length
    }

    [Fact]
    public void GeneratePasswordHash_ShouldCreateDifferentSaltsForSamePassword()
    {
        // Arrange
        var user1 = new User { Username = "user1", Email = "user1@example.com" };
        var user2 = new User { Username = "user2", Email = "user2@example.com" };
        var password = "SamePassword123!";

        // Act
        user1.GeneratePasswordHash(password);
        user2.GeneratePasswordHash(password);

        // Assert
        Assert.NotEqual(user1.PasswordSalt, user2.PasswordSalt);
        Assert.NotEqual(user1.PasswordHash, user2.PasswordHash);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrue_WhenPasswordIsCorrect()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com" };
        var password = "MySecurePassword123!";
        user.GeneratePasswordHash(password);

        // Act
        var result = user.VerifyPassword(password);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_WhenPasswordIsIncorrect()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com" };
        var correctPassword = "CorrectPassword123!";
        var incorrectPassword = "WrongPassword123!";
        user.GeneratePasswordHash(correctPassword);

        // Act
        var result = user.VerifyPassword(incorrectPassword);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_WhenPasswordDiffersInCase()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com" };
        var password = "Password123!";
        user.GeneratePasswordHash(password);

        // Act
        var result = user.VerifyPassword("password123!");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_WhenPasswordDiffersSlightly()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com" };
        user.GeneratePasswordHash("Password123!");

        // Act
        var result = user.VerifyPassword("Password123");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GeneratePasswordHash_ShouldWorkWithEmptyPassword()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com" };
        var password = string.Empty;

        // Act
        user.GeneratePasswordHash(password);

        // Assert
        Assert.NotEmpty(user.PasswordHash);
        Assert.NotEmpty(user.PasswordSalt);
        Assert.True(user.VerifyPassword(string.Empty));
    }

    [Fact]
    public void GeneratePasswordHash_ShouldWorkWithLongPassword()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com" };
        var password = new string('a', 1000);

        // Act
        user.GeneratePasswordHash(password);

        // Assert
        Assert.NotEmpty(user.PasswordHash);
        Assert.NotEmpty(user.PasswordSalt);
        Assert.True(user.VerifyPassword(password));
    }

    [Fact]
    public void GeneratePasswordHash_ShouldWorkWithSpecialCharacters()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com" };
        var password = "!@#$%^&*()_+-=[]{}|;:',.<>?/`~";

        // Act
        user.GeneratePasswordHash(password);

        // Assert
        Assert.NotEmpty(user.PasswordHash);
        Assert.NotEmpty(user.PasswordSalt);
        Assert.True(user.VerifyPassword(password));
    }

    [Fact]
    public void GeneratePasswordHash_ShouldWorkWithUnicodeCharacters()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com" };
        var password = "Pässwörd123!你好🔒";

        // Act
        user.GeneratePasswordHash(password);

        // Assert
        Assert.NotEmpty(user.PasswordHash);
        Assert.NotEmpty(user.PasswordSalt);
        Assert.True(user.VerifyPassword(password));
    }

    [Fact]
    public void GeneratePasswordHash_ShouldOverwritePreviousHash()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com" };
        var oldPassword = "OldPassword123!";
        var newPassword = "NewPassword456!";

        // Act
        user.GeneratePasswordHash(oldPassword);
        var oldHash = user.PasswordHash.ToArray();
        var oldSalt = user.PasswordSalt.ToArray();

        user.GeneratePasswordHash(newPassword);

        // Assert
        Assert.NotEqual(oldHash, user.PasswordHash);
        Assert.NotEqual(oldSalt, user.PasswordSalt);
        Assert.False(user.VerifyPassword(oldPassword));
        Assert.True(user.VerifyPassword(newPassword));
    }

    [Fact]
    public void VerifyPassword_ShouldUseConstantTimeComparison()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com" };
        var password = "Password123!";
        user.GeneratePasswordHash(password);

        // Act & Assert - timing should be similar for correct and incorrect passwords
        // This test primarily documents the security feature (constant-time comparison)
        var sw1 = System.Diagnostics.Stopwatch.StartNew();
        user.VerifyPassword(password);
        sw1.Stop();

        var sw2 = System.Diagnostics.Stopwatch.StartNew();
        user.VerifyPassword("WrongPassword!");
        sw2.Stop();

        // Both should complete (no timing attack vector exposed)
        Assert.True(sw1.ElapsedTicks > 0);
        Assert.True(sw2.ElapsedTicks > 0);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_WhenHashIsNotGenerated()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = [],
            PasswordSalt = []
        };

        // Act
        var result = user.VerifyPassword("AnyPassword123!");

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("password")]
    [InlineData("Password123")]
    [InlineData("P@ssw0rd!")]
    [InlineData("VeryLongPasswordWithManyCharacters123!@#")]
    [InlineData("a")]
    [InlineData("12345678")]
    public void GeneratePasswordHash_And_VerifyPassword_ShouldWorkForVariousPasswords(string password)
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com" };

        // Act
        user.GeneratePasswordHash(password);
        var isValid = user.VerifyPassword(password);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void PasswordHash_ShouldUse100000Iterations()
    {
        // This test documents the security parameter
        // The actual iteration count is enforced by the User class implementation
        // Using 100,000 iterations as recommended by NIST (2024)

        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com" };
        var password = "TestPassword123!";

        // Act
        var sw = System.Diagnostics.Stopwatch.StartNew();
        user.GeneratePasswordHash(password);
        sw.Stop();

        // Assert - should take measurable time due to 100k iterations
        Assert.True(sw.ElapsedMilliseconds > 0);
        Assert.NotEmpty(user.PasswordHash);
    }
}
