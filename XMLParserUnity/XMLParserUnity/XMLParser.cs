using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;
using System.IO;


namespace XMLParserUnity
{
    public class XMLParser
    {
        //hash tables use for store the data and retrive them in a fast way
        private Dictionary<int, DataForElements> element_to_nodes = new Dictionary<int, DataForElements>();
        private Dictionary<int, DataStructure> nodes_data = new Dictionary<int, DataStructure>();
        private double max_lon_b = Double.MinValue, min_lon_b = Double.MaxValue, max_lat_b = Double.MinValue, min_lat_b = Double.MaxValue, max_lon_s = Double.MinValue, min_lon_s = Double.MaxValue, max_lat_s = Double.MinValue, min_lat_s = Double.MaxValue;
        private class DataStructure
        {
             public string attribute = null;
             public string value = null;           
             public double lat;
             public double lon;
             public double height;

            public DataStructure(double lat, double lon, double height, string attribute, string value)
            {
              this.lat = lat;
              this.lon = lon;
              this.height = height;
              this.attribute = attribute;
              this.value = value;
            }

        }

        private class DataForElements
        {
            public List<int> nodes = null;
            public string attribute = null;

            public DataForElements(string attribute, List<int> nodes)
            {
                this.attribute = attribute;
                this.nodes = nodes;
            }
        }

        ///<summary>
        ///Enumeration used for indicate whic kind of file the user want to create from the dataset
        ///</summary>
        public enum FileToCreate
        {
            Buildings,
            Streets,
            Nature,
            Railways
        }

