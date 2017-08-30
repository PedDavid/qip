create table dbo.Preferences(
	id VARCHAR(128) not null,
	favorites varchar(max) null,
	penColors varchar(max) null, --column to save new pen colors that user add	
	constraint pk_user primary key (id),
)