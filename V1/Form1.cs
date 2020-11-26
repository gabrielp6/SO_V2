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

namespace V1
{
    public partial class Form1 : Form
    {
        Socket server;
        Thread atender;
        string ID_jugador, nombre;
        int port = 9090;
        string ip = "192.168.1.55";
        int conectado = 0;
        int num_conectados=0;
        string invitado;
        string[] conectados;
        List<string> lista_conectados = new List<string>();


        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            // Para que los elementos de los formularios puedan ser accedidos
            // desde threads diferentes
        }

        private void AtenderServidor()
        {
            while (true)
            {
                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                 
                int codigo = Convert.ToInt32(trozos[0]);
                string mensaje;
                if (codigo == 6) 
                {
                    num_conectados = Convert.ToInt32(trozos[1]);
                    mensaje = trozos[2].Split('\0')[0];
                }
                else 
                    mensaje = trozos[1].Split('\0')[0];

                switch (codigo)
                {
                    case 1: // Iniciar sesion
                        //La respuesta sera 0 si no se ha encontrado el usuario en labase de datos, sinó enviara su ID
                        if (mensaje == "0")
                            MessageBox.Show("Este usuario no existe");
                        else
                        {
                            ID_jugador = mensaje;
                            MessageBox.Show("Se ha iniciado sesión correctamente, tu ID de jugador es: " + mensaje);
                            /*
                            groupBox_inciar.Visible = false;
                            groupBox_registro.Visible = false;
                            groupBox_consultas.Visible = true;
                            Conectados_groupBox.Visible = true;*/
                        }
                        break;

                    case 2: // Registrarse
                        //La respuesta será 0 si se ha encontrado el usuario en labase de datos, sinó enviara su ID
                        if (mensaje == "0")
                            MessageBox.Show("Este usuario ya existe");
                        else
                        {
                            ID_jugador = mensaje;
                            MessageBox.Show("Te has registrado correctamente, tu ID de jugador es: " + mensaje);
                            /*
                            groupBox_inciar.Visible = false;
                            groupBox_registro.Visible = false;
                            groupBox_consultas.Visible = true;
                            Conectados_groupBox.Visible = true;
                             */
                        }

                        break;

                    case 3: // quien tiene el record
                        MessageBox.Show(mensaje);
                        break;

                    case 4: // que personajes se escogieron en la partida
                        MessageBox.Show(mensaje);
                        break;

                    case 5: // cuantas partidas ha jugado el jugador
                        MessageBox.Show(mensaje);
                        break;

                    case 6: // conectados
                        
                         /*conectadosGrid.ColumnCount = 1;
                         conectadosGrid.RowCount = num_conectados;

                         conectados = mensaje.Split(',');

                         for (int i = 0; i < num_conectados; i++)
                            conectadosGrid.Rows[i].Cells[0].Value = conectados[i];      */
                 
                        break;

                    case 7: // ivitación
                        invitacion_label.Text = mensaje + " te ha invitado a una partida";
                        Invitacion_groupBox.Visible = true;
                        break;
                    case 8: // respuesta a la invitacion
                        int respuesta = Convert.ToInt32(mensaje);
                        if (respuesta == 0)
                            MessageBox.Show("Han rechazado tu invitación");
                        else
                        {
                            MessageBox.Show("Han aceptado tu invitación");
                            
                            /*Conectados_groupBox.Visible = false;
                            Tablero.Visible = true;
                            Turno.Visible = true;
                            Pregunta.Visible = true;
                            Responde.Visible = true;
                            Chat_groupBox.Visible = true;*/
                        }
                        groupBox1.Visible = false;
                        break;
                }
            }
        }

        private void Consulta_Button_Click(object sender, EventArgs e)
        {
            if (conectado == 0)
            {
                //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
                //al que deseamos conectarnos
                IPAddress direc = IPAddress.Parse(ip);
                IPEndPoint ipep = new IPEndPoint(direc, port);


                //Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    server.Connect(ipep);//Intentamos conectar el socket               
                    //MessageBox.Show("Conectado");
                    conectado = 1;
                }
                catch (SocketException)
                {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                    MessageBox.Show("No he podido conectar con el servidor");
                    return;
                }
                
            }

