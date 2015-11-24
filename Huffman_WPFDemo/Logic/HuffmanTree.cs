using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Huffman_WPFDemo.Logic
{
    [Serializable()]
    public class HuffmanTree: ISerializable
    {
        public BitArray compressedText;
        private List<Node> nodes = new List<Node>();
        public Node Root { get; set; }

        public HuffmanTree()
        {

        }

        public HuffmanTree(SerializationInfo info, StreamingContext ctxt)
        {
            compressedText = (BitArray) info.GetValue("bitArray", typeof(BitArray));
            Root = (Node)info.GetValue("root", typeof(Node));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            //You can use any custom name for your name-value pair. But make sure you
            // read the values with the same name. For ex:- If you write EmpId as "EmployeeId"
            // then you should read the same with "EmployeeId"
            //info.AddValue("EmployeeId", EmpId);
            //info.AddValue("EmployeeName", EmpName);
            info.AddValue("bitArray", compressedText);
            info.AddValue("root", Root);
        }

        public Dictionary<char, int> Frequencies = new Dictionary<char, int>();

        public void Build(String source)
        {
            for (int i = 0; i < source.Length;i++ )//First add the frecuencies of the letter
            {
                if(!Frequencies.ContainsKey(source[i]))
                {
                    Frequencies.Add(source[i],0);
                }
                Frequencies[source[i]]++;
            }//ENd of the frequency 
            foreach(KeyValuePair<char,int> Symbol in Frequencies)// Just add some objects nodes in a List of node
            {
                nodes.Add(new Node() {Symbol = Symbol.Key , Frequency = Symbol.Value });
            }//All node in the list.

            while(nodes.Count > 1)//Creation of the Three
            {
                List<Node> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList<Node>();//Order the node by the node 0 will be the less frequency, the last will have a biggest frequency.
                if(nodes.Count>=1)
                {
                    List<Node> taken = orderedNodes.Take(2).ToList<Node>();//Take the first two Elements. The first two elements are the less frequency

                    Node parent = new Node() { Symbol = '*', Frequency=taken[0].Frequency+taken[1].Frequency, Left=taken[0],Right=taken[1]};
                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    nodes.Add(parent);
                }
                this.Root = nodes.FirstOrDefault();//Finally the here end the creation of the Trees
            }

        }

        public bool IsLeaf(Node node)
        {
            return (node.Left == null && node.Right == null);
        }

        public System.Collections.BitArray Encode(string source)
        {
            List<bool> encodedSource = new List<bool>();

            for (int i = 0; i < source.Length; i++)
            {
                if(this.Root != null)
                {
                    List<bool> encodedSymbol = this.Root.Traverse(source[i], new List<bool>());
                    encodedSource.AddRange(encodedSymbol);
                }
                if(this.Root == null)
                {
                    
                    
                }

                
            }

            BitArray bits = new BitArray(encodedSource.ToArray());

            return bits;
        }

        public string Decode(BitArray bits)
        {
            Node current = this.Root;
            string decoded = "";

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    if (current.Right != null)
                    {
                        current = current.Right;
                    }
                }
                else
                {
                    if (current.Left != null)
                    {
                        current = current.Left;
                    }
                }

                if (IsLeaf(current))
                {
                    decoded += current.Symbol;
                    current = this.Root;
                }
            }

            return decoded;
        }
    }
}
