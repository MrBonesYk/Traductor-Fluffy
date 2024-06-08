using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TraductorFluffy
{
    public partial class Form1 : Form
    {
        // Llama al diccionario en formato json.
        private Dictionary<string, List<string>> specificReplacements = new Dictionary<string, List<string>>();

        private readonly Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            LoadTranslations();
            AbrirTraductor();
        }

        private void LoadTranslations()
        {
            try
            {
                string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "translations.json");
                string jsonText = File.ReadAllText(jsonFilePath);
                specificReplacements = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonText);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading translations: {ex.Message}");
                specificReplacements = new Dictionary<string, List<string>>();
            }
        }

        #region Sonidos
        private void AbrirTraductor()
        {
            SoundPlayer Traductor = new SoundPlayer(Properties.Resources.iniciar_traductor);
            Traductor.Play();
        }
        private void LimpiarTextBox()
        {
            SoundPlayer Traductor = new SoundPlayer(Properties.Resources.limpiar);
            Traductor.Play();
        }
        private void Traducir()
        {
            SoundPlayer Traductor = new SoundPlayer(Properties.Resources.traducir);
            Traductor.Play();
        }
        private void AbrirVentana()
        {
            SoundPlayer Traductor = new SoundPlayer(Properties.Resources.abrir_creditos);
            Traductor.Play();
        }
        #endregion Sonidos

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void buttonModify_Click(object sender, EventArgs e)
        {
            string input = textBoxInput.Text;
            string modifiedText = ModifyText(input);
            textBoxOutput.Text = modifiedText;
            Traducir();
        }

        private string ModifyText(string input)
        {
            // Metodo para reemplazar palabras específicas usando el diccionario
            foreach (var replacement in specificReplacements)
            {
                input = Regex.Replace(input, $@"\b{replacement.Key}\b", match => GetRandomReplacement(replacement.Value), RegexOptions.IgnoreCase);
            }

            // Reemplazar 'll' y 'LL' aleatoriamente por 'w' o 'd'
            input = ReplaceLl(input);

            // Reemplazar 'r' y 'R' aleatoriamente por 'w' o 'd', con excepciones para palabras que empiezan con 'R'
            input = ReplaceRr(input);

            // Reemplazar 'l' y 'L' por 'd' 
            string result = input.Replace('l', 'd')
                                 .Replace('L', 'D');

            // Usar una expresión regular para reemplazar 'w' al final de las palabras por 'd'
            result = Regex.Replace(result, @"w\b", "d", RegexOptions.IgnoreCase);

            return result;
        }

        private string ReplaceLl(string input)
        {
            // Reemplazar 'll' y 'LL' aleatoriamente por 'w' o 'd'
            input = Regex.Replace(input, "ll", match => GetRandomReplacement(new List<string> { "w", "d" }), RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "LL", match => GetRandomReplacement(new List<string> { "W", "D" }), RegexOptions.IgnoreCase);
            return input;
        }

        private string ReplaceRr(string input)
        {
            // Reemplazar 'r' y 'R' aleatoriamente por 'w' o 'd'
            input = Regex.Replace(input, @"\br", match => "d", RegexOptions.IgnoreCase); // Restringir palabras que empiezan con 'r' o 'R'
            input = Regex.Replace(input, @"\bR", match => "D", RegexOptions.IgnoreCase); // Restringir Palabras que empiezan con 'R' x2 xd
            input = Regex.Replace(input, "r", match => GetRandomReplacement(new List<string> { "d", "w" }), RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "R", match => GetRandomReplacement(new List<string> { "D", "W" }), RegexOptions.IgnoreCase);
            return input;
        }

        private string GetRandomReplacement(List<string> replacements)
        {
            // Seleccionar un reemplazo aleatorio de la lista
            return replacements[random.Next(replacements.Count)];
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            // Limpiar el contenido de los cuadros de texto
            textBoxInput.Text = string.Empty;
            textBoxOutput.Text = string.Empty;
            LimpiarTextBox();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            // Minimizar el programa
            this.WindowState = FormWindowState.Minimized;
        }

        private void label5_Click(object sender, EventArgs e)
        {
            // Cerrar el programa
            Application.Exit();
            Application.ExitThread();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AbrirVentana();
            // Mostrar el formulario de créditos
            CreditsForm creditsForm = new CreditsForm();
            creditsForm.ShowDialog();
        }
    }
}
