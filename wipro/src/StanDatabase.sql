DROP DATABASE IF EXISTS StanDB;
CREATE DATABASE StanDB;
USE StanDB;

-- https://www.w3schools.com/mysql/mysql_datatypes.asp
CREATE TABLE DiscordCategories (
    DiscordCategoryId INT AUTO_INCREMENT NOT NULL,
    DiscordCategoryName VARCHAR(255) NOT NULL,
    PRIMARY KEY (DiscordCategoryId)
);

CREATE TABLE DiscordRoles (
    DiscordRoleId INT AUTO_INCREMENT NOT NULL,
    DiscordRoleName VARCHAR(255) NOT NULL,
    PRIMARY KEY (DiscordRoleId)
);

CREATE TABLE Houses (
    HouseId INT AUTO_INCREMENT NOT NULL,
    HouseName VARCHAR(255) NOT NULL,
    FkDiscordRoleId INT,
    PRIMARY KEY (HouseId),
    FOREIGN KEY (FkDiscordRoleId) REFERENCES DiscordRoles(DiscordRoleId)
);

CREATE TABLE Modules (
    ModuleId INT AUTO_INCREMENT NOT NULL,
    ChannelName VARCHAR(255) NOT NULL,
    FullModuleName VARCHAR(255),
    FkDiscordCategoryId INT,
    PRIMARY KEY (ModuleId),
    FOREIGN KEY (FkDiscordCategoryId) REFERENCES DiscordCategories(DiscordCategoryId)
);

CREATE TABLE Students (
    StudentId INT AUTO_INCREMENT NOT NULL,
    StudentEmail VARCHAR(255) NOT NULL,
    StillStudying BOOL NOT NULL,
    Semester TINYINT NOT NULL,
    FkHouseId INT NOT NULL,
    IsDiscordAdmin BOOL NOT NULL,
    PRIMARY KEY (StudentId),
    FOREIGN KEY (FkHouseId) REFERENCES Houses(HouseId)
);

CREATE TABLE DiscordAccounts (
    DiscordAccountId INT AUTO_INCREMENT NOT NULL,
    Username VARCHAR(255),
    AccountId INT,
    VerifiedDate DATETIME,
	FkStudentId INT,
    PRIMARY KEY (DiscordAccountId),
	FOREIGN KEY (FkStudentId) REFERENCES Students(StudentId)
);

CREATE TABLE DiscordAccountsModules (
    CreationDate DATETIME NOT NULL,
    FkDiscordAccountId INT,
    FkModuleId INT,
    -- https://stackoverflow.com/questions/5835978/how-to-properly-create-composite-primary-keys-mysql
    PRIMARY KEY (FkDiscordAccountId, FkModuleId),
    FOREIGN KEY (FkDiscordAccountId) REFERENCES DiscordAccounts(DiscordAccountId),
    FOREIGN KEY (FkModuleId) REFERENCES Modules(ModuleId)
);

CREATE TABLE DiscordAccountsDiscordRoles (
    FkDiscordAccountId INT NOT NULL,
    FkDiscordRoleId INT NOT NULL,
	PRIMARY KEY (FkDiscordAccountId, FkDiscordRoleId),
    FOREIGN KEY (FkDiscordAccountId) REFERENCES DiscordAccounts(DiscordAccountId),
    FOREIGN KEY (FkDiscordRoleId) REFERENCES DiscordRoles(DiscordRoleId)
);

-- Add houses
-- Roles must have the same names as the ones created in Discord
INSERT INTO DiscordRoles (DiscordRoleName)
VALUES
	("student"),
    ("House_Grey"),
    ("House_Yellow"),
    ("House_Blue"),
    ("House_Purple"),
    ("House_Orange"),
    ("House_Red");

INSERT INTO Houses (HouseName, FkDiscordRoleId)
VALUES
    ("GREY", (SELECT DiscordRoleId FROM DiscordRoles WHERE DiscordRoleName = "House_Grey")),
    ("Yellow", (SELECT DiscordRoleId FROM DiscordRoles WHERE DiscordRoleName = "House_Yellow")),
    ("BLUE", (SELECT DiscordRoleId FROM DiscordRoles WHERE DiscordRoleName = "House_Blue")),
    ("PURPLE", (SELECT DiscordRoleId FROM DiscordRoles WHERE DiscordRoleName = "House_Purple")),
    ("ORANGE", (SELECT DiscordRoleId FROM DiscordRoles WHERE DiscordRoleName = "House_Orange")),
    ("RED", (SELECT DiscordRoleId FROM DiscordRoles WHERE DiscordRoleName = "House_Red"));
