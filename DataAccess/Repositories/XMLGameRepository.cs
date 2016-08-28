using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using DataAccess.Types;

namespace DataAccess.Repositories
{
    sealed class XMLGameRepository : AbstractGameRepository
    {
        #region Attributes and Constructor
        private bool allLoaded = false;
        private ObservableGameCollection all;
        public override ObservableGameCollection All
        {
            get
            {
                if (!allLoaded)
                {
                    LoadAll();
                    allLoaded = true;
                }
                return all;
            }
        }

        private XMLRepositoryFactory factory;
        private Dictionary<string, Game> gamesByTitle;
        private Dictionary<long, Game> gamesByID;
        private int nextID;
        internal XMLGameRepository(XMLRepositoryFactory _factory, int _nextID)
        {
            factory = _factory;
            nextID = _nextID;
            gamesByTitle = new Dictionary<string, Game>();
            gamesByID = new Dictionary<long, Game>();
            all = new ObservableGameCollection();
        }
        #endregion
        #region Interface Overrides
        public override Game GetGame(int id)
        {
            Game _game;
            //Check if we already have the game stored
            if (gamesByID.TryGetValue(id, out _game)) { return _game; }
            //If not, find the correct element in the file
            XElement element = FindElementByID(id);
            //Send the element to parsing, which will return null if it's blank.
            return ParseGame(element);
        }
        public override Game GetGame(string title)
        {
            Game _game;
            //Check if we already have the game stored
            if (gamesByTitle.TryGetValue(title, out _game)) { return _game; }
            XElement element = FindElementByTitle(title);
            if (element == null) { return null; }
            else { return ParseGame(element); }
        }
        public override Game CreateOrGetGame(string title, bool persist = true)
        {
            //See if we already have the game
            Game _game = GetGame(title);
            if (_game == null)
            {
                //Create and store a new game
                _game = new Game();
                _game.ID = nextID;
                _game.Title = title;
                //Put game in dictionaries
                gamesByTitle.Add(_game.Title, _game);
                gamesByID.Add(_game.ID, _game);
                //Put the game into the XML document
                XElement element = new XElement("Game",
                    new XAttribute("Title", _game.Title),
                    new XAttribute("ID", _game.ID));
                factory.Document.Root.Element("Games").Add(element);
                //Update the NextGameID, both here and in configuration.
                //This will also persist the element above if called for.
                nextID++;
                factory.ConfigurationRepository.SetValue("NextGameID", Convert.ToString(nextID), persist);
                all.Add(_game);
            }
            return _game;
        }
        public override void UpdateGame(Game updated, bool persist = true)
        {
            //Find the corresponding element in the document
            XElement element = FindElementByID(updated.ID);
            //Attribute - title
            XAttribute _title = element.Attribute("Title");
            if (_title != null) { _title.Value = updated.Title; }
            else { element.Add(new XAttribute("Title", updated.Title)); }
            //TODO: Change title dictionary
            //Attribute - year
            XAttribute _year = element.Attribute("Year");
            if (updated.Year > 0)
            {
                if (_year != null) { _year.Value = Convert.ToString(updated.Year); }
                else { element.Add(new XAttribute("Year", updated.Year)); }
            }
            else
            {
                if (_year != null) { _year.Remove(); }
            }
            //Attribute - url
            XAttribute _url = element.Attribute("Url");
            if (updated.Url != null && !updated.Url.Equals(""))
            {
                if (_url != null) { _url.Value = updated.Url; }
                else { element.Add(new XAttribute("Url", updated.Url)); }
            }
            else
            {
                if (_url != null) { _url.Remove(); }
            }
            //Attribute - publisher
            XAttribute _publisher = element.Attribute("Publisher");
            if (updated.Publisher != null && !updated.Publisher.Equals(""))
            {
                if (_publisher != null) { _publisher.Value = updated.Publisher; }
                else { element.Add(new XAttribute("Publisher", updated.Publisher)); }
            }
            else
            {
                if (_publisher != null) { _publisher.Remove(); }
            }
            //Attribute - basePath
            XAttribute _basePath = element.Attribute("BasePath");
            if (updated.BasePath != null && !updated.BasePath.Equals(""))
            {
                if (_basePath != null) { _basePath.Value = updated.BasePath; }
                else { element.Add(new XAttribute("BasePath", updated.BasePath)); }
            }
            else
            {
                if (_basePath != null) { _basePath.Remove(); }
            }
            //Attribute - logoPath
            XAttribute _logoPath = element.Attribute("LogoPath");
            if (updated.LogoPath != null && !updated.LogoPath.Equals(""))
            {
                if (_logoPath != null) { _logoPath.Value = updated.LogoPath; }
                else { element.Add(new XAttribute("LogoPath", updated.LogoPath)); }
            }
            else
            {
                if (_logoPath != null) { _logoPath.Remove(); }
            }
            //Attribute - bannerPath
            XAttribute _bannerPath = element.Attribute("BannerPath");
            if (updated.BannerPath != null && !updated.BannerPath.Equals(""))
            {
                if (_bannerPath != null) { _bannerPath.Value = updated.BannerPath; }
                else { element.Add(new XAttribute("BannerPath", updated.BannerPath)); }
            }
            else
            {
                if (_bannerPath != null) { _bannerPath.Remove(); }
            }
            //Ignored: ID, child elements
            //Flush the file
            if (persist) { factory.Save(); }
        }
        public override void DeleteGame(Game deleted, bool cascade = false)
        {
            if (cascade)
            {
                foreach (Faction faction in deleted.Factions)
                {
                    factory.FactionRepository.DeleteFaction(faction, true);
                }
            }
            if (deleted.Factions.Count == 0)
            {
                //Updated references and cache
                gamesByTitle.Remove(deleted.Title);
                gamesByID.Remove(deleted.ID);
                //Remove from tree & persist
                FindElementByID(deleted.ID).Remove();
                factory.Save();
            }
        }
        #endregion
        #region XML Handling
        internal Game ParseGame(XElement element)
        {
            if (element == null) { return null; }
            Game _game;
            //ID attribute
            XAttribute _id = element.Attribute("ID");
            if (_id == null)
            {
                //This really shouldn't be possible, but we can always fix it by setting a new ID.
                _game = new Game();
                _game.ID = nextID;
                element.Add(new XAttribute("ID", _game.ID));
                nextID++;
                factory.ConfigurationRepository.SetValue("NextGameID", Convert.ToString(nextID));
            }
            else
            {
                //Check if the ID is stored first
                if (gamesByID.TryGetValue(Convert.ToInt32(_id.Value), out _game)) { return _game; }
                //It's not stored, so create a new and parse it
                _game = new Game();
                _game.ID = Convert.ToInt32(_id.Value);
            }
            //Attribute - title
            XAttribute _title = element.Attribute("Title");
            if (_title != null) { _game.Title = _title.Value; }
            //Attribute - year
            XAttribute _year = element.Attribute("Year");
            if (_year != null) { _game.Year = Convert.ToInt32(_year.Value); }
            //Attribute - url
            XAttribute _url = element.Attribute("Url");
            if (_url != null) { _game.Url = _url.Value; }
            //Attribute - publisher
            XAttribute _publisher = element.Attribute("Publisher");
            if (_publisher != null) { _game.Publisher = _publisher.Value; }
            //Attribute - basePath
            XAttribute _basePath = element.Attribute("BasePath");
            if (_basePath != null) { _game.BasePath = _basePath.Value; }
            //Attribute - logoPath
            XAttribute _logoPath = element.Attribute("LogoPath");
            if (_logoPath != null) { _game.LogoPath = _logoPath.Value; }
            //Attribute - bannerPath
            XAttribute _bannerPath = element.Attribute("BannerPath");
            if (_bannerPath != null) { _game.BannerPath = _bannerPath.Value; }
            //Add to dictionaries
            gamesByID.Add(_game.ID, _game);
            gamesByTitle.Add(_game.Title, _game);
            //Recursive parsing of the game's contents.
            //Factions
            IEnumerable<XElement> factions = element.Elements("Faction");
            foreach (XElement faction in factions)
            {
                //This will recursively add the episodes back to the season
                (factory.FactionRepository as XMLFactionRepository).ParseFaction(faction);
            }
            all.Add(_game);
            return _game;
        }
        internal XElement FindElementByTitle(string title)
        {
            return (from XElement in factory.Document.Descendants("Game")
                    where XElement.Attribute("Title").Value.Equals(title)
                    select XElement).FirstOrDefault();
        }
        internal XElement FindElementByID(long id)
        {
            return (from XElement in factory.Document.Descendants("Game")
                    where XElement.Attribute("ID").Value.Equals(Convert.ToString(id))
                    select XElement).FirstOrDefault();
        }
        internal void LoadAll()
        {
            IEnumerable<XElement> elements = (from XElement in factory.Document.Descendants("game")
                    select XElement);
            foreach (XElement element in elements)
            {
                ParseGame(element);
            }
        }
        #endregion
    }
}
