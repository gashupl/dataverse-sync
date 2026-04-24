CREATE TABLE [Identity].[RefreshToken]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UserId] INT NOT NULL,
    [Token] NVARCHAR(500) NOT NULL,
    [ExpiresAt] DATETIME NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    [RevokedAt] DATETIME NULL,
    CONSTRAINT [FK_RefreshToken_User] FOREIGN KEY ([UserId]) REFERENCES [Identity].[User]([Id]),
    CONSTRAINT [UQ_RefreshToken_Token] UNIQUE ([Token])
);