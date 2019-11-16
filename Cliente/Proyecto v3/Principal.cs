using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WMPLib;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Principal : Form
    {
        Socket server;
        Thread atender;
        Invitations[] vectorInvitaciones = new Invitations[15]; //Vector para acumular invitaciones

        int Socket = 50053;
        int[] contadorAceptados = new int[100];
        bool mostrar = false;
        bool conectadoConUsuario;
        bool conectadoSinUsuario;
        bool threadOn;
        bool vistoBueno = true; //Permiso para mostrar la siguiente notificación de invitación
        bool invitadoEncontrado; //Para no invitar 2 veces a la misma persona
        bool invitacionEnCurso; //En caso que durante la invitación un jugador se desconecte
        string hostEnApp; //Variable para salvar el nombre de quien acaba de iniciar sesión
        string resultadoConsulta;
        string Player2;
        string duoPlayer1;
        string duoPlayer2;
        string duoPlayer3;
        string duoPlayer4;
        string idPartidaChat;
        string idPartidaAux;
        
        List<string> listaInvitaciones = new List<string>(); //Para acumular invitaciones
        formTablero[] vectorForms = new formTablero[20]; //Para acumular partidas simultáneas

        //Delegados para poder modificar con la protección cross-threading activa
        delegate void DelegadoParaModificar();
        delegate void DelegadoParaModificar2(int num, string mensaje1, string mensaje2);
        delegate void DelegadoParaModificar3(int num);
        delegate void DelegadoParaModificar4(string mensaje);

        //Añadir sonidos al programa
        WindowsMediaPlayer chatAlert = new WindowsMediaPlayer();
        string Directorio = Directory.GetCurrentDirectory(); //Buscar la ruta del programa para la ejecución universal de los archivos

        public Principal()
        {
            InitializeComponent();
            //Declarar un objeto de la clase invitación en cada posión del vector
            for (int i = 0; i < 15; i++)
            {
                this.vectorInvitaciones[i] = new Invitations();
            }
            //Parámetros estéticos
            label1.Hide();
            labelConsultar.Hide();
            consultasBox.Hide();
            Reconectar.Hide();
            label7.Hide(); //Indica el estado de conexión con el servidor
            consulta1.Hide();
            consulta2.Hide();
            consulta3.Hide();
            consulta4.Hide();
            consulta5.Hide();
            mostrarConsultas.Hide();
            p1.Hide();
            consultar.Hide();
            listaconectados.Hide();
            amigos.Hide();
            AceptarBaja.Hide();
            regCancel.Hide();
            regAceptar.Hide();
            label4.Hide();
            off.Hide();
            OcultarAmigos.Hide();
            controlChat(2, null, null); //Ocultar chat en general
            nombreJugador.Hide();
            crearPartida.Hide();
            contadorAceptaciones.Hide();
            aceptarInvitacion.Hide();
            rechazarInvitacion.Hide();
            textoInvitacion.Hide();
            pictureBoxInvitaciones.Controls.Add(textoInvitacion);
            textoInvitacion.Top = 1;
            textoInvitacion.Left = 1;
            textoInvitacion.BringToFront();
            pictureBoxInvitaciones.Hide();
            this.Show();
            p.PasswordChar = '*'; //Para proteger la contraseña del usuario

            //--------------------- v Sonido del chat v ---------------------//
            chatAlert.URL = @"" + Directorio + "/chatAlert.mp3"; //Busca la dirección del sonido del chat
            //Parar sonido
            chatAlert.controls.stop();
            //--------------------- ^ Sonido del chat ^ ---------------------//
            ConectarAlServidor();
        }

        public void MostrarForm() //Para forzar el muestreo del form principal desde cualquier otro form
        {
            this.Show();
        }

        public void OcultarForm() //Ocultar el form principal al iniciar el juego
        {
            this.Hide();
        }

        //Explicación de lo que se tiene que introducir para realizar las consultas

        private void consulta1_CheckedChanged(object sender, EventArgs e)
        {
            DelegadoParaModificar4 delInstrucciones = new DelegadoParaModificar4(InformacionConsulta);
            Invoke(delInstrucciones, new Object[] { "Introduzca el número de la partida" });
            labelConsultar.Show();
        }

        private void consulta2_CheckedChanged(object sender, EventArgs e)
        {
            DelegadoParaModificar4 delInstrucciones = new DelegadoParaModificar4(InformacionConsulta);
            Invoke(delInstrucciones, new Object[] { "Introduzca el número de puntos" });
            labelConsultar.Show();
        }

        private void consulta3_CheckedChanged(object sender, EventArgs e)
        {
            DelegadoParaModificar4 delInstrucciones = new DelegadoParaModificar4(InformacionConsulta);
            Invoke(delInstrucciones, new Object[] { "Introduzca el nombre de un jugador" });
            labelConsultar.Show();
        }

        private void consulta4_CheckedChanged(object sender, EventArgs e)
        {
            DelegadoParaModificar4 delInstrucciones = new DelegadoParaModificar4(InformacionConsulta);
            Invoke(delInstrucciones, new Object[] { "No tiene que introducir nada. Pulse click en \"Consultar\"" });
            labelConsultar.Show();
        }

        private void consulta5_CheckedChanged(object sender, EventArgs e)
        {
            DelegadoParaModificar4 delInstrucciones = new DelegadoParaModificar4(InformacionConsulta);
            Invoke(delInstrucciones, new Object[] { "Introduzca el nombre de un jugador" });
            labelConsultar.Show();
        }
       
        ///////////////////////////////////////////////////////////////////////////
      
        public void InformacionConsulta(string informacion)//Cambiar el texto del label de consultas
        {
            labelConsultar.Text = informacion;
        }

        private void consultar_Click(object sender, EventArgs e) //Consultas a la BBDD del servidor
        {
            if (p1.Text != "" && consulta4.Checked == false)
            {
                if (consulta1.Checked == false && consulta2.Checked == false && consulta3.Checked == false && consulta5.Checked == false)
                    MessageBox.Show("No has seleccionado ninguna de las consultas.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                {
                    if (consulta1.Checked) //Para saber cuántos jugadores han usado la misma skin en una partida
                    {
                        try
                        {
                            int aux = Convert.ToInt32(p1.Text);
                            if (aux >= 0)
                            {
                                string mensaje = "1/" + p1.Text;
                                //Enviamos al servidor el número de partida tecleado
                                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                                server.Send(msg);
                                consulta1.Checked = false;
                            }
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("Debes introducir un número entero.", "Error");
                        }
                    }

                    if (consulta2.Checked) //Para saber los nombres de los jugadores que han obtenido más de X puntos
                    {
                        try
                        {
                            int aux = Convert.ToInt32(p1.Text);
                            if (aux >= 0)
                            {
                                string mensaje = "2/" + p1.Text;
                                //Enviamos al servidor el número de puntos tecleado
                                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                                server.Send(msg);
                                consulta2.Checked = false;
                            }
                        }
                        catch (FormatException) 
                        {
                            MessageBox.Show("Debes introducir un número entero.", "Error");
                        }
                    }

                    if (consulta3.Checked) //Para saber cuántas partidas ha ganado un jugador
                    {
                        string mensaje = "3/" + p1.Text;
                        //Enviamos al servidor el nombre tecleado
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                        consulta3.Checked = false;
                    }
                    if (consulta5.Checked)
                    {
                        string mensaje = "15/" + hostEnApp + "/" + p1.Text;
                        //Enviamos al servidor el nombre tecleado
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                        consulta5.Checked = false;
                    }
                }
            }
            else if (consulta4.Checked == false)
                MessageBox.Show("El campo está sin rellenar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else if (consulta4.Checked == true) //Consulta para saber con qué jugadores he jugado
            {
                string mensaje = "14/" + hostEnApp;
                //Enviamos al servidor el número de consulta
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                consulta4.Checked = false;
            }
            labelConsultar.Hide();
        }

        private void ConectarAlServidor() //Función para que el cliente establezca conexión con el servidor
        {
            label8.Show();
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("147.83.117.22");
            IPEndPoint ipep = new IPEndPoint(direc, Socket);

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep); //Intentamos conectar el socket
                ThreadStart ts = delegate { AtenderServidor(); }; //Declaramos el delegado
                atender = new Thread(ts); //Declaramos el thread
                conectadoSinUsuario = true;
            }
            catch (SocketException)
            {
                //Si hay excepción imprimimos error y salimos del programa con return 
                MessageBox.Show("No se ha podido establecer conexión con el servidor");
                Reconectar.Show();
                label7.Show();
                label7.Text = "Sin conexión";
                label7.ForeColor = Color.White;
                label8.Hide();
                return;
            }
            label7.Show();
            label7.Text = "Hay conexión";
            label7.ForeColor = Color.LimeGreen;
            label8.Hide();
            Reconectar.Hide();
        }
        
        private void conectar_Click_1(object sender, EventArgs e) //Función con la que un usuario inicia sesión
        {
            if (conectadoSinUsuario == true)
            {
                if (u.Text != "" && p.Text != "") //Si usuario y contraseña no están vacíos
                {
                    try
                    {
                        ThreadStart ts = delegate { AtenderServidor(); }; //Declaramos el delegado
                        atender = new Thread(ts); //Declaramos el thread
                    }
                    catch (SocketException)
                    {
                        //Si hay excepción imprimimos error y salimos del programa con return 
                        MessageBox.Show("No se ha podido establecer conexión con el servidor");
                        return;
                    }

                    // Recoger el nombre de usuario y contraseña
                    string user = u.Text;
                    hostEnApp = u.Text;
                    string password = p.Text;
                    string mensaje = "4/" + user + "/" + password + "/";

                    // Enviamos al servidor el login tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    // Recibimos la respuesta del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    user = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                    if (user == "0") //Si es 0, los datos introducidos son correctos. Se inicia sesión
                    {
                        this.conectadoConUsuario = true;
                        chat.Items.Add("¡Bienvenido de nuevo " + hostEnApp + "!");
                        nombreJugador.Text = "Conectado como: " + hostEnApp;
                        nombreJugador.Show();
                        amigos.Show();
                        vistoBueno = true;
                        p.Clear();
                        u.Clear();
                        registrar.Hide();
                        conectar.Hide();
                        mostrarConsultas.Show();
                        //jugar.Show();
                        off.Show();
                        u.Hide();
                        p.Hide();
                        DarseBaja.Hide();
                        label2.Hide();
                        label3.Hide();
                        this.BackColor = Color.LightGreen;
                        controlChat(1, null, null);
                        atender.Start();
                        threadOn = true;
                    }
                    else if (user == "1") //Si es 1, el nombre de usuario no existe
                    {
                        MessageBox.Show("El nombre de usuario no existe.\n\nPor favor, considera registrarte.");
                    }
                    else if (user == "2") //Si es 2, el usuario o contraseña incorrecta
                    {
                        MessageBox.Show("¡Usuario o contraseña incorrecta!\n\nIntroduce los datos de nuevo o regístrate.");
                    }
                    else if (user == "3")
                    {
                        MessageBox.Show("¡No puedes iniciar sesión 2 veces!");
                    }
                }
                else //Si no se ha rellenado los campos de inicio de sesión
                {
                    MessageBox.Show("Tiene campos obligatorios * sin rellenar.");
                }
            }
            else
                MessageBox.Show("Lo sentimos, no se puede conectar con el servidor.");
        }

        private void registrar_Click_1(object sender, EventArgs e) //Acceder al formulario para registrar un nuevo usuario
        {
            u.BackColor = Color.PaleTurquoise;
            p.BackColor = Color.PaleTurquoise;
            registrar.Hide();
            regCancel.Show();
            regAceptar.Show();
            conectar.Hide();
            DarseBaja.Hide();
            AceptarBaja.Hide();
            label4.Show();
            label4.Text = "Introduce el Usuario y Contraseña que desees.";
        }

        private void off_Click(object sender, EventArgs e) //Desconectarse del servidor
        {
            //Mensaje de desconexión
            string mensaje = "0/" + hostEnApp + "/";

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
                           
            // Nos desconectamos
            atender.Abort();
            server.Shutdown(SocketShutdown.Both);
            server.Close();
            ConectarAlServidor();

            //this.conectado = false;
            this.threadOn = false;
            this.conectadoConUsuario = false;
            this.BackColor = Color.Tomato;
            label1.Hide();
            amigos.Hide();
            consultasBox.Hide();
            consultasBox.Items.Clear();
            crearPartida.Hide();
            listaconectados.Hide();
            labelConsultar.Hide();
            consulta1.Hide();
            consulta2.Hide();
            consulta3.Hide();
            consulta4.Hide();
            consulta5.Hide();
            mostrarConsultas.Hide();
            p1.Hide();
            consultar.Hide();
            registrar.Show();
            DarseBaja.Show();
            conectar.Show();
            off.Show();
            OcultarAmigos.Hide();
            mostrarConsultas.Text = "Mostrar Consultas";
            off.Hide();
            u.Show();
            p.Show();
            label2.Show();
            label3.Show();
            nombreJugador.Hide();
            controlChat(2, null, null);
            //Controles Invitaciones
            contadorAceptaciones.Hide();
            aceptarInvitacion.Hide();
            rechazarInvitacion.Hide();
            pictureBoxInvitaciones.Hide();
        }

        private void amigos_Click(object sender, EventArgs e) //Ver la lista de conectados
        {
            if (mostrar)
            {
                listaconectados.Show();
                OcultarAmigos.Show();
                amigos.Hide();
            }
            else
                MessageBox.Show("Ninguno de tus amigos está conectado.");
        }
            
        private void regCancel_Click(object sender, EventArgs e) //Botón para cancelar el registro y ejecuciones varias
        {
            u.BackColor = Color.White;
            p.BackColor = Color.White;
            u.Clear();
            p.Clear();
            registrar.Show();
            AceptarBaja.Hide();
            DarseBaja.Show();
            regCancel.Hide();
            regAceptar.Hide();
            conectar.Show();
            label4.Hide();
        }

        private void AceptarBaja_Click(object sender, EventArgs e) //Función darse de baja
        {
            if (conectadoSinUsuario == true)
            {
                registrar.Hide();
                if (u.Text != "" && p.Text != "") //Si los campos de usuario y contraseña no son nulos
                {
                    // Recoger el nombre de usuario y contraseña
                    string user = u.Text;
                    string password = p.Text;

                    //Nueva consulta para la base de datos para eliminar el usuario de esta
                    string mensaje = "11/" + user + "/" + password + "/";

                    // Enviamos al servidor el login tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    //Recibimos la respuesta del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    user = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                    if (user == "0") //Si el usuario y la contraseña son correctos
                    {
                        MessageBox.Show("Se ha dado de baja correctamente.");
                        AceptarBaja.Hide();
                        DarseBaja.Show();
                        u.BackColor = Color.White;
                        p.BackColor = Color.White;
                        registrar.Show();
                        regCancel.Hide();
                        regAceptar.Hide();
                        conectar.Show();
                        label4.Hide();
                        u.Clear();
                        p.Clear();
                        this.conectadoSinUsuario = true;
                    }
                    else if (user == "-1") //Si el nombre de usuario no existe
                    {
                        MessageBox.Show("El nombre de usuario no existe.\n\n Por favor, vuelva a introducir el nombre de usuario.");
                        this.conectadoSinUsuario = true;
                    }
                }
                else //Si no se han rellenado los campos obligatorios
                    MessageBox.Show("Tiene campos obligatorios * sin rellenar.");
            }
            else
                MessageBox.Show("No hay conexión con el servidor.");
        }
       
        private void regAceptar_Click(object sender, EventArgs e) //Función para registrar nuevos usuarios
        {
            if (conectadoSinUsuario == true)
            {
                if (u.Text != "" && p.Text != "") //Si los campos de usuario y contraseña no son nulos
                {
                    // Recoger el nombre de usuario y contraseña
                    string user = u.Text;
                    hostEnApp = u.Text;
                    string password = p.Text;
                    string mensaje = "6/" + user + "/" + password + "/";

                    // Enviamos al servidor el login tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    //Recibimos la respuesta del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    user = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                    if (user == "0")
                    {
                        MessageBox.Show("Bienvenido " + hostEnApp + ".\n\nAhora ya puedes iniciar sesión.");
                        registrar.Hide();
                        u.BackColor = Color.White;
                        p.BackColor = Color.White;
                        registrar.Show();
                        regCancel.Hide();
                        DarseBaja.Show();
                        regAceptar.Hide();
                        conectar.Show();
                        label4.Hide();
                        u.Clear();
                        p.Clear();
                        this.conectadoSinUsuario = true;
                    }
                    else if (user == "-1") //Si el nombre de usuario ya está en uso
                    {
                        MessageBox.Show("El nombre de usuario escogido ya está en uso.\n\n Por favor, escoge otro nombre de usuario.");
                        this.conectadoSinUsuario = true;
                    }
                }
                else //Si no se han rellenado los campos obligatorios
                    MessageBox.Show("Tiene campos obligatorios * sin rellenar.");
            }
            else
                MessageBox.Show("No hay conexión con el servidor.");
        }

        private void ejecutarTablero(int nForm, string jugadores) //Arrancar el tablero de juego. nForm, corresponde a la id de la partida y es esa la posición que ocupará en la lista
        {
            ThreadStart ts = delegate { PonEnMarchaFormulario(nForm, jugadores);}; 
            Thread T = new Thread(ts);
            T.Start();
        }

        private void PonEnMarchaFormulario(int nForm, string jugadores) //Poner en marcha el Tablero
        {
            formTablero f = new formTablero(nForm, server, this.hostEnApp, jugadores);
            vectorForms[nForm] = f;
            f.ShowDialog();
        }

        private void OcultarAmigos_Click(object sender, EventArgs e) //Ocultar la lista de amigos conectados
        {
            listaconectados.Hide();
            amigos.Show();
            OcultarAmigos.Hide();
        }

        private void listaconectados_CellContentClick(object sender, DataGridViewCellEventArgs e) //Elegir a quien se enviará la invitación a partida haciendo click en el nombre
        {
            //Este es el caso de crear una partida de 4 personas
            if (listaInvitaciones.Count == 3)
            {
                MessageBox.Show("Las partidas son 4 jugadores como máximo!", "Error.");
            }
            else //Enviar invitación al jugador seleccionado
            {
                string jugadorInvitado = listaconectados.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                DialogResult confirmacion = MessageBox.Show("¿Seguro que quieres invitar a: " + jugadorInvitado + "?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {                    
                    for (int i = 0; i < listaInvitaciones.Count; i++)
                    {
                        if (jugadorInvitado == listaInvitaciones[i])
                        {
                            invitadoEncontrado = true;
                        }
                    }
                    if (invitadoEncontrado == true)
                        MessageBox.Show("¡" + jugadorInvitado + " ya está en la lista de invitaciones!", "Atención", MessageBoxButtons.OK,MessageBoxIcon.Information);
                    else
                    {
                        listaInvitaciones.Add(jugadorInvitado);
                        invitadoEncontrado = false;
                    }
                    crearPartida.Show();
                }
            }
        }

        private void crearPartida_Click(object sender, EventArgs e) //Observa si los jugadores en lista son 2 o 4 y envía las invitaciones correspondientes
        {
            if (listaInvitaciones.Count == 1) //Enviar invitación a 1 jugador. Partida 1vs1
            {
                DialogResult confirmacion = MessageBox.Show("¿Seguro que quieres invitar a: " + listaInvitaciones[0] + " a una partida 1vs1?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                //Envíamos el nombre de la persona que queremos invitar
                if (confirmacion == DialogResult.Yes)
                {
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("7/0/" + hostEnApp + "/" + listaInvitaciones[0] + "/");
                    server.Send(msg);
                    invitacionEnCurso = true;
                }
            }
            else if (listaInvitaciones.Count == 3) //Enviar invitación a 3 jugadores. Partida 2vs2
            {
                DialogResult confirmacion = MessageBox.Show("Aliado: " + listaInvitaciones[0] + "\nContrincantes: " + listaInvitaciones[1] + ", " + listaInvitaciones[2] + "\n¿Quieres crear la partida 2vs2?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                //Envíamos los nombres de las personas que queremos invitar

                if (confirmacion == DialogResult.Yes)
                {
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("7/3/" + hostEnApp + "/" + listaInvitaciones[0] + "/" + listaInvitaciones[1] + "/" + listaInvitaciones[2] + "/");
                    server.Send(msg);
                    duoPlayer1 = hostEnApp;
                    duoPlayer2 = listaInvitaciones[0];
                    duoPlayer3 = listaInvitaciones[1];
                    duoPlayer4 = listaInvitaciones[2];
                    listaInvitaciones.Clear();
                    contadorAceptaciones.Show();
                    contadorAceptaciones.Text = "Esperando... \nHan aceptado: 0/4";
                    contadorAceptados[Convert.ToInt32(idPartidaAux)]++;
                    DelegadoParaModificar3 delegadoContadorAceptaciones = new DelegadoParaModificar3(actualizarContadorAceptaciones);
                    Invoke(delegadoContadorAceptaciones, new Object[] { contadorAceptados[Convert.ToInt32(idPartidaAux)] });
                    invitacionEnCurso = true;
                }                
            }
            else
                MessageBox.Show("Las partidas son de 2 o 4 personas.\nTienes que invitar a un jugador más o eliminar a uno de la lista.");
            crearPartida.Hide();
            listaInvitaciones.Clear();
        }

        private void actualizarContadorAceptaciones(int contador) //Para mostrar el número de jugadores que han aceptado jugar la partida
        {
            contadorAceptaciones.Show();
            contadorAceptaciones.Text = "Esperando... \nHan aceptado: " + Convert.ToString(contador) + "/4";
        }

        private void mostrarListaConectados() //Muestra la lista de conectados
        {
            amigos.Show();
            listaconectados.Hide();
            OcultarAmigos.Hide();
        }

        private void actualizarFilasListaConectados(int contador) //Actualizar la cantidad de filas en dataGrid de la lista de conectados
        {
            listaconectados.RowCount = contador - 1;
        }

        private void limpiarFormularioConsulta() //Al hacer click en consultar, vaciar el textBox que se ha utilizado para hacer la consulta
        {
            p1.Clear();
        }

        private void enviarMensaje_Click(object sender, EventArgs e) //Enviar mensaje en el chat global
        {
            if (textoChat.Text != "")
            {
                string mensaje = "8/" + hostEnApp + "/" + textoChat.Text + "/";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                textoChat.Clear();
            }
        }

        public void controlChat(int codigo, string nombre, string mensajeChat) //Acciones en el chat global dependiendo del código seleccionado 
        {
            switch (codigo)
            {
                case 1: //Mostrar el chat
                    chat.Show();
                    textoChat.Show();
                    enviarMensaje.Show();
                    break;
                case 2: //Limpiar y ocultar el chat
                    chat.Hide();
                    chat.Items.Clear();
                    textoChat.Hide();
                    enviarMensaje.Hide();
                    break;
                case 3: //Mostrar mensaje recibido
                    chatAlert.controls.play();
                    chat.Items.Add(nombre + ": " + mensajeChat);                    
                    break;
                case 4://Respuestas de las consultas
                    consultasBox.Items.Add(mensajeChat);   
                    break;
                case 5: //Notificaciones del juego
                    chat.Items.Add(mensajeChat);
                    break;
            }
        }

        public void ocultarContadorAceptaciones() //Mostrar u ocultar cosas al empezar partida (Antiguo mostrar el tablero cuando se va a iniciar una partida)
        {
            contadorAceptaciones.Hide();
        }

        private void mostrarInvitacion() //Mostrar siguiente invitación cuando la actual ha dejado de estar pendiente de aceptar o rechazar 
        {
            int i = 0;
            bool encontrado = false;
            if (vistoBueno == true)
            {
                while (!encontrado)
                {
                    if (this.vectorInvitaciones[i].GetProcesada() == false)
                    {
                        this.vectorInvitaciones[i].SetProcesada(true);
                        pictureBoxInvitaciones.Show();
                        aceptarInvitacion.Show();
                        rechazarInvitacion.Show();
                        textoInvitacion.Text = this.vectorInvitaciones[i].GetMensajeInvitacion();
                        pictureBoxInvitaciones.Controls.Add(textoInvitacion);
                        textoInvitacion.Show();
                        encontrado = true;
                        vistoBueno = false;
                    }
                    else
                    {
                        i++;
                        if (i == 15)
                        {
                            contadorAceptaciones.Hide();
                            aceptarInvitacion.Hide();
                            rechazarInvitacion.Hide();
                            pictureBoxInvitaciones.Hide();
                            return;
                        }
                    }
                }
            }
        }

        private void ocultarInvitacion() //Ocultar el cuadro de invitación
        {
            contadorAceptaciones.Hide();
            aceptarInvitacion.Hide();
            rechazarInvitacion.Hide();
            pictureBoxInvitaciones.Hide();
        }

        private void aceptarInvitacion_Click(object sender, EventArgs e) //Enviar mensaje al servidor cuando un jugador acepta la partida
        {
            int i = 0;
            int j = 0;
            bool encontrado = false;
            while (!encontrado)
            {
                if (vectorInvitaciones[i].GetProcesada() == true && vectorInvitaciones[i].GetPendiente() == true)
                {
                    encontrado = true;
                    vectorInvitaciones[i].SetPendiente(false);
                    j = i;
                }
                i++;
            }
            invitacionEnCurso = false;
            vistoBueno = true; // Permite a la siguiente invitación ser procesada
            DelegadoParaModificar delegado2 = new DelegadoParaModificar(mostrarInvitacion);
            Invoke(delegado2);

            byte[] msg = System.Text.Encoding.ASCII.GetBytes("7/4/" + vectorInvitaciones[j].GetidPartida() + "/" + hostEnApp + "/");
            server.Send(msg);
        }

        private void rechazarInvitacion_Click(object sender, EventArgs e) //Enviar mensaje al servidor cuando un jugador rechaza la partida
        {
            contadorAceptados[Convert.ToInt32(idPartidaAux)] = 0;
            DelegadoParaModificar delegado = new DelegadoParaModificar(ocultarInvitacion);
            Invoke(delegado);
            vistoBueno = true; // Permite a la siguiente invitación ser procesada
            invitacionEnCurso = false;

            byte[] msg = System.Text.Encoding.ASCII.GetBytes("7/5/" + idPartidaAux + "/" + hostEnApp + "/");
            server.Send(msg);
        }

        private void mostrarConsultas_Click(object sender, EventArgs e) //Para mostrar/ocultar consultas
        {
            if (mostrarConsultas.Text == "Ocultar Consultas")
            {
                label1.Hide();
                consultasBox.Hide();
                labelConsultar.Hide();
                consulta1.Hide();
                consulta2.Hide();
                consulta3.Hide();
                consulta4.Hide();
                consulta5.Hide();
                p1.Hide();
                consultar.Hide();
                mostrarConsultas.Text = "Mostrar Consultas";
            }
            else
            {
                label1.Show();
                labelConsultar.Hide();
                consultasBox.Show();
                consulta1.Show();
                consulta2.Show();
                consulta3.Show();
                consulta4.Show();
                consulta5.Show();
                p1.Show();
                consultar.Show();
                mostrarConsultas.Text = "Ocultar Consultas";
            }
        }

        private void Principal_FormClosing(object sender, FormClosingEventArgs e) //Cerrar todo el programa de forma segura
        {

            string mensaje;
            if (this.conectadoConUsuario == true)
            {
                //Mensaje de desconexión
                mensaje = "0/" + hostEnApp + "/";

                if (threadOn == true)
                {
                    atender.Abort();
                }
            }
            else
            {
                mensaje = "12/";
            }
            if (this.conectadoSinUsuario == true || this.conectadoConUsuario == true)
            {
                if (invitacionEnCurso == true)
                {
                    byte[] msg2 = System.Text.Encoding.ASCII.GetBytes("7/5/" + idPartidaAux + "/" + hostEnApp + "/");
                    server.Send(msg2);
                }
                Thread.Sleep(100); //Retardo para que las peticiones no se solapen
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                server.Shutdown(SocketShutdown.Both);
                server.Close();
            }
        }

        private void cerrar_Click_1(object sender, EventArgs e) //Desconectarse y cerrar de forma segura un form
        {
            this.Close();
        }

        private void DarseBaja_Click(object sender, EventArgs e) //Ocultar y mostrar elementos a la hora de querer darse de baja
        {
            u.Clear();
            p.Clear();
            u.BackColor = Color.Silver;
            p.BackColor = Color.Silver;
            AceptarBaja.Show();
            DarseBaja.Hide();
            registrar.Hide();
            regAceptar.Hide();
            regCancel.Show();
            conectar.Hide();
            label4.Show();
            label4.Text = "Introduce el usuario y contraseña para darte de baja.";
        }

        private void Reconectar_Click(object sender, EventArgs e) //Restablecer conexión con el servidor
        {
            ConectarAlServidor();
        }     
        
        private void AtenderServidor() //Thread para escuchar y atender notificaciones entrantes
        {
            while (true)
            {           
                //Recibimos mensaje del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);
                string mensaje = trozos[1].Split('\0')[0];

                switch (codigo)
                {
                    //EMPIEZAN LAS CONSULTAS
                    case 1: //Para saber cuántos jugadores han usado la misma skin en una partida
                        DelegadoParaModificar delegado1 = new DelegadoParaModificar(limpiarFormularioConsulta);
                        DelegadoParaModificar2 del1 = new DelegadoParaModificar2(controlChat);
                        Invoke(del1, new Object[] { 4, "", "Respuesta de su consulta:" });
                        if (mensaje == "0")
                            Invoke(del1, new Object[] { 4, "", "No se han obtenido datos." });
                        else if (mensaje == "-1")
                            Invoke(del1, new Object[] { 4, "", "Error en la consulta!" });
                        else
                        {
                            string[] skin = mensaje.Split(',');
                            Invoke(del1, new Object[] { 4, "", "En la partida " + p1.Text + " la skin PRO la usaron: " + skin[0] + " jugadores" });
                            Invoke(del1, new Object[] { 4, "", " y la Noob: " + skin[1] + " jugadores." });
                        }
                        Invoke(delegado1);
                        break;

                    case 2: //Para saber los nombres de los jugadores que han obtenido más de X puntos
                        DelegadoParaModificar delegado2 = new DelegadoParaModificar(limpiarFormularioConsulta);
                        DelegadoParaModificar2 del2 = new DelegadoParaModificar2(controlChat);
                        Invoke(del2, new Object[] { 4, "", "Respuesta de su consulta:" });
                        if (mensaje == "0")
                            Invoke(del2, new Object[] { 4, "", "No se han obtenido datos." });
                        else if (mensaje == "-1")
                            Invoke(del2, new Object[] { 4, "", "Error en la consulta!" });
                        else
                        {
                            Invoke(del2, new Object[] { 4, "", "Los siguientes jugadores son los que tienen más de " });
                            Invoke(del2, new Object[] { 4, "", p1.Text + " puntos: " + mensaje });
                        }
                        Invoke(delegado2);
                        break;

                    case 3: //Para saber cuántas partidas ha ganado un jugador
                        DelegadoParaModificar delegado3 = new DelegadoParaModificar(limpiarFormularioConsulta);
                        DelegadoParaModificar2 del3 = new DelegadoParaModificar2(controlChat);
                        Invoke(del3, new Object[] { 4, "", "Respuesta de su consulta:" });
                        if (mensaje == "0")
                            Invoke(del3, new Object[] { 4, "", "No se han obtenido datos." });
                        else if (mensaje == "-1")
                            Invoke(del3, new Object[] { 4, "", "Error en la consulta!" });
                        else
                            Invoke(del3, new Object[] { 4, "", "El jugador " + p1.Text + " ha ganado: " + mensaje + " veces." });
                        Invoke(delegado3);
                        break;

                    case 14: //Saber con que jugadores el cliente ha jugado al menos una vez
                        DelegadoParaModificar delegado14 = new DelegadoParaModificar(limpiarFormularioConsulta);
                        DelegadoParaModificar2 del14 = new DelegadoParaModificar2(controlChat);
                        Invoke(del14, new Object[] { 4, "", "Respuesta de su consulta:" });
                        if (mensaje == "0")
                            Invoke(del14, new Object[] { 4, "", "No se han obtenido datos." });
                        else if (mensaje == "-1")
                            Invoke(del14, new Object[] { 4, "", "Error en la consulta!" });
                        else
                        {
                            string[] nombresConsulta = mensaje.Split(',').Distinct().ToArray();//Para eliminar los datos repetidos
                            int numTrozos = nombresConsulta.Length;//Para saber cuantas vueltas tiene que dar el bucle para plasmar la información
                            this.resultadoConsulta = "";
                            if (nombresConsulta[0] != hostEnApp)
                            {
                                this.resultadoConsulta = nombresConsulta[0];
                            }
                            for (int i = 1; i < numTrozos; i++)
                            {
                                if (nombresConsulta[i] != hostEnApp)
                                {
                                    this.resultadoConsulta = this.resultadoConsulta + ", " + nombresConsulta[i];
                                }
                            }
                            Invoke(del14, new Object[] { 4, "", "Has jugado con estos jugadores:" });
                            Invoke(del14, new Object[] { 4, "", this.resultadoConsulta });
                        }
                        Invoke(delegado14);
                        break;

                    case 15: //Saber la puntuación del cliente en las partidas jugadas con el jugador introducido
                        DelegadoParaModificar delegado15 = new DelegadoParaModificar(limpiarFormularioConsulta);
                        DelegadoParaModificar2 del15 = new DelegadoParaModificar2(controlChat);
                        Invoke(del15, new Object[] { 4, "", "Respuesta de su consulta:" });
                        if (mensaje == "0")
                            Invoke(del15, new Object[] { 4, "", "No se han obtenido datos." });
                        else if (mensaje == "-1")
                            Invoke(del15, new Object[] { 4, "", "Error en la consulta!" });
                        else
                        {
                            string[] datosConsulta1 = mensaje.Split('$');
                            string nJugador = datosConsulta1[0];
                            string[] datosConsulta2 = datosConsulta1[1].Split(',');
                            int cuantos = datosConsulta2.Length;//Para saber cuantas vueltas tiene que dar el bucle para plasmar la información

                            for (int j = 0; j < cuantos; j += 2)
                            {
                                Invoke(del15, new Object[] { 4, "", "En la partida " + datosConsulta2[j + 1] + " jugada con " + nJugador + " ha obtenido " });
                                Invoke(del15, new Object[] { 4, "", datosConsulta2[j] + " puntos." });
                            }
                        }
                        Invoke(delegado15);
                        break;

                    //ACABAN LAS CONSULTAS

                    case 5: //Mostrar la lista de conectados
                        string[] resultado = mensaje.Split(',');
                        int contador = Convert.ToInt32(resultado[0]);
                        listaconectados.ColumnCount = 1;
                        if (contador > 1)
                        {
                            DelegadoParaModificar3 delegado = new DelegadoParaModificar3(actualizarFilasListaConectados); //Delegado para poder mostrar los mensajes
                            Invoke(delegado, new Object[] { contador });
                        }
                        listaconectados.ColumnHeadersVisible = false;
                        listaconectados.RowHeadersVisible = false;
                        //listaconectados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                        if (contador > 1) //"Si hay más de un jugador conectado, entonces..."
                        {
                            mostrar = true;
                            int i = 1; //contador para recorrer el vector de strings
                            int cont = 0; //contador para indicar la posicion en la tabla
                            while (i <= 2 * contador) //Porque i aumenta de 2 en 2 para evitar los sockets que también recibimos, de esta forma dará tantas vueltas como conectados hay
                            {
                                if (resultado[i] != hostEnApp)
                                {
                                    listaconectados[0, cont].Value = resultado[i];
                                    cont++;
                                }
                                i += 2; //Así nos aseguramos de que i solo coincidirá con las posiciones donde hay nombres de jugadores
                            }
                        }
                        else //Si no, avisa de que no hay ningún jugador conectado
                        {
                            mostrar = false;
                            DelegadoParaModificar delegado = new DelegadoParaModificar(mostrarListaConectados);
                            Invoke(delegado);
                        }
                        break;

                    case 7: //Invitaciones
                        string[] mensaje2 = mensaje.Split('$');
                        string[] auxInvitacion;
                        int auxSubSubCodigo;
                        int subcodigo = Convert.ToInt32(mensaje2[0]);

                        if (subcodigo == 0) //El jugador recibe una invitación para jugar
                        {
                            string[] invitacion = mensaje2[1].Split(',');
                            string idPartida = invitacion[0];
                            idPartidaAux = idPartida;
                            Player2 = invitacion[1];
                            DialogResult confirmacion = MessageBox.Show("El jugador " + invitacion[1] + " te ha invitado a la partida: " + idPartida + "\n¿Quieres aceptar?", "Invitación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            invitacionEnCurso = true;

                            if (confirmacion == DialogResult.Yes)
                            {
                                idPartidaChat = idPartida;
                                DelegadoParaModificar delegado = new DelegadoParaModificar(ocultarContadorAceptaciones);
                                Invoke(delegado);
                                string jugadores = Player2 + "/" + hostEnApp + "/2";
                                ejecutarTablero(Convert.ToInt32(idPartida), jugadores);

                                byte[] msg = System.Text.Encoding.ASCII.GetBytes("7/1/" + idPartida + "/");
                                server.Send(msg);
                            }
                            if (confirmacion == DialogResult.No)
                            {
                                byte[] msg = System.Text.Encoding.ASCII.GetBytes("7/2/" + idPartida + "/");
                                server.Send(msg);
                            }
                        }
                        if (subcodigo == 1) //El invitado acepta jugar
                        {
                            string[] invitacion = mensaje2[1].Split(',');
                            string idPartida = invitacion[0];
                            idPartidaAux = idPartida;
                            Player2 = invitacion[1];
                            idPartidaChat = idPartida;
                            DelegadoParaModificar delegado = new DelegadoParaModificar(ocultarContadorAceptaciones);
                            Invoke(delegado);
                            DelegadoParaModificar2 delegado4 = new DelegadoParaModificar2(controlChat); //Delegado para mostrar el chat en caso de que el invitado haya aceptado la invitación
                            Invoke(delegado4, new Object[] { 3, "BOT", "¡" + Player2 + " ha aceptado jugar contigo!" });
                            string jugadores = hostEnApp + "/" + Player2 + "/2";
                            ejecutarTablero(Convert.ToInt32(idPartida), jugadores);
                        }
                        if (subcodigo == 2) //El invitado rechaza jugar
                        {
                            string[] invitacion = mensaje2[1].Split(',');
                            auxInvitacion = invitacion;
                            string invitado = invitacion[0];
                            MessageBox.Show("Lo sentimos, " + invitado + " no quiere jugar contigo :(");
                        }

                        if (subcodigo == 3) //El jugador recibe una invitación para jugar a 2vs2
                        {
                            int subSubcodigo = Convert.ToInt32(mensaje2[1]);
                            auxSubSubCodigo = subSubcodigo;
                            string[] invitacion = mensaje2[2].Split(',');
                            string idPartida = invitacion[0];
                            contadorAceptados[Convert.ToInt32(idPartida)] = 1;
                            DelegadoParaModificar3 delegadoContadorAceptaciones = new DelegadoParaModificar3(actualizarContadorAceptaciones);
                            Invoke(delegadoContadorAceptaciones, new Object[] { contadorAceptados[Convert.ToInt32(idPartida)] });
                            DelegadoParaModificar delegadoInvitaciones = new DelegadoParaModificar(mostrarInvitacion);
                            invitacionEnCurso = true;

                            if (subSubcodigo == 2)//Lo que pasa en la parte del aliado
                            {
                                duoPlayer1 = invitacion[1];
                                duoPlayer2 = hostEnApp;
                                duoPlayer3 = invitacion[2];
                                duoPlayer4 = invitacion[3];
                                idPartidaAux = idPartida; //Envia la id de partida para los botones aceptar y rechazar
                                string mensajeInvitacion = "Invitación recibida para partida con id " + idPartida + " para \njugar con " + duoPlayer1 + " como  su aliado contra:\n " + duoPlayer3 + "\n" + duoPlayer4 + "\n¿Quieres aceptar?";
                                this.vectorInvitaciones[Convert.ToInt32(idPartida)].SetOrden(2);
                                this.vectorInvitaciones[Convert.ToInt32(idPartida)].SetidPartida(Convert.ToInt32(idPartida));
                                this.vectorInvitaciones[Convert.ToInt32(idPartida)].SetProcesada(false);
                                this.vectorInvitaciones[Convert.ToInt32(idPartida)].SetPendiente(true);
                                this.vectorInvitaciones[Convert.ToInt32(idPartida)].SetMensajeInvitacion(mensajeInvitacion);
                                Invoke(delegadoInvitaciones);
                            }

                            if (subSubcodigo == 3)//Lo que pasa en la parte del contrincante 1
                            {
                                duoPlayer1 = invitacion[1];
                                duoPlayer2 = invitacion[2];
                                duoPlayer3 = hostEnApp;
                                duoPlayer4 = invitacion[3];
                                idPartidaAux = idPartida; //Envia la id de partida para los botones aceptar y rechazar
                                string mensajeInvitacion = "Invitación recibida para partida con id " + idPartida + " para \njugar con " + duoPlayer4 + " como  su aliado contra:\n " + duoPlayer1 + "\n" + duoPlayer2 + "\n¿Quieres aceptar?";
                                this.vectorInvitaciones[Convert.ToInt32(idPartida)].SetOrden(3);
                                this.vectorInvitaciones[Convert.ToInt32(idPartida)].SetidPartida(Convert.ToInt32(idPartida));
                                this.vectorInvitaciones[Convert.ToInt32(idPartida)].SetProcesada(false);
                                this.vectorInvitaciones[Convert.ToInt32(idPartida)].SetPendiente(true);
                                this.vectorInvitaciones[Convert.ToInt32(idPartida)].SetMensajeInvitacion(mensajeInvitacion);                                
                                Invoke(delegadoInvitaciones);
                            }
                            if (subSubcodigo == 4)//Lo que pasa en la parte del contrincante 2
                            {
                                duoPlayer1 = invitacion[1];
                                duoPlayer2 = invitacion[2];
                                duoPlayer3 = invitacion[3];
                                duoPlayer4 = hostEnApp;
                                idPartidaAux = idPartida; //Envia la id de partida para los botones aceptar y rechazar
                                string mensajeInvitacion = "Invitación recibida para partida con id " + idPartida + " para \njugar con " + duoPlayer3 + " como  su aliado contra:\n " + duoPlayer1 + "\n" + duoPlayer2 + "\n¿Quieres aceptar?";
                                this.vectorInvitaciones[Convert.ToInt32(idPartida)].SetOrden(4);
                                this.vectorInvitaciones[Convert.ToInt32(idPartida)].SetidPartida(Convert.ToInt32(idPartida));
                                this.vectorInvitaciones[Convert.ToInt32(idPartida)].SetProcesada(false);
                                this.vectorInvitaciones[Convert.ToInt32(idPartida)].SetPendiente(true);
                                this.vectorInvitaciones[Convert.ToInt32(idPartida)].SetMensajeInvitacion(mensajeInvitacion);                                
                                Invoke(delegadoInvitaciones);
                            }
                        }
                        if (subcodigo == 4) //Los invitados aceptan jugar
                        {
                            string[] invitacion = mensaje2[1].Split(',');
                            int idPartida = Convert.ToInt32(invitacion[0]);
                            contadorAceptados[Convert.ToInt32(idPartida)] = 0;
                            DelegadoParaModificar delegado = new DelegadoParaModificar(ocultarContadorAceptaciones);
                            Invoke(delegado);
                            DelegadoParaModificar2 delegado4 = new DelegadoParaModificar2(controlChat); //Delegado para mostrar el chat en caso de que el invitado haya aceptado la invitación
                            Invoke(delegado4, new Object[] { 3, "BOT", "¡Todos los jugadores han aceptado la partida!" });
                            if (vectorInvitaciones[idPartida].GetOrden() == 2)
                            {
                                string jugadores = duoPlayer1 + "/" + duoPlayer2 + "/" + duoPlayer3 + "/" + duoPlayer4 + "/2";
                                ejecutarTablero(idPartida, jugadores);
                            }
                            if (vectorInvitaciones[idPartida].GetOrden() == 3)
                            {
                                string jugadores = duoPlayer1 + "/" + duoPlayer2 + "/" + duoPlayer3 + "/" + duoPlayer4 + "/3";
                                ejecutarTablero(idPartida, jugadores);
                            }
                            if (vectorInvitaciones[idPartida].GetOrden() == 4)
                            {
                                string jugadores = duoPlayer1 + "/" + duoPlayer2 + "/" + duoPlayer3 + "/" + duoPlayer4 + "/4";
                                ejecutarTablero(idPartida, jugadores);
                            }
                            if (vectorInvitaciones[idPartida].GetOrden() == 0)
                            {
                                string jugadores = duoPlayer1 + "/" + duoPlayer2 + "/" + duoPlayer3 + "/" + duoPlayer4 + "/1";
                                ejecutarTablero(idPartida, jugadores);
                            }
                        }
                        if (subcodigo == 5) //Los invitados rechazan jugar
                        {
                            string[] invitacion = mensaje2[1].Split(',');
                            int idPartida = Convert.ToInt32(invitacion[0]);
                            DelegadoParaModificar delegado = new DelegadoParaModificar(ocultarInvitacion);
                            Invoke(delegado);
                            contadorAceptados[idPartida] = 0;
                            DelegadoParaModificar2 delegado6 = new DelegadoParaModificar2(controlChat); //Delegado para poder mostrar los mensajes
                            Invoke(delegado6, new Object[] { 5, null, "Un jugador ha rechazado." });
                            invitacionEnCurso = false;
                        }
                        if (subcodigo == 6) //Contador de cuántas personas han aceptado jugar
                        {
                            int idPartida = Convert.ToInt32(mensaje2[1]);
                            contadorAceptados[idPartida]++;
                            DelegadoParaModificar3 delegado = new DelegadoParaModificar3(actualizarContadorAceptaciones);
                            Invoke(delegado, new Object[] { contadorAceptados[idPartida] });
                        }
                        break;

                    case 8: //Chat Global
                        string[] mensaje3 = mensaje.Split('$');
                        string nombre = mensaje3[0];
                        string mensajeChat = mensaje3[1];
                        DelegadoParaModificar2 delegado5 = new DelegadoParaModificar2(controlChat); //Delegado para poder mostrar los mensajes
                        Invoke(delegado5, new Object[] { 3, nombre, mensajeChat });
                        break;

                    case 9: //Chat Partida                   
                        string[] mensajePartida = mensaje.Split('$');
                        int idP = Convert.ToInt32(mensajePartida[0]);
                        string mensajeChatPartida = mensajePartida[1] + "$" + mensajePartida[2];
                        vectorForms[idP].SetMensajeChat(mensajeChatPartida);
                        break;

                    case 10: //Aviso de quien se conecta o desconecta
                        string[] notiConect = mensaje.Split(',');
                        if (notiConect[0] == "1")
                        {
                            DelegadoParaModificar2 delegado6 = new DelegadoParaModificar2(controlChat);
                            Invoke(delegado6, new Object[] { 5, null, "BOT: " + notiConect[1] + " se ha conectado." });
                        }
                        else
                        {
                            DelegadoParaModificar2 delegado7 = new DelegadoParaModificar2(controlChat);
                            Invoke(delegado7, new Object[] { 5, null, "BOT: " + notiConect[1] + " se ha desconectado." });
                        }
                        break;

                    case 13: //Avisar a los jugadores de que alguien ha abandonado la partida y anularla
                        int nForm = Convert.ToInt32(mensaje);
                        vectorForms[nForm].cerrarForm();
                        DelegadoParaModificar2 delegado13 = new DelegadoParaModificar2(controlChat);
                        Invoke(delegado13, new Object[] { 3, "BOT", "Alguien ha abandonado la partida." });
                        break;

                    case 16: //El anfitrión recibe la ID de la partida
                        idPartidaAux = mensaje;
                        break;

                    case 30: //Recibimos cual es la carta de nuestro contrincante y se las pasamos al form de partida correspondiente
                        string[] nCarta = mensaje.Split(',');
                        vectorForms[Convert.ToInt32(nCarta[0])].asignarCartaContrincante(Convert.ToInt32(nCarta[1]));
                        break;

                    case 31: //Recibimos las cartas de los jugadores y se las pasamos al form de partida correspondiente
                        string[] nCartas = mensaje.Split(',');
                        vectorForms[Convert.ToInt32(nCartas[0])].asignarCartas(Convert.ToInt32(nCartas[1]), nCartas[2]);
                        break;

                    case 32: //Recibimos el daño causado por ataque físico a la partida correspondiente
                        string[] infoAtaque = mensaje.Split(',');
                        vectorForms[Convert.ToInt32(infoAtaque[0])].asignarAtaqueFisico(Convert.ToInt32(infoAtaque[1]), infoAtaque[2]);
                        break;

                    case 33: //Recibimos el daño causado por hechizo a la partida correspondiente
                        string[] infoHechizo = mensaje.Split(',');
                        vectorForms[Convert.ToInt32(infoHechizo[0])].asignarHechizo(Convert.ToInt32(infoHechizo[1]), infoHechizo[2]);
                        break;

                    case 34: //Avisar a los jugadores de que alguien ha abandonado la partida y anularla
                        int numForm = Convert.ToInt32(mensaje);
                        vectorForms[numForm].cerrarForm();
                        DelegadoParaModificar2 delegado34 = new DelegadoParaModificar2(controlChat);
                        Invoke(delegado34, new Object[] { 3, "BOT", "La partida ha finalizado." });
                        break;
                }
            }
        }
    }
}