#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>

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

typedef struct{//Tabla de partidas
	char anfitrion[20];
	char invitado1[20];
	char invitado2[20];
	char invitado3[20];
	int socketA;
	int socketI1;
	int socketI2;
	int socketI3;
	int contadorAceptados;
}TablaPartidas;

//Declaración de las variables 
ListaConectados lista;
TablaPartidas tabla[100];

char user[20];
char password[80];
int enviarLista = 0;
int contadorAceptados = 0;
int contadorNotificacionGanador = 0;

// Estructura especial para almacenar resultados de consultas 
MYSQL_RES *resultado;
MYSQL_ROW row;
MYSQL *conn;

void ConectarBBDD()//Abrir conexión con la BBDD
{
	//Creamos una conexion al servidor MYSQL 
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	//Inicializar la conexión
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "gameDB",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
}


void IniciarSesion(char churro[512], int sock_conn, char buff2[512])//Verificar si el usuario está en la base de datos y de ser así, hacer LOGIN (Consulta 4)
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
	int encontrado = 0;
	int i = 0; 
	

	while(i < lista.numero){
		if (strcmp(user,lista.conectados[i].nombre) == 0){
			encontrado = 1;
		}
		i++;
	}
	if (encontrado == 1){
		resp=3;
	}
	else{		
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
				printf("Contraseña verificada. user: %s\n",user);						
				int cod = PonConectado(user,sock_conn,&lista);
				enviarLista = 1;
				if (cod == -1)
					printf("Error al añadir datos a la lista de conectados\n");
			}
			else{
				resp=2;
				printf("Contraseña incorrecta.\n");
			}
			row = mysql_fetch_row (resultado);
		}	
		
		if (resp==0)
			printf ("Resp %d User %s Password %s .\n",resp,user,password);
	}
	//Preparamos el resultado
	sprintf(buff2,"%d",resp);
	//Enviamos el resultado
	write (sock_conn,buff2, strlen(buff2));
}


void AspectoCont(int idPartida, int codigo, int sock_conn, char buff2[512])//Dame cuantos jugadores han usado cada aspecto en una partida (Consulta 1)
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
		sprintf(buff2,"1/-1");
		exit (1);
	}
	//recogemos el resultado de la consulta 
	resultado = mysql_store_result (conn); 
	row = mysql_fetch_row (resultado);
	if (row == NULL)
	{
		printf ("No se han obtenido datos en la consulta.\n");
		sprintf(buff2,"1/0");
	}
	
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
		printf ("En la partida, %d jugadores llevaban la skin PRO y %d la Noob.\n",cont1,cont2);
		//Preparamos el resultado
		sprintf(buff2,"1/%d,%d",cont1,cont2);
	}
	//Enviamos el resultado
	write (sock_conn,buff2,strlen(buff2));			
}


void RegistrarUsuario(char churro[512], int codigo, int sock_conn, char buff2[512])//Registrar usuario (Consulta 6)
{
	//Declarar las variables
	int err;
	char consulta [80];
	char name[20];
	char pass[22];
	int existe = 0;
	
	char *a;
	a = strtok(churro,"/");	
	strcpy(name,a);
	a = strtok( NULL, "/");
	strcpy(pass,a);
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
	mysql_close (conn);
	}
	
	printf ("Respuesta con código: %d.\n",existe);
	//Preparamos el resultado
	sprintf(buff2,"%d",existe);
	//Enviamos el resultado
	write (sock_conn,buff2, strlen(buff2));
}



void DarDeBaja(char churro[512], int sock_conn, char buff2[512])//Usuario quiere darse de baja(Consulta 11)
{
	//Extraer las dos componentes del mensaje recibido 
	//y asignarlas en las variables user y password
	char *b;
	b = strtok(churro, "/");
	strcpy (user, b);
	b = strtok( NULL, "/");
	strcpy (password, b);
	
	//Declarar las variables
	int err;
	int resp;
	char pass[80];
	char consulta[200];
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
		}
		else{
			resp=-1;
			printf("Contraseña incorrecta.\n");}
		row = mysql_fetch_row (resultado);
	}	
	
	if (resp==0){
		printf ("Se va a dar de baja a este usuario: %s Password %s .\n",user,password);
		strcpy(consulta,"");
		sprintf(consulta,"DELETE FROM Players WHERE Players.idPlayer = '%s'",user);
		printf("\n%s\n",consulta);
		//Hacemos la consulta
		err=mysql_query (conn, consulta); 
		if (err!=0) {
			printf ("Error al consultar datos de la base %u %s\n",
					mysql_errno(conn), mysql_error(conn));
			exit (1);
		}
	}
	
	mysql_close (conn);
	//Preparamos el resultado
	sprintf(buff2,"%d",resp);
	//Enviamos el resultado
	write (sock_conn,buff2, strlen(buff2));
}