        public virtual void xmlParser()
        {

            //Create the XmlDocument.
            XmlDocument xml_doc = new XmlDocument();
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "XML Files|*.xml";
            string file_path = string.Empty;
            //Set the starting directory and the title.

            file.InitialDirectory = "c:\\";
            file.Title = "Select a text file";

            if (file.ShowDialog() == DialogResult.OK)
            {
                file_path = file.FileName;
            }

            if (file_path == string.Empty)
            {
                return;
            }

            xml_doc.Load(file_path);
         
            XmlNodeReader node = new XmlNodeReader(xml_doc);
            node.Read();
            double minlat=0;
            double minlon=0;
            double maxlat = 0;
            double maxlon = 0;

            while(!node.EOF)
            {
                
                switch (node.NodeType)
                {
                    case XmlNodeType.Element:

                        string temp = null;
                        //check the node name

                        //if i´m considering the node where there are all the boundaries
                        if (node.LocalName == "bounds")
                        {
                         

                            if (node.GetAttribute("minlat") == null || node.GetAttribute("minlon") == null ||
                                node.GetAttribute("maxlat") == null || node.GetAttribute("maxlon") == null)
                            {
                                throw new ArgumentException("In the xml file is missing boundatries");
                            }
                           
                                temp = node.GetAttribute("minlat").Replace('.', ',');
                                Double.TryParse(temp, out minlat);

                                temp = node.GetAttribute("minlon").Replace('.', ',');
                                Double.TryParse(temp, out minlon);
                               
                                temp = node.GetAttribute("maxlat").Replace('.', ',');
                                Double.TryParse(temp, out maxlat);
                          
                                temp = node.GetAttribute("maxlon").Replace('.', ',');
                                Double.TryParse(temp, out maxlon);

                                
                                break;
                        }

                        //i´m considering the nodes that are gonna be part of the elements in the city
                        if (node.LocalName == "node" )
                        {

                            string attribute = null;
                            string value = null;
                            int id;
                            double lat;
                            double lon;
                            double height;

                            if (node.GetAttribute("id") == null)
                            {
                                throw new ArgumentException("A node miss the id reference. Its position is in line " + node.Depth);
                            }

                            if (node.GetAttribute("lon") == null)
                            {
                                throw new ArgumentException("A node miss the longitude. The id of the node is " + node.GetAttribute("id"));
                            }
                            if (node.GetAttribute("lat") == null)
                            {
                                throw new ArgumentException("A node miss the latitude. The id of the node is " + node.GetAttribute("id"));
                            }

                            temp = node.GetAttribute("id").Replace('.', ',');
                            if(!Int32.TryParse(temp, out id))
                            {
                                throw new ArgumentException("The node can not be parsed " + node.GetAttribute("id"));
                            }
                           
                            temp = node.GetAttribute("lat").Replace('.', ',');
                            if (!Double.TryParse(temp, out lat))
                            {
                                throw new ArgumentException("The node can not be parsed " + node.GetAttribute("id"));
                            }

                            //normalization(ref lat, maxlat,minlat,0,1);
                            temp = node.GetAttribute("lon").Replace('.', ',');
                            if (!Double.TryParse(temp, out lon))
                            {
                                throw new ArgumentException("The node can not be parsed " + node.GetAttribute("id"));
                            }

                            //normalization(ref lon, maxlon, minlon, 0, 1);
                            //height value
                            if (node.GetAttribute("height") != null)
                            {
                                temp = node.GetAttribute("height").Replace('.', ',');
                                Double.TryParse(temp, out height);
                            }
                            else
                            {
                                height = 0;
                            }


                         
                            /*
                             * check if there is a tag section. Because with XmlReader when we use readInnerXML() for check the tags
                             * skip to the next line i have to put the code here and can't perform the check before
                             */
                            
                            node.Read();
                            bool valid = checkValidTag(node,ref attribute,ref value);
                            
                            //if is a valid node that i want to use in my application
                            if(valid)
                            {
                               
                            }

                            if (!nodes_data.ContainsKey(id))
                            {
                                nodes_data.Add(id, new DataStructure(lat, lon, height, attribute, value));
                            }
                            else
                            {
                                throw new ArgumentException("The node is already inside the hash table more than 1 reference inside the file. The id of the node is " + id);
                            }
                            continue;
                          
                        }

                        if(node.LocalName == "way")
                        {
                            string attribute = null;
                            string value = null;
                            int id;
                            List<int> list_of_nodes = null;
                            
                            if (node.GetAttribute("id") == null)
                            {
                                throw new ArgumentException("A node miss the id reference. Its position is in line " + node.Depth);
                            }

                            temp = node.GetAttribute("id").Replace('.', ',');
                            if (!Int32.TryParse(temp, out id))
                            {
                                throw new ArgumentException("The node can not be parsed " + node.GetAttribute("id"));
                            }
                            node.Read();
                            list_of_nodes = nodesConnected(node);
                            if(checkValidTag(node, ref attribute, ref value))
                            {
                                DataForElements data = new DataForElements(attribute,list_of_nodes);

                                if (attribute == "highway") {
                                    foreach (int element in list_of_nodes) {
                                        if (max_lat_s < nodes_data[element].lat) {
                                            max_lat_s = nodes_data[element].lat;
                                        }

                                        if (min_lat_s > nodes_data[element].lat) {
                                            min_lat_s = nodes_data[element].lat;
                                        }

                                        if (max_lon_s < nodes_data[element].lon) {
                                            max_lon_s = nodes_data[element].lon;
                                        }

                                        if (min_lon_s > nodes_data[element].lon) {
                                            min_lon_s = nodes_data[element].lon;
                                        }
                                    }
                                }

                                if (attribute == "building") {
                                    foreach (int element in list_of_nodes) {
                                        if (max_lat_b < nodes_data[element].lat) {
                                            max_lat_b = nodes_data[element].lat;
                                        }

                                        if (min_lat_b > nodes_data[element].lat) {
                                            min_lat_b = nodes_data[element].lat;
                                        }

                                        if (max_lon_b < nodes_data[element].lon) {
                                            max_lon_b = nodes_data[element].lon;
                                        }

                                        if (min_lon_b > nodes_data[element].lon) {
                                            min_lon_b = nodes_data[element].lon;
                                        }
                                    }
                                }


                                if (!element_to_nodes.ContainsKey(id))
                                {
                                    element_to_nodes.Add(id, data);
                                }
                                else
                                {
                                    throw new ArgumentException("The node is already inside the hash table more than 1 reference inside the file. The id of the node is " + id);
                                }

                            
                            }

                            continue;
                        }

                        break;
                }

                node.Read();
               

            }

        }


        //normalize the data
        private double normalization(double number, double max, double min,int min_range, int max_range)
        {
            number = ((number - min) / (max - min)) * (max_range - min_range) + min_range;
            return number;
        }

