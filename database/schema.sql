CREATE DATABASE NimDB;
GO
USE NimDB;
GO

CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    SenhaHash NVARCHAR(255) NOT NULL,
    DataCadastro DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Partidas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Jogador1Id INT NOT NULL,
    Jogador2Id INT NULL, -- NULL se contra IA
    VencedorId INT NULL,
    EstadoJogo NVARCHAR(MAX) NOT NULL, -- JSON do estado atual [palitos por coluna]
    TurnoAtualId INT NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'EmAndamento', -- EmAndamento, Finalizada
    ModoJogo VARCHAR(20) NOT NULL, -- PvP ou PvE
    DataInicio DATETIME DEFAULT GETDATE(),
    DataFim DATETIME NULL,

    CONSTRAINT FK_Partidas_Jogador1 FOREIGN KEY (Jogador1Id) REFERENCES Usuarios(Id),
    CONSTRAINT FK_Partidas_Jogador2 FOREIGN KEY (Jogador2Id) REFERENCES Usuarios(Id),
    CONSTRAINT FK_Partidas_Vencedor FOREIGN KEY (VencedorId) REFERENCES Usuarios(Id),
    CONSTRAINT FK_Partidas_TurnoAtual FOREIGN KEY (TurnoAtualId) REFERENCES Usuarios(Id)
);
GO


CREATE TABLE Jogadas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PartidaId INT NOT NULL,
    JogadorId INT NOT NULL,
    ColunaEscolhida INT NOT NULL,
    PalitosRemovidos INT NOT NULL,
    Momento DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_Jogadas_Partida FOREIGN KEY (PartidaId) REFERENCES Partidas(Id),
    CONSTRAINT FK_Jogadas_Jogador FOREIGN KEY (JogadorId) REFERENCES Usuarios(Id)
);
GO

CREATE TABLE Estatisticas (
    UsuarioId INT PRIMARY KEY,
    PartidasJogadas INT DEFAULT 0,
    Vitorias INT DEFAULT 0,
    Derrotas INT DEFAULT 0,

    CONSTRAINT FK_Estatisticas_Usuario FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);
GO


CREATE INDEX IDX_Partidas_Status ON Partidas(Status);
CREATE INDEX IDX_Jogadas_Partida ON Jogadas(PartidaId);
CREATE INDEX IDX_Jogadas_Jogador ON Jogadas(JogadorId);
GO


CREATE PROCEDURE CriarPartida
    @Jogador1Id INT,
    @Jogador2Id INT = NULL,
    @EstadoInicial NVARCHAR(MAX), -- JSON da configuração inicial do jogo
    @ModoJogo VARCHAR(20) = 'PvP',
    @PartidaId INT OUTPUT
AS
BEGIN
    INSERT INTO Partidas (Jogador1Id, Jogador2Id, EstadoJogo, TurnoAtualId, Status, ModoJogo)
    VALUES (@Jogador1Id, @Jogador2Id, @EstadoInicial, @Jogador1Id, 'EmAndamento', @ModoJogo);

    SET @PartidaId = SCOPE_IDENTITY();
END
GO

-- Procedure para registrar uma jogada
CREATE PROCEDURE RegistrarJogada
    @PartidaId INT,
    @JogadorId INT,
    @Coluna INT,
    @PalitosRemovidos INT,
    @NovoEstado NVARCHAR(MAX),
    @TurnoAtualNovo INT,
    @Status VARCHAR(20) = 'EmAndamento',
    @VencedorId INT = NULL
AS
BEGIN
    -- Inserir jogada
    INSERT INTO Jogadas (PartidaId, JogadorId, ColunaEscolhida, PalitosRemovidos)
    VALUES (@PartidaId, @JogadorId, @Coluna, @PalitosRemovidos);

    -- Atualizar partida
    UPDATE Partidas
    SET EstadoJogo = @NovoEstado,
        TurnoAtualId = @TurnoAtualNovo,
        Status = @Status,
        VencedorId = @VencedorId,
        DataFim = CASE WHEN @Status = 'Finalizada' THEN GETDATE() ELSE NULL END
    WHERE Id = @PartidaId;
END
GO

-- Trigger para atualizar estatísticas ao finalizar uma partida
CREATE TRIGGER AtualizarEstatisticasAoFinalizarPartida
ON Partidas
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Atualizar só se status mudou para Finalizada
    IF EXISTS (SELECT 1 FROM inserted i JOIN deleted d ON i.Id = d.Id WHERE i.Status = 'Finalizada' AND d.Status <> 'Finalizada')
    BEGIN
        DECLARE @PartidaId INT, @Jogador1 INT, @Jogador2 INT, @Vencedor INT;

        SELECT @PartidaId = i.Id, @Jogador1 = i.Jogador1Id, @Jogador2 = i.Jogador2Id, @Vencedor = i.VencedorId
        FROM inserted i WHERE i.Id IN (SELECT Id FROM inserted);

        -- Atualizar partidas jogadas
        MERGE Estatisticas AS target
        USING (VALUES (@Jogador1)) AS source(UsuarioId)
        ON target.UsuarioId = source.UsuarioId
        WHEN MATCHED THEN
            UPDATE SET PartidasJogadas = PartidasJogadas + 1
        WHEN NOT MATCHED THEN
            INSERT (UsuarioId, PartidasJogadas, Vitorias, Derrotas)
            VALUES (@Jogador1, 1, 0, 0);

        IF @Jogador2 IS NOT NULL
        BEGIN
            MERGE Estatisticas AS target
            USING (VALUES (@Jogador2)) AS source(UsuarioId)
            ON target.UsuarioId = source.UsuarioId
            WHEN MATCHED THEN
                UPDATE SET PartidasJogadas = PartidasJogadas + 1
            WHEN NOT MATCHED THEN
                INSERT (UsuarioId, PartidasJogadas, Vitorias, Derrotas)
                VALUES (@Jogador2, 1, 0, 0);
        END

        -- Atualizar vitorias e derrotas
        IF @Vencedor IS NOT NULL
        BEGIN
            -- Vitoria
            MERGE Estatisticas AS target
            USING (VALUES (@Vencedor)) AS source(UsuarioId)
            ON target.UsuarioId = source.UsuarioId
            WHEN MATCHED THEN
                UPDATE SET Vitorias = Vitorias + 1
            WHEN NOT MATCHED THEN
                INSERT (UsuarioId, PartidasJogadas, Vitorias, Derrotas)
                VALUES (@Vencedor, 1, 1, 0);

            -- Derrota para o adversário
            DECLARE @Perdedor INT;

            IF @Vencedor = @Jogador1 SET @Perdedor = @Jogador2;
            ELSE IF @Vencedor = @Jogador2 SET @Perdedor = @Jogador1;

            IF @Perdedor IS NOT NULL
            BEGIN
                MERGE Estatisticas AS target
                USING (VALUES (@Perdedor)) AS source(UsuarioId)
                ON target.UsuarioId = source.UsuarioId
                WHEN MATCHED THEN
                    UPDATE SET Derrotas = Derrotas + 1
                WHEN NOT MATCHED THEN
                    INSERT (UsuarioId, PartidasJogadas, Vitorias, Derrotas)
                    VALUES (@Perdedor, 1, 0, 1);
            END
        END
    END
END
GO

-- View para ranking de jogadores
CREATE VIEW RankingJogadores AS
SELECT
    u.Id AS UsuarioId,
    u.Nome,
    ISNULL(e.PartidasJogadas, 0) AS PartidasJogadas,
    ISNULL(e.Vitorias, 0) AS Vitorias,
    ISNULL(e.Derrotas, 0) AS Derrotas,
    CASE WHEN ISNULL(e.PartidasJogadas, 0) > 0
         THEN CAST(100.0 * ISNULL(e.Vitorias, 0) / e.PartidasJogadas AS DECIMAL(5,2))
         ELSE 0 END AS PercentualVitorias
FROM Usuarios u
LEFT JOIN Estatisticas e ON u.Id = e.UsuarioId
ORDER BY PercentualVitorias DESC, Vitorias DESC;
GO
