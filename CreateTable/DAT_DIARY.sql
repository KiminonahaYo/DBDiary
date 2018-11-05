Drop Table DAT_DIARY;

Create Table DAT_DIARY
(
	SEQ_NO				DECIMAL(10)	not null,
	WRITE_DATETIME		DATETIME	not null,
	VALUE				TEXT,
	INS_TIME			DATETIME	not null,
	UPD_TIME			DATETIME
);
