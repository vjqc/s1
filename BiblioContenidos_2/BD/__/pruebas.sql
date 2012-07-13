use BiblioContenidos;

--insert into Usuarios (Avatar,ApPaterno,ApMaterno,Nombres,Ubicacion,Karma,UserId) values('','','','Admin','Potosi',0,'B37D3498-0AAE-4625-B8F2-F96C4EF418A1');

--select * from aspnet_Users;
--select * from Usuarios;

--select * from aspnet_Membership;
--select * from Categorias;
--insert into Categorias(Descripcion, Estado) Values ('Programación', 'Aceptado')
/*
update Contenidos 
set Estado='Pendiente'
where Id=4
*/
select * from Usuarios;
select * from Contenidos;
select * from Libros;
select * from Categorias;
select * from RelContenidosCategorias;
--select * from Gustas;

--delete from RelContenidosCategorias
--delete from Categorias
--delete from Gustas
--delete from Libros
--delete from Contenidos
