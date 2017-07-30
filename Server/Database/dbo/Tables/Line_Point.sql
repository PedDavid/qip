--nota: pointStyleId está nesta tabela para evitar que hajam pontos com as mesmas coordenadas repetidas
create table dbo.Line_Point(
	figureId bigint not null,
	boardId bigint not null,
	pointId bigint not null,
	pointIdx int not null, --this allows a figure to have a point twice.
	[pointStyle] VARCHAR(MAX) not null,
	constraint pk_linePoint primary key (figureId, boardId, pointId, pointIdx),
	constraint fk_linePoint_figure foreign key (figureId, boardId) references dbo.Figure(id, boardId),
	constraint fk_linePoint_point foreign key (pointId) references dbo.Point(id)
)