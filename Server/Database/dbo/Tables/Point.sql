--pensar se é util ter este id
create table dbo.Point(
	id bigint identity(0,1) not null,
	x int not null,
	y int not null,
	constraint pk_point primary key (id),
	constraint ak_point_x_y unique(x,y),
	constraint ck_point_x check(x>=0),
	constraint ck_point_y check(y>=0),
)