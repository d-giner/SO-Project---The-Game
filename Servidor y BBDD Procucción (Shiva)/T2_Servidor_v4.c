#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>
#include <my_global.h>

//Declaramos la variable para la exclusión mutua
pthread_mutex_t mymutex = PTHREAD_MUTEX_INITIALIZER;

typedef struct{//Clase conectados
	char nombre[50];
	int socket;
}Conectado;

typedef struct{//Lista de conectados
	int numero;
	Conectado conectados[100];
}ListaConectados;

typedef struct{
	char anfitrion[20];
	char invitado[20];
	int socketA;
	int socketI;
}TablaPartidas;

ListaConectados lista;
TablaPartidas tabla[100];

char user[20];
char password[80];
int enviarLista = 0;

// Estructura especial para almacenar resultados de consultas 
MYSQL_RES *resultado;
MYSQL_ROW row;
MYSQL *conn;

//Abrir conexión con la BBDD
void ConectarBBDD()
{
	//Creamos una conexion al servidor MYSQL 
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	//inicializar la conexion
	conn = mysql_real_connect (conn, "shiva2.upc.es","root", "mysql", "T2_gameDB",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
}

//Verificar si el usuario está en la base de datos y de ser que sí,
//hacer LOGIN (Consulta 4)
void IniciarSesion(char churro[512], int sock_conn, char buff2[512])
{
	//Extraer las dos componentes del mensaje recibido 
	//y asignarlas en las variables user y password
	char *p;
	p = strtok(churro, "/");
	strcpy (user, p);
	p = strtok( NULL, "/");
	strcpy (password, p);
	
	//Declarar las variables
	int err;
	int resp;
	char pass[80];
	char consulta[80];
	int PassCoin;
	
	// consulta SQL para obtener una tabla con todos los datos
	// de la base de datos
	sprintf (consulta,"SELECT Players.pass FROM Players WHERE Players.idPlayer='%s'",user); 
	// hacemos la consulta 
	err=mysql_query (conn, consulta); 
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	//Si el vector esta vacio, resp=1 para informar que no existe el 
	//usuario
	if (row == NULL){
		printf ("No se han obtenido datos en la consulta\n");
		resp=1;
		printf("El usuario no existe.\n");}
	//Si el vector no esta vacio, significara que existe el usuario
	//entonces se comparara la contraseña introducida con las obtenidas 
	//por la consulta
	else{
		sprintf(pass,row[0]);
		PassCoin=strcmp(password,pass);
		if (PassCoin == 0){
			resp=0;
			printf("Contraseña verificada.\n");						
			int cod = PonConectado(user,sock_conn,&lista);
			enviarLista = 1;
			if (cod == -1)
				printf("Error al añadir datos a la lista de conectados");
		}
		else{
			resp=2;
			printf("Contraseña incorrecta.\n");}
		row = mysql_fetch_row (resultado);
	}	
	
	if (resp==0)
		printf ("Resp %d User %s Password %s .\n",resp,user,password);
	//Preparamos el resultado
	sprintf(buff2,"%d",resp);
	//Enviamos el resultado
	write (sock_conn,buff2, strlen(buff2));
}

//Dame cuantos jugadores han usado cada aspecto en una partida (Consulta 1)
void AspectoCont(int idPartida, int codigo, int sock_conn, char buff2[512])
{
	printf ("Codigo: %d, Partida con ID: %d\n", codigo, idPartida);
	
	//Declarar las variables
	int err;
	int cont1 = 0;
	int cont2 = 0;
	int skinCoin;
	char consulta [200];
	char skin [4];
	
	sprintf(consulta, "SELECT Players.skin FROM Players,Games,PlayersGames WHERE Games.idGame = %d AND PlayersGames.idGame = Games.idGame AND PlayersGames.idPlayer = Players.idPlayer", idPartida);
	//hacemos la consulta 
	printf("Consulta:%s\n",consulta);
	err=mysql_query (conn, consulta); 
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	//recogemos el resultado de la consulta 
	resultado = mysql_store_result (conn); 
	row = mysql_fetch_row (resultado);
	if (row == NULL)
		printf ("No se han obtenido datos en la consulta.\n");
	else{
		while (row != NULL){
			if (row[0] != NULL){
				skinCoin = strcmp(row[0],"PRO");
				if (skinCoin == 0)
					cont1++;
				else
					cont2++;
				row = mysql_fetch_row (resultado);
			}
		}
	}
	printf("He salido del bucle.\n");
	printf ("En la partida, %d jugadores llevaban la skin PRO y %d la Noob.\n",cont1,cont2);
	//Preparamos el resultado
	sprintf(buff2,"1/%d,%d",cont1,cont2);
	//Enviamos el resultado
	write (sock_conn,buff2,strlen(buff2));			
}

//Registrar usuario (Consulta 6)
void RegistrarUsuario(char churro[512], int codigo, int sock_conn, char buff2[512])
{
	//Declarar las variables
	int err;
	char consulta [80];
	char name[20];
	char pass[22];
	int existe = 0;
	
	char *p;
	p = strtok(churro,"/");	
	strcpy(name,p);
	p = strtok( NULL, "/");
	strcpy(pass,p);
	printf ("Número de petición: %d, Usuario a registrar: %s\n", codigo, name);
	
	// consulta SQL para obtener una tabla con todos los datos
	// de la base de datos
	sprintf (consulta,"SELECT idPlayer FROM Players WHERE Players.idPlayer='%s'",name); 
	printf("Consulta:%s\n ",consulta);
	// hacemos la consulta 
	err=mysql_query (conn, consulta); 
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	if (row == NULL){
		printf ("El nombre está disponible, se inicia el registro.\n");				
	}
	else{
		existe = -1;
		printf("El nombre de usuario escogido ya está en uso.\n");				
	}
	
	if (!existe){
		sprintf (consulta,"INSERT INTO Players VALUES('%s','%s',0,0,0,'Default')",name,pass);
		printf("%s\n",consulta);
		err=mysql_query (conn, consulta); 
		if (err!=0) {
			printf ("Error al consultar datos de la base %u %s\n",
					mysql_errno(conn), mysql_error(conn));
			exit (1);
		}
	}
	
	printf ("Respuesta con código: %d.\n",existe);
	//Preparamos el resultado
	sprintf(buff2,"%d",existe);
	//Enviamos el resultado
	write (sock_conn,buff2, strlen(buff2));
}

//Dame usuarios con mas de X puntos (Consulta 2)
void DamePuntos(int puntos, int codigo, int sock_conn, char buff2[512])
{
	printf ("Codigo: %d, Puntos: %d\n", codigo, puntos);
	
	//Declaramos las variables
	int err;
	char consulta [80];
	char nombres[200]="";
	char cadena[100]="";
	//printf("Dentro de la funcion %d",puntos);
	
	// construimos la consulta SQL
	sprintf (consulta,"SELECT idPlayer FROM Players WHERE totalPoints > %d", puntos); 
	
	// hacemos la consulta 
	err=mysql_query (conn, consulta); 
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	resultado = mysql_store_result (conn); 
	row = mysql_fetch_row (resultado);
	int t=1;
	if (row == NULL){
		printf ("No se han obtenido datos en la consulta\n");
		t=0;
	}
	else{
		while(row!=NULL){
			sprintf(cadena, "%s,%s", cadena, row[0]);
			row = mysql_fetch_row (resultado);
		}
		sprintf(nombres,"%s",cadena);}

	if(t==1){
		printf ("Estos jugadores %s tienen mas de %d puntos.\n",nombres,puntos);
		//Preparamos el resultado
		sprintf(buff2,"2/%s",nombres);
		//Enviamos el resultado
		write (sock_conn,buff2, strlen(buff2));		
	}
}

//Dame cuantas partidas ha ganado X jugador (Consulta 3)
void DamePartidas(char churro[512], int codigo, int sock_conn, char buff[512], char buff2[512])
{
	//Declarar las variables
	int err;
	char consulta [80];
	char name[20];
	char victorias[22] = "";
	strcpy (name, churro);
	printf ("Codigo: %d, Nombre: %s\n", codigo, name);
	
	// consulta SQL para obtener una tabla con todos los datos
	// de la base de datos
	sprintf (consulta,"SELECT Players.numberWins FROM Players WHERE Players.idPlayer='%s'",name); 
	printf("Consulta:%s\n ",consulta);
	// hacemos la consulta 
	err=mysql_query (conn, consulta); 
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	if (row == NULL)
		printf ("No se han obtenido datos en la consulta\n");
	else{				
		sprintf(victorias,row[0]);
		printf("Victorias:%s",victorias);				
	}
	
	printf ("El jugador %s tiene %s victorias.\n",name,victorias);
	//Preparamos el resultado
	sprintf(buff2,"3/%s",victorias);
	//Enviamos el resultado
	write (sock_conn,buff2, strlen(buff2));
}

//Añade los usuarios conectados a la lista de conectados
//Retorna 0 si ok. -1 si la lista está llena
int PonConectado(char nombre[50], int socket, ListaConectados*lista)
{
	if (lista->numero<100)
	{
		printf("Nombre a añadir a la lista de conectados: %s\n",nombre);
		printf("Socket a añadir a la lista de conectados: %d\n",socket);
		lista->conectados[lista->numero].socket = socket;
		strcpy(lista->conectados[lista->numero].nombre, nombre);
		printf("%s se acaba de conectar.\n",lista->conectados[lista->numero].nombre);
		printf("Socket: %d\n",lista->conectados[lista->numero].socket);
		lista->numero++;
		printf("Numero de jugadores conectados: %d\n",lista->numero);		
		return 0;
	}
	else
		return -1;
}

//Elimina de la lista de conectados a los usuarios que se desconectan
//Retorna 0 si lo ha eliminado bien
void EliminarConectado(ListaConectados*lista,char nombre[50])
{
	int i = 0;
	int encontrado = 0;
	
	while(i<lista->numero && !encontrado)
	{
		if (strcmp(nombre,lista->conectados[i].nombre)==0){
			encontrado = 1;
			printf("%s se ha desconectado.\n",nombre,lista->conectados[i].nombre);
		}
		else
			i++;
	}
	if (encontrado == 1){
		while (i < lista->numero)
		{
			lista->conectados[i] = lista->conectados[i+1];
			i++;
		}
		lista->numero--;
		printf("Numero de jugadores conectados: %d\n",lista->numero);
	}
}

void DameConectados(ListaConectados *lista,char conectados[100])
{
	int i;
	sprintf(conectados,"5/%d",lista->numero);
	for(i=0;i<lista->numero;i++)
	{
		sprintf(conectados,"%s,%s,%d",conectados,lista->conectados[i].nombre,lista->conectados[i].socket);
	}
	printf("Se envía la lista de conectados: %s\n",conectados);
}

int DameSocket(ListaConectados *lista,char invitado[20])
{
	int i = 0;
	int encontrado = 0;
	while(i<lista->numero && !encontrado)
	{
		if (strcmp(invitado,lista->conectados[i].nombre)==0){
			encontrado = 1;
			return lista->conectados[i].socket;
		}
		else
			i++;
	}
}

int CrearPartida(char anfitrion[20],char invitado[20], int socketA, int socketI, TablaPartidas tabla[100])
{
	int encontrado = 0;
	int i=0;
	
	while(i<100 && encontrado==0){
		char empty[5] = "\0";
		if(strcmp(tabla[i].anfitrion,empty)==0 ){
			encontrado=1;
			strcpy(tabla[i].anfitrion,anfitrion);
			strcpy(tabla[i].invitado,invitado);
			tabla[i].socketA = socketA;
			tabla[i].socketI = socketI;
			printf("Se ha creado partida con: \n-ID: %d \n-Anfitrión: %s \n-Invitado: %s\n",i,tabla[i].anfitrion,tabla[i].invitado);
		}
		else
			i++;
	}
	return i;
}

void EliminarPartida(int idPartida, TablaPartidas tabla[100])
{	
	strcpy(tabla[idPartida].anfitrion,"\0");
	strcpy(tabla[idPartida].invitado,"\0");
	tabla[idPartida].socketA = "\0";
	tabla[idPartida].socketI = "\0";
}

void *AtenderCliente (void *socket)
{
	int sock_conn;
	int *s;
	s= (int *) socket;
	sock_conn= *s;
	int ret;
	//Declaramos variables
	char buff[512];
	char buff2[512] = "";
	char churro[512] = "";
	int terminar = 0;
	int i;
	//InicializarIds(tabla);
	// Atenderemos hasta 100 peticiones
	while (terminar ==0){
		printf ("He recibido conexión.\n");
		
		// Ahora recibimos su nombre, que dejamos en buff
		ret=read(sock_conn,buff, sizeof(buff));
		printf ("Recibido.\n");
		
		// Tenemos que a?adirle la marca de fin de string 
		// para que no escriba lo que hay despues en el buffer
		buff[ret]='\0';
		
		//Escribimos el nombre en la consola
		printf ("Se ha recibido la petición: %s\n",buff);
		//Extraer el codigo de consulta (0,1,2,3) del mensaje para poder 
		//identificar la consulta 
		char *p = strtok(buff, "/");
		int codigo =  atoi (p);
		int subcodigo;
		//El cliente se desconecta del servidor
		if (codigo == 0){
			char nombre[50] = "";
			p = strtok(NULL,"/");
			strcpy(nombre,p);
			EliminarConectado(&lista,nombre);
			enviarLista = 1;
			terminar = 1;
			// cerrar la conexion con el servidor MYSQL
			if (lista.numero == 0)
			mysql_close (conn);
		}		
		//Consulta nº 1, saber cuantos jugadores han usado la misma skin en una
		//partida concreta
		if (codigo == 1){
			//Extraer la componente del identificador de partida del mensaje
			p = strtok( NULL, "/");
			int idPartida = atoi (p);
			AspectoCont(idPartida,codigo,sock_conn,buff2);
		}
		//Consulta nº 2, saber el nombre de los jugadores que tengan más de X
		//puntos que han sido introducidos por el teclado
		if (codigo == 2){
			//Extraer la componente del numero de puntos del mensaje
			p = strtok( NULL, "/");
			int puntos = atoi (p);
			DamePuntos(puntos,codigo,sock_conn,buff2);
		}	
		
		//Consulta nº 3, saber el número de partidas ganadas por el jugador cuyo
		//nombre ha sido introducido por el teclado
		if (codigo == 3){
			p = strtok( NULL, "/");
			strcpy(churro,p);
			printf("%s",churro);
			DamePartidas(churro,codigo,sock_conn,buff,buff2);
		}
		//Consulta nº4, verificar la existencia del usuario y LOGIN
		if (codigo == 4){
			p = strtok(NULL,"NULL");
			strcpy(churro,p);
			printf("%s",churro);
			ConectarBBDD();
			IniciarSesion(churro,sock_conn,buff2);
		}
		//Registrar a un nuevo usuario
		if (codigo == 6){
			//Extraer la componente del nuombre del jugador del mensaje
			p = strtok( NULL, "NULL");
			strcpy(churro,p);
			//Abrimos conexión con la BBDD
			ConectarBBDD();
			RegistrarUsuario(churro,codigo,sock_conn,buff2);			
		}
		//Enviar invitacion
		if (codigo == 7){
			p = strtok(NULL,"/");
			subcodigo = atoi(p);
			char anfitrion[20];
			char nombreInvitado[20];
			int socketA;
			int socketI;
			int idPartida;
			if (subcodigo == 0){
				p = strtok(NULL,"/");
				strcpy(anfitrion,p);
				p = strtok(NULL,"/");
				strcpy(nombreInvitado,p);
				int socketA = DameSocket(&lista,anfitrion);
				int socketI = DameSocket(&lista,nombreInvitado);
				idPartida = CrearPartida(anfitrion,nombreInvitado,socketA,socketI,tabla);
				printf("Id partida: %d El anfitrion %s envía invitación a: %s\n",idPartida,anfitrion,nombreInvitado);
				sprintf(buff2,"7/0$%d,%s",idPartida,anfitrion);
				//int sock_aux = DameSocket(&lista,nombreInvitado);
				write (socketI,buff2,strlen(buff2));
			}
			if (subcodigo == 1){
				p = strtok(NULL,"/");
				idPartida = atoi(p);
				printf("El jugador %s, ha aceptado jugar con %s. ID partida: %d.\n",tabla[idPartida].invitado,tabla[idPartida].anfitrion,idPartida);
				sprintf(buff2,"7/1$%d,%s",idPartida,tabla[idPartida].invitado);
				write (tabla[idPartida].socketA,buff2,strlen(buff2));				
			}
			if (subcodigo == 2){
				p = strtok(NULL,"/");
				idPartida = atoi(p);
				printf("El jugador %s, ha rechazado jugar con %s. ID partida: %d.\n",tabla[idPartida].invitado,tabla[idPartida].anfitrion,idPartida);
				sprintf(buff2,"7/2$%s",tabla[idPartida].invitado);
				write (tabla[idPartida].socketA,buff2,strlen(buff2));
				EliminarPartida(idPartida,tabla);	
			}
		}
		//Chat 
		if(codigo==8){
			char jugadorEnvia[20];
			char mensajeEnvia[512];
			p = strtok(NULL,"/");
			int idP = atoi(p);
			p = strtok(NULL,"/");
			strcpy(jugadorEnvia,p);
			p = strtok(NULL,"NULL");
			strcpy(mensajeEnvia,p);
			sprintf(buff2,"8/%s$%s",jugadorEnvia,mensajeEnvia);
			write (tabla[idP].socketA,buff2,strlen(buff2));
			write (tabla[idP].socketI,buff2,strlen(buff2));			
		}
			
		//Para ver la lista de los conectados. Esto usa código 5
		if (enviarLista == 1){
			char conectados[100];
			DameConectados(&lista,conectados);
			//Preparamos el resultado
			sprintf(buff2,"%s",conectados);			
			//Enviamos el resultado
			int j;
			//Envia la lista de conectados a los clientes conectados
			for (j=0;j<lista.numero;j++){
				sock_conn = lista.conectados[j].socket;
				write (sock_conn,buff2, strlen(buff2));
			}
			enviarLista = 0; //Seguro para que solo envíe la lista cuando haya modificaciones en ella
		}
	}
}

int main(int argc, char *argv[])
{	
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;	
	
	// INICIALITZACIONS
	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
	// Fem el bind al port	
	
	memset(&serv_adr, 0, sizeof(serv_adr));// inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	// asocia el socket a cualquiera de las IP de la m?quina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	// establecemos el puerto de escucha
	serv_adr.sin_port = htons(50053);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0){
		printf ("Error de socket...\n");
	}
	else{	
	if (listen(sock_listen, 3) < 0)
		printf("Error en el Listen");
	
	int i;
	int sockets[100];
	pthread_t thread;
	i=0;
	
	// Bucle para atender a 5 clientes
	for (;;){
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexion\n");
		
		sockets[i] =sock_conn;
		//sock_conn es el socket que usaremos para este cliente
		
		// Crear thread y decirle lo que tiene que hacer		
		pthread_create (&thread, NULL, AtenderCliente,&sockets[i]);
		i=i+1;		
	}
	}
}

