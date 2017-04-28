using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace VideoKeywordClipping
{

    
    
    class Program
    {
       
        public struct Time
        {
            public Time(int h_, int m_, float s_)
            {
                this.h = h_; this.m = m_; this.s = s_;
            }
            public int h;
            public int m;
            public float s;
        }
        public struct SentenceStruct
        {
            public SentenceStruct(int idx_, bool cv, string an, string vf, string cf, string sj, string sc, Time st, Time et)
            {
                this.idx = idx_;
                this.clipValid = cv;
                this.animeName = an;
                this.videoFilePos = vf;
                this.clipFilePos = cf;
                this.sentenceJap = sj;
                this.sentenceCN = sc;
                this.startTime = st;
                this.endTime = et;
            }
            public int idx;
            public bool clipValid;//whether have a valid clip.
            public string animeName;
            public string videoFilePos;
            public string clipFilePos;//if clip valid.
            public string sentenceJap;
            public string sentenceCN;
            public Time startTime;
            public Time endTime;
        }

        static void InvokeCmd(string command)
        {
           // command = "cd";
            System.Diagnostics.ProcessStartInfo procStartInfo =  new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);
            Console.WriteLine(command);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            //procStartInfo.CreateNoWindow = true;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            
            proc.StartInfo = procStartInfo;
            proc.Start();
            string result = proc.StandardOutput.ReadToEnd();
            // Display the command output.
            Console.WriteLine(result);
        }
        static void Clipping(List<SentenceStruct> Sentences, int idx, string keyword)
        {
            Time sc = Sentences[idx].startTime;
            Time ec = Sentences[idx].endTime;
            if (idx >= 1)
            {
                if (Sentences[idx].animeName == Sentences[idx-1].animeName)
                {
                    sc = Sentences[idx - 1].startTime;
                }
            }
            if(idx < Sentences.Count-1)
            {
                if (Sentences[idx].animeName == Sentences[idx + 1].animeName)
                {
                    ec = Sentences[idx + 1].endTime;
                }
            }
            string file_to_clip = Sentences[idx].videoFilePos;
            //string clip_file = Sentences[idx].clipFilePos + "/"+keyword+"/"+Sentences[idx].sentenceJap + ".mkv";
            string clip_file = Sentences[idx].clipFilePos +  "/" + Sentences[idx].sentenceJap + ".mkv";

            string invoke_cmd = "ffmpeg -i \"" +  file_to_clip + "\" -ss " + sc.h.ToString() + ":" + sc.m.ToString() + ":" + sc.s.ToString() + " -to " + ec.h.ToString() + ":" + ec.m.ToString() + ":" + ec.s.ToString() + " -c copy " 
                + "\"" + clip_file + "\"";
            InvokeCmd(invoke_cmd);
        }

        static void parseAssFile(string contents, List<SentenceStruct> Sentences)
        {
            SentenceStruct ss;


        }
        static void SentenceProcessing(string subtitleDir, List<SentenceStruct> Sentences)
        {
           
        }
        static void Main(string[] args)
        {
            string videoPoolDir = args[0];//Notice in C#, args[0] is the first arg, instead of args[1] in C++
            string subtitleDir = videoPoolDir + "\\subtitle";//fetch the data.txt file from each dir
            string clipPollDir = videoPoolDir + "\\clips";
            List<string> keywords = new List<string>();
            List<SentenceStruct> Sentences = new List<SentenceStruct>();

            SentenceProcessing(subtitleDir, Sentences);

           

            // Read the file and display it line by line.
            FileInfo fileinfo = new FileInfo("C:/Users/Lunam/Dropbox/Projects/VKC/VideoKeywordClipping/VideoKeywordClipping/keywords.txt");
            
            foreach(string line in File.ReadLines(fileinfo.FullName))
            {
                keywords.Add(line);
            }

            // Read data.txt file to init the SetenceStruct list
            FileInfo fi = new FileInfo("C:/Users/Lunam/Desktop/Books/Hibike/subtitle/data.txt");

            List<string> anime_data = new List<string>();
            foreach (string line in File.ReadLines(fi.FullName))
            {
                anime_data.Add(line);
            }
            for(int i = 0; i < anime_data.Count/9; i++)
            {

                int j = 9 * i;
                int idx_ = i;
                bool cp = false;
                string animeName = "Hebiki";
                string vf = "C:/Users/Lunam/Desktop/Books/Hibike/"+anime_data[j+8];
                string cf = "C:/Users/Lunam/Desktop/Books/Hibike/clips";
                //Console.WriteLine(anime_data[i]);

                Time start_t = new Time(Convert.ToInt32(anime_data[j]), Convert.ToInt32(anime_data[j + 1]), Convert.ToSingle(anime_data[j + 2]));
                Time end_t = new Time(Convert.ToInt32(anime_data[j+3]), Convert.ToInt32(anime_data[j + 4]), Convert.ToSingle(anime_data[j + 5]));
                string js = anime_data[j + 6];
                string cs = anime_data[j + 7];
                SentenceStruct st = new SentenceStruct(idx_,cp,animeName, vf,cf,js,cs,start_t,end_t);
                Sentences.Add(st);
            }
            
            for(int i = 0; i < keywords.Count; i++)
            {
                for(int j = 0; j < Sentences.Count; j++)
                {
                    if(Sentences[j].sentenceJap.Contains(keywords[i]))
                    {
                        Clipping(Sentences, j, keywords[i]);
                    }
                }
            }


            Console.WriteLine(videoPoolDir);
        }
    }
}
