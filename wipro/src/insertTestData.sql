use standb;
insert into students
(StudentEmail, StillStudying, Semester, FkHouseId, IsDiscordAdmin)
values
("yannis.kraemer@stud.hslu.ch", true, 6, 1, false);

insert into discordaccounts
(Username, AccountId,VerifiedDate, FkStudentId)
values
("Yannis", 4100, CURDATE(), 1);
