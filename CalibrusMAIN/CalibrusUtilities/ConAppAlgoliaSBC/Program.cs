using Algolia.Search;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConAppAlgoliaSBC
{
    class Program
    {
        static void Main(string[] args)
        {
            AlgoliaClient client = new AlgoliaClient("SCVMS5FE3S", "4f9af7603070818440603e882fa48cbf");
                                    
            try
            {
                
                var txtFileToImportFromPath = @"\\Tmppro3\sbc\reports\";


                var listFolderNames = Directory.EnumerateDirectories(txtFileToImportFromPath, "*.*", SearchOption.TopDirectoryOnly);

                var dt = new DateTime(2017, 5, 12, 0, 0, 0);
                var dt2 = new DateTime(2017, 5, 13, 0, 0, 0);

                List<BoomFile> boomFile = new List<BoomFile>();

                int count = 0;

                if (listFolderNames.Count() > 0)
                {
                    foreach (var currentFolder in listFolderNames)
                    {

                        Console.WriteLine(currentFolder);

                        //if (new DirectoryInfo(currentFolder).CreationTime > dt && new DirectoryInfo(currentFolder).CreationTime < dt2)
                        //{
                            var directory = new DirectoryInfo(currentFolder);
                        var files = directory.GetFiles("*.txt")
                        //var files = directory.GetFiles("*.xls")
                                .Where(v => v.CreationTime.Date >= v.CreationTime.Date.AddSeconds(-30)
                                        && v.CreationTime.Date <= v.CreationTime.Date.AddSeconds(30)
                                        && v.CreationTime > dt && v.CreationTime < dt2)
                                .OrderBy(d => d.CreationTime).ThenBy(z => z.Name).ToList();

                        Console.WriteLine(files.Count);

                            foreach (var file in files)
                            {


                                var boom = new BoomFile();
                                boom.FileName = file.Name;
                                boom.CreationTime = file.LastWriteTime.AddTicks(-(file.LastWriteTime.Ticks % TimeSpan.TicksPerSecond));
                                boom.FolderPath = currentFolder;

                                

                                var fullPath = currentFolder + "\\" + file.Name;


                                //if (file.Name.Contains(".xls") || file.Name.Contains(".xlsx"))
                                //{
                                //    string pathToExcelFile = fullPath;
                                //    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                                //    var worksheetsList = excelFile.GetWorksheetNames();
                                //    excelFile.GetWorksheetNames();
                                    
                                //    //var columnNames = excelFile.GetColumnNames(selectedSheetName);


                                //}
                                //else
                                //{




                                    if (new FileInfo(fullPath).Length == 0)
                                    {
                                        // file is empty
                                    }
                                    else
                                    {
                                        var fs = new FileStream(fullPath, FileMode.Open);

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


                                //}

                            }

                        //}

                    }
                }
            }
            catch (Exception ex)
            {
                var e = ex.Message;


            }

            
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
