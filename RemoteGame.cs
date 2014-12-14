using System;
using System.Collections.Generic;
using System.Text;

namespace Rsync_Copy
{
    public class RemoteGame
    {
        string _name, _path;

        public RemoteGame(string Name, string Path)
        {
            _name = Name;
            _path = Path;
        }

        public string Name{
            get { return _name; }
        }

        public string Path
        {
            get { return _path; }
        }

    }
}
