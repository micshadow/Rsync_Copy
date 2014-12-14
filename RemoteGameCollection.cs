using System;
using System.Collections.Generic;
using System.Text;

namespace Rsync_Copy
{
    public class RemoteGameCollection
    {
        public RemoteGame[] collection;
        string[] keys, paths;
        string HostName;

        public static void FlushLocalCollectionToDisk( RemoteGameCollection games,string Directory)
        {
            for (int i = 0; i < games.collection.Length; i++)
            {
                if (!System.IO.Directory.Exists(Directory + "\\" + games.collection[i].Name))
                {
                    System.IO.Directory.CreateDirectory(Directory + "\\" + games.collection[i].Name
                        );
                }
                System.IO.File.WriteAllText(
                    Directory + "\\" + games.collection[i].Name,
                    games.collection[i].Path);
            }
        }

        public RemoteGameCollection(RemoteGame[] Games,string Hostname)
        {
            collection = Games;
            

        }

        public RemoteGame GetItem(string name)
        {
            for (int i = 0; i < collection.Length; i++)
            {
                if (collection[i].Name == name)
                {
                    return collection[i];
                }
            }
            return null;
        }

        public void EditItem(string NameToSearchFor,string newName, string newPath)
        {
            for (int i = 0; i < collection.Length; i++)
            {
                if (collection[i].Name == NameToSearchFor)
                {
                    collection[i] = new RemoteGame(newName, newPath);
                    
                }
            }
        }

        public void RemoveItem(string Name)
        {
            for (int i = 0; i < collection.Length; i++)
            {
                if (collection[i].Name == Name)
                {
                    collection[i] = null;

                }
            }

            RemoteGame[] newList = new RemoteGame[collection.Length - 1];
            for (int i2 = 0; i2 < collection.Length; i2++)
            {
                if (collection[i2] != null) newList[i2] = collection[i2];
            }
            collection = newList;
        }

        public bool ContainsKey(string key)
        {
            for (int i = 0; i < collection.Length; i++)
            {
                if (collection[i].Name == key)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddItem(RemoteGame item)
        {
            if (ContainsKey(item.Name)) return;
            if (item.Name == "") return;
            Array.Resize(ref collection, collection.Length + 1);
            collection[collection.Length - 1] = item;

        }
    }
}
