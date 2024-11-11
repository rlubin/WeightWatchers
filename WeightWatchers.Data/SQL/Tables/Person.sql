CREATE TABLE WeightWatchers.dbo.Person(
	PersonId int IDENTITY(1,1) NOT NULL,
	FirstName varchar(50) NOT NULL,
	LastName varchar(50) NOT NULL,
	PRIMARY KEY (PersonId),
	CONSTRAINT UC_Person UNIQUE (FirstName, LastName)
);