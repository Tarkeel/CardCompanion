using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace DataAccess.Types
{
    public class ObservableCardCollection : ObservableCollection<Card> { }
    public class Card : Observable
    {
        private long id;
        public long ID { get { return id; } }
        private Game game;
        public Game Game { get { return Game; } }

        internal Card(long _id, Game _game)
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
        private string code;
        public string Code
        {
            get { return code; }
            set { VerifyPropertyChange<string>("Code", ref code, ref value); }
        }
        private Cardtype cardtype;
        public Cardtype Cardtype
        {
            get { return cardtype; }
            set { VerifyPropertyChange<Cardtype>("Cardtype", ref cardtype, ref value); }
        }
        private Faction faction;
        public Faction Faction
        {
            get { return faction; }
            set { VerifyPropertyChange<Faction>("Faction", ref faction, ref value); }
        }
    }
}
