-- Create tables
create table TodoItems
(
	Id int identity primary key,
	ItemName nvarchar(256) not null,
	ItemCreateDate DateTime not null
)
