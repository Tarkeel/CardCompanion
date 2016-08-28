using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using DataAccess.Types;

namespace DataAccess.Repositories
{
    sealed class XMLCardtypeRepository : AbstractCardtypeRepository
    {
        #region Attributes and Constructor
        private XMLRepositoryFactory factory;
        private Dictionary<string, Cardtype> cardtypesByComposite;
        private Dictionary<long, Cardtype> cardtypesByID;
        private int nextID;
        internal XMLCardtypeRepository(XMLRepositoryFactory _factory, int _nextID)
        {
            factory = _factory;
            nextID = _nextID;
            cardtypesByComposite = new Dictionary<string, Cardtype>();
            cardtypesByID = new Dictionary<long, Cardtype>();
        }
        #endregion
        #region Interface Overrides
        public override Cardtype GetCardtype(int id)
        {
            Cardtype _cardtype;
            //Check if we already have the cardtype stored
            if (cardtypesByID.TryGetValue(id, out _cardtype)) { return _cardtype; }
            //If not, find the correct element in the file
            XElement element = FindElementByID(id);
            //Send the element to parsing, which will return null if it's blank.
            return ParseCardtype(element);
        }
        public override Cardtype GetCardtype(Game game, string title)
        {
            Cardtype _cardtype;
            //Check if we already have the cardtype stored
            if (cardtypesByComposite.TryGetValue(BuildComposite(game, title), out _cardtype)) { return _cardtype; }
            //All cardtypes would have been loaded with the game, so no need to search XML
            return null;
        }
        public override Cardtype CreateOrGetCardtype(Game game, string title, bool persist = true)
        {
            //See if we already have the cardtype
            Cardtype _cardtype = GetCardtype(game, title);
            if (_cardtype == null)
            {
                //Create and store a new cardtype
                _cardtype = new Cardtype(nextID, game);
                _cardtype.Title = title;
                //Put cardtype in dictionaries
                cardtypesByComposite.Add(BuildComposite(game, _cardtype.Title), _cardtype);
                cardtypesByID.Add(_cardtype.ID, _cardtype);
                //Put the cardtype into the XML document
                XElement element = new XElement("Cardtype",
                    new XAttribute("Title", _cardtype.Title),
                    new XAttribute("ID", _cardtype.ID));
                (factory.GameRepository as XMLGameRepository).FindElementByID(game.ID).Add(element);
                //Update the NextCardtypeID, both here and in configuration.
                //This will also persist the element above if called for.
                nextID++;
                factory.ConfigurationRepository.SetValue("NextCardtypeID", Convert.ToString(nextID), persist);
            }
            return _cardtype;
        }
        public override void UpdateCardtype(Cardtype updated, bool persist = true)
        {
            //Find the corresponding element in the document
            XElement element = FindElementByID(updated.ID);
            //Attribute - title
            XAttribute _title = element.Attribute("Title");
            if (_title != null) { _title.Value = updated.Title; }
            else { element.Add(new XAttribute("Title", updated.Title)); }
            //TODO: Change title dictionary
            //Attribute - basePath
            XAttribute _iconPath = element.Attribute("IconPath");
            if (updated.IconPath != null && !updated.IconPath.Equals(""))
            {
                if (_iconPath != null) { _iconPath.Value = updated.IconPath; }
                else { element.Add(new XAttribute("IconPath", updated.IconPath)); }
            }
            else
            {
                if (_iconPath != null) { _iconPath.Remove(); }
            }
            //Ignored: ID, Game
            //Flush the file
            if (persist) { factory.Save(); }
        }
        public override void DeleteCardtype(Cardtype deleted, bool cascade = false)
        {
            //Updated references and cache
            cardtypesByComposite.Remove(BuildComposite(deleted.Game, deleted.Title));
            cardtypesByID.Remove(deleted.ID);
            //Remove from tree & persist
            FindElementByID(deleted.ID).Remove();
            factory.Save();
        }
        #endregion
        #region XML Handling
        internal Cardtype ParseCardtype(XElement element)
        {
            if (element == null) { return null; }
            //Parent - Game
            Game _game = (factory.GameRepository as XMLGameRepository).ParseGame(element.Parent);
            Cardtype _cardtype;
            //ID attribute
            XAttribute _id = element.Attribute("ID");
            if (_id == null)
            {
                //This really shouldn't be possible, but we can always fix it by setting a new ID.
                _cardtype = new Cardtype(nextID, _game);
                element.Add(new XAttribute("ID", _cardtype.ID));
                nextID++;
                factory.ConfigurationRepository.SetValue("NextCardtypeID", Convert.ToString(nextID));
            }
            else
            {
                //Check if the ID is stored first
                if (cardtypesByID.TryGetValue(Convert.ToInt32(_id.Value), out _cardtype)) { return _cardtype; }
                //It's not stored, so create a new and parse it
                _cardtype = new Cardtype(Convert.ToInt32(_id.Value), _game);
            }
            //Attribute - title
            XAttribute _title = element.Attribute("Title");
            if (_title != null) { _cardtype.Title = _title.Value; }
            //Attribute - iconPath
            XAttribute _iconPath = element.Attribute("IconPath");
            if (_iconPath != null) { _cardtype.IconPath = _iconPath.Value; }
            //Add to dictionaries and parent
            cardtypesByID.Add(_cardtype.ID, _cardtype);
            cardtypesByComposite.Add(BuildComposite(_cardtype.Game, _cardtype.Title), _cardtype);
            _game.Cardtypes.Add(_cardtype);
            return _cardtype;
        }
        internal XElement FindElementByID(long id)
        {
            return (from XElement in factory.Document.Descendants("Cardtype")
                    where XElement.Attribute("ID").Value.Equals(Convert.ToString(id))
                    select XElement).FirstOrDefault();
        }
        #endregion
        private string BuildComposite(Game game, string cardtype)
        {
            return string.Format("{0}#S{1}", game.Title, cardtype);
        }

    }
}
