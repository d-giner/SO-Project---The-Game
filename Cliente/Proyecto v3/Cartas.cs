using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1
{
    public class Cartas
    {
        int hp; //health points
        int idCarta; //identificador de la carta
        string nombre; //nombre de la carta
        int hechizo; //daño del hechizo
        int ataqueFisico; //daño del ataque físico
        string nombreAtaqueH; //nombre del ataque
        string nombreAtaqueF; //nombre del hechizo

        public void Carta(int hp, int idCarta, string nombre, int hechizo, int ataqueFisico, string nombreAtaqueH, string nombreAtaqueF) //Constructor para las cartas
        {
            this.hp = hp;
            this.idCarta = idCarta;
            this.nombre = nombre;
            this.hechizo = hechizo;
            this.ataqueFisico = ataqueFisico;
            this.nombreAtaqueH = nombreAtaqueH;
            this.nombreAtaqueF = nombreAtaqueF;
        }

        //Funciones para asignar los parámetros de las cartas
        public void SetHp(int hp)
        {
            this.hp = hp;
        }
        public void SetidCarta(int idCarta)
        {
            this.idCarta = idCarta;
        }
        public void SetNombre(string nombre)
        {
            this.nombre = nombre;
        }
        public void SetHechizo(int hechizo)
        {
            this.hechizo = hechizo;
        }
        public void SetAtaqueFisico(int ataqueFisico)
        {
            this.ataqueFisico = ataqueFisico;
        }
        public void SetNombreH(string nombreAtaqueH)
        {
            this.nombreAtaqueH = nombreAtaqueH;
        }
        public void SetNombreF(string nombreAtaqueF)
        {
            this.nombreAtaqueF = nombreAtaqueF;
        }

        //Funciones para devolver los parámetros
        public int GetHp()
        {
            return this.hp;
        }
        public int GetidCarta()
        {
            return this.idCarta;
        }
        public string GetNombre()
        {
            return this.nombre;
        }
        public int GetHechizo()
        {
            return this.hechizo;
        }
        public int GetAtaqueFisico()
        {
            return this.ataqueFisico;
        }
        public string GetNombreH()
        {
            return this.nombreAtaqueH;
        }
        public string GetNombreF()
        {
            return this.nombreAtaqueF;
        }        
    }   
}