        ///<summary>
        ///Create the type of file that the user select from the dataset
        ///</summary>
        ///<param name="type_of_file_to_create"> An array of enums indicating which kind of files to create</param>
        public void createFile(params FileToCreate[] type_of_file_to_create)
        {
            bool building = false;
            bool street = false;
            bool railway = false;
            bool nature = false;

            for (int i = 0; i < type_of_file_to_create.Length; i++)
            {
                if(type_of_file_to_create[i] == FileToCreate.Buildings)
                {
                    building = true;
                }

                if (type_of_file_to_create[i] == FileToCreate.Nature)
                {
                    nature = true;
                }

                if (type_of_file_to_create[i] == FileToCreate.Railways)
                {
                    railway = true;
                }

                if (type_of_file_to_create[i] == FileToCreate.Streets)
                {
                    street = true;
                }
            }
        

            foreach (int key in element_to_nodes.Keys)
            {
                DataForElements list_of_nodes = null;
                element_to_nodes.TryGetValue(key, out list_of_nodes);

                //check which kind of node i'm considering
                switch (list_of_nodes.attribute)
                {
                    case "building":
                        if (building)
                        {
                            writeFile("Building.txt", key, ref list_of_nodes);
                        }
                        break;

                    case "highway":
                        if (street)
                        {
                            writeFile("Highway.txt", key, ref list_of_nodes);
                        }
                        break;

                    case "railway":
                        if (railway)
                        {
                            writeFile("Railway.txt", key, ref list_of_nodes);
                        }
                        break;

                    case "natural":
                        if (nature)
                        {
                            writeFile("Natural.txt", key, ref list_of_nodes);
                        }
                        break;

                }

            }
        }

        ///<summary>
        ///Delete user selected files
        ///</summary>
        ///<param name="type_of_file_to_create"> An array of enums indicating which kind of files to delete</param>
        public void deleteFiles(FileToCreate[] type_of_file_to_create)
        {
            bool building = false;
            bool street = false;
            bool railway = false;
            bool nature = false;

            for (int i = 0; i < type_of_file_to_create.Length; i++)
            {
                if (type_of_file_to_create[i] == FileToCreate.Buildings)
                {
                    building = true;
                }

                if (type_of_file_to_create[i] == FileToCreate.Nature)
                {
                    nature = true;
                }

                if (type_of_file_to_create[i] == FileToCreate.Railways)
                {
                    railway = true;
                }

                if (type_of_file_to_create[i] == FileToCreate.Streets)
                {
                    street = true;
                }
            }

            //delete the files if already exist

            if (File.Exists("Building.txt") && building)
            {
                File.Delete("Building.txt");

            }
            if (File.Exists("Highway.txt") && street)
            {
                File.Delete("Highway.txt");

            }
            if (File.Exists("Railway.txt") && railway)
            {
                File.Delete("Railway.txt");

            }
            if (File.Exists("Natural.txt") && nature)
            {
                File.Delete("Natural.txt");

            }
            
        }

        ///<summary>
        ///Delete all the files
        ///</summary>
        public void deleteAllFiles()
        {

            //delete the files if already exist

            if (File.Exists("Building.txt"))
            {
                File.Delete("Building.txt");

            }
            if (File.Exists("Highway.txt"))
            {
                File.Delete("Highway.txt");

            }
            if (File.Exists("Railway.txt"))
            {
                File.Delete("Railway.txt");

            }
            if (File.Exists("Natural.txt"))
            {
                File.Delete("Natural.txt");

            }

        }
        
   
        ///<summary>
        ///Read the selected file and return the list of nodes composing every element inside that file
        ///</summary>
        ///<param name="file_name"> the name of the file to read</param>
        ///<returns> A List of elements where for each element there are the nodes composing it</returns>
        public List<List<double[]>> read(string file_name)
        {
            List<List<double[]>> data = new List<List<double[]>>();
            string[] lines = System.IO.File.ReadAllLines(file_name);
            char[] delimiterChars = {':', ' '};
            List<double[]> temp_list = new List<double[]>(); 
            double[] temp = null;

            if(!File.Exists(file_name))
            {
                throw new ArgumentException("The file you are trying to open doesn't exist");
            }
            foreach (string line in lines)
            {
                if(line.Contains("id"))
                {
                    if(temp_list.Count != 0)
                    {
                        data.Add(temp_list);
                    }
                    
                    temp_list =  new List<double[]>();
                }

                if(line.Contains("latitude"))
                {
                    temp = new double[3];
                    string[] words = line.Split(delimiterChars);                    
                    Double.TryParse(words[3], out temp[0]);
                    Double.TryParse(words[5], out temp[1]);
                    Double.TryParse(words[7], out temp[2]);
                   
                    temp_list.Add(temp);                   
                }
                
                                   
                              
            }
            data.Add(temp_list);
            return data;
        }

