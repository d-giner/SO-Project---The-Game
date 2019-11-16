using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1
{
    class Invitations
    {
        int idPartida;
        int orden;
        bool procesada = true; //Booleano para mientras la invitación está en cola
        bool pendiente = false; //Booleano para cuando la invitación ya se ha procesado y está a la espera de aceptación
        string mensajeInvitacion;


        public void Invitacion(int idPartida, int orden, bool procesada, bool pendiente, string mensajeInvitacion) //Constructor de las invitaciones
        {
            this.idPartida = idPartida;
            this.orden = orden;
            this.procesada = procesada;
            this.pendiente = pendiente;
            this.mensajeInvitacion = mensajeInvitacion;
        }
        

        //Funciones para asignar los parámetros correspondientes

        public void SetidPartida(int idPartida)
        {
            this.idPartida = idPartida;
        }
        public int GetidPartida()
        {
            return this.idPartida;
        }
        public void SetOrden(int orden)
        {
            this.orden = orden;
        }
        public int GetOrden()
        {
            return this.orden;
        }
        public void SetProcesada(bool procesada)
        {
            this.procesada = procesada;
        }
        public bool GetProcesada()
        {
            return this.procesada;
        }
        public void SetPendiente(bool pendiente)
        {
            this.pendiente = pendiente;
        }
        public bool GetPendiente()
        {
            return this.pendiente;
        }
        public void SetMensajeInvitacion(string mensajeInvitacion)
        {
            this.mensajeInvitacion = mensajeInvitacion;
        }
        public string GetMensajeInvitacion()
        {
            return this.mensajeInvitacion;
        }
    }
}
