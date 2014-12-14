#define wtfmode
#undef wtfmode
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Rsync_Copy.Daemon
{
    public class DaemonDiscovery
    {
        public const int MSG_PORT = 4555;

        System.Net.Sockets.UdpClient _uc;

        public DaemonDiscovery()
        {
            System.Threading.Thread th = new System.Threading.Thread(
                new System.Threading.ThreadStart(Listen));
            _uc = SafeStartListen();
            th.Start();
        }
        static System.Net.Sockets.UdpClient SafeStartListen()
        {
            System.Net.Sockets.UdpClient u = null;
            try
            {
                u = new System.Net.Sockets.UdpClient();
                IPEndPoint ipe = new IPEndPoint(IPAddress.Any,MSG_PORT);
                u.Client.ExclusiveAddressUse = false;
                
                u.Client.Bind((EndPoint)ipe);
                
                
            }


            catch (Exception ex)
            {
                Program.StillRun = false;
                System.Windows.Forms.MessageBox.Show("Could not create a new UDP client. Closing program now\nError: " + ex.Message,
                    "Could not bind", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();


            }


            return u;
        }

        public byte[] GetGameListFromConf(string confLocation)
        {

            RemoteGameCollection rgc = ServerDaemon.ReadLocalPathsCollection(confLocation);

            if (rgc == null) return null;

            byte[] buffer = ASCIIEncoding.ASCII.GetBytes(Environment.MachineName);
            Array.Resize(ref buffer, buffer.Length + 1);
            buffer[buffer.Length - 1] = 0xFF;

            int pos = buffer.Length;

            for (int i = 0; i < rgc.collection.Length; i++)
            {
                string path = rgc.collection[i].Path;
                string name = rgc.collection[i].Name;
                int bcount = ASCIIEncoding.ASCII.GetByteCount(name) +
                    ASCIIEncoding.ASCII.GetByteCount(path) + 2;

                byte[] nameBuffer = ASCIIEncoding.ASCII.GetBytes(name);
                byte[] pathBuffer = ASCIIEncoding.ASCII.GetBytes(path);

                Array.Resize(ref buffer, buffer.Length + bcount);
                Array.Copy(nameBuffer, 0, buffer, pos, nameBuffer.Length);
                buffer[pos + nameBuffer.Length] = 0x00;

                Array.Copy(pathBuffer, 0, buffer, (pos + nameBuffer.Length + 1), pathBuffer.Length);
                buffer[pos + nameBuffer.Length + 1 + pathBuffer.Length] = 0xFF;

                pos += bcount;
            }
            return buffer;
        }

        public byte[] GetGameListFromObsoleteScripts(string Directory)
        {
            if (!System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.CreateDirectory(Directory);
            }
            string[] files = System.IO.Directory.GetFiles(Directory);
            byte[] buffer = ASCIIEncoding.ASCII.GetBytes(Environment.MachineName);
            Array.Resize(ref buffer, buffer.Length + 1);
            buffer[buffer.Length - 1] = 0xFF;

            int pos = buffer.Length;

            for (int i = 0; i < files.Length; i++)
            {
                string path = System.IO.File.ReadAllText(files[i]);
                string name = System.IO.Path.GetFileNameWithoutExtension(files[i]);
                int bcount = ASCIIEncoding.ASCII.GetByteCount(name) +
                    ASCIIEncoding.ASCII.GetByteCount(path) + 2;

                byte[] nameBuffer = ASCIIEncoding.ASCII.GetBytes(name);
                byte[] pathBuffer = ASCIIEncoding.ASCII.GetBytes(path);

                Array.Resize(ref buffer, buffer.Length + bcount);
                Array.Copy(nameBuffer, 0, buffer, pos, nameBuffer.Length);
                buffer[pos + nameBuffer.Length] = 0x00;

                Array.Copy(pathBuffer, 0, buffer, (pos + nameBuffer.Length + 1), pathBuffer.Length);
                buffer[pos + nameBuffer.Length + 1 + pathBuffer.Length] = 0xFF;

                pos += bcount;



            }

            return buffer;

        }

        public void Shutdown()
        {
            this._uc.Close();

        }

        public void BroadcastAvailability()
        {
            if (Program.gs.ServerRunning)
            {
                IPEndPoint ipe = (IPEndPoint)(new System.Net.IPEndPoint(IPAddress.Broadcast, MSG_PORT));
                byte[] broadcastHeader = new byte[] { 0x42, 0x42, 0x42, 0xFF };

                byte[] GameData = GetGameListFromConf(Environment.CurrentDirectory + "\\bin\\rsyncd.conf");
                if (GameData == null) return;

                byte[] buffer = new byte[broadcastHeader.Length + GameData.Length];

               
                Array.Copy(broadcastHeader, 0, buffer, 0, broadcastHeader.Length);
                Array.Copy(GameData, 0, buffer, 4, GameData.Length);

                _uc.Send(buffer, buffer.Length, ipe);
            }
        }

        public void WriteScripts(byte[] buffer, string Directory)
        {
            if (buffer == null) return;

            int pos = 0;

            int refint = 0;

            string hostname = ASCIIEncoding.ASCII.GetString(ReadUntilTerminatorChar(buffer, 0, 25, 0xFF, ref refint));

            pos += refint;

            bool cont = true;

            while (cont == true)
            {
                string name, path;

                byte[] namebuf = ReadUntilTerminatorChar(buffer, pos, buffer.Length, 0x00, ref refint);
                if (refint == -1) { cont = false; continue; }
                name = ASCIIEncoding.ASCII.GetString(namebuf).Trim('\0');
                pos = refint;

                byte[] pathbuf = ReadUntilTerminatorChar(buffer, pos, buffer.Length, 0xff, ref refint);
                if (refint == -1) { cont = false; continue; }
                path = ASCIIEncoding.ASCII.GetString(pathbuf).Trim('\0');
                pos = refint;


                if (!System.IO.Directory.Exists(Directory + "\\" + hostname))
                { System.IO.Directory.CreateDirectory(Directory + "\\" + hostname); }
                System.IO.File.WriteAllText(Directory + "\\" + hostname + "\\" + name, path);
            }


        }





        public byte[] ReadUntilTerminatorChar(byte[] buffer, int start, int max, byte terminator, ref int TerminatorIndex)
        {
            byte[] buf = new byte[max];

            if(max < start) return null;

            for (int i = start; i < max; i++)
            {
                byte cur = buffer[i];
                if(cur == terminator)
                {
                    TerminatorIndex = i;
                    Array.Resize(ref buf, i - start);
                    Array.Copy(buffer, start,buf,0,i - start);
                    return buf;
                }
            }
            Array.Copy(buffer, start, buf, 0, max - start);
            TerminatorIndex = -1;
            return buf;
        }

        public void Listen()
        {
            InitUDPIfNull();
           
            while (Program.StillRun)
            {
                
                if (_uc.Available > 0)
                {
                    byte[] buffer = new byte[_uc.Available];
                    //System.Net.IPEndPoint ipe = null;
                    _uc.Client.Receive( buffer);
                    

                    if (IsRequestForReBroadcast(buffer))
                    {
                        BroadcastAvailability();
                    }
                    else if (IsGameBroadcast(buffer))
                    {
                       int refint = 0;
                       string hostname = ASCIIEncoding.ASCII.GetString(
                           ReadUntilTerminatorChar(buffer, 4, 255, 0xFF, ref refint)).Trim('\0');
                       byte[] data = new byte[buffer.Length - refint];
                       Array.Copy(buffer, refint, data, 0, data.Length);
                       RemoteGameCollection rgc = GetGamesFromBufferShort(data);
                       SafeAddToRemoteList(ref Program.remoteList, rgc,
                           hostname);


                        //WriteScripts(data, Environment.CurrentDirectory + "\\bin\\scripts");
                    }

                

                }
                 
                System.Threading.Thread.Sleep(10);
            }
            _uc.Close();
            frmMain.KillRsync();
        }

        void SafeAddToRemoteList(ref Dictionary<string, RemoteGameCollection> dict, RemoteGameCollection rgc, string hostname)
        {
            if (rgc == null) return;
           
            Program.remoteList.Remove(hostname);
            Program.remoteList.Add(hostname, rgc);
            if (Program.mainForm != null)
            {
                if (Program.mainForm.IsDisposed || (!Program.StillRun)) return;

                if (Program.mainForm.IsHandleCreated)
                {


                    frmMain.UpdateHostsDelegate uhd = new frmMain.UpdateHostsDelegate(
                        Program.mainForm.UpdateHosts);
                    Program.mainForm.BeginInvoke((Delegate)uhd);
                }
            }
        }

        string ExtractHostNameFromGameList(byte[] buffer)
        {
            int refint = 0;
            string hostname = ASCIIEncoding.ASCII.GetString(
                ReadUntilTerminatorChar(buffer, 0, 255, 0xff, ref refint));
            return hostname;

        }
        public RemoteGameCollection GetGamesFromBuffer(byte[] buffer)
        {
            if (buffer == null) return null;

            int pos = 0;


            RemoteGameCollection rgc;
            RemoteGame[] rg = new RemoteGame[0];
            int refint = 0;

            string hostname = ASCIIEncoding.ASCII.GetString(ReadUntilTerminatorChar(buffer, 0, 25, 0xFF, ref refint));

            pos += refint;

            bool cont = true;

            while (cont == true)
            {
                string name, path;

                byte[] namebuf = ReadUntilTerminatorChar(buffer, pos, buffer.Length, 0x00, ref refint);
                if (refint == -1) { cont = false; continue; }
                name = ASCIIEncoding.ASCII.GetString(namebuf).Trim('\0');
                pos = refint;

                byte[] pathbuf = ReadUntilTerminatorChar(buffer, pos, buffer.Length, 0xff, ref refint);
                if (refint == -1) { cont = false; continue; }
                path = ASCIIEncoding.ASCII.GetString(pathbuf).Trim('\0');
                pos = refint;

                Array.Resize(ref rg, rg.Length + 1);
                rg[rg.Length - 1] = new RemoteGame(name, path);
            }
            rgc = new RemoteGameCollection(rg, Environment.MachineName);
            return rgc;


        }
        public RemoteGameCollection GetGamesFromBufferShort(byte[] buffer)
        {
            if (buffer == null) return null;

            int pos = 0;


            RemoteGameCollection rgc;
            RemoteGame[] rg = new RemoteGame[0];
            int refint = 0;



            pos = 0;

            bool cont = true;

            while (cont == true)
            {
                string name, path;

                byte[] namebuf = ReadUntilTerminatorChar(buffer, pos + 1, buffer.Length, 0x00, ref refint);
                if (refint == -1) { cont = false; continue; }
                name = ASCIIEncoding.ASCII.GetString(namebuf).Trim('\0');
                pos = refint + 1;

                byte[] pathbuf = ReadUntilTerminatorChar(buffer, pos, buffer.Length, 0xff, ref refint);
                if (refint == -1) { cont = false; continue; }
                path = ASCIIEncoding.ASCII.GetString(pathbuf).Trim('\0');
                pos = refint + 1;

                Array.Resize(ref rg, rg.Length + 1);
                rg[rg.Length - 1] = new RemoteGame(name, path);
                //if (name == "" || path == "") return null;
                if (pos >= buffer.Length) cont = false;
            }
            
            rgc = new RemoteGameCollection(rg, Environment.MachineName);
            return rgc;


        }

        bool IsRequestForReBroadcast(byte[] buffer)
        {
            if (buffer != null)
            {
                if (buffer.Length == 3)
                {
                    if (buffer[0] == 0x42 && buffer[1] == 0xff &
                        buffer[2] == 0x42)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void RequestGameBroadcast()
        {
            
            byte[] buffer = { 0x42, 0xff, 0x42 };            
            _uc.Send(buffer, buffer.Length, new IPEndPoint(IPAddress.Broadcast, MSG_PORT));

        }

        public void InitUDPIfNull()
        {
            if (_uc == null) _uc = SafeStartListen();
        }

        bool IsGameBroadcast(byte[] buffer)
        {
            if (buffer != null)
            {
                if (buffer.Length > 4)
                {
                    if (buffer[0] == 0x42 && buffer[1] == 0x42 &
                        buffer[2] == 0x42 && buffer[3] == 0xff)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