        private void writeFile(string file_name,int key,ref DataForElements list_of_nodes)
        {
            /*
            XmlDocument xmlDoc = new XmlDocument();
            
            if (!File.Exists(file_name))
            {
                XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                XmlComment comment = xmlDoc.CreateComment("This is a generated XML file");
                xmlDoc.AppendChild(declaration);
                xmlDoc.AppendChild(comment);
                xmlDoc.AppendChild(xmlDoc.CreateElement("root"));
                xmlDoc.Save(file_name);
            }
            else
            {
               
                xmlDoc.Load(file_name);
            }

            //for each node componing this element
            foreach (int elm in list_of_nodes.nodes)
            {
                //look up in the hash table where all the nodes are resident with the information

                //if the hash table doesn't contain the node throw exception
                if (!nodes_data.ContainsKey(elm))
                {
                    throw new ArgumentException("The hash table doesn't contain this node id: " + elm);
                }
                else
                {
                    //write the data in the file
                    DataStructure data = nodes_data[elm];
                    //create node and add value



                    XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, "Node_id", key.ToString());
                    //node.InnerText = "this is new node";

                    XmlNode productsNode = xmlDoc.CreateElement("data");
                   // xmlDoc.AppendChild(productsNode);

                    XmlAttribute node_title = xmlDoc.CreateAttribute("id_sub_node");
                    node_title.Value = elm.ToString();


                    XmlAttribute node_latitude = xmlDoc.CreateAttribute("latitude");
                    node_latitude.Value = data.lat.ToString();

                    XmlAttribute node_longitude = xmlDoc.CreateAttribute("longitude");
                    node_longitude.Value = data.lon.ToString();

                    XmlAttribute node_highness = xmlDoc.CreateAttribute("highness");
                    node_highness.Value = data.height.ToString();

                    productsNode.Attributes.Append(node_title);
                    productsNode.Attributes.Append(node_latitude);
                    productsNode.Attributes.Append(node_longitude);
                    productsNode.Attributes.Append(node_highness);

                    //add to parent node
                    node.AppendChild(productsNode);
                                    
                    //add to elements collection
                    xmlDoc.DocumentElement.AppendChild(node);
                    //save back
                    xmlDoc.Save(file_name);
                }

            }
            */
            
            using (StreamWriter w = File.AppendText(file_name))
            {
                w.WriteLine("id:" + key);

                if (file_name == "Building.txt") {
                    int break1;
                    break1 = 1;
                }
                //for each node componing this element
                foreach (int elm in list_of_nodes.nodes)
                {
                    //look up in the hash table where all the nodes are resident with the information

                    //if the hash table doesn't contain the node throw exception
                    if (!nodes_data.ContainsKey(elm))
                    {
                        throw new ArgumentException("The hash table doesn't contain this node id:" + elm);
                    }
                    else
                    {
                        //write the data in the file
                        DataStructure data = nodes_data[elm];
                        if(file_name == "Building.txt")
                            w.WriteLine("sub_node:" + elm + " longitude:" + normalization(data.lon, max_lon_b, min_lon_b, 0, 1) + " latitude:" + normalization(data.lat, max_lat_b, min_lat_b, 0, 1) + " height:" + data.height);
                        else if(file_name == "Highway.txt")
                                w.WriteLine("sub_node:" + elm + " longitude:" + normalization(data.lon, max_lon_s, min_lon_s, 0, 1) + " latitude:" + normalization(data.lat, max_lat_s, min_lat_s, 0, 1) + " height:" + data.height);
                    }

                }

            }
             
        }

        private bool checkValidTag(XmlNodeReader node, ref string attribute, ref string value)
        {
            bool valid_node = false;
            
            if(!(node.LocalName == "tag"))
            {
                return true;
            }
            while(node.LocalName == "tag")
            {
                if (node.GetAttribute("k").ToLower() == "building")
                {
                    attribute = node.GetAttribute("k");
                    value = node.GetAttribute("v");                
                    valid_node = true;
                }

                if (node.GetAttribute("k").ToLower() == "highway")
                {
                    attribute = node.GetAttribute("k");
                    value = node.GetAttribute("v");
                    valid_node = true;
                }

                if (node.GetAttribute("k").ToLower() == "natural")
                {
                    attribute = node.GetAttribute("k");
                    value = node.GetAttribute("v");
                    valid_node = true;
                }

                if (node.GetAttribute("k").ToLower() == "railway")
                {
                    attribute = node.GetAttribute("k");
                    value = node.GetAttribute("v");
                    valid_node = true;
                }

                node.Read();
            }

            return valid_node;
        }

