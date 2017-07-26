using Algolia.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;


namespace ConAppAlgolia
{
    class Program
    {
        static void Main(string[] args)
        {

            AlgoliaClient client = new AlgoliaClient("SCVMS5FE3S", "4f9af7603070818440603e882fa48cbf");

            //https://www.algolia.com/doc/guides/indexing/import-synchronize-data/csharp/#importing-data


            //List arrayOfRecordsFromDatabase = new List();
            // load your records from database
            // [...]
            //index.SaveObjects(arrayOfRecordsFromDatabase);

            //var res = index.AddObject(JObject.Parse(@"{""firstname"":""Jimmie"", 
            //                               ""lastname"":""Barninger""}"), "myID");
            // Asynchronous
            // var res = await index.AddObjectAsync(JObject.Parse(@"{""firstname"":""Jimmie"",
            //                                                       ""lastname"":""Barninger""}"), "myID");
            //index.WaitTask(res["taskID"].ToString());
            // Asynchronous
            // await index.WaitTaskAsync(res["taskID"].ToString());


            //  \\TMPPRO2\c$\SBC\Reports\BoomerangHourlyReports


            try
            {



                var txtFileToImportFromPath = @"\\Tmppro3\sbc\reports\SWBConsumer\";  // @"\\TMPPRO2\c$\SBC\Reports\BoomerangHourlyReports\";
                //var txtFileToImportFromPath = @"\\TMPPRO2\c$\SBC\Reports\BoomerangHourlyReports\";

                //var listFolderNames = Directory.EnumerateDirectories(txtFileToImportFromPath, "*.*", SearchOption.TopDirectoryOnly);

                //var dt = new DateTime(2017, 5, 6, 0, 0, 0);
                //var dt2 = new DateTime(2017, 5, 12, 0, 0, 0);


                var dt = new DateTime(2017, 5, 12, 0, 0, 0);
                var dt2 = new DateTime(2017, 5, 13, 0, 0, 0);

                List<BoomFile> boomFile = new List<BoomFile>();

                int count = 0;
                // if (listFolderNames.Count() > 0)
                // {
                // foreach (var currentFolder in listFolderNames)
                // {
                // if (new DirectoryInfo(currentFolder).CreationTime > dt && new DirectoryInfo(currentFolder).CreationTime < dt2)
                //{
                var directory = new DirectoryInfo(txtFileToImportFromPath);
                        var files = directory.GetFiles("*.txt")
                            .Where(v => v.CreationTime.Date >= v.CreationTime.Date.AddSeconds(-30) 
                                    && v.CreationTime.Date <= v.CreationTime.Date.AddSeconds(30)
                                    && v.CreationTime > dt && v.CreationTime < dt2)
                            .OrderBy(d => d.CreationTime).ThenBy(z => z.Name).ToList();

                        foreach (var file in files)
                        {

                    
                            var boom = new BoomFile();
                            boom.FileName = file.Name;
                            boom.CreationTime = file.LastWriteTime.AddTicks(-(file.LastWriteTime.Ticks % TimeSpan.TicksPerSecond));
                            boom.FolderPath = txtFileToImportFromPath;

                    var fullPath = txtFileToImportFromPath + "\\" + file.Name;

                    if (new FileInfo(fullPath).Length == 0)
                    {
                        // file is empty
                    }
                    else
                    {
                        var fs = new FileStream(txtFileToImportFromPath + file.Name, FileMode.Open);

                        using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
                        {
                            boom.body = reader.ReadToEnd();
                        }


                        Index index = client.InitIndex("boomerang");
                        index.AddObject(boom);
                        Console.WriteLine(fullPath);
                        count++;
                        Console.WriteLine("written total to algolia: " + count);
                    }
                        }
                                                       

            }
            catch (Exception ex)
            {
                var e = ex.Message;

                
            }



            // Load JSON file
            //StreamReader re = File.OpenText("contacts.json");
            //JsonTextReader reader = new JsonTextReader(re);
            //JArray batch = JArray.Load(reader);
            //// Add objects
            //Index index = client.InitIndex("contacts");
            //index.AddObjects(batch);
            //// Asynchronous
            //// await index.AddObjectsAsync(batch);

            //Console.WriteLine(index.Search(new Query("jimmie")));



            // search by firstname
            // System.Diagnostics.Debug.WriteLine(index.Search(new Query("jimmie")));
            // Asynchronous
            // System.Diagnostics.Debug.WriteLine(await index.SearchAsync(new Query("jimmie")));
            // search a firstname with typo
            //System.Diagnostics.Debug.WriteLine(index.Search(new Query("jimie")));
            // Asynchronous
            // System.Diagnostics.Debug.WriteLine(await index.SearchAsync(new Query("jimie")));
            // search for a company
            //System.Diagnostics.Debug.WriteLine(index.Search(new Query("california paint")));
            // Asynchronous
            // System.Diagnostics.Debug.WriteLine(await index.SearchAsync(new Query("california paint")));
            // search for a firstname & company
            //System.Diagnostics.Debug.WriteLine(index.Search(new Query("jimmie paint")));
            // Asynchronous
            // System.Diagnostics.Debug.WriteLine(await index.SearchAsync(new Query("jimmie paint")));


        }

    }


    public class BoomFile
    {
        public string FileName { get; set; }
        public string body { get; set; }
        public DateTime CreationTime { get; set; }
        public string FolderPath { get; set; }

    }



}
