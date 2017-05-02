IF(NOT EXISTS(SELECT Id FROM Users WHERE Username = 'Prarthito.Chatterjee'))
BEGIN
INSERT INTO [dbo].[Users]([TitleId],[FirstName],[LastName],[Username],[EmailAddress],[IsIETStaff],[LastActivityDate],[CreatedOn],[CreatedBy],[LastUpdatedOn],[LastUpdatedBy],[DomainName])VALUES
(1,'Prarthito','Chatterjee','Prarthito.Chatterjee','Prarthito.Chatterjee@northgate-is.com',1,getdate(),getdate(),1,getdate(),1,'RAVE-TECH.CO.IN')
END


declare @userId int

select @userId =Id from Users where username='Prarthito.Chatterjee'
IF(NOT EXISTS(SELECT Id FROM UserRoles WHERE UserId = @userId AND RoleId = (Select Id from Roles Where Name = 'IET Admin')))
BEGIN
INSERT INTO [dbo].[UserRoles]([UserId],[RoleId],[CreatedBy],[CreatedOn])VALUES(@userId,(Select Id from Roles Where Name = 'IET Admin'),1,getdate())
END

SELECT * FROM Users
