create table dbo.User_Board(
	userId varchar(128) not null, -- o length pode não ser o mais adequado, não existe um valor documentado para se usar (o max não dá para ser pk)
	boardId bigint not null,
	permission tinyint default 0 not null, -- 2->owner / 1->edit / 0->view
	constraint pk_userBoard primary key (userId, boardId),
	constraint ck_userBoard_permission check (permission=0 or permission=1 or permission=2),
	constraint fk_userBoard_board foreign key (boardId) references dbo.Board(id)
)