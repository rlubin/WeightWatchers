-- USE [master];
-- GO

CREATE DATABASE WeightWatchers;
GO

USE WeightWatchers;
GO

CREATE TABLE WeightWatchers.dbo.Person(
	PersonId int IDENTITY(1,1) NOT NULL,
	FirstName varchar(50) NOT NULL,
	LastName varchar(50) NOT NULL,
	PRIMARY KEY (PersonId),
	CONSTRAINT UC_Person UNIQUE (FirstName, LastName)
);
GO

CREATE TABLE WeightWatchers.dbo.Weight(
	Date date NOT NULL,
	PersonId int NOT NULL,
	Lbs decimal(4, 1) NOT NULL,
	Kgs decimal(4, 1) NOT NULL,
	PRIMARY KEY (Date, PersonId),
	CONSTRAINT FK_PersonId FOREIGN KEY (PersonId)
	REFERENCES Person(PersonId)
);
GO