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

namespace Huffman_WPFDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public String FinalString { get; set; }
        public HuffmanTree huffmanTree { get; set; }

        public BitArray encodedBitArray { get; set; }
        
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
                string[] files = System.IO.Directory.GetFiles(path, "*.txt");
                if(files.Length == 0)
                {
                    MessageBox.Show("No text files found, select another folder","Information");
                }
                if(files.Length > 0)
                {
                    createHuffmanTextFiles(files);
                    Console.Write(FinalString);
                    huffmanTree = new HuffmanTree();
                    huffmanTree.Build(FinalString);
                    fillCharCount();
                    fillOriginalText();
                    LabelURL.Content = "";
                    LabelURL.Content = path;
                    fillEncodeText(FinalString);
                    
                }
            }
        }

        public void createHuffmanTextFiles(string[] textFiles)
        {
            
            System.IO.StreamReader reader;
            for (int i = 0; i < textFiles.Length;i++ )
            {
                 reader = new System.IO.StreamReader(textFiles[i], System.Text.Encoding.UTF8);//I use unicode, gime a lot of problems to do that :(
                 String empty = reader.ReadToEnd();
                 //FinalString = FinalString + reader.ReadToEnd() + System.Environment.NewLine;
                 byte[] bytes = Encoding.Default.GetBytes(reader.ReadToEnd() );
                 FinalString = empty;
            }
            
        }

        public void fillCharCount()
            
        {
            String count;
            StringBuilder sb = new StringBuilder();
            TextBoxChar.Text = "";
            var list = huffmanTree.Frequencies.ToList();
            
            foreach (KeyValuePair<char, int> kvp in list.OrderBy(i => i.Value) )
            {
                Console.WriteLine("Character = {0}, Frecuency = {1}", kvp.Key, kvp.Value);
                 count =  "Char= " + kvp.Key.ToString() + "Fre= "+ kvp.Value.ToString() + System.Environment.NewLine;
                 sb.AppendLine("Char= "+kvp.Key.ToString() +"   "+"Freq: "+kvp.Value.ToString() );
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
            resultString.AppendLine(result) ;
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
    }
}
