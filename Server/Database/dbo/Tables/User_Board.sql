create table dbo.User_Board(
	userId varchar(128) not null, -- o length pode não ser o mais adequado, não existe um valor documentado para se usar (o max não dá para ser pk)
	boardId bigint not null,
	permission tinyint default 0 not null, -- 3->owner / 2->edit / 1->view
	constraint pk_userBoard primary key (userId, boardId),
	constraint ck_userBoard_permission check (permission >= 1 AND permission <=3),
	constraint fk_userBoard_board foreign key (boardId) references dbo.Board(id)
)