CREATE TABLE WeightWatchers.dbo.Weight(
	Date date NOT NULL,
	PersonId int NOT NULL,
	Lbs decimal(4, 1) NOT NULL,
	Kgs decimal(4, 1) NOT NULL,
	PRIMARY KEY (Date, PersonId),
	CONSTRAINT FK_PersonId FOREIGN KEY (PersonId)
	REFERENCES Person(PersonId)
);