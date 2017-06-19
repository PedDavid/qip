create table dbo.Board(
	id bigint identity(0,1) not null,
    [name] varchar(100) not null,
	maxDistPoints tinyint not null, --default(0) --pensar num valor!
	constraint pk_board primary key (id),
	--constraint ck_board_maxDistPoints check(maxDistPoints > ?????) -- defenir valor para ?????
)