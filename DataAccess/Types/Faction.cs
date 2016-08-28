using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

namespace DataAccess.Types
{
    public class ObservableFactionCollection : ObservableCollection<Faction> { }
    public class Faction : Observable
    {
        internal Faction(Game _game)
        {
            game = _game;
        }
        private long id;
        public long ID
        {
            get { return id; }
            set { VerifyPropertyChange<long>("ID", ref id, ref value); }
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
        private string colourText;
        public string ColourText
        {
            get { return colourText; }
            set { VerifyPropertyChange<string>("ColourText", ref colourText, ref value); }
        }
        private string colourBackground;
        public string ColourBackground
        {
            get { return colourBackground; }
            set { VerifyPropertyChange<string>("ColourBackground", ref colourBackground, ref value); }
        }
        //NOTE: Changing the game of a faction on the fly has dire consequences, so we disable it.
        private Game game;
        public Game Game { get { return Game; } }
    }
}