        //take all the nodes that constitue a building or something else
        private List<int> nodesConnected(XmlNodeReader node)
        {
            List<int> nodes = new List<int>();
            int temp;
            while (node.LocalName == "nd")
            {
              Int32.TryParse(node.GetAttribute("ref"),out temp);
              nodes.Add(temp);
              node.Read();
            }

            return nodes;
        }
       
        /*
         private void databaseConnectionSql()
        {
            int result;

            SqlConnection connection = new SqlConnection();
            
            //the connection don't work. Is to change how he refers to the database
            connection.ConnectionString = "Data Source=.\\SQLEXPRESS;Database=Database.mdf;Integrated Security=True;User Instance=True";
           
            connection.Open();
              using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText =
                            @" 
            IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'RX_CMMData' AND TABLE_NAME = 'Buildings')
            BEGIN
            CREATE TABLE Buildings (
              Id       integer PRIMARY KEY NOT NULL,
              Longitude  integer NOT NULL,
              Latitude   integer NOT NULL,
              Highness   integer NULL,
             
            ); END";
                        cmd.ExecuteNonQuery();
                    }

            try
            {

             System.Data.SqlClient.SqlCommand dataCommand = new SqlCommand();
             dataCommand.Connection = connection;
 
             //tell the compiler and database that we're using parameters (thus the @first, @last, @nick)
	        dataCommand.CommandText = ("INSERT Buildings (Id, Longitude, Latitude) VALUES (@Id, @Longitude, @Latitude)");
	        
	        //add our parameters to our command object
	        dataCommand.Parameters.AddWithValue("@Id", 2);
            dataCommand.Parameters.AddWithValue("@Longitude", 3);
            dataCommand.Parameters.AddWithValue("@Latitude", 4);
            dataCommand.ExecuteNonQuery();
                /*
                table = rs.GetSchemaTable();
                DataRow new_row = table.NewRow();
                new_row["ID"] = 3;
                new_row["Longitude"] = 4;
                new_row["Latitude"] = 6;
                table.Rows.Add(new_row);
                rs.Update();
                 * 
                */
                 
                //t.DataSet = rs;
                /*
                SqlCommand com1 = new SqlCommand("SELECT ID FROM Attributes", connection);
                {
                    SqlDataReader reader = com1.ExecuteReader();
                    while (reader.Read())
                    {
                        int num = reader.GetInt32(0);
                        //Console.WriteLine(num);
                    }
                }

            }

            catch (SqlException sqlexception)
            {



            }

            catch (Exception ex)
            {


            }

        
                connection.Close();
         
            
        }
        */
        /*
        private void databaseConnectionSqlCE()
        {
            int result;
            SqlCeConnection connection = new SqlCeConnection();


            connection.ConnectionString = "Data Source = .\\Database.sdf";
           
            connection.Open();

            try
            {

              SqlCeCommand com = new SqlCeCommand(@"INSERT INTO Attributes (ID, Longitude, Latitude)
                    VALUES (@ID, @Longitude, @Latitude)", connection);
                {
                    com.Parameters.AddWithValue("@ID", 10);
                    com.Parameters.AddWithValue("@Longitude", 11);
                    com.Parameters.AddWithValue("@Latitude", 12);
 
                    com.ExecuteNonQuery();
                }
                /*
                table = rs.GetSchemaTable();
                DataRow new_row = table.NewRow();
                new_row["ID"] = 3;
                new_row["Longitude"] = 4;
                new_row["Latitude"] = 6;
                table.Rows.Add(new_row);
                rs.Update();
                 * 
                */
                 
                //t.DataSet = rs;
                /*
                SqlCeCommand com1 = new SqlCeCommand("SELECT ID FROM Attributes", connection);
                {
                    SqlCeDataReader reader = com1.ExecuteReader();
                    while (reader.Read())
                    {
                        int num = reader.GetInt32(0);
                        //Console.WriteLine(num);
                    }
                }

            }

            catch (SqlCeException sqlexception)
            {



            }

            catch (Exception ex)
            {


            }

        
                connection.Close();
         
            
        }
                 * */
        /*
        public int readSqlCe()
        {
            int num = 0;
            using (SqlCeCommand com1 = new SqlCeCommand("SELECT ID FROM Attributes", connection))
            {
                SqlCeDataReader reader = com1.ExecuteReader();
                while (reader.Read())
                {
                     num += reader.GetInt32(0);
                    //Console.WriteLine(num);
                }
            }

            return num;
        }
        */
      
    }
    
}
