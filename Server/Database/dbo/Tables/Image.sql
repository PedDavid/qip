create table dbo.[Image](
	figureId bigint not null,
	boardId bigint not null,
	initPointId bigint not null,
	src nvarchar(MAX) not null,
	width int not null,
	height int not null,
	constraint pk_image primary key (figureId, boardId),
	constraint fk_image_figure foreign key (figureId, boardId) references dbo.Figure(id, boardId),
	constraint fk_image_point foreign key (initPointId) references dbo.Point(id)
)