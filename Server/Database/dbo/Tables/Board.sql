﻿create table dbo.Board(
	id bigint identity(0,1) not null, -- o length pode não ser o mais adequado, não existe um valor documentado para se usar (o max não dá para ser pk)
    [name] varchar(100) not null,
	maxDistPoints tinyint not null, --default(0) --pensar num valor!
	basePermission TINYINT DEFAULT(0) NOT null, -- 0->private, 1->read public, 2->write public
	constraint pk_board primary key (id),
	--constraint ck_board_maxDistPoints check(maxDistPoints > ?????), -- definir valor para ?????
	constraint ck_board_basePermission check (basePermission >= 0 AND basePermission <=2),
)