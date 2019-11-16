using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Windows;
using WMPLib;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class formTablero : Form
    {
        Random random = new Random();//Lo usaremos para elegir las cartas de forma aleatoria a la hora de empezar la partida
        PictureBox[] cartas_picture_vector = new PictureBox[MAX]; //Vector con los picture boxes de las cartas
        Cartas[] cartas = new Cartas[8]; //Vector para cargar las cartas
        Cartas[] cartasPartida = new Cartas[4]; //Vector para guardar las cartas de los jugadores en la partida
        WindowsMediaPlayer Reproductor = new WindowsMediaPlayer(); //Añadir sonidos al programa
        string Directorio = Directory.GetCurrentDirectory(); //Buscar la ruta del programa para la ejecución universal de los archivos

        int cont = 0;
        int indice;
        int tiempoEspera = 0;
        int tiempoEspera2 = 0;
        int cartaEscogida1;
        const int MAX = 8;
        int nForm;
        Socket server;
        delegate void DelegadoParaChat(string nombre, string mensaje);
        delegate void DelegadoParaCerrar();
        delegate void DelegadoParaMostrarCarta(int num);
        delegate void DelegadoParaMostrarCartas(int num, string nombre);
        string hostEnApp;
        string anfitrionPartida;
        string Player2;
        string Player3;
        string Player4;
        string nombreGanador;
        int cantidadJugadores;
        bool permisoParaCoger = true; //Permitir o no coger otra carta del mazo
        bool permisoParaAtacar = false; //Permiso para poder atacar o no
        bool cargaCorrecta; //verificar la carga correcta de los atributos de las cartas
        bool securityClose = false; //Cierre seguro del tablero.
        bool partidaGanada; //Se activa cuando alguien gana la partida. Cierre seguro del tablero.

        public formTablero(int nForm, Socket server, string hostEnApp, string jugadores) //Recupera los datos del form principal
        {
            InitializeComponent();
            this.nForm = nForm; //Corresponde al id de la partida
            this.server = server;
            this.hostEnApp = hostEnApp; //Variable para salvar el nombre de quien acaba de iniciar sesión
            string[] trozos = jugadores.Split('/');
            if (trozos[2] == "2")
            {
                this.anfitrionPartida = trozos[0];
                this.Player2 = trozos[1];
                this.cantidadJugadores = 2;
                anfitrion.Text = this.anfitrionPartida;
                contrincante1.Text = this.Player2;
                aliado.Hide();
                contrincante2.Hide();
                vidaAliado.Hide();
                vidaContrincante2.Hide();
                resultadoPartida.Hide();
                if (anfitrionPartida == hostEnApp)
                {
                    anfitrion.ForeColor = Color.LimeGreen;
                    contrincante1.ForeColor = Color.White;
                }
                else
                {
                    contrincante1.ForeColor = Color.LimeGreen;
                    anfitrion.ForeColor = Color.White;
                }
            }
            else //Se van a asignar las posiciones de los jugadores en el orden correspondiente
            {
                if (trozos[4] == "1")
                {
                    this.anfitrionPartida = trozos[0];
                    this.Player2 = trozos[1];
                    this.Player3 = trozos[2];
                    this.Player4 = trozos[3];
                    anfitrion.ForeColor = Color.LimeGreen;
                    aliado.ForeColor = Color.Yellow;
                    contrincante1.ForeColor = Color.White;
                    contrincante2.ForeColor = Color.White;
                }
                if (trozos[4] == "2")
                {
                    this.anfitrionPartida = trozos[0];
                    this.Player2 = trozos[1];
                    this.Player3 = trozos[2];
                    this.Player4 = trozos[3];
                    aliado.ForeColor = Color.LimeGreen;
                    anfitrion.ForeColor = Color.Yellow;
                    contrincante1.ForeColor = Color.White;
                    contrincante2.ForeColor = Color.White;
                }
                if (trozos[4] == "3")
                {
                    this.anfitrionPartida = trozos[0];
                    this.Player2 = trozos[1];
                    this.Player3 = trozos[2];
                    this.Player4 = trozos[3];
                    contrincante1.ForeColor = Color.LimeGreen;
                    contrincante2.ForeColor = Color.Yellow;
                    anfitrion.ForeColor = Color.White;
                    aliado.ForeColor = Color.White;
                }
                if (trozos[4] == "4")
                {
                    this.anfitrionPartida = trozos[0];
                    this.Player2 = trozos[1];
                    this.Player3 = trozos[2];
                    this.Player4 = trozos[3];
                    contrincante2.ForeColor = Color.LimeGreen;
                    contrincante1.ForeColor = Color.Yellow;
                    anfitrion.ForeColor = Color.White;
                    aliado.ForeColor = Color.White;
                }
                cantidadJugadores = 4;
                resultadoPartida.Hide();
                menu.Hide();
                anfitrion.Text = this.anfitrionPartida;
                aliado.Text = this.Player2;
                contrincante1.Text = this.Player3;
                contrincante2.Text = this.Player4;
            }
            CargarCartas(); //Jugadores colocados, se cargan los datos de las cartas
        }

        public void CargarCartas() //Método cargar el fichero con los datos de las cartas
        {
            string[] campos = new string[7];
            string linealeida;
            StreamReader Fdatos;
            int i = 0;
            this.cargaCorrecta = true;
            string name = @"" + Directorio + "/Cartas.txt";

            try
            {
                Fdatos = new StreamReader(name);
                linealeida = Fdatos.ReadLine();

                while (linealeida != null)
                {
                    campos = linealeida.Split('/');
                    this.cartas[i] = new Cartas();
                    this.cartas[i].SetHp(Convert.ToInt32(campos[0]));
                    this.cartas[i].SetidCarta(Convert.ToInt32(campos[1]));
                    this.cartas[i].SetNombre(campos[2]);
                    this.cartas[i].SetHechizo(Convert.ToInt32(campos[3]));
                    this.cartas[i].SetAtaqueFisico(Convert.ToInt32(campos[4]));
                    this.cartas[i].SetNombreH(campos[5]);
                    this.cartas[i].SetNombreF(campos[6]);

                    linealeida = Fdatos.ReadLine();
                    i++;
                }
                Fdatos.Close();
            }
            catch (FileNotFoundException)
            {
                this.cargaCorrecta = false;
            }
            catch (FormatException)
            {
                this.cargaCorrecta = false;
            }
            catch (IndexOutOfRangeException)
            {
                this.cargaCorrecta = false;
            }
            if (this.cargaCorrecta == false)
                MessageBox.Show("Error al cargar los datos de las cartas.");
        }

        public void asignarCartaContrincante(int nCarta) //Muestra la carta de nuestro contrincante
        {
            DelegadoParaMostrarCarta delegadoCarta = new DelegadoParaMostrarCarta(mostrarCartaContrincante);
            Invoke(delegadoCarta, new Object[] { nCarta });
        }

        private void mostrarCartaContrincante(int nCarta) //Muestra la carta de nuestro contrincante
        {
            if (anfitrionPartida == hostEnApp) //Si nosotros somos los anfitriones, la carta que llega, sí o sí es del rival
            {
                picCard2.ImageLocation = Directorio + "/Cartas/card" + nCarta + ".PNG";
                timer2.Enabled = true;
                cartasPartida[1] = new Cartas();
                cartasPartida[1] = cartas[nCarta - 1];
                vidaContrincante1.Text = "Vida: " + cartasPartida[1].GetHp();
            }
            else
            {
                picCard1.ImageLocation = Directorio + "/Cartas/card" + nCarta + ".PNG";
                timer1.Enabled = true;
                cartasPartida[0] = new Cartas();
                cartasPartida[0] = cartas[nCarta - 1];
                vidaAnfitrion.Text = "Vida: " + cartasPartida[0].GetHp();
            }
            permisoParaAtacar = true;
        }

        public void asignarCartas(int nCarta, string nombre) //Recibimos información sobre las cartas de los oponentes
        {
            DelegadoParaMostrarCartas delegadoCarta = new DelegadoParaMostrarCartas(mostrarCartas);
            Invoke(delegadoCarta, new Object[] { nCarta, nombre });
        }

        private void mostrarCartas(int nCarta, string nombre) //Muestra la carta de nuestros contrincantes
        {
            if (nombre == anfitrionPartida)
            {
                picCard1.ImageLocation = Directorio + "/Cartas/card" + nCarta + ".PNG";
                picCard1.Tag = nCarta - 1;
                timer3.Enabled = true;
            }

            if (nombre == Player2)
            {
                picCard2.ImageLocation = Directorio + "/Cartas/card" + nCarta + ".PNG";
                picCard2.Tag = nCarta - 1;
                timer4.Enabled = true;
            }

            if (nombre == Player3)
            {
                picCard3.ImageLocation = Directorio + "/Cartas/card" + nCarta + ".PNG";
                picCard3.Tag = nCarta - 1;
                timer5.Enabled = true;
            }

            if (nombre == Player4)
            {
                picCard4.ImageLocation = Directorio + "/Cartas/card" + nCarta + ".PNG";
                picCard4.Tag = nCarta - 1;
                timer6.Enabled = true;
            }
            permisoParaAtacar = true;
        }

        private void Tablero_Load(object sender, EventArgs e) //Carga el tablero de juego
        {
            Reproductor.URL = @"" + Directorio + "/BackgroundMusic.mp3";
            Reproductor.controls.play();
            fisico.Hide();
            magico.Hide();
            pasar.Hide();

        }

        public void SetMensajeChat(string mensajeChat) //Recibe los mensajes para el chat de partida
        {
            string[] trozos = mensajeChat.Split('$');
            DelegadoParaChat delegado = new DelegadoParaChat(añadirMensaje);
            Invoke(delegado, new Object[] { trozos[0], trozos[1] });
        }

        private void añadirMensaje(string nombre, string mensaje) //Función para añadir mensaje en un chat
        {
            chatPartida.Items.Add(nombre + ": " + mensaje);
        }

        //Funciones de Ataques
        public void asignarAtaqueFisico(int ataque, string nombre) //Recibimos información sobre un ataque y ordenamos ejecutarlo
        {
            DelegadoParaMostrarCartas delegado = new DelegadoParaMostrarCartas(ejecutarAtaqueFisico);
            Invoke(delegado, new Object[] { ataque, nombre });
            this.nombreGanador = nombre;
        }

        public void ejecutarAtaqueFisico(int ataque, string nombre) //Recibimos información sobre el ataque y mostramos los resutados de este sobre los jugadores
        {
            DelegadoParaChat delegado = new DelegadoParaChat(añadirMensaje);
            if (nombre == anfitrionPartida)
            {
                cartasPartida[1].SetHp(cartasPartida[1].GetHp() - ataque);
                this.indice = 1;
                if (cartasPartida[1].GetHp() > 0)
                {
                    vidaContrincante1.Text = "Vida: " + Convert.ToString(cartasPartida[1].GetHp());
                    timerAtaque2.Enabled = true;
                }
                else //El hostenAPP ha ganado la partida
                {
                    partidaGanada = true;
                    vidaContrincante1.ForeColor = Color.Red;
                    vidaContrincante1.Text = "Vida: 0";
                    string mensaje = "34/" + nForm + "/" + cartasPartida[0].GetHp() + "/" + anfitrionPartida + "/" + Player2;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    ocultarBotones();
                    if (anfitrionPartida == hostEnApp)
                        resultadoPartida.Text = "Victoria";
                    else
                        resultadoPartida.Text = "Derrota";
                }
            }
            else if (nombre == Player2)
            {
                cartasPartida[0].SetHp(cartasPartida[0].GetHp() - ataque);
                this.indice = 0;
                if (cartasPartida[0].GetHp() > 0)
                {
                    vidaAnfitrion.Text = "Vida: " + Convert.ToString(cartasPartida[0].GetHp());
                    timerAtaque.Enabled = true;
                }
                else //El hostenApp ha perdido la partida
                {
                    partidaGanada = true;
                    vidaAnfitrion.ForeColor = Color.Red;
                    vidaAnfitrion.Text = "Vida: 0";
                    string mensaje = "34/" + nForm + "/" + cartasPartida[1].GetHp() + "/" + Player2 + "/" + anfitrionPartida;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    ocultarBotones();
                    if (Player2 == hostEnApp)
                        resultadoPartida.Text = "Victoria";
                    else
                        resultadoPartida.Text = "Derrota";
                }
            }
            Invoke(delegado, new Object[] { "BOT", "El jugador " + nombre + " ha ejecutado " + cartasPartida[this.indice].GetNombreF() + " restando " + Convert.ToString(ataque) + " de vida al contrincante." });
        }

        public void asignarHechizo(int ataque, string nombre) //Recibimos información sobre un hechizo
        {
            DelegadoParaMostrarCartas delegado = new DelegadoParaMostrarCartas(ejecutarHechizo);
            Invoke(delegado, new Object[] { ataque, nombre });
            this.nombreGanador = nombre;
        }

        public void ejecutarHechizo(int ataque, string nombre) //Recibimos información sobre el ataque y mostramos los resutados de este sobre los jugadores
        {
            DelegadoParaChat delegado = new DelegadoParaChat(añadirMensaje);
            if (nombre == anfitrionPartida)
            {
                cartasPartida[1].SetHp(cartasPartida[1].GetHp() - ataque);
                this.indice = 1;
                if (cartasPartida[1].GetHp() > 0) //El jugador pierde vida
                {
                    vidaContrincante1.Text = "Vida: " + Convert.ToString(cartasPartida[1].GetHp());
                }
                else //hostenApp ha ganado la partida
                {
                    partidaGanada = true;
                    vidaContrincante1.ForeColor = Color.Red;
                    vidaContrincante1.Text = "Vida: 0";
                    string mensaje = "34/" + nForm + "/" + cartasPartida[0].GetHp() + "/" + anfitrionPartida + "/" + Player2;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    ocultarBotones();
                    if (anfitrionPartida == hostEnApp)
                        resultadoPartida.Text = "Victoria";
                    else
                        resultadoPartida.Text = "Derrota";
                }
            }
            else if (nombre == Player2)
            {
                cartasPartida[0].SetHp(cartasPartida[0].GetHp() - ataque);
                this.indice = 0;
                if (cartasPartida[0].GetHp() > 0) //El jugador pierde vida
                {
                    vidaAnfitrion.Text = "Vida: " + Convert.ToString(cartasPartida[0].GetHp());
                }
                else //HostenApp ha perdido la partida
                {
                    partidaGanada = true;
                    vidaAnfitrion.ForeColor = Color.Red;
                    vidaAnfitrion.Text = "Vida: 0";
                    string mensaje = "34/" + nForm + "/" + cartasPartida[1].GetHp() + "/" + Player2 + "/" + anfitrionPartida;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    ocultarBotones();
                    if (Player2 == hostEnApp)
                        resultadoPartida.Text = "Victoria";
                    else
                        resultadoPartida.Text = "Derrota";
                }
            }
            Invoke(delegado, new Object[] { "BOT", "El jugador " + nombre + " ha ejecutado " + cartasPartida[this.indice].GetNombreH() + " restando " + Convert.ToString(ataque) + " de vida al contrincante." });
        }

        private void ocultarBotones() //Ocultar los botones al finalizar la partida para evitar un uso inadecuado
        {
            menu.Hide();
            fisico.Hide();
            magico.Hide();
            pasar.Hide();
            Salir.Hide();
            resultadoPartida.Show();
            this.tiempoEspera = 0;
            timerRetardo.Enabled = true;
        }

        //A continuación aparecen 4 funciones que permiten la movilidad de las cartas en 1vs1

        private void MoverCarta1X()
        {
            picCard1.Left = picCard1.Left - 7;
        }

        private void MoverCarta1Y()
        {
            picCard1.Top = picCard1.Top + 6;
        }

        private void MoverCarta2X()
        {
            picCard2.Left = picCard2.Left + 7;
        }

        private void MoverCarta2Y()
        {
            picCard2.Top = picCard2.Top + 6;
        }

        //A continuación aparecen 8 funciones que permiten la movilidad de las cartas en 2vs2

        private void MoverCarta3X()
        {
            picCard1.Left = picCard1.Left - 7;
        }

        private void MoverCarta3Y()
        {
            picCard1.Top = picCard1.Top + 6;
        }

        private void MoverCarta4X()
        {
            picCard2.Left = picCard2.Left - 7;
        }

        private void MoverCarta4Y()
        {
            picCard2.Top = picCard2.Top + 6;
        }

        private void MoverCarta5X()
        {
            picCard3.Left = picCard3.Left + 7;
        }

        private void MoverCarta5Y()
        {
            picCard3.Top = picCard3.Top + 6;
        }

        private void MoverCarta6X()
        {
            picCard4.Left = picCard4.Left + 7;
        }

        private void MoverCarta6Y()
        {
            picCard4.Top = picCard4.Top + 6;
        }

        ///////////////////////////////////////////////////////////////////////////

        public void StopSound() //Para poder parar la música desde cualquier form por si a caso el jugador cierra el juego desde la x de la ventana en lugar del botón.
        {
            Reproductor.controls.stop();
        }

        private void Reproducir_Click(object sender, EventArgs e) //Reactivar el hilo músical
        {
            if (Reproducir.Text == "Desactivar Música")
            {
                Reproductor.controls.pause();
                Reproducir.Text = "Activar Música";
            }
            else
            {
                Reproducir.Text = "Desactivar Música";
                Reproductor.controls.play();
            }
        }

        private void Salir_Click(object sender, EventArgs e) //Salir de la partida y volver a la ventana principal
        {
            string mensaje = "13/" + this.nForm;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            DelegadoParaCerrar delegadoCerrar = new DelegadoParaCerrar(abandonarPartida);
            Invoke(delegadoCerrar);
        }

        private void abandonarPartida() //Función para cerrar el form en caso de abandono de partida
        {
            this.Close();
        }

        public void cerrarForm() //Función para invocar la función abandonarPartida
        {
            securityClose = true;
            DelegadoParaCerrar delegadoCerrar = new DelegadoParaCerrar(abandonarPartida);
            Invoke(delegadoCerrar);
        }

        private void eventoClick(object sender, EventArgs e) //Listar los datos de una carta al hacer click sobre ella
        {
            PictureBox p = (PictureBox)sender;
            int tag = (int)p.Tag;
            MessageBox.Show("Nombre: " + cartas[tag].GetNombre() + "\n\n-HP: " + cartas[tag].GetHp() + "\n\n-Ataque Mágico: " + cartas[tag].GetNombreH() + "\n Daño: " + cartas[tag].GetHechizo() + "\n\n-Ataque Físico: " + cartas[tag].GetNombreF() + "\n Daño: " + cartas[tag].GetAtaqueFisico(), "Atributos");
        }

        private void PickCard_Click(object sender, EventArgs e) //Escoger las cartas del jugador de forma aleatoria y de 1 en 1
        {
            int mazo1, mazo2, mazo3, mazo4;
            mazo1 = random.Next(1, 9);
            mazo2 = random.Next(1, 9);
            mazo2 = random.Next(1, 9);
            mazo2 = random.Next(1, 9);
            mazo3 = random.Next(1, 9);
            mazo3 = random.Next(1, 9);
            mazo4 = random.Next(1, 9);

            if (!permisoParaCoger)
                chatPartida.Items.Add("No puedes coger más cartas, tu carta actual aún está viva.");
            else
            {
                if (permisoParaCoger && cantidadJugadores == 2 && anfitrionPartida == hostEnApp)
                {
                    switch (mazo1)
                    {
                        case 1:
                            picCard1.ImageLocation = Directorio + "/Cartas/card1.PNG";
                            break;
                        case 2:
                            picCard1.ImageLocation = Directorio + "/Cartas/card2.PNG";
                            break;
                        case 3:
                            picCard1.ImageLocation = Directorio + "/Cartas/card3.PNG";
                            break;
                        case 4:
                            picCard1.ImageLocation = Directorio + "/Cartas/card4.PNG";
                            break;
                        case 5:
                            picCard1.ImageLocation = Directorio + "/Cartas/card5.PNG";
                            break;
                        case 6:
                            picCard1.ImageLocation = Directorio + "/Cartas/card6.PNG";
                            break;
                        case 7:
                            picCard1.ImageLocation = Directorio + "/Cartas/card7.PNG";
                            break;
                        case 8:
                            picCard1.ImageLocation = Directorio + "/Cartas/card8.PNG";
                            break;
                    }
                    permisoParaCoger = false;
                    cont++;
                    string mensaje = "30/" + this.nForm + "/" + mazo1;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    cartaEscogida1 = mazo1;
                    cartas_picture_vector[1] = picCard1;
                    picCard1.Tag = mazo1 - 1;
                    picCard1.Click += new System.EventHandler(this.eventoClick);
                    timer1.Enabled = true;
                    cartasPartida[0] = new Cartas();
                    cartasPartida[0] = cartas[mazo1 - 1];
                    vidaAnfitrion.Text = "Vida: " + cartasPartida[0].GetHp();
                }
                else if (permisoParaCoger && cantidadJugadores == 2 && anfitrionPartida != hostEnApp)
                {
                    switch (mazo2)
                    {
                        case 1:
                            picCard2.ImageLocation = Directorio + "/Cartas/card1.PNG";
                            break;
                        case 2:
                            picCard2.ImageLocation = Directorio + "/Cartas/card2.PNG";
                            break;
                        case 3:
                            picCard2.ImageLocation = Directorio + "/Cartas/card3.PNG";
                            break;
                        case 4:
                            picCard2.ImageLocation = Directorio + "/Cartas/card4.PNG";
                            break;
                        case 5:
                            picCard2.ImageLocation = Directorio + "/Cartas/card5.PNG";
                            break;
                        case 6:
                            picCard2.ImageLocation = Directorio + "/Cartas/card6.PNG";
                            break;
                        case 7:
                            picCard2.ImageLocation = Directorio + "/Cartas/card7.PNG";
                            break;
                        case 8:
                            picCard2.ImageLocation = Directorio + "/Cartas/card8.PNG";
                            break;
                    }
                    permisoParaCoger = false;
                    cont++;
                    string mensaje = "30/" + this.nForm + "/" + mazo2;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    cartaEscogida1 = mazo2;
                    cartas_picture_vector[1] = picCard2;
                    picCard2.Tag = mazo2 - 1;
                    picCard2.Click += new System.EventHandler(this.eventoClick);
                    timer2.Enabled = true;
                    cartasPartida[1] = new Cartas();
                    cartasPartida[1] = cartas[mazo2 - 1];
                    vidaContrincante1.Text = "Vida: " + cartasPartida[1].GetHp();
                }

                if (permisoParaCoger && cantidadJugadores == 4)
                {
                    if (hostEnApp == anfitrionPartida)
                    {
                        switch (mazo1)
                        {
                            case 1:
                                picCard1.ImageLocation = Directorio + "/Cartas/card1.PNG";
                                break;
                            case 2:
                                picCard1.ImageLocation = Directorio + "/Cartas/card2.PNG";
                                break;
                            case 3:
                                picCard1.ImageLocation = Directorio + "/Cartas/card3.PNG";
                                break;
                            case 4:
                                picCard1.ImageLocation = Directorio + "/Cartas/card4.PNG";
                                break;
                            case 5:
                                picCard1.ImageLocation = Directorio + "/Cartas/card5.PNG";
                                break;
                            case 6:
                                picCard1.ImageLocation = Directorio + "/Cartas/card6.PNG";
                                break;
                            case 7:
                                picCard1.ImageLocation = Directorio + "/Cartas/card7.PNG";
                                break;
                            case 8:
                                picCard1.ImageLocation = Directorio + "/Cartas/card8.PNG";
                                break;
                        }
                        permisoParaCoger = false;
                        cont++;
                        string mensaje = "31/" + this.nForm + "/" + mazo1 + "/" + hostEnApp;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                        cartaEscogida1 = mazo1;
                        cartas_picture_vector[1] = picCard1;
                        picCard1.Tag = mazo1 - 1;
                        picCard1.Click += new System.EventHandler(this.eventoClick);
                        timer3.Enabled = true;
                    }
                    if (hostEnApp == Player2)
                    {
                        switch (mazo2)
                        {
                            case 1:
                                picCard2.ImageLocation = Directorio + "/Cartas/card1.PNG";
                                break;
                            case 2:
                                picCard2.ImageLocation = Directorio + "/Cartas/card2.PNG";
                                break;
                            case 3:
                                picCard2.ImageLocation = Directorio + "/Cartas/card3.PNG";
                                break;
                            case 4:
                                picCard2.ImageLocation = Directorio + "/Cartas/card4.PNG";
                                break;
                            case 5:
                                picCard2.ImageLocation = Directorio + "/Cartas/card5.PNG";
                                break;
                            case 6:
                                picCard2.ImageLocation = Directorio + "/Cartas/card6.PNG";
                                break;
                            case 7:
                                picCard2.ImageLocation = Directorio + "/Cartas/card7.PNG";
                                break;
                            case 8:
                                picCard2.ImageLocation = Directorio + "/Cartas/card8.PNG";
                                break;
                        }
                        permisoParaCoger = false;
                        cont++;
                        string mensaje = "31/" + this.nForm + "/" + mazo2 + "/" + hostEnApp;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                        cartaEscogida1 = mazo2;
                        cartas_picture_vector[1] = picCard2;
                        picCard2.Tag = mazo2 - 1;
                        picCard2.Click += new System.EventHandler(this.eventoClick);
                        timer4.Enabled = true;
                    }
                    if (hostEnApp == Player3)
                    {
                        switch (mazo3)
                        {
                            case 1:
                                picCard3.ImageLocation = Directorio + "/Cartas/card1.PNG";
                                break;
                            case 2:
                                picCard3.ImageLocation = Directorio + "/Cartas/card2.PNG";
                                break;
                            case 3:
                                picCard3.ImageLocation = Directorio + "/Cartas/card3.PNG";
                                break;
                            case 4:
                                picCard3.ImageLocation = Directorio + "/Cartas/card4.PNG";
                                break;
                            case 5:
                                picCard3.ImageLocation = Directorio + "/Cartas/card5.PNG";
                                break;
                            case 6:
                                picCard3.ImageLocation = Directorio + "/Cartas/card6.PNG";
                                break;
                            case 7:
                                picCard3.ImageLocation = Directorio + "/Cartas/card7.PNG";
                                break;
                            case 8:
                                picCard3.ImageLocation = Directorio + "/Cartas/card8.PNG";
                                break;
                        }
                        permisoParaCoger = false;
                        cont++;
                        string mensaje = "31/" + this.nForm + "/" + mazo3 + "/" + hostEnApp;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                        cartaEscogida1 = mazo3;
                        cartas_picture_vector[1] = picCard3;
                        picCard3.Tag = mazo3 - 1;
                        picCard3.Click += new System.EventHandler(this.eventoClick);
                        timer5.Enabled = true;
                    }
                    if (hostEnApp == Player4)
                    {
                        switch (mazo4)
                        {
                            case 1:
                                picCard4.ImageLocation = Directorio + "/Cartas/card1.PNG";
                                break;
                            case 2:
                                picCard4.ImageLocation = Directorio + "/Cartas/card2.PNG";
                                break;
                            case 3:
                                picCard4.ImageLocation = Directorio + "/Cartas/card3.PNG";
                                break;
                            case 4:
                                picCard4.ImageLocation = Directorio + "/Cartas/card4.PNG";
                                break;
                            case 5:
                                picCard4.ImageLocation = Directorio + "/Cartas/card5.PNG";
                                break;
                            case 6:
                                picCard4.ImageLocation = Directorio + "/Cartas/card6.PNG";
                                break;
                            case 7:
                                picCard4.ImageLocation = Directorio + "/Cartas/card7.PNG";
                                break;
                            case 8:
                                picCard4.ImageLocation = Directorio + "/Cartas/card8.PNG";
                                break;
                        }
                        permisoParaCoger = false;
                        cont++;
                        string mensaje = "31/" + this.nForm + "/" + mazo4 + "/" + hostEnApp;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                        cartaEscogida1 = mazo4;
                        cartas_picture_vector[1] = picCard4;
                        picCard4.Tag = mazo4 - 1;
                        picCard4.Click += new System.EventHandler(this.eventoClick);
                        timer6.Enabled = true;
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e) //Temporizador carta anfitrión
        {
            if (picCard1.Left >= 328)
                MoverCarta1X();
            if (picCard1.Top <= 324)
                MoverCarta1Y();
            else if (picCard1.Left <= 328 && picCard1.Top >= 324)
            {
                timer1.Enabled = false;
            }
        }

        private void timer2_Tick(object sender, EventArgs e) //Temporizador carta contrincante
        {
            if (picCard2.Left <= 1098)
                MoverCarta2X();
            if (picCard2.Top <= 324)
                MoverCarta2Y();
            else if (picCard2.Left >= 1098 && picCard2.Top >= 324)
            {
                timer2.Enabled = false;
            }
        }

        private void timer3_Tick(object sender, EventArgs e) //Temporizador carta anfitrión
        {
            if (picCard1.Left >= 328)
                MoverCarta3X();
            if (picCard1.Top <= 177)
                MoverCarta3Y();
            else if (picCard1.Left <= 328 && picCard1.Top >= 177)
            {
                timer3.Enabled = false;
            }
        }

        private void timer4_Tick(object sender, EventArgs e) //Temporizador carta aliado
        {
            if (picCard2.Left >= 328)
                MoverCarta4X();
            if (picCard2.Top <= 463)
                MoverCarta4Y();
            else if (picCard2.Left <= 328 && picCard2.Top >= 463)
            {
                timer4.Enabled = false;
            }
        }

        private void timer5_Tick(object sender, EventArgs e) //Temporizador carta contrincante1
        {
            if (picCard3.Left <= 1098)
                MoverCarta5X();
            if (picCard3.Top <= 177)
                MoverCarta5Y();
            else if (picCard3.Left >= 1098 && picCard3.Top >= 177)
            {
                timer5.Enabled = false;
            }
        }

        private void timer6_Tick_1(object sender, EventArgs e) //Temporizador carta contrincante2
        {
            if (picCard4.Left <= 1098)
                MoverCarta6X();
            if (picCard4.Top <= 463)
                MoverCarta6Y();
            else if (picCard4.Left >= 1098 && picCard4.Top >= 463)
            {
                timer6.Enabled = false;
            }
        }

        private void menu_Click(object sender, EventArgs e) //Al abrir el menú, aparecerán las acciones que se pueden tomar a la hora de la partida
        {
            if (permisoParaCoger == false && permisoParaAtacar == true)
            {
                if (menu.Text == "Menú")
                {
                    fisico.Show();
                    magico.Show();
                    pasar.Show();
                    menu.Text = "Ocultar Menú";

                }
                else
                {
                    fisico.Hide();
                    magico.Hide();
                    pasar.Hide();
                    menu.Text = "Menú";
                }
            }
            else if (permisoParaCoger == true && permisoParaAtacar == false)
                chatPartida.Items.Add("¡Aún no tienes carta!");
            else if (permisoParaCoger == false && permisoParaAtacar == false)
                chatPartida.Items.Add("¡Espera a que el contrincante escoja carta!");
            else if (permisoParaCoger == true && permisoParaAtacar == true)
                chatPartida.Items.Add("¡Aún no tienes carta!");
        }

        private void enviarMensaje_Click(object sender, EventArgs e) //Enviar mensaje en el chat
        {
            // Quiere enviar un mensaje
            string mensaje = "9/" + this.nForm + "/" + hostEnApp + "/" + textoChat.Text + "/";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            textoChat.Clear();
        }

        private void formTablero_FormClosing(object sender, FormClosingEventArgs e) //Función para enviar al servidor el código 13 en caso de abandono de partida al ser pulsada la X
        {
            Reproductor.controls.stop();
            if (!securityClose && !partidaGanada)
            {
                string mensaje = "13/" + this.nForm;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
        }

        private void fisico_Click(object sender, EventArgs e) //Lanzamos un ataque físico al oponente y se transmite al servidor
        {
            if (hostEnApp == anfitrionPartida)
            {
                string mensaje = "32/" + nForm + "/" + cartasPartida[0].GetAtaqueFisico() + "/" + hostEnApp;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else
            {
                string mensaje = "32/" + nForm + "/" + cartasPartida[1].GetAtaqueFisico() + "/" + hostEnApp;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
        }

        private void magico_Click(object sender, EventArgs e) //Lanzamos un hechizo al oponente y se transmite al servidor
        {
            if (hostEnApp == anfitrionPartida)
            {
                string mensaje = "33/" + nForm + "/" + cartasPartida[0].GetHechizo() + "/" + hostEnApp;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else
            {
                string mensaje = "33/" + nForm + "/" + cartasPartida[1].GetHechizo() + "/" + hostEnApp;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
        }

        private void timerRetardo_Tick(object sender, EventArgs e) //Retardo para mostrar el resultado de la partida antes de concluirla
        {
            this.tiempoEspera++;
            if (this.tiempoEspera > 5)
            {
                timerRetardo.Enabled = false;
                tiempoEspera = 0;
                this.Close();
            }
        }

        private void movimientoAtaque() //La carta se lanza hacia el oponente
        {
                picCard2.Left = 488;
        }

        private void movimientoRetroceso() //La carta retrocede a su posición
        {
                picCard2.Left = 1098;
        }

        private void movimientoAtaqueRecibido() //La carta se lanza hacia el oponente
        {
                picCard1.Left = 920;
        }

        private void movimientoRetrocesoRecibido() //La carta retrocede a su posición
        {
                picCard1.Left = 328;
        }

        private void timerAtaque_Tick(object sender, EventArgs e) //Animar las cartas cuando se ejecutan ataques físicos
        {
            movimientoAtaque();
            this.tiempoEspera++;
            if (this.tiempoEspera == 2)
            {
                movimientoRetroceso();
                timerAtaque.Enabled = false;
                this.tiempoEspera = 0;
            }
        }

        private void timerAtaque2_Tick(object sender, EventArgs e) //Animar las cartas cuando se ejecutan ataques físicos
        {
            movimientoAtaqueRecibido();
            this.tiempoEspera2++;
            if (this.tiempoEspera2 == 2)
            {
                movimientoRetrocesoRecibido();
                timerAtaque2.Enabled = false;
                this.tiempoEspera2 = 0;
            }
        }
    }
}