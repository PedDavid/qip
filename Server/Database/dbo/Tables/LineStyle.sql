create table dbo.LineStyle(
	lineStyleId bigint identity(0,1) not null,
	color varchar(20) not null,
	constraint pk_linestyle primary key (lineStyleId)
)