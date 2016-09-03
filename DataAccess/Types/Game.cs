using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

namespace DataAccess.Types
{
    public class ObservableGameCollection : ObservableCollection<Game> { }
    public class Game : Observable
    {
        private long id;
        public long ID { get { return id; } }
        internal Game(long _id)
        {
            id = _id;
            factions = new ObservableFactionCollection();
            cardtypes = new ObservableCardtypeCollection();
            cards = new ObservableCardCollection();
        }
        #region Collections
        private ObservableFactionCollection factions;
        public ObservableFactionCollection Factions {  get { return factions; } }
        private ObservableCardtypeCollection cardtypes;
        public ObservableCardtypeCollection Cardtypes { get { return cardtypes; } }
        private ObservableCardCollection cards;
        public ObservableCardCollection Cards { get { return cards; } }
        #endregion
        #region Attributes
        private string title;
        public string Title
        {
            get { return title; }
            set { VerifyPropertyChange<string>("Title", ref title, ref value); }
        }
        private long year;
        public long Year
        {
            get { return year; }
            set { VerifyPropertyChange<long>("Year", ref year, ref value); }
        }
        private string url;
        public string Url
        {
            get { return url; }
            set { VerifyPropertyChange<string>("Url", ref url, ref value); }
        }
        private string publisher;
        public string Publisher
        {
            get { return publisher; }
            set { VerifyPropertyChange<string>("Publisher", ref publisher, ref value); }
        }
        private string basePath;
        public string BasePath
        {
            get { return basePath; }
            set { VerifyPropertyChange<string>("BasePath", ref basePath, ref value); }
        }
        private string logoPath;
        public string LogoPath
        {
            get { return logoPath; }
            set { VerifyPropertyChange<string>("LogoPath", ref logoPath, ref value); }
        }
        private string bannerPath;
        public string BannerPath
        {
            get { return bannerPath; }
            set { VerifyPropertyChange<string>("BannerPath", ref bannerPath, ref value); }
        }
        #endregion
    }
}
