﻿using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EscritorioHorseBooking
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IFirebaseConfig config = new FirebaseConfig

        {
            AuthSecret = "b4EKkNKwSvFpmAdcRdMRWPv90myyYgIirOv6QULs",
            BasePath = "https://horsebooking-54bbe-default-rtdb.europe-west1.firebasedatabase.app"
        };
        IFirebaseClient client;

        public MainWindow()
        {
            InitializeComponent();

            client = new FireSharp.FirebaseClient(config);
            if (client == null)
            {
                MessageBox.Show("Error en la conexión a Firebase");
            }
        }

        private async void registrar_Click(object sender, EventArgs e)
        {
            string emailTexto = email.Text.ToString();
            string contrasenaTexto = contrasena.Text.ToString();

            // Registrar usuario en Firebase Authentication

            // Añadir usuario a la base de datos en tiempo real
            FirebaseResponse response = await client.SetAsync("trabajadores/" + emailTexto.Replace('.', ','), new { Email = emailTexto, Password = contrasenaTexto });

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                MessageBox.Show("Usuario registrado correctamente en Firebase");
            }
            else
            {
                MessageBox.Show("Error al registrar el usuario en Firebase");
            }

            InicioSesion inicio = new InicioSesion();
            inicio.Show();
            this.Close();
        
    }

        private void email_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.Text = ""; // Borra el texto cuando el TextBox recibe el foco
                textBox.Foreground = Brushes.Black; // Cambia el color del texto a negro
            }
        }

        private void email_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Email";
                textBox.Foreground = Brushes.LightGray; // Cambia el color del texto a gris (opcional)
            }
        }

    }
}