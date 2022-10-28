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
    SEMESTER TINYINT NOT NULL,
    FkHouseId INT NOT NULL,
    IsDiscordAdmin BOOL NOT NULL,
    PRIMARY KEY (StudentId),
    FOREIGN KEY (FkHouseId) REFERENCES Houses(HouseId)
);

CREATE TABLE DiscordAccounts (
    DiscordAccountId INT AUTO_INCREMENT NOT NULL,
    Username VARCHAR(255),
    AccountId INT,
    ActivationCode VARCHAR(255),
    VerifiedDate DATETIME,
    RegisterDate DATETIME,
    PRIMARY KEY (DiscordAccountId)
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
    #--PRIMARY KEY (FkDiscordAccountId, FkDiscordRoleId),
    FOREIGN KEY (FkDiscordAccountId) REFERENCES DiscordAccounts(DiscordAccountId),
    FOREIGN KEY (FkDiscordRoleId) REFERENCES DiscordRoles(DiscordRoleId)
);

-- Add houses
INSERT INTO DiscordRoles (DiscordRoleName)
VALUES
    ("HouseGrey"),
    ("HouseYellow"),
    ("HouseBlue"),
    ("HousePurple"),
    ("HouseOrange"),
    ("HouseRed");

INSERT INTO Houses (HouseName, FkDiscordRoleId)
VALUES
    ("GREY", (SELECT DiscordRoleId FROM DiscordRoles WHERE DiscordRoleName = "HouseGrey")),
    ("Yellow", (SELECT DiscordRoleId FROM DiscordRoles WHERE DiscordRoleName = "HouseYellow")),
    ("BLUE", (SELECT DiscordRoleId FROM DiscordRoles WHERE DiscordRoleName = "HouseBlue")),
    ("PURPLE", (SELECT DiscordRoleId FROM DiscordRoles WHERE DiscordRoleName = "HousePurple")),
    ("ORANGE", (SELECT DiscordRoleId FROM DiscordRoles WHERE DiscordRoleName = "HouseOrange")),
    ("RED", (SELECT DiscordRoleId FROM DiscordRoles WHERE DiscordRoleName = "HouseRed"));
