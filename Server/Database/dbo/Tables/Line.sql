create table dbo.Line(
	figureId bigint not null,
	boardId bigint not null,
	isClosedForm bit default (0) not null,
	lineStyleId bigint not null,
	constraint pk_line primary key (figureId, boardId),
	constraint fk_line_figure foreign key (figureId, boardId) references dbo.Figure(id, boardId),
	constraint fk_line_linestyle foreign key (lineStyleId) references dbo.LineStyle(lineStyleId)
)