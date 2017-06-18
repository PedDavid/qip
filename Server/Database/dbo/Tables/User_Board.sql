create table dbo.User_Board(
	userId bigint not null,
	boardId bigint not null,
	permission tinyint default 0 not null, -- 2->owner / 1->edit / 0->view
	constraint pk_userBoard primary key (userId, boardId),
	constraint ck_userBoard_permission check (permission=0 or permission=1 or permission=2),
	constraint fk_userBoard_user foreign key (userId) references dbo.[User](id),
	constraint fk_userBoard_board foreign key (boardId) references dbo.Board(id)
)