            if (Record.Checked)
            {
                // Quiere saber quien tiene el record
                string mensaje = "3/";
                // Enviamos al servidor el codigo
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);                
            }

            else if (Personajes.Checked)
            {
                // Quiere saber que personajes se escogieron en la partida seleccionada
                string mensaje = "4/" + ID_Partida.Text;
                // Enviamos al servidor el ID de la partida tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                ID_Partida.Text = "";
            }

            else if (Partidas.Checked)
            {
                //Quiere cuantas partidas ha jugado el jugador seleccionado
                string mensaje = "5/" + ID_Jugador_txtBox.Text;
                // Enviamos al servidor el nombre y la altura tecleados
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                ID_Jugador_txtBox.Text="";
            }
        }

        private void Registrarse_Button_Click(object sender, EventArgs e)
        {
            if (conectado == 0)
            {
                //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
                //al que deseamos conectarnos
                IPAddress direc = IPAddress.Parse(ip);
                IPEndPoint ipep = new IPEndPoint(direc, port);


                //Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    server.Connect(ipep);//Intentamos conectar el socket               
                    //MessageBox.Show("Conectado");
                    conectado = 1;
                }
                catch (SocketException)
                {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                    MessageBox.Show("No he podido conectar con el servidor");
                    return;
                }

                // pongo en marcha el thread que atendera los mensajes del servidor
                ThreadStart ts = delegate { AtenderServidor(); };
                atender = new Thread(ts);
                atender.Start();
            }
            if ((Nombre_Registro.Text == "") || (Contraseña_Registro.Text == ""))
                MessageBox.Show("No se han rellenado correctamente todos los campos");
            else
            {
                string msj = "2/" + Nombre_Registro.Text + "/" + Contraseña_Registro.Text;
                nombre = Nombre_Registro.Text;
                // Enviamos al servidor el nombre y la contraseña del tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(msj);
                server.Send(msg);
            }
        }

        private void Iniciar_Button_Click(object sender, EventArgs e)
        {
            if (conectado == 0)
            {
                //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
                //al que deseamos conectarnos
                IPAddress direc = IPAddress.Parse(ip);
                IPEndPoint ipep = new IPEndPoint(direc, port);


                //Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    server.Connect(ipep);//Intentamos conectar el socket               
                    //MessageBox.Show("Conectado");
                    conectado = 1;
                }
                catch (SocketException)
                {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                    MessageBox.Show("No he podido conectar con el servidor");
                    return;
                }

                // pongo en marcha el thread que atendera los mensajes del servidor
                ThreadStart ts = delegate { AtenderServidor(); };
                atender = new Thread(ts);
                atender.Start();
            }

            string msj = "1/" + Nombre.Text + "/" + Contraseña.Text;
            nombre = Nombre.Text;
            // Enviamos al servidor el nombre y la contraseño tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(msj);
            server.Send(msg);

        }

        private void Desconectar_Click(object sender, EventArgs e)
        {
            string msj = "0/";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(msj);
            server.Send(msg);

            // Nos desconectamos
            atender.Abort();
            server.Shutdown(SocketShutdown.Both);
            conectado = 0;
            server.Close();
            MessageBox.Show("Desconectado");
            
            /*groupBox_consultas.Visible = false;
            groupBox_inciar.Visible = true;
            groupBox_registro.Visible = true;*/
        }

        private void conectadosGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int fila = e.RowIndex;
            invitado = conectados[fila];
        }

        private void Invitar_button_Click(object sender, EventArgs e)
        {
            string msj = "7/" + invitado;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(msj);
            server.Send(msg);
            groupBox1.Visible = true;
        }

        private void aceptar_button_Click(object sender, EventArgs e)
            //Cuando el cliente acepta la invitacion enviamos un 1 al servidor
        {
            string msj = "8/" + nombre + "/1";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(msj);
            server.Send(msg);
            
            Invitacion_groupBox.Visible = false;
            /*Conectados_groupBox.Visible = false;
            Tablero.Visible = true;
            Turno.Visible = true;
            Pregunta.Visible = true;
            Responde.Visible = true;
            Chat_groupBox.Visible = true;*/

        }

        private void rechazar_button_Click(object sender, EventArgs e)
            //Cuando el cliente rechaza la invitacion enviamos un o al servidor
        {
            string msj = "8/" + nombre + "/0";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(msj);
            server.Send(msg);

            Invitacion_groupBox.Visible = false;
        }

    }
}
