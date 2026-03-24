CREATE TABLE [dbo].[Usuarios] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [NombreUsuario]     NVARCHAR (100)   NOT NULL,
    [CorreoElectronico] NVARCHAR (200)   NOT NULL,
    [PasswordHash]      NVARCHAR (500)   NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

