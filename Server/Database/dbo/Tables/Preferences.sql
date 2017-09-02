create table dbo.Preferences(
	id VARCHAR(128) not null,
	favorites varchar(max) null,
	penColors varchar(max) null, --column to save new pen colors that user add	
	defaultPen VARCHAR(256) null,
	defaultEraser VARCHAR(256) null,
	currTool VARCHAR(256) null,
	settings VARCHAR(256) null,
	constraint pk_user primary key (id),
)