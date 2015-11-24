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
using Ookii.Dialogs.Wpf;
using Huffman_WPFDemo.Logic;
using System.Collections;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Huffman_WPFDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Serializable()]
    public partial class MainWindow : Window
    {
        public String FinalString { get; set; }
        public HuffmanTree huffmanTree { get; set; }
        public BitArray encodedBitArray { get; set; }
        public String actualPath { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBoxChar.Text = "";
            TextBoxCompress.Text = "";
            TextBoxSource.Text = "";
            FinalString = "";
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            //dlg.SelectedPath = Properties.Settings.Default.StoreFolder;
            dlg.ShowNewFolderButton = true;

            if (dlg.ShowDialog() == true)
            {
                string path = dlg.SelectedPath;
                actualPath = path;

                string[] filters = new[] { "*.txt", "*.hf" };
                string[] files = filters.SelectMany(f => Directory.GetFiles(path, f)).ToArray();

                if (files.Length == 0)
                {
                    MessageBox.Show("No existen archivos selecciona otra carpeta", "Informacion");
                }

                if (files.Length > 0)
                {
                    List<Archivo> listaArchivos = new List<Archivo>();
                    foreach (string a in files)
                    {
                        Archivo archivo = new Archivo(a);
                        listaArchivos.Add(archivo);
                    }
                    listBox.ItemsSource = listaArchivos;
                }
            }
        }
        public void cargarLista(string path)
        {
            string[] filters = new[] { "*.txt", "*.hf" };
            string[] files = filters.SelectMany(f => Directory.GetFiles(path, f)).ToArray();

            if (files.Length == 0)
            {
                MessageBox.Show("No existen archivos selecciona otra carpeta", "Informacion");
            }

            if (files.Length > 0)
            {
                List<Archivo> listaArchivos = new List<Archivo>();
                foreach (string a in files)
                {
                    Archivo archivo = new Archivo(a);
                    listaArchivos.Add(archivo);
                }
                listBox.ItemsSource = listaArchivos;
            }
        }

        public void createHuffmanTextFiles(string[] textFiles)
        {

            System.IO.StreamReader reader;
            for (int i = 0; i < textFiles.Length; i++)
            {
                reader = new System.IO.StreamReader(textFiles[i], System.Text.Encoding.UTF8);//I use unicode, gime a lot of problems to do that :(
                String empty = reader.ReadToEnd();
                //FinalString = FinalString + reader.ReadToEnd() + System.Environment.NewLine;
                byte[] bytes = Encoding.Default.GetBytes(reader.ReadToEnd());
                FinalString = empty;
            }

        }

        public void fillCharCount()
        {
            String count;
            StringBuilder sb = new StringBuilder();
            TextBoxChar.Text = "";
            var list = huffmanTree.Frequencies.ToList();

            foreach (KeyValuePair<char, int> kvp in list.OrderBy(i => i.Value))
            {
                Console.WriteLine("Character = {0}, Frecuency = {1}", kvp.Key, kvp.Value);
                count = "Char= " + kvp.Key.ToString() + "Fre= " + kvp.Value.ToString() + System.Environment.NewLine;
                sb.AppendLine("Char= " + kvp.Key.ToString() + "   " + "Freq: " + kvp.Value.ToString());
            }
            TextBoxChar.Text = sb.ToString();

        }
        public void fillOriginalText()
        {
            TextBoxSource.Text = "";
            TextBoxSource.Text = FinalString;
        }

        public void fillEncodeText(String input)
        {
            // Encode
            TextBoxCompress.Text = "";
            BitArray encoded = huffmanTree.Encode(input);
            encodedBitArray = encoded;
            StringBuilder resultString = new StringBuilder();
            String result = "Encode: " + System.Environment.NewLine;
            resultString.AppendLine(result);
            StringBuilder stringB = new StringBuilder();
            //stringB.AppendLine("Encode: ");
            foreach (bool bit in encoded)
            {
                Console.Write((bit ? 1 : 0) + "");
                stringB.Append(string.Format((bit ? 1 : 0) + ""));
            }
            TextBoxCompress.Text = stringB.ToString();
        }

        private void ButtonDecode_Click(object sender, RoutedEventArgs e)
        {
            String result = huffmanTree.Decode(encodedBitArray);
            TextBoxCompress.Text = result;
        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Archivo curArchivo = (Archivo)listBox.SelectedItem;
            string[] files = new string[1];
            files[0] = curArchivo.Ruta;

            if (System.IO.Path.GetExtension(curArchivo.Ruta).Contains("txt"))
            {
                createHuffmanTextFiles(files);
                Console.Write(FinalString);
                huffmanTree = new HuffmanTree();
                huffmanTree.Build(FinalString);
                fillCharCount();
                fillOriginalText();
                LabelURL.Content = "";
                LabelURL.Content = curArchivo.Ruta;
                fillEncodeText(FinalString);
            }
            if ((System.IO.Path.GetExtension(curArchivo.Ruta).Contains("hf")))
            {
                Stream stream = File.Open(curArchivo.Ruta, FileMode.Open);
                BinaryFormatter bformatter = new BinaryFormatter();

                HuffmanTree objetoRecuperado = (HuffmanTree)bformatter.Deserialize(stream);
                stream.Close();
                //mostrar la informacion en el computador una vez mas
                var sb = new StringBuilder();

                for (int i = 0; i < objetoRecuperado.compressedText.Count; i++)
                {
                    char c = objetoRecuperado.compressedText[i] ? '1' : '0';
                    sb.Append(c);
                }
                
                //return sb.ToString();
                TextBoxCompress.Text = sb.ToString();
                encodedBitArray = objetoRecuperado.compressedText;
                huffmanTree = objetoRecuperado;
            }

            
        }
        private void serializarObjeto()
        {
            Archivo curArchivo = (Archivo)listBox.SelectedItem;

            string result = System.IO.Path.ChangeExtension(curArchivo.Ruta, ".hf");


            HuffmanTree saveFiles = new HuffmanTree();
            saveFiles = huffmanTree;
            if(encodedBitArray != null)
            {
            saveFiles.compressedText = encodedBitArray;
            

            Stream stream = File.Open(result, FileMode.Create);
            BinaryFormatter bformatter = new BinaryFormatter();

            Console.WriteLine("Writing Employee Information");
            bformatter.Serialize(stream, saveFiles);
            stream.Close();
            cargarLista(actualPath);
            }
            else if(encodedBitArray == null)
            {
                MessageBox.Show("Selecciona un archivo primero.");
            }

            

        }

        private void ButtonGuardar_Click(object sender, RoutedEventArgs e)
        {
            serializarObjeto();
        }
    }
}
