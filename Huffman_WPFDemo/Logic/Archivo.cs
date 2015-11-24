using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Huffman_WPFDemo.Logic
{
    public class Archivo
    {
        public string Nombre { get; set; }
        public string Ruta { get; set; }

        public Archivo(string ruta)
        {
            this.Ruta = ruta;
            this.Nombre = Path.GetFileName(ruta);
        }

        public override string ToString() 
        {
            return Nombre;
        }
        
    }
}