void DamePuntos(int puntos, int codigo, int sock_conn, char buff2[512])//Dame usuarios con mas de X puntos (Consulta 2)
{
	printf ("Codigo: %d, Puntos: %d\n", codigo, puntos);
	
	//Declaramos las variables
	int err;
	char consulta [80];
	char nombres[200]="";
	char cadena[100]="";
	
	// construimos la consulta SQL
	sprintf (consulta,"SELECT idPlayer FROM Players WHERE totalPoints > %d", puntos); 
	
	// hacemos la consulta 
	err=mysql_query (conn, consulta); 
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		sprintf(buff2,"2/-1");
		exit (1);
	}
	resultado = mysql_store_result (conn); 
	row = mysql_fetch_row (resultado);
	int t=1;
	if (row == NULL){
		printf ("No se han obtenido datos en la consulta\n");
		t=0;
		sprintf(buff2,"2/0");
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
	}
	//Enviamos el resultado
	write (sock_conn,buff2, strlen(buff2));	
}


void actualizarDatosJuego(char nombreGanador[20], char nombrePerdedor[20], int idPartida, int puntos) //Para actualizar y/o salvar los datos de la partida finalizada
{
	int puntosTotales;
	int numPartidas;
	int numVictorias;
	char consulta[512];
	char insertar[512];	
	 
	int err;
	puntosTotales = puntosTotales + puntos;
	numPartidas = numPartidas+1;
	numVictorias = numVictorias +1;
	
	printf("Se va a actualizar la base de datos...\n"); 
	sprintf(insertar,"UPDATE Players SET totalPoints = totalPoints+%d WHERE idPlayer = '%s'",puntos,nombreGanador);
	printf("%s\n",insertar);
	err=mysql_query (conn, insertar); 
	if (err!=0) {
		printf ("Error al introducir datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	sprintf(insertar,"UPDATE Players SET numberGames = numberGames+1 WHERE idPlayer = '%s'",nombreGanador);
	printf("%s\n",insertar);
	err=mysql_query (conn, insertar); 
	if (err!=0) {
		printf ("Error al introducir datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	sprintf(insertar,"UPDATE Players SET numberWins = numberWins+1 WHERE idPlayer = '%s'",nombreGanador);
	printf("%s\n",insertar);
	err=mysql_query (conn, insertar); 
	if (err!=0) {
		printf ("Error al introducir datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	sprintf(insertar,"UPDATE Players SET numberGames = numberGames+1 WHERE idPlayer = '%s'",nombrePerdedor);
	printf("%s\n",insertar);
	err=mysql_query (conn, insertar); 
	if (err!=0) {
		printf ("Error al introducir datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	printf("Base de datos actualizada.\n"); 
} 

void DamePartidas(char churro[512], int codigo, int sock_conn, char buff[512], char buff2[512])//Dame cuantas partidas ha ganado X jugador (Consulta 3)
{
	//Declarar las variables
	int err;
	char consulta [80];
	char name[20];
	char victorias[22] = "";
	strcpy (name, churro);
	printf ("Codigo: %d, Nombre: %s\n", codigo, name);
	
	// consulta SQL para obtener una tabla con todos los datos de la base de datos
	sprintf (consulta,"SELECT Players.numberWins FROM Players WHERE Players.idPlayer='%s'",name); 
	printf("Consulta:%s\n ",consulta);
	// hacemos la consulta 
	err=mysql_query (conn, consulta); 
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		sprintf(buff2,"3/-1");
		exit (1);
	}
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	if (row == NULL)
	{
		printf ("No se han obtenido datos en la consulta\n");
		sprintf(buff2,"3/0");
	}
	else{				
		sprintf(victorias,row[0]);
		printf("Victorias:%s",victorias);	
		printf ("El jugador %s tiene %s victorias.\n",name,victorias);
		//Preparamos el resultado
		sprintf(buff2,"3/%s",victorias);
	}
	//Enviamos el resultado
	write (sock_conn,buff2, strlen(buff2));
}


int PonConectado(char nombre[50], int socket, ListaConectados*lista)//Añade los usuarios conectados a la lista de conectados. Retorna 0 si ok. -1 si la lista está llena
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

void EliminarConectado(ListaConectados*lista,char nombre[50])//Elimina de la lista de conectados a los usuarios que se desconectan. Retorna 0 si lo ha eliminado bien
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


void DameConectados(ListaConectados *lista,char conectados[100])//Envía la lista de conectados 
{
	int i;
	sprintf(conectados,"5/%d",lista->numero);
	for(i=0;i<lista->numero;i++)
	{
		sprintf(conectados,"%s,%s,%d",conectados,lista->conectados[i].nombre,lista->conectados[i].socket);
	}
	printf("Se envía la lista de conectados: %s\n",conectados);
}


int DameSocket(ListaConectados *lista,char invitado[20])//Retorna el socket del jugador introducido
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


int CrearPartidaSolo(char anfitrion[20],char invitado1[20], int socketA, int socketI1, TablaPartidas tabla[100])//Crea partida en la tabla de partidas 1vs1
{
	int encontrado = 0;
	int i=0;
	
	while(i<100 && encontrado==0){
		char empty[5] = "\0";
		if(strcmp(tabla[i].anfitrion,empty)==0 ){
			encontrado=1;
			strcpy(tabla[i].anfitrion,anfitrion);
			strcpy(tabla[i].invitado1,invitado1);
			tabla[i].socketA = socketA;
			tabla[i].socketI1 = socketI1;
			printf("Se ha creado partida con: \n-ID: %d \n-Anfitrión: %s \n-Invitado: %s\n",i,tabla[i].anfitrion,tabla[i].invitado1);
		}
		else
			i++;
	}
	return i;
}

int CrearPartidaDuo(char anfitrion[20], char nombreInvitado1[20], char nombreInvitado2[20], char nombreInvitado3[20], int socketA, int socketI1, int socketI2, int socketI3,TablaPartidas tabla[100])//Crea partida en la tabla de partidas 2vs2
{
	int encontrado = 0;
	int i=0;
	
	while(i<100 && encontrado==0){
		char empty[5] = "\0";
		if(strcmp(tabla[i].anfitrion,empty)==0 ){
			encontrado=1;
			strcpy(tabla[i].anfitrion,anfitrion);
			strcpy(tabla[i].invitado1,nombreInvitado1);
			strcpy(tabla[i].invitado2,nombreInvitado2);
			strcpy(tabla[i].invitado3,nombreInvitado3);
			tabla[i].socketA = socketA;
			tabla[i].socketI1 = socketI1;
			tabla[i].socketI2 = socketI2;
			tabla[i].socketI3 = socketI3;
			printf("Se ha creado partida con: \n-ID: %d \n-Anfitrión: %s \n-Jugadores: %s, %s, %s\n",i,tabla[i].anfitrion,tabla[i].invitado1,tabla[i].invitado2,tabla[i].invitado3);
		}
		else
		   i++;
	}
	return i;
}

void EliminarPartida(int idPartida, TablaPartidas tabla[100])//Elimina una partida de la tabla de partidas
{	
	strcpy(tabla[idPartida].anfitrion,"\0");
	strcpy(tabla[idPartida].invitado1,"\0");
	strcpy(tabla[idPartida].invitado2,"\0");
	strcpy(tabla[idPartida].invitado3,"\0");
	tabla[idPartida].socketA = "\0";
	tabla[idPartida].socketI1 = "\0";
	tabla[idPartida].socketI2 = "\0";
	tabla[idPartida].socketI3 = "\0";
	tabla[idPartida].contadorAceptados = 0;
}

void DameListaJugadores (int socketCliente, char nombreCliente[20],char buff2[512])//Funcion para buscar jugadores que hayan jugado una partida con el cliente (Consulta 14)
{
	char listaNombres[200]="";
	char consulta[512];
	int err;
	
	//escribimos la consulta
	sprintf(consulta,"SELECT Players.idPlayer FROM Players, Games, PlayersGames WHERE Players.idPlayer=PlayersGames.idPlayer AND PlayersGames.idGame=Games.idGame AND Games.idGame IN(SELECT Games.idGame FROM PlayersGames,Games, Players WHERE Games.idGame=PlayersGames.idGame AND PlayersGames.idPlayer=Players.idPlayer AND Players.idPlayer='%s')",nombreCliente);
	printf("Consulta: %s\n",consulta);
	// hacemos la consulta 
	err=mysql_query (conn, consulta); 
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		sprintf(buff2,"14/-1");
		write (socketCliente,buff2, strlen(buff2));	
		exit (1);
	}
	resultado = mysql_store_result (conn); 
	row = mysql_fetch_row (resultado);
	int t=1;
	if (row == NULL){
		printf ("No se han obtenido datos en la consulta\n");
		t=0;
		sprintf(buff2,"14/0");
		write (socketCliente,buff2, strlen(buff2));	
	}
	else{
		while(row!=NULL){
			sprintf(listaNombres,"%s,%s",listaNombres, row[0]);
			row = mysql_fetch_row (resultado);
		}
	}
	
	sscanf(listaNombres,",%s",listaNombres);
	printf("%s\n",listaNombres);
	
	if(t==1){
		printf ("Estos jugadores %s han jugado con este cliente.\n",listaNombres);
		//Preparamos el resultado
		sprintf(buff2,"14/%s",listaNombres);
		//Enviamos el resultado
		write (socketCliente,buff2, strlen(buff2));		
	}
}

void DamePuntosConJugador(char nombreJugador[20], char nombreCliente[20], int socketCliente, char buff2[512])//Funcion para buscar los puntos de las partidas jugadas con un jugador (Consulta 15)
{
	char listaPuntos[512]="";
	char consulta[512];
	int err;
	
	//escribimos la consulta
	sprintf(consulta,"SELECT PlayersGames.points,PlayersGames.idGame FROM Players, Games, PlayersGames WHERE Players.idPlayer='%s' AND Players.idPlayer=PlayersGames.idPlayer AND PlayersGames.idGame=Games.idGame AND Games.idGame IN(SELECT Games.idGame FROM Players, Games, PlayersGames WHERE Players.idPlayer='%s' AND Players.idPlayer=PlayersGames.idPlayer AND PlayersGames.idGame=Games.idGame)",nombreCliente, nombreJugador);
	printf("Consulta: %s\n",consulta);
	// hacemos la consulta 
	err=mysql_query (conn, consulta); 
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		sprintf(buff2,"15/-1");
		exit (1);
	}
	resultado = mysql_store_result (conn); 
	row = mysql_fetch_row (resultado);
	int t=1;
	if (row == NULL){
		printf ("No se han obtenido datos en la consulta\n");
		sprintf(buff2,"15/0");
		t=0;
	}
	else{
		while(row!=NULL){
			sprintf(listaPuntos,"%s,%s",listaPuntos, row[0]);
			sprintf(listaPuntos,"%s,%s",listaPuntos, row[1]);
			row = mysql_fetch_row (resultado);
		}
	}
	
	sscanf(listaPuntos,",%s",listaPuntos);
	printf("%s\n",listaPuntos);
	
	if(t==1){
		printf ("Los puntos obtenidos con %s son:%s\n",nombreJugador,listaPuntos);
		//Preparamos el resultado
		sprintf(buff2,"15/%s$%s",nombreJugador,listaPuntos);
	}
	//Enviamos el resultado
	write (socketCliente,buff2, strlen(buff2));		
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

	while (terminar == 0){
		printf ("He recibido conexión.\n");
		
		// Ahora recibimos su nombre, que dejamos en buff
		ret=read(sock_conn,buff, sizeof(buff));
		printf ("Recibido.\n");
		
		// Tenemos que a?adirle la marca de fin de string para que no escriba lo que hay despues en el buffer
		buff[ret]='\0';
		
		//Escribimos el nombre en la consola
		printf ("Se ha recibido la petición: %s\n",buff);
		
		//Extraer el codigo de consulta (0,1,2,3) del mensaje para poder identificar la consulta 
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
			char buff4[20];
			sprintf(buff4,"10/2,%s,stop",nombre);
			for (int i = 0; i < lista.numero; i++){
				write (lista.conectados[i].socket,buff4,strlen(buff4));				
			}
			//Cerrar la conexion con el servidor MYSQL
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
			//Extraer la componente del nombre del jugador del mensaje
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
			char nombreInvitado1[20]="";
			char nombreInvitado2[20]="";
			char nombreInvitado3[20]="";
			int socketA;
			int socketI1;
			int socketI2;
			int socketI3;
			int idPartida;
			
			if (subcodigo == 0){//Enviar invitación 1vs1
				p = strtok(NULL,"/");
				strcpy(anfitrion,p);
				p = strtok(NULL,"/");
				strcpy(nombreInvitado1,p);
				int socketA = DameSocket(&lista,anfitrion);
				int socketI1 = DameSocket(&lista,nombreInvitado1);
				idPartida = CrearPartidaSolo(anfitrion,nombreInvitado1,socketA,socketI1,tabla);
				sprintf(buff2,"16/%d",idPartida); //Enviar la id de partida al anfintrion
				write(sock_conn,buff2,strlen(buff2));
				printf("Id partida: %d El anfitrion %s envía invitación a: %s\n",idPartida,anfitrion,nombreInvitado1);
				sprintf(buff2,"7/0$%d,%s",idPartida,anfitrion);
				//int sock_aux = DameSocket(&lista,nombreInvitado);
				write (socketI1,buff2,strlen(buff2));
			}
			
			if (subcodigo == 1){//Acepta invitación
				p = strtok(NULL,"/");
				idPartida = atoi(p);
				printf("El jugador %s, ha aceptado jugar con %s. ID partida: %d.\n",tabla[idPartida].invitado1,tabla[idPartida].anfitrion,idPartida);
				sprintf(buff2,"7/1$%d,%s",idPartida,tabla[idPartida].invitado1);
				write (tabla[idPartida].socketA,buff2,strlen(buff2));				
			}
			
			if (subcodigo == 2){//Rechaza invitación 1vs1
				p = strtok(NULL,"/");
				idPartida = atoi(p);
				printf("El jugador %s, ha rechazado jugar con %s. ID partida: %d.\n",tabla[idPartida].invitado1,tabla[idPartida].anfitrion,idPartida);
				sprintf(buff2,"7/2$%s",tabla[idPartida].invitado1);
				write (tabla[idPartida].socketA,buff2,strlen(buff2));
				EliminarPartida(idPartida,tabla);	
			}
			
			if (subcodigo == 3){//Enviar invitación 2vs2
				p = strtok(NULL,"/");
				strcpy(anfitrion,p);
				p = strtok(NULL,"/");
				strcpy(nombreInvitado1,p);
				int socketA = DameSocket(&lista,anfitrion);
				int socketI1 = DameSocket(&lista,nombreInvitado1);
				p = strtok(NULL,"/");
				strcpy(nombreInvitado2,p);
				p = strtok(NULL,"/");
				strcpy(nombreInvitado3,p);
				int socketI2 = DameSocket(&lista,nombreInvitado2);
				int socketI3 = DameSocket(&lista,nombreInvitado3);			
				
				idPartida = CrearPartidaDuo(anfitrion,nombreInvitado1,nombreInvitado2,nombreInvitado3,socketA,socketI1,socketI2,socketI3,tabla);
				sprintf(buff2,"16/%d",idPartida); //Enviar la id de partida al anfintrion
				write(sock_conn,buff2,strlen(buff2));
				printf("Id partida: %d El anfitrion %s envía invitación a: %s como aliado y a %s y %s como contrincantes\n",idPartida,anfitrion,nombreInvitado1,nombreInvitado2,nombreInvitado3);
				sprintf(buff2,"7/3$2$%d,%s,%s,%s",idPartida,anfitrion,nombreInvitado2,nombreInvitado3);
				write (socketI1,buff2,strlen(buff2));				
				sprintf(buff2,"7/3$3$%d,%s,%s,%s",idPartida,anfitrion,nombreInvitado1,nombreInvitado3);
				write (socketI2,buff2,strlen(buff2));				
				sprintf(buff2,"7/3$4$%d,%s,%s,%s",idPartida,anfitrion,nombreInvitado1,nombreInvitado2);
				write (socketI3,buff2,strlen(buff2));				
			}
			
			if (subcodigo == 4){//Todos los jugadores aceptan la invitación	2vs2			
				p = strtok(NULL,"/");
				idPartida = atoi(p);
				p = strtok(NULL,"/");
				char aux[20];
				strcpy(aux,p);
				
				if (strcmp(aux,tabla[idPartida].invitado1)==0){ //Se envía la aceptación del aliado
					sprintf(buff2,"7/6$%d",idPartida);
					write (tabla[idPartida].socketA,buff2,strlen(buff2));
					write (tabla[idPartida].socketI1,buff2,strlen(buff2));
					write (tabla[idPartida].socketI2,buff2,strlen(buff2));
					write (tabla[idPartida].socketI3,buff2,strlen(buff2));
					tabla[idPartida].contadorAceptados++;
					printf("El aliado ha aceptado la partida. Enviando 7/6\n");
					//printf("\n kjsdhfkjsdhfksjh %d\n",tabla[idPartida].contadorAceptados);
				}
				
				if (strcmp(aux,tabla[idPartida].invitado2)==0){ //Se envía la aceptación del contrincante 1 
					sprintf(buff2,"7/6$%d",idPartida);
					write (tabla[idPartida].socketA,buff2,strlen(buff2));
					write (tabla[idPartida].socketI1,buff2,strlen(buff2));
					write (tabla[idPartida].socketI2,buff2,strlen(buff2));
					write (tabla[idPartida].socketI3,buff2,strlen(buff2));
					tabla[idPartida].contadorAceptados++;
					printf("El contrincante 3 ha aceptado la partida. Enviando 7/6\n");
					//printf("\n kjsdhfkjsdhfksjh %d\n",tabla[idPartida].contadorAceptados);
				}
				
				if (strcmp(aux,tabla[idPartida].invitado3)==0){ //Se envía la aceptación del contrincante 2
					sprintf(buff2,"7/6$%d",idPartida);
					write (tabla[idPartida].socketA,buff2,strlen(buff2));
					write (tabla[idPartida].socketI1,buff2,strlen(buff2));
					write (tabla[idPartida].socketI2,buff2,strlen(buff2));
					write (tabla[idPartida].socketI3,buff2,strlen(buff2));
					tabla[idPartida].contadorAceptados++;
					printf("El contrincante 4 ha aceptado la partida. Enviando 7/6\n");
					//printf("\n kjsdhfkjsdhfksjh %d\n",tabla[idPartida].contadorAceptados);
				}
				
				//printf("El jugador %s, ha aceptado jugar la partida con ID: %d.\n",aux,idPartida);
				
				if (tabla[idPartida].contadorAceptados == 3){ //Como todos los jugadores han aceptado, enviamos orden de arrancar el juego
					sprintf(buff2,"7/4$%d,%s,%s,%s",idPartida,tabla[idPartida].invitado1,tabla[idPartida].invitado2,tabla[idPartida].invitado3);
					write (tabla[idPartida].socketA,buff2,strlen(buff2));
					write (tabla[idPartida].socketI1,buff2,strlen(buff2));
					write (tabla[idPartida].socketI2,buff2,strlen(buff2));
					write (tabla[idPartida].socketI3,buff2,strlen(buff2));
					tabla[idPartida].contadorAceptados = 0;
					printf("Todos los jugadores han aceptado. Enviando orden 7/4.\n");
					//printf("\n  CONTADOR %d\n",tabla[idPartida].contadorAceptados);
				}
			}
			if (subcodigo == 5){//Rechaza invitación 2vs2
				p = strtok(NULL,"/");
				idPartida = atoi(p);
				p = strtok(NULL,"/");
				char aux[20];
				strcpy(aux,p);
				printf("El jugador %s, ha rechazado jugar la partida con ID: %d.\n",aux,idPartida);
				sprintf(buff2,"7/5$%d,%s",idPartida,tabla[idPartida].invitado1);
				if (strcmp(tabla[idPartida].invitado3,"\0")==0){
					write (tabla[idPartida].socketA,buff2,strlen(buff2));
					write (tabla[idPartida].socketI1,buff2,strlen(buff2));
				}
				else{
					write (tabla[idPartida].socketA,buff2,strlen(buff2));
					write (tabla[idPartida].socketI1,buff2,strlen(buff2));
					write (tabla[idPartida].socketI2,buff2,strlen(buff2));
					write (tabla[idPartida].socketI3,buff2,strlen(buff2));
				}
				EliminarPartida(idPartida,tabla);
			}			
		}
		
		//Chat Global 
		if(codigo == 8){
			char jugadorEnvia[20];
			char mensajeEnvia[512];
			p = strtok(NULL,"/");
			strcpy(jugadorEnvia,p);
			p = strtok(NULL,"/");
			strcpy(mensajeEnvia,p);
			sprintf(buff2,"8/%s$%s",jugadorEnvia,mensajeEnvia);
			for (int j=0;j<lista.numero;j++){
				write (lista.conectados[j].socket,buff2, strlen(buff2));
			}
		}
		
		//Chat Partida
		if(codigo == 9){
			char jugadorEnvia[20];
			char mensajeEnvia[512];
			p = strtok(NULL,"/");
			int idP = atoi(p);
			p = strtok(NULL,"/");
			strcpy(jugadorEnvia,p);
			p = strtok(NULL,"/");
			strcpy(mensajeEnvia,p);
			sprintf(buff2,"9/%d$%s$%s",idP,jugadorEnvia,mensajeEnvia);
			if (strcmp(tabla[idP].invitado3,"\0")==0){
				write (tabla[idP].socketA,buff2,strlen(buff2));
				write (tabla[idP].socketI1,buff2,strlen(buff2));
			}
			else{
				write (tabla[idP].socketA,buff2,strlen(buff2));
				write (tabla[idP].socketI1,buff2,strlen(buff2));
				write (tabla[idP].socketI2,buff2,strlen(buff2));
				write (tabla[idP].socketI3,buff2,strlen(buff2));
			}
		}
			
		
		//Para darse de baja
		if(codigo == 11){
			//Extraer la componente del nombre del jugador del mensaje
			p = strtok( NULL, "NULL");
			strcpy(churro,p);
			//Abrimos conexión con la BBDD
			ConectarBBDD();
			DarDeBaja(churro,sock_conn,buff2);
		}
		
		//El cliente se desconecta sin haber accedido con ningún usuario
		if (codigo == 12){			
			terminar = 1;
			printf("El cliente con socket: %d, se ha desconectado.\n",sock_conn);
		}
		//Un jugador ha abandonado la partida
		if (codigo == 13){
			int idP;
			p = strtok(NULL,"/");
			idP = atoi(p);
			sprintf(buff2,"13/%d",idP);
			if (strcmp(tabla[idP].invitado3,"\0")==0){
				if(sock_conn != tabla[idP].socketA)
					write (tabla[idP].socketA,buff2,strlen(buff2));
				else
					write (tabla[idP].socketI1,buff2,strlen(buff2));
			}
			else{
				if (sock_conn != tabla[idP].socketA)
					write (tabla[idP].socketA,buff2,strlen(buff2));
				if (sock_conn != tabla[idP].socketI1)
					write (tabla[idP].socketI1,buff2,strlen(buff2));
				if (sock_conn != tabla[idP].socketI2)
					write (tabla[idP].socketI2,buff2,strlen(buff2));
				if (sock_conn != tabla[idP].socketI3)
					write (tabla[idP].socketI3,buff2,strlen(buff2));
			}
			EliminarPartida(idP,tabla);
		}
		//COnsulta para saber con que jugadores ha jugado el cliente
		if (codigo == 14){
			p = strtok( NULL, "/");
			char nombreCliente[20];
			strcpy(nombreCliente,p);
			DameListaJugadores(sock_conn,nombreCliente,buff2);
		}
		//Consulta para obtener los puntos de las partidas jugadas con un jugador
		if (codigo == 15){
			char nombreCliente[20];
			char nombreJugador[20];
			
			p = strtok( NULL, "/");
			strcpy(nombreCliente,p);
			p = strtok( NULL, "/");
			strcpy(nombreJugador,p);
			DamePuntosConJugador(nombreJugador, nombreCliente, sock_conn, buff2);
		}
		
		//Para ver la lista de los conectados y avisar por chat de quién se conecta o desconecta. Esto usa el código 5
		if (enviarLista == 1){
			char conectados[100];
			DameConectados(&lista,conectados);
			//Preparamos el resultado
			sprintf(buff2,"%s",conectados);
			char buff5[20];
			sprintf(buff5,"10/1,%s",lista.conectados[lista.numero - 1].nombre);
			//Enviamos el resultado
			int j;
			//Envia la lista de conectados a los clientes conectados
			for (j=0;j<lista.numero;j++){
				write (lista.conectados[j].socket,buff2, strlen(buff2));
				write (lista.conectados[j].socket,buff5, strlen(buff5));
			}
			enviarLista = 0; //Seguro para que solo envíe la lista cuando haya modificaciones en ella
		}
		//El cliente envía su carta al contrincante para mostrarla en su tablero
		if (codigo == 30){ 
			int idP;
			int nCarta;
			p = strtok(NULL,"/");
			idP = atoi(p);
			p = strtok(NULL,"/");
			nCarta = atoi(p);		
			sprintf(buff2,"30/%d,%d",idP,nCarta);
			printf("Enviamos CARTA %s\n",buff2);
			if (sock_conn != tabla[idP].socketA)
				write(tabla[idP].socketA,buff2, strlen(buff2));
			else
				write(tabla[idP].socketI1,buff2, strlen(buff2));
		}
		//El cliente envía su carta a los demas jugadores para mostrarla en su tablero
		if (codigo == 31){
			int idP;
			int nCarta;
			char nomJugador[20]="";
			p = strtok(NULL,"/");
			idP = atoi(p);
			p = strtok(NULL,"/");
			nCarta = atoi(p);
			p = strtok(NULL,"/");
			strcpy(nomJugador,p);
			sprintf(buff2,"31/%d,%d,%s",idP,nCarta,nomJugador);
			printf("ENViamos CARTA %s\n",buff2);
			if (sock_conn != tabla[idP].socketA)
				write(tabla[idP].socketA,buff2, strlen(buff2));
			if (sock_conn != tabla[idP].socketI1)
				write(tabla[idP].socketI1,buff2, strlen(buff2));
			if (sock_conn != tabla[idP].socketI2)
				write(tabla[idP].socketI2,buff2, strlen(buff2));
			if (sock_conn != tabla[idP].socketI3)
				write(tabla[idP].socketI3,buff2, strlen(buff2));
		}
		//El cliente envía su ataque fisico a los demas jugadores 
		if (codigo == 32){	
			int idP;
			int Ataque;
			char nomJugador[20] = "";
			p = strtok(NULL,"/");
			idP = atoi(p);
			p = strtok(NULL,"/");
			Ataque = atoi(p);
			p = strtok(NULL,"/");
			strcpy(nomJugador,p);
			sprintf(buff2,"32/%d,%d,%s",idP,Ataque,nomJugador);
			printf("Enviamos la informacion del ataque: %d\n", Ataque);
			write(tabla[idP].socketA,buff2, strlen(buff2));
			write(tabla[idP].socketI1,buff2, strlen(buff2));
		}
		
		//El cliente envía su hechizo a los demas jugadores 
		if (codigo == 33){	
			int idP;
			int Ataque;
			char nomJugador[20] = "";
			p = strtok(NULL,"/");
			idP = atoi(p);
			p = strtok(NULL,"/");
			Ataque = atoi(p);
			p = strtok(NULL,"/");
			strcpy(nomJugador,p);
			sprintf(buff2,"33/%d,%d,%s",idP,Ataque,nomJugador);
			printf("Enviamos la informacion del hechizo: %d\n", Ataque);
			write(tabla[idP].socketA,buff2, strlen(buff2));
			write(tabla[idP].socketI1,buff2, strlen(buff2));
		}
		
		//Se envia un mensaje a mostrar en el chat en caso de una visctoria 
		if (codigo == 34){	
			int idP;
			int cont = 0;
			int puntos;
			char nomGanador[20];
			char nomPerdedor[20];
			p = strtok(NULL,"/");
			idP = atoi(p);
			p = strtok(NULL,"/");
			puntos = atoi(p);
			p = strtok(NULL,"/");
			strcpy(nomGanador,p);
			p = strtok(NULL,"/");
			strcpy(nomPerdedor,p);
			sprintf(buff2,"8/BOT$El jugador %s ha ganado la partida.$stop",nomGanador);
			printf("El jugador %s ha ganado la partida %d. Puntos: %d.\n",nomGanador,idP,puntos);
			contadorNotificacionGanador = contadorNotificacionGanador + 1;
			if (contadorNotificacionGanador == 2)
			{
			//if (sock_conn != tabla[idP].socketA)
				write(tabla[idP].socketA,buff2, strlen(buff2));
			//if (sock_conn != tabla[idP].socketI1)
				write(tabla[idP].socketI1,buff2, strlen(buff2));
				actualizarDatosJuego(nomGanador,nomPerdedor,idP,puntos);
				EliminarPartida(idP,tabla);
				contadorNotificacionGanador = 0;
			}
		}
	}
}

int main(int argc, char *argv[])
{	
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;	
	
	// INICIALIZACIONES
	// Abrimos el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
	// Hacemos el bind en el puerto	
	
	memset(&serv_adr, 0, sizeof(serv_adr));// inicializa a cero serv_addr
	serv_adr.sin_family = AF_INET;
	
	//Asocia el socket a cualquiera de las IP de la maquina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	//Establecemos el puerto de escucha
	serv_adr.sin_port = htons(9002);
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
	
	//Bucle infinito
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

