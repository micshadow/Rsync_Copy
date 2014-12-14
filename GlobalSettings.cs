using System;
using System.Collections.Generic;

namespace Rsync_Copy.Settings
{
    public class GlobalSettings
    {
        public string _defSavePath, _client_hostname, smbRemotePath, localScriptDirectory, _lastrunon;
        public bool StrictMode, ServerRunning;


        IniReader ir;

        public GlobalSettings(string SettingsFile)
        {
            ir = new IniReader(Environment.CurrentDirectory +
                "\\bin\\settings.ini");
            if (System.IO.File.Exists(
                Environment.CurrentDirectory + "\\bin\\settings.ini"))
            {
                _ReadSettings();
            }
            else
            {
                CreateDefaultIni();
                _ReadSettings();
            }
        }

        void _ReadSettings()
        {

            _defSavePath = ir.ReadString("Options", "DefaultSavePath",
                "C:\\Games");

            _lastrunon = ir.ReadString("Options", "LastRunOn", Environment.MachineName);


            if (!System.IO.Directory.Exists(_defSavePath)) _defSavePath = "C:\\Games";

            StrictMode = bool.Parse(ir.ReadString("Options",
                "StrictMode", "false"));

            localScriptDirectory = ir.ReadString("Options", "LocalScriptPath", Environment.CurrentDirectory +
                "\\bin\\Scripts\\" + Environment.MachineName);
            if (!System.IO.Directory.Exists(localScriptDirectory))
            {
                localScriptDirectory = Environment.CurrentDirectory + "\\bin\\Scripts\\" + Environment.MachineName;
                if(!System.IO.Directory.Exists(localScriptDirectory))
                {
                    System.IO.Directory.CreateDirectory(localScriptDirectory);
                }
            }

            ServerRunning = ir.ReadBoolean("Options", "ServerRunning", false);


            if (_lastrunon != Environment.MachineName)
            {
                System.IO.File.Delete(Environment.CurrentDirectory + "\\bin\\rsyncd.conf");
                CreateDefaultIni();
                _ReadSettings();
            }
        }

        public void SaveAllSettings()
        {
            ir.Write("Options", "DefaultSavePath", _defSavePath);
            ir.Write("Options", "StrictMode", StrictMode.ToString());
            ir.Write("Options", "LocalScriptPath", localScriptDirectory);
            ir.Write("Options", "ServerRunning", ServerRunning);
            ir.Write("Options", "LastRunOn", _lastrunon);



        }

        void CreateDefaultIni()
        {
            System.IO.File.WriteAllText(Environment.CurrentDirectory +
                "\\bin\\settings.ini",
                "[Options]\r\n" +
                "DefaultSavePath = C:\\Games\r\n" +
                "StrictMode = false\r\n" +
                "LastRunOn = " + Environment.MachineName + "\r\n" +
                "ServerRunning = false\r\n" +
                "LocalScriptPath = " + Environment.CurrentDirectory
                + "\\bin\\Scripts\\" + Environment.MachineName + "\r\n");
        }

    }

}
