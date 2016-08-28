using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace DataAccess.Types
{
    public class ObservableCardtypeCollection : ObservableCollection<Cardtype> { }
    public class Cardtype : Observable
    {
        private long id;
        public long ID { get { return id; } }
        private Game game;
        public Game Game { get { return Game; } }

        internal Cardtype(long _id, Game _game)
        {
            id = _id;
            game = _game;
        }
        private string title;
        public string Title
        {
            get { return title; }
            set { VerifyPropertyChange<string>("Title", ref title, ref value); }
        }
        private string iconPath;
        public string IconPath
        {
            get { return iconPath; }
            set { VerifyPropertyChange<string>("IconPath", ref iconPath, ref value); }
        }
    }
}
