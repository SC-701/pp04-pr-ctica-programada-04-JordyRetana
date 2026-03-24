CREATE TABLE [dbo].[UsuariosPerfiles] (
    [UsuarioId] UNIQUEIDENTIFIER NOT NULL,
    [PerfilId]  INT              NOT NULL,
    CONSTRAINT [PK_UsuariosPerfiles] PRIMARY KEY CLUSTERED ([UsuarioId] ASC, [PerfilId] ASC),
    CONSTRAINT [FK_UsuariosPerfiles_Perfiles] FOREIGN KEY ([PerfilId]) REFERENCES [dbo].[Perfiles] ([Id]),
    CONSTRAINT [FK_UsuariosPerfiles_Usuarios] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuarios] ([Id])
);

