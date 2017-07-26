using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConAppFolderFileFinder
{
    class Program
    {

   

        public static void WriteCSV<T>(IEnumerable<T> items, string path)
        {
            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(p => p.Name);

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));

                foreach (var item in items)
                {

                    writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null))));
                }
            }
        }



        static void Main(string[] args)
        {

         
            var txtFileToImportFromPath = @"\\tmpfil2\wave2\";

            var listFolderNames = Directory.EnumerateDirectories(txtFileToImportFromPath, "*.*", SearchOption.TopDirectoryOnly);
                        
            var dt = new DateTime(2017,5,6,0,0,0);
            var dt2 = new DateTime(2017,5,12,0,0,0);

            List<FileStuff> fileStuff = new List<FileStuff>();
            List<FinalFile> finalFile = new List<FinalFile>();
            List<FinalFile> finalFile1 = new List<FinalFile>();
            List<FinalFile> noMatch = new List<FinalFile>();

            List<FinalFile> mainAll = new List<FinalFile>();
            List<FinalFile> wavAll = new List<FinalFile>();

            List<WavStuff> wavFile = new List<WavStuff>();

            var count = 0;
            var testcount = 0;

            if (listFolderNames.Count() > 0)
            {
                foreach (var currentFolder in listFolderNames)
                {
                    if (new DirectoryInfo(currentFolder).CreationTime > dt && new DirectoryInfo(currentFolder).CreationTime < dt2)
                    {



                        //DateTime startTime = new DateTime(Directory.GetCreationTime(currentFolder)).AddSeconds(30);
                        //var range1 = new DateTime(Directory.GetCreationTime(currentFolder));



                        var directory = new DirectoryInfo(currentFolder);
                        var files = directory.GetFiles("*.txt")
                            .Where(v => v.CreationTime.Date >= v.CreationTime.Date.AddSeconds(-30) && v.CreationTime.Date <= v.CreationTime.Date.AddSeconds(30))
                            .OrderBy(d => d.CreationTime).ThenBy(z => z.Name).ToList();

                        foreach(var file in files)
                        {

                            string[] parts = Path.GetFileNameWithoutExtension(file.Name).Split('_');
                            string a = parts[0].Trim();
                            string b = parts[1].Trim();





                            if (b.Contains("249704"))
                            {
                                var aa = "";
                            }

                            if (b.Contains("111255532120170507"))
                            {
                                var xt = "";
                            }


                            if (b.Length < 12)
                            {

                                var f = new FileStuff();
                                f.FileName = file.Name;
                                f.ClientName = a;
                                f.CreationTime = file.CreationTime.AddTicks(-(file.CreationTime.Ticks % TimeSpan.TicksPerSecond));
                                f.LastWriteTime = file.LastWriteTime.AddTicks(-(file.LastWriteTime.Ticks % TimeSpan.TicksPerSecond));
                                f.ParsedName = b;
                                f.Length = file.Length; 
                                f.FileLength = file.Name.Length;
                                f.FolderName = currentFolder;

                                fileStuff.Add(f);
                            }
                            else
                            {
                                var ff = new WavStuff();
                                ff.FileName = file.Name;
                                ff.ClientName = a;
                                ff.CreationTime = file.CreationTime.AddTicks(-(file.CreationTime.Ticks % TimeSpan.TicksPerSecond));
                                ff.LastWriteTime = file.LastWriteTime.AddTicks(-(file.LastWriteTime.Ticks % TimeSpan.TicksPerSecond));
                                ff.ParsedName = b;
                                ff.FileLength = file.Name.Length;
                                ff.FolderName = currentFolder;
                                ff.Length = file.Length;

                                wavFile.Add(ff);
                            }
                                                        

                        }

                        


                    }

                }
            }//end if

            //todo  loop over collection and insert into a calibrus db table

            foreach (var main in fileStuff)
            {

                var final = new FinalFile();
                var notMatching = new FinalFile();

                foreach (var wav in wavFile)
                {

                    // .Where(v => v.CreationTime.Date >= v.CreationTime.Date.AddSeconds(-30) && v.CreationTime.Date <= v.CreationTime.Date.AddSeconds(30))
                    //  ff.CreationTime = file.CreationTime.AddTicks(-(file.CreationTime.Ticks % TimeSpan.TicksPerSecond));

                    //if (main.ClientName == wav.ClientName && main.CreationTime == wav.CreationTime && main.Length == wav.Length)

                    if (main.ParsedName.Contains("249704"))
                    {
                        if (wav.ParsedName.Contains("111255532120170507"))
                        {
                            var xt = "";
                        }
                    }

                   

                    if (main.ClientName == wav.ClientName && (main.LastWriteTime.AddSeconds(30) >= wav.LastWriteTime && main.LastWriteTime.AddSeconds(-30) <= wav.LastWriteTime) && main.Length == wav.Length)
                    {
                        final.ClientName = main.ClientName;
                        final.MainId = main.ParsedName;
                        final.WavName = Convert.ToInt64(wav.ParsedName);
                        final.CreationTime = main.CreationTime;
                        final.folder = main.FolderName;
                        finalFile.Add(final);
                    }
                   
                }
                
            }


            //Insert all the main without wav match 
            //foreach(var mainOnly in fileStuff)
            //{
            //    var finalCollection = new FinalFile();


            //    //var result = finalFile.FirstOrDefault(s => s.MainId.Contains(mainOnly.ParsedName));

            //    //if (result == null)
            //    if(!finalFile.Any(s => s.MainId == mainOnly.ParsedName))
            //    {
            //        finalCollection.ClientName = mainOnly.ClientName;
            //        finalCollection.MainId = mainOnly.ParsedName;
            //        //finalCollection.WavName = Convert.ToInt64(wav.ParsedName);
            //        finalCollection.CreationTime = mainOnly.CreationTime;
            //        finalCollection.folder = mainOnly.FolderName;
            //        mainAll.Add(finalCollection);
            //    }
              
                
            //}

            //WriteCSV(mainAll, @"C:\temp\MainOnly.csv");


            // WAV only

            //foreach (var wavOnly in wavFile)
            //{
            //    var wfinalCollection = new FinalFile();


            //    //var result = finalFile.FirstOrDefault(s => s.MainId.Contains(mainOnly.ParsedName));

            //    //if (result == null)
            //    if (!finalFile.Any(s => s.WavName.ToString() == wavOnly.ParsedName))
            //    {
            //        wfinalCollection.ClientName = wavOnly.ClientName;
            //        //finalCollection.MainId = mainOnly.ParsedName;
            //        wfinalCollection.WavName = Convert.ToInt64(wavOnly.ParsedName);
            //        wfinalCollection.CreationTime = wavOnly.CreationTime;
            //        wfinalCollection.folder = wavOnly.FolderName;
            //        wavAll.Add(wfinalCollection);
            //    }


            //}

            //WriteCSV(wavAll, @"C:\temp\WavOnly.csv");




            //var combined = finalFile.Concat(mainAll).Concat(wavAll);


            //WriteCSV(combined, @"C:\temp\matchingMainWav.csv");

            WriteCSV(finalFile, @"C:\temp\MainWavFinal.csv");



            string xx = "aa";

       }

    }


    public class FileStuff
    {
        public string ClientName { get; set; }
        public string FileName { get; set; }
        public int FileLength { get; set; }
        public string ParsedName { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string FolderName { get; set; }
        public long Length { get; set; }


    }

    public class WavStuff
    {
        public string ClientName { get; set; }
        public string FileName { get; set; }
        public int FileLength { get; set; }
        public string ParsedName { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string FolderName { get; set; }

        public long Length { get; set; }

    }



    public class FinalFile
    {
        public string ClientName { get; set; }
        public string MainId { get; set; }
        public long WavName { get; set; }
        public DateTime CreationTime { get; set; }
        public string folder { get; set; }

    }




}
