-- Borramos la BBDD en caso de que exista
DROP DATABASE IF EXISTS T2_gameDB;
CREATE DATABASE T2_gameDB; 

USE T2_gameDB; -- Conectamos con la BBDD

CREATE TABLE Players (
idPlayer VARCHAR(20) PRIMARY KEY NOT NULL,
pass VARCHAR(20) NOT NULL,
totalPoints INTEGER NOT NULL,
numberGames INTEGER NOT NULL,
numberWins INTEGER NOT NULL,
skin VARCHAR(10) NOT NULL
)ENGINE = InnoDB;

CREATE TABLE Games (
winner VARCHAR(20) NOT NULL,
maxPoints INTEGER NOT NULL,
gmode VARCHAR(20) NOT NULL,
idGame INTEGER PRIMARY KEY NOT NULL,
duration VARCHAR(10) NOT NULL
)ENGINE = InnoDB;

CREATE TABLE PlayersGames (
idPlayer VARCHAR(20) NOT NULL,
points INTEGER,
idGame INTEGER NOT NULL,
FOREIGN KEY (idPlayer) REFERENCES Players(idPlayer),
FOREIGN KEY (idGame) REFERENCES Games(idGame)
)ENGINE = InnoDB;


-- Introduzco datos en la BD
INSERT INTO Players VALUES("q","q",234,3,45,"PRO");
INSERT INTO Players VALUES("Destroyer","des123",40,52,33,"PRO");
INSERT INTO Players VALUES("xX_Killeur_Xx","killkill",510,72,10,"Noob");
INSERT INTO Players VALUES("Tiny_Nani","boom12",250,53,11,"PRO");


INSERT INTO Games(winner,maxPoints,gmode,idGame,duration) VALUES("Destroyer",5,"single",1,10);
INSERT INTO Games(winner,maxPoints,gmode,idGame,duration) VALUES("xX_Killeur_Xx",10,"single",2,15);
INSERT INTO Games(winner,maxPoints,gmode,idGame,duration) VALUES("Destroyer",5,"single",2,15);
INSERT INTO Games(winner,maxPoints,gmode,idGame,duration) VALUES("xX_Killeur_Xx",5,"single",1,10);
INSERT INTO Games(winner,maxPoints,gmode,idGame,duration) VALUES("q",5,"single",1,10);

INSERT INTO PlayersGames(idPlayer,points,idGame) VALUES("Destroyer",524,1);
INSERT INTO PlayersGames(idPlayer,points,idGame) VALUES("xX_Killeur_Xx",147,1);
INSERT INTO PlayersGames(idPlayer,points,idGame) VALUES("Tiny_Nani",78,1);
INSERT INTO PlayersGames(idPlayer,points,idGame) VALUES("Destroyer",87,2);
INSERT INTO PlayersGames(idPlayer,points,idGame) VALUES("xX_Killeur_Xx",456,2);
INSERT INTO PlayersGames(idPlayer,points,idGame) VALUES("q",132,1);
INSERT INTO PlayersGames(idPlayer,points,idGame) VALUES("q",412,2);
