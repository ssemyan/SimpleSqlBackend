-- Create User
CREATE USER ApplicationUser WITH PASSWORD = 'YourStrongPassword1';
ALTER ROLE db_datareader ADD MEMBER ApplicationUser;
ALTER ROLE db_datawriter ADD MEMBER ApplicationUser;

-- Create tables
create table TodoItems
(
	Id int identity primary key,
	ItemName nvarchar(256) not null,
	ItemCreateDate DateTime not null
)
