--create database db2;
--use db2;

create table Contenidos(
	Id int,
	Titulo varchar(50),
	primary key(Id)
)
insert into Contenidos Values(1, 'Libro de PHP');
insert into Contenidos Values(2, 'Articulo HTML5');

create table Seccions(
	Id int,
	SubTitulo varchar(50),
	IdContenido int,
	primary key(Id),
	foreign key(IdContenido) references Contenidos(Id)
)
insert into Seccions Values(1, 'Seccion 1 d HTML5', 2);

create table Archivos(
	Id int,
	Nombre varchar(50),
	primary key(Id),
	IdContenido int,
	IdSeccion int,
	foreign key(IdContenido) references Contenidos(Id),
	foreign key(IdSeccion) references Seccions(Id)
)
insert into Archivos (Id,Nombre,IdContenido, IdSeccion) Values(1, 'pdf PHP', 1, null);
insert into Archivos (Id,Nombre,IdContenido, IdSeccion) Values(2, 'txt HMTML5', null, 1);

select * from Contenidos;
select * from Seccions;
select * from Archivos;