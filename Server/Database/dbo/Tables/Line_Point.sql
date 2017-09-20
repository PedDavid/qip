create table dbo.Line_Point(
	figureId bigint not null,
	boardId bigint not null,
	pointId bigint not null,
	pointIdx int not null, --this allows a figure to have a point twice.
	[pointStyleId] BIGINT not null,
	constraint pk_linePoint primary key (figureId, boardId, pointId, pointIdx),
	constraint fk_linePoint_line foreign key (figureId, boardId) references dbo.Line(figureId, boardId),
	constraint fk_linePoint_point foreign key (pointId) references dbo.Point(id),
	constraint fk_linePoint_pointStyle FOREIGN KEY (pointStyleId) REFERENCES dbo.PointStyle(id)
)