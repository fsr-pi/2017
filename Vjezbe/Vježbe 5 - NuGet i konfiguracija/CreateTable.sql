CREATE TABLE Pribavitelj (
    ID int IDENTITY (1,1) NOT NULL,
    Ime varchar(255) NOT NULL,
    Prezime varchar(255),
    Godine int,
    CONSTRAINT PK_Person PRIMARY KEY (ID)
);

CREATE TABLE Police (
    ID int IDENTITY (1,1) NOT NULL PRIMARY KEY,
    VrstaPolice nvarchar(6) NOT NULL,
	BrojPolice int NOT NULL,
	DatumPocetka date NOT NULL,
	DatumKraja date NOT NULL,
	DatumIzdavanja datetime NULL,
	Premija money NOT NULL,
	Porez decimal(10,2) NULL,
    Pribavitelj int NOT NULL,
	CONSTRAINT FK_Pribavitelj FOREIGN KEY (Pribavitelj)
    REFERENCES Pribavitelj(ID)
);

ALTER TABLE Police
  ADD Posrednik varchar(50);

ALTER TABLE Police
  ALTER COLUMN Porez money NOT NULL;

INSERT INTO dbo.Pribavitelj(Ime, Prezime, Godine) 
    VALUES ('Ivan', 'Ivaniæ', 25);

INSERT INTO dbo.Pribavitelj(Ime, Prezime, Godine) 
    VALUES ('Ante', 'Antiæ', 29);

INSERT INTO dbo.Pribavitelj(Ime, Prezime, Godine) 
    VALUES ('Milan', 'Milovanoviæ', 54);

UPDATE Pribavitelj
SET Ime = 'Alfred', Prezime= 'Schmidt', Godine = 29
WHERE ID = 1;

INSERT INTO dbo.Police(VrstaPolice,BrojPolice,DatumPocetka,DatumKraja,DatumIzdavanja,Premija,Porez,Pribavitelj)
VALUES ('01001',123456,'20170301','20180301','20170301',274.55,0,7);

INSERT INTO dbo.Police(VrstaPolice,BrojPolice,DatumPocetka,DatumKraja,DatumIzdavanja,Premija,Porez,Pribavitelj)
VALUES ('01001',123456,'20170301','20180301','20170301',274.55,0,1);

DELETE FROM Police WHERE ID = 2

--Stored procedura

--
