using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using XMLParserUnity;

namespace Main
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            XMLParser xml_parser = new XMLParser();
            xml_parser.xmlParser();
            XMLParser.FileToCreate[] temp = new XMLParser.FileToCreate[2];
            temp[0] = XMLParser.FileToCreate.Buildings;
            temp[1] = XMLParser.FileToCreate.Streets;
            xml_parser.createFile(temp);
        }
    }
}
