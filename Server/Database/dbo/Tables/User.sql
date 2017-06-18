create table dbo.[User](
	id bigint identity(0,1) not null,
	username varchar(200) not null,
	pwdHash varchar(128) not null,
	pwdSalt varchar(128) not null,
	[name] varchar(200) not null,
	favorites nvarchar(max) null,
	penColors nvarchar(max) null, --column to save new pen colors that user add	
	constraint pk_user primary key (id),
	constraint ak_user unique (username)
)