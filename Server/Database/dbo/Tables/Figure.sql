create table dbo.Figure(
	id bigint not null, --não está a gerar automaticamente para que seja controlado pela a app. Note que o id é usado para decidir a ordem das figuras
    figureType varchar(5) not null,
	boardId bigint not null, 
	constraint pk_figure primary key (id, boardId),
	constraint ck_figure check(figureType='image' or figureType='line'),
	constraint fk_figure_board foreign key (boardId) references dbo.Board(id)
)