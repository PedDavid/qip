create table dbo.PointStyle(
	pointStyleId bigint identity(0,1) not null,
	width int not null,
	constraint pk_pointstyle primary key (pointStyleId),
	constraint ck_pointstyle_width check (width>-1)
)