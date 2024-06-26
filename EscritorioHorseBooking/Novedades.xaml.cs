﻿using Firebase.Storage;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace EscritorioHorseBooking
{
    /// <summary>
    /// Lógica de interacción para Novedades.xaml
    /// </summary>
    public partial class Novedades : Window
    {
        IFirebaseConfig config = new FirebaseConfig

        {
            AuthSecret = "b4EKkNKwSvFpmAdcRdMRWPv90myyYgIirOv6QULs",
            BasePath = "https://horsebooking-54bbe-default-rtdb.europe-west1.firebasedatabase.app"
        };
        IFirebaseClient client;
        FirebaseStorage firebaseStorage;
        public List<Novedad> ListaNovedades { get; set; }

        public Novedades()
        {
            InitializeComponent();
            ListaNovedades = new List<Novedad>();
        client = new FireSharp.FirebaseClient(config);
            firebaseStorage = new FirebaseStorage("horsebooking-54bbe.appspot.com", new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult("AIzaSyAMBaq52-eEyy_wMCDnTlx3gilW-gXRyfo"),
                ThrowOnCancel = true
            });
            if (client == null)
            {
                MessageBox.Show("Error en la conexión a Firebase");
            }
        }

        private async void crearNovedad_Click(object sender, RoutedEventArgs e)
        {
            string tituloNovedad = textBoxtituloNoticia.Text;
            string descripcion = textBoxdescNoticia.Text;

            FirebaseResponse response = await client.PushAsync("novedades/", new { titulo = tituloNovedad, descripcion = descripcion, fecha = DateTime.Now });
            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Body);
            string novedadId = result["name"];

            await client.UpdateAsync($"novedades/{novedadId}", new { id = novedadId, titulo = tituloNovedad, descripcion = descripcion, fecha = DateTime.Now });
            FirebaseResponse checkExistence = await client.GetAsync($"novedades/{novedadId}");
            if (checkExistence.Body == "null")
            {
                MessageBox.Show("Error: Novedad no encontrada después de creación, no se puede cargar la imagen.");
                return;
            }
            if (displayImage.Source != null)
            {
                try
                {
                    var stream = new MemoryStream();
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)displayImage.Source));
                    encoder.Save(stream);
                    stream.Position = 0;

                    var storageReference = firebaseStorage.Child("imagenesNovedades").Child($"{novedadId}.png");
                    MessageBox.Show("Generated Novedad ID: " + novedadId);
                    await storageReference.PutAsync(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar la imagen: {ex.Message}");
                }
            }
        }

        private void Inicio_Click(object sender, RoutedEventArgs e)
        {
            var pantalla = new PantallaInicial();
            pantalla.Show();
            this.Close();
        }

        private void Chat_Click(object sender, RoutedEventArgs e)
        {
            var chat = new Chat();
            chat.Show();
            this.Close();
        }

        private void Clases_Click(object sender, RoutedEventArgs e)
        {
            var crearClase = new crearClase();
            crearClase.Show();
            this.Close();
        }

        private void PerfilUsuario_Click(object sender, RoutedEventArgs e)
        {
            var perfil = new PerfilUsuario();
            perfil.Show();
            this.Close();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Seleccionar imagen";
            openFileDialog.Filter = "Archivos de Imagen (*.jpg;*.jpeg;*.gif;*.bmp;*.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                displayImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        public class Novedad
        {
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public string FechaInicio { get; set; }
        }

        private async void verNoticias_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("novedades/");
                var data = JsonConvert.DeserializeObject<Dictionary<string, Novedad>>(response.Body);
                if (data == null)
                {
                    MessageBox.Show("No se encontraron datos.");
                    return;
                }

                ListaNovedades.Clear();
                foreach (var item in data.Values)
                {
                    ListaNovedades.Add(item);
                }

                dataGridClases.ItemsSource = ListaNovedades;
                dataGridClases.Items.Refresh();
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show("Error al parsear los datos de Firebase: " + jsonEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar clases desde Firebase: " + ex.Message);
            }
        }
    }
}
