namespace WindowsFormsApplication1
{
    partial class Principal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.consulta1 = new System.Windows.Forms.RadioButton();
            this.consulta2 = new System.Windows.Forms.RadioButton();
            this.consulta3 = new System.Windows.Forms.RadioButton();
            this.p1 = new System.Windows.Forms.TextBox();
            this.cerrar = new System.Windows.Forms.Button();
            this.consultar = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.conectar = new System.Windows.Forms.Button();
            this.registrar = new System.Windows.Forms.Button();
            this.u = new System.Windows.Forms.TextBox();
            this.p = new System.Windows.Forms.TextBox();
            this.off = new System.Windows.Forms.Button();
            this.amigos = new System.Windows.Forms.Button();
            this.listaconectados = new System.Windows.Forms.DataGridView();
            this.regAceptar = new System.Windows.Forms.Button();
            this.regCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.OcultarAmigos = new System.Windows.Forms.Button();
            this.textoChat = new System.Windows.Forms.TextBox();
            this.enviarMensaje = new System.Windows.Forms.Button();
            this.nombreJugador = new System.Windows.Forms.Label();
            this.chat = new System.Windows.Forms.ListBox();
            this.crearPartida = new System.Windows.Forms.Button();
            this.contadorAceptaciones = new System.Windows.Forms.Label();
            this.aceptarInvitacion = new System.Windows.Forms.Button();
            this.rechazarInvitacion = new System.Windows.Forms.Button();
            this.pictureBoxInvitaciones = new System.Windows.Forms.PictureBox();
            this.textoInvitacion = new System.Windows.Forms.Label();
            this.mostrarConsultas = new System.Windows.Forms.Button();
            this.AceptarBaja = new System.Windows.Forms.Button();
            this.DarseBaja = new System.Windows.Forms.Button();
            this.Reconectar = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.consulta4 = new System.Windows.Forms.RadioButton();
            this.consulta5 = new System.Windows.Forms.RadioButton();
            this.consultasBox = new System.Windows.Forms.ListBox();
            this.labelConsultar = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.listaconectados)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInvitaciones)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 477);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Selecciona tu consulta:";
            // 
            // consulta1
            // 
            this.consulta1.AutoSize = true;
            this.consulta1.Location = new System.Drawing.Point(10, 493);
            this.consulta1.Name = "consulta1";
            this.consulta1.Size = new System.Drawing.Size(295, 17);
            this.consulta1.TabIndex = 1;
            this.consulta1.TabStop = true;
            this.consulta1.Text = "Cuántos jugadores han usado la misma skin en la partida:";
            this.consulta1.UseVisualStyleBackColor = true;
            this.consulta1.CheckedChanged += new System.EventHandler(this.consulta1_CheckedChanged);
            // 
            // consulta2
            // 
            this.consulta2.AutoSize = true;
            this.consulta2.Location = new System.Drawing.Point(10, 517);
            this.consulta2.Name = "consulta2";
            this.consulta2.Size = new System.Drawing.Size(313, 17);
            this.consulta2.TabIndex = 2;
            this.consulta2.TabStop = true;
            this.consulta2.Text = "Nombre de los jugadores que han obtenido más de X puntos:";
            this.consulta2.UseVisualStyleBackColor = true;
            this.consulta2.CheckedChanged += new System.EventHandler(this.consulta2_CheckedChanged);
            // 
            // consulta3
            // 
            this.consulta3.AutoSize = true;
            this.consulta3.Location = new System.Drawing.Point(10, 540);
            this.consulta3.Name = "consulta3";
            this.consulta3.Size = new System.Drawing.Size(210, 17);
            this.consulta3.TabIndex = 3;
            this.consulta3.TabStop = true;
            this.consulta3.Text = "Cuántas partidas ha ganado el jugador:";
            this.consulta3.UseVisualStyleBackColor = true;
            this.consulta3.CheckedChanged += new System.EventHandler(this.consulta3_CheckedChanged);
            // 
            // p1
            // 
            this.p1.Location = new System.Drawing.Point(10, 624);
            this.p1.Name = "p1";
            this.p1.Size = new System.Drawing.Size(100, 20);
            this.p1.TabIndex = 4;
            // 
            // cerrar
            // 
            this.cerrar.Location = new System.Drawing.Point(10, 650);
            this.cerrar.Name = "cerrar";
            this.cerrar.Size = new System.Drawing.Size(75, 23);
            this.cerrar.TabIndex = 7;
            this.cerrar.Text = "Cerrar";
            this.cerrar.UseVisualStyleBackColor = true;
            this.cerrar.Click += new System.EventHandler(this.cerrar_Click_1);
            // 
            // consultar
            // 
            this.consultar.Location = new System.Drawing.Point(121, 621);
            this.consultar.Name = "consultar";
            this.consultar.Size = new System.Drawing.Size(75, 23);
            this.consultar.TabIndex = 8;
            this.consultar.Text = "Consultar";
            this.consultar.UseVisualStyleBackColor = true;
            this.consultar.Click += new System.EventHandler(this.consultar_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(118, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Usuario *";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(118, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Contraseña *";
            // 
            // conectar
            // 
            this.conectar.Location = new System.Drawing.Point(315, 68);
            this.conectar.Name = "conectar";
            this.conectar.Size = new System.Drawing.Size(82, 23);
            this.conectar.TabIndex = 11;
            this.conectar.Text = "Conectar";
            this.conectar.UseVisualStyleBackColor = true;
            this.conectar.Click += new System.EventHandler(this.conectar_Click_1);
            // 
            // registrar
            // 
            this.registrar.Location = new System.Drawing.Point(416, 69);
            this.registrar.Name = "registrar";
            this.registrar.Size = new System.Drawing.Size(82, 23);
            this.registrar.TabIndex = 12;
            this.registrar.Text = "Registrarse";
            this.registrar.UseVisualStyleBackColor = true;
            this.registrar.Click += new System.EventHandler(this.registrar_Click_1);
            // 
            // u
            // 
            this.u.Location = new System.Drawing.Point(192, 66);
            this.u.Name = "u";
            this.u.Size = new System.Drawing.Size(100, 20);
            this.u.TabIndex = 13;
            // 
            // p
            // 
            this.p.Location = new System.Drawing.Point(192, 108);
            this.p.Name = "p";
            this.p.Size = new System.Drawing.Size(100, 20);
            this.p.TabIndex = 14;
            // 
            // off
            // 
            this.off.Location = new System.Drawing.Point(22, 32);
            this.off.Name = "off";
            this.off.Size = new System.Drawing.Size(82, 23);
            this.off.TabIndex = 15;
            this.off.Text = "Desconectar";
            this.off.UseVisualStyleBackColor = true;
            this.off.Click += new System.EventHandler(this.off_Click);
            // 
            // amigos
            // 
            this.amigos.Location = new System.Drawing.Point(22, 59);
            this.amigos.Name = "amigos";
            this.amigos.Size = new System.Drawing.Size(116, 23);
            this.amigos.TabIndex = 16;
            this.amigos.Text = "Amigos Conectados";
            this.amigos.UseVisualStyleBackColor = true;
            this.amigos.Click += new System.EventHandler(this.amigos_Click);
            // 
            // listaconectados
            // 
            this.listaconectados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.listaconectados.Location = new System.Drawing.Point(29, 88);
            this.listaconectados.Name = "listaconectados";
            this.listaconectados.Size = new System.Drawing.Size(105, 148);
            this.listaconectados.TabIndex = 17;
            this.listaconectados.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.listaconectados_CellContentClick);
            // 
            // regAceptar
            // 
            this.regAceptar.Location = new System.Drawing.Point(315, 68);
            this.regAceptar.Name = "regAceptar";
            this.regAceptar.Size = new System.Drawing.Size(82, 23);
            this.regAceptar.TabIndex = 20;
            this.regAceptar.Text = "Aceptar";
            this.regAceptar.UseVisualStyleBackColor = true;
            this.regAceptar.Click += new System.EventHandler(this.regAceptar_Click);
            // 
            // regCancel
            // 
            this.regCancel.Location = new System.Drawing.Point(315, 108);
            this.regCancel.Name = "regCancel";
            this.regCancel.Size = new System.Drawing.Size(82, 23);
            this.regCancel.TabIndex = 21;
            this.regCancel.Text = "Cancelar";
            this.regCancel.UseVisualStyleBackColor = true;
            this.regCancel.Click += new System.EventHandler(this.regCancel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label4.Location = new System.Drawing.Point(189, 139);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(228, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Introduce el Usuario y Contraseña que desees.";
            // 
            // OcultarAmigos
            // 
            this.OcultarAmigos.Location = new System.Drawing.Point(22, 59);
            this.OcultarAmigos.Name = "OcultarAmigos";
            this.OcultarAmigos.Size = new System.Drawing.Size(116, 23);
            this.OcultarAmigos.TabIndex = 24;
            this.OcultarAmigos.Text = "Ocultar lista";
            this.OcultarAmigos.UseVisualStyleBackColor = true;
            this.OcultarAmigos.Click += new System.EventHandler(this.OcultarAmigos_Click);
            // 
            // textoChat
            // 
            this.textoChat.Location = new System.Drawing.Point(171, 289);
            this.textoChat.Multiline = true;
            this.textoChat.Name = "textoChat";
            this.textoChat.Size = new System.Drawing.Size(320, 32);
            this.textoChat.TabIndex = 27;
            // 
            // enviarMensaje
            // 
            this.enviarMensaje.BackColor = System.Drawing.Color.DodgerBlue;
            this.enviarMensaje.ForeColor = System.Drawing.Color.Snow;
            this.enviarMensaje.Location = new System.Drawing.Point(509, 289);
            this.enviarMensaje.Name = "enviarMensaje";
            this.enviarMensaje.Size = new System.Drawing.Size(69, 32);
            this.enviarMensaje.TabIndex = 28;
            this.enviarMensaje.Text = "Enviar";
            this.enviarMensaje.UseVisualStyleBackColor = false;
            this.enviarMensaje.Click += new System.EventHandler(this.enviarMensaje_Click);
            // 
            // nombreJugador
            // 
            this.nombreJugador.AutoSize = true;
            this.nombreJugador.BackColor = System.Drawing.Color.Transparent;
            this.nombreJugador.Font = new System.Drawing.Font("Monotype Corsiva", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nombreJugador.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.nombreJugador.Location = new System.Drawing.Point(25, 5);
            this.nombreJugador.Name = "nombreJugador";
            this.nombreJugador.Size = new System.Drawing.Size(65, 22);
            this.nombreJugador.TabIndex = 29;
            this.nombreJugador.Text = "Jugador";
            // 
            // chat
            // 
            this.chat.FormattingEnabled = true;
            this.chat.Location = new System.Drawing.Point(171, 32);
            this.chat.Name = "chat";
            this.chat.Size = new System.Drawing.Size(407, 251);
            this.chat.TabIndex = 26;
            // 
            // crearPartida
            // 
            this.crearPartida.BackColor = System.Drawing.Color.Yellow;
            this.crearPartida.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.crearPartida.Location = new System.Drawing.Point(33, 242);
            this.crearPartida.Name = "crearPartida";
            this.crearPartida.Size = new System.Drawing.Size(105, 35);
            this.crearPartida.TabIndex = 50;
            this.crearPartida.Text = "Crear Partida";
            this.crearPartida.UseVisualStyleBackColor = false;
            this.crearPartida.Click += new System.EventHandler(this.crearPartida_Click);
            // 
            // contadorAceptaciones
            // 
            this.contadorAceptaciones.AutoSize = true;
            this.contadorAceptaciones.BackColor = System.Drawing.Color.Transparent;
            this.contadorAceptaciones.Font = new System.Drawing.Font("Monotype Corsiva", 21.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contadorAceptaciones.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.contadorAceptaciones.Location = new System.Drawing.Point(27, 344);
            this.contadorAceptaciones.Name = "contadorAceptaciones";
            this.contadorAceptaciones.Size = new System.Drawing.Size(51, 36);
            this.contadorAceptaciones.TabIndex = 51;
            this.contadorAceptaciones.Text = "0/4";
            // 
            // aceptarInvitacion
            // 
            this.aceptarInvitacion.BackColor = System.Drawing.Color.Chartreuse;
            this.aceptarInvitacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aceptarInvitacion.Location = new System.Drawing.Point(369, 417);
            this.aceptarInvitacion.Name = "aceptarInvitacion";
            this.aceptarInvitacion.Size = new System.Drawing.Size(75, 35);
            this.aceptarInvitacion.TabIndex = 52;
            this.aceptarInvitacion.Text = "Aceptar";
            this.aceptarInvitacion.UseVisualStyleBackColor = false;
            this.aceptarInvitacion.Click += new System.EventHandler(this.aceptarInvitacion_Click);
            // 
            // rechazarInvitacion
            // 
            this.rechazarInvitacion.BackColor = System.Drawing.Color.Salmon;
            this.rechazarInvitacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rechazarInvitacion.Location = new System.Drawing.Point(473, 417);
            this.rechazarInvitacion.Name = "rechazarInvitacion";
            this.rechazarInvitacion.Size = new System.Drawing.Size(75, 35);
            this.rechazarInvitacion.TabIndex = 53;
            this.rechazarInvitacion.Text = "Rechazar";
            this.rechazarInvitacion.UseVisualStyleBackColor = false;
            this.rechazarInvitacion.Click += new System.EventHandler(this.rechazarInvitacion_Click);
            // 
            // pictureBoxInvitaciones
            // 
            this.pictureBoxInvitaciones.BackColor = System.Drawing.Color.Aquamarine;
            this.pictureBoxInvitaciones.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxInvitaciones.Location = new System.Drawing.Point(346, 327);
            this.pictureBoxInvitaciones.Name = "pictureBoxInvitaciones";
            this.pictureBoxInvitaciones.Size = new System.Drawing.Size(232, 131);
            this.pictureBoxInvitaciones.TabIndex = 54;
            this.pictureBoxInvitaciones.TabStop = false;
            // 
            // textoInvitacion
            // 
            this.textoInvitacion.AutoSize = true;
            this.textoInvitacion.BackColor = System.Drawing.Color.Transparent;
            this.textoInvitacion.Location = new System.Drawing.Point(366, 344);
            this.textoInvitacion.Name = "textoInvitacion";
            this.textoInvitacion.Size = new System.Drawing.Size(76, 13);
            this.textoInvitacion.TabIndex = 55;
            this.textoInvitacion.Text = "textoInvitacion";
            // 
            // mostrarConsultas
            // 
            this.mostrarConsultas.Location = new System.Drawing.Point(10, 446);
            this.mostrarConsultas.Name = "mostrarConsultas";
            this.mostrarConsultas.Size = new System.Drawing.Size(117, 28);
            this.mostrarConsultas.TabIndex = 56;
            this.mostrarConsultas.Text = "Mostrar Consultas";
            this.mostrarConsultas.UseVisualStyleBackColor = true;
            this.mostrarConsultas.Click += new System.EventHandler(this.mostrarConsultas_Click);
            // 
            // AceptarBaja
            // 
            this.AceptarBaja.Location = new System.Drawing.Point(315, 68);
            this.AceptarBaja.Name = "AceptarBaja";
            this.AceptarBaja.Size = new System.Drawing.Size(82, 23);
            this.AceptarBaja.TabIndex = 57;
            this.AceptarBaja.Text = "Aceptar";
            this.AceptarBaja.UseVisualStyleBackColor = true;
            this.AceptarBaja.Click += new System.EventHandler(this.AceptarBaja_Click);
            // 
            // DarseBaja
            // 
            this.DarseBaja.Location = new System.Drawing.Point(416, 108);
            this.DarseBaja.Name = "DarseBaja";
            this.DarseBaja.Size = new System.Drawing.Size(82, 23);
            this.DarseBaja.TabIndex = 58;
            this.DarseBaja.Text = "Darse de baja";
            this.DarseBaja.UseVisualStyleBackColor = true;
            this.DarseBaja.Click += new System.EventHandler(this.DarseBaja_Click);
            // 
            // Reconectar
            // 
            this.Reconectar.BackColor = System.Drawing.Color.Yellow;
            this.Reconectar.Location = new System.Drawing.Point(244, 344);
            this.Reconectar.Name = "Reconectar";
            this.Reconectar.Size = new System.Drawing.Size(117, 51);
            this.Reconectar.TabIndex = 59;
            this.Reconectar.Text = "Reintentar Conexión";
            this.Reconectar.UseVisualStyleBackColor = false;
            this.Reconectar.Click += new System.EventHandler(this.Reconectar_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(487, 5);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 15);
            this.label7.TabIndex = 60;
            this.label7.Text = "label7";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(238, 314);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(128, 24);
            this.label8.TabIndex = 61;
            this.label8.Text = "Conectando...";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // consulta4
            // 
            this.consulta4.AutoSize = true;
            this.consulta4.Location = new System.Drawing.Point(10, 563);
            this.consulta4.Name = "consulta4";
            this.consulta4.Size = new System.Drawing.Size(257, 17);
            this.consulta4.TabIndex = 62;
            this.consulta4.TabStop = true;
            this.consulta4.Text = "Saber con qué jugadores has jugado alguna vez:";
            this.consulta4.UseVisualStyleBackColor = true;
            this.consulta4.CheckedChanged += new System.EventHandler(this.consulta4_CheckedChanged);
            // 
            // consulta5
            // 
            this.consulta5.AutoSize = true;
            this.consulta5.Location = new System.Drawing.Point(11, 584);
            this.consulta5.Name = "consulta5";
            this.consulta5.Size = new System.Drawing.Size(223, 17);
            this.consulta5.TabIndex = 63;
            this.consulta5.TabStop = true;
            this.consulta5.Text = "Cuántos puntos he obtenido jugando con:";
            this.consulta5.UseVisualStyleBackColor = true;
            this.consulta5.CheckedChanged += new System.EventHandler(this.consulta5_CheckedChanged);
            // 
            // consultasBox
            // 
            this.consultasBox.FormattingEnabled = true;
            this.consultasBox.Location = new System.Drawing.Point(329, 487);
            this.consultasBox.Name = "consultasBox";
            this.consultasBox.Size = new System.Drawing.Size(249, 186);
            this.consultasBox.TabIndex = 64;
            // 
            // labelConsultar
            // 
            this.labelConsultar.AutoSize = true;
            this.labelConsultar.ForeColor = System.Drawing.Color.Blue;
            this.labelConsultar.Location = new System.Drawing.Point(9, 608);
            this.labelConsultar.Name = "labelConsultar";
            this.labelConsultar.Size = new System.Drawing.Size(52, 13);
            this.labelConsultar.TabIndex = 65;
            this.labelConsultar.Text = "Introduce";
            // 
            // Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Tomato;
            this.ClientSize = new System.Drawing.Size(597, 686);
            this.Controls.Add(this.labelConsultar);
            this.Controls.Add(this.consultasBox);
            this.Controls.Add(this.consulta5);
            this.Controls.Add(this.consulta4);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.Reconectar);
            this.Controls.Add(this.DarseBaja);
            this.Controls.Add(this.AceptarBaja);
            this.Controls.Add(this.contadorAceptaciones);
            this.Controls.Add(this.mostrarConsultas);
            this.Controls.Add(this.textoInvitacion);
            this.Controls.Add(this.rechazarInvitacion);
            this.Controls.Add(this.aceptarInvitacion);
            this.Controls.Add(this.crearPartida);
            this.Controls.Add(this.nombreJugador);
            this.Controls.Add(this.enviarMensaje);
            this.Controls.Add(this.textoChat);
            this.Controls.Add(this.OcultarAmigos);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.regCancel);
            this.Controls.Add(this.regAceptar);
            this.Controls.Add(this.amigos);
            this.Controls.Add(this.off);
            this.Controls.Add(this.p);
            this.Controls.Add(this.u);
            this.Controls.Add(this.registrar);
            this.Controls.Add(this.conectar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.consultar);
            this.Controls.Add(this.cerrar);
            this.Controls.Add(this.p1);
            this.Controls.Add(this.consulta3);
            this.Controls.Add(this.consulta2);
            this.Controls.Add(this.consulta1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxInvitaciones);
            this.Controls.Add(this.chat);
            this.Controls.Add(this.listaconectados);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Principal";
            this.Opacity = 0.95D;
            this.Text = "The Game";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Principal_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.listaconectados)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInvitaciones)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton consulta1;
        private System.Windows.Forms.RadioButton consulta2;
        private System.Windows.Forms.RadioButton consulta3;
        private System.Windows.Forms.TextBox p1;
        private System.Windows.Forms.Button cerrar;
        private System.Windows.Forms.Button consultar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button conectar;
        private System.Windows.Forms.Button registrar;
        private System.Windows.Forms.TextBox u;
        private System.Windows.Forms.TextBox p;
        private System.Windows.Forms.Button off;
        private System.Windows.Forms.Button amigos;
        private System.Windows.Forms.DataGridView listaconectados;
        private System.Windows.Forms.Button regAceptar;
        private System.Windows.Forms.Button regCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button OcultarAmigos;
        private System.Windows.Forms.TextBox textoChat;
        private System.Windows.Forms.Button enviarMensaje;
        private System.Windows.Forms.Label nombreJugador;
        private System.Windows.Forms.ListBox chat;
        private System.Windows.Forms.Button crearPartida;
        private System.Windows.Forms.Label contadorAceptaciones;
        private System.Windows.Forms.Button aceptarInvitacion;
        private System.Windows.Forms.Button rechazarInvitacion;
        private System.Windows.Forms.PictureBox pictureBoxInvitaciones;
        private System.Windows.Forms.Label textoInvitacion;
        private System.Windows.Forms.Button mostrarConsultas;
        private System.Windows.Forms.Button AceptarBaja;
        private System.Windows.Forms.Button DarseBaja;
        private System.Windows.Forms.Button Reconectar;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton consulta4;
        private System.Windows.Forms.RadioButton consulta5;
        private System.Windows.Forms.ListBox consultasBox;
        private System.Windows.Forms.Label labelConsultar;
    }
}