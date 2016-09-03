using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using DataAccess.Types;

namespace DataAccess.Repositories
{
    sealed class XMLCardRepository : AbstractCardRepository
    {
        #region Attributes and Constructor
        private XMLRepositoryFactory factory;
        private Dictionary<string, Card> cardsByComposite;
        private Dictionary<long, Card> cardsByID;
        private int nextID;
        internal XMLCardRepository(XMLRepositoryFactory _factory, int _nextID)
        {
            factory = _factory;
            nextID = _nextID;
            cardsByComposite = new Dictionary<string, Card>();
            cardsByID = new Dictionary<long, Card>();
        }
        #endregion
        #region Interface Overrides
        public override Card GetCard(int id)
        {
            Card _card;
            //Check if we already have the card stored
            if (cardsByID.TryGetValue(id, out _card)) { return _card; }
            //If not, find the correct element in the file
            XElement element = FindElementByID(id);
            //Send the element to parsing, which will return null if it's blank.
            return ParseCard(element);
        }
        public override Card GetCard(Game game, string code)
        {
            Card _card;
            //Check if we already have the card stored
            if (cardsByComposite.TryGetValue(BuildComposite(game, code), out _card)) { return _card; }
            //All cards would have been loaded with the game, so no need to search XML
            return null;
        }
        public override Card CreateOrGetCard(Game game, string code, bool persist = true)
        {
            //See if we already have the card
            Card _card = GetCard(game, code);
            if (_card == null)
            {
                //Create and store a new card
                _card = new Card(nextID, game);
                _card.Code = code;
                //Put card in dictionaries
                cardsByComposite.Add(BuildComposite(game, _card.Code), _card);
                cardsByID.Add(_card.ID, _card);
                //Put the card into the XML document
                XElement element = new XElement("Card",
                    new XAttribute("Code", _card.Code),
                    new XAttribute("ID", _card.ID));
                (factory.GameRepository as XMLGameRepository).FindElementByID(game.ID).Add(element);
                //Update the NextCardID, both here and in configuration.
                //This will also persist the element above if called for.
                nextID++;
                factory.ConfigurationRepository.SetValue("NextCardID", Convert.ToString(nextID), persist);
            }
            return _card;
        }
        public override void UpdateCard(Card updated, bool persist = true)
        {
            //Find the corresponding element in the document
            XElement element = FindElementByID(updated.ID);
            //Attribute - code
            XAttribute _code = element.Attribute("Code");
            if (_code != null) { _code.Value = updated.Code; }
            else { element.Add(new XAttribute("Code", updated.Code)); }
            //TODO: Change dictionary
            //Attribute - title
            XAttribute _title = element.Attribute("Title");
            if (updated.Title != null && !updated.Title.Equals(""))
            {
                if (_title != null) { _title.Value = updated.Title; }
                else { element.Add(new XAttribute("Title", updated.Title)); }
            }
            else
            {
                if (_title != null) { _title.Remove(); }
            }
            //Attribute - faction
            XAttribute _faction = element.Attribute("Faction");
            if (updated.Faction != null)
            {
                if (_faction != null) { _faction.Value = Convert.ToString(updated.Faction.ID); }
                else { element.Add(new XAttribute("Faction", Convert.ToString(updated.Faction.ID))); }
            }
            else
            {
                if (_faction != null) { _faction.Remove(); }
            }
            //Attribute - cardtype
            XAttribute _cardtype = element.Attribute("Cardtype");
            if (updated.Cardtype != null)
            {
                if (_cardtype != null) { _cardtype.Value = Convert.ToString(updated.Cardtype.ID); }
                else { element.Add(new XAttribute("Cardtype", Convert.ToString(updated.Cardtype.ID))); }
            }
            else
            {
                if (_cardtype != null) { _cardtype.Remove(); }
            }
            //Ignored: ID, Game
            //Flush the file
            if (persist) { factory.Save(); }
        }
        public override void DeleteCard(Card deleted, bool cascade = false)
        {
            //Updated references and cache
            cardsByComposite.Remove(BuildComposite(deleted.Game, deleted.Title));
            cardsByID.Remove(deleted.ID);
            //Remove from tree & persist
            FindElementByID(deleted.ID).Remove();
            factory.Save();
        }
        #endregion
        #region XML Handling
        internal Card ParseCard(XElement element)
        {
            if (element == null) { return null; }
            //Parent - Game
            Game _game = (factory.GameRepository as XMLGameRepository).ParseGame(element.Parent);
            Card _card;
            //ID attribute
            XAttribute _id = element.Attribute("ID");
            if (_id == null)
            {
                //This really shouldn't be possible, but we can always fix it by setting a new ID.
                _card = new Card(nextID, _game);
                element.Add(new XAttribute("ID", _card.ID));
                nextID++;
                factory.ConfigurationRepository.SetValue("NextCardID", Convert.ToString(nextID));
            }
            else
            {
                //Check if the ID is stored first
                if (cardsByID.TryGetValue(Convert.ToInt32(_id.Value), out _card)) { return _card; }
                //It's not stored, so create a new and parse it
                _card = new Card(Convert.ToInt32(_id.Value), _game);
            }
            //Attribute - code
            XAttribute _code = element.Attribute("Code");
            if (_code != null) { _card.Code = _code.Value; }
            //Attribute - title
            XAttribute _title = element.Attribute("Title");
            if (_title != null) { _card.Title = _title.Value; }
            //Attribute - faction
            XAttribute _faction = element.Attribute("Faction");
            if (_faction != null) { _card.Faction = (factory.FactionRepository as XMLFactionRepository).GetFaction(Convert.ToInt32(_faction.Value)); }
            //Attribute - cardtype
            XAttribute _cardtype = element.Attribute("Cardtype");
            if (_cardtype != null) { _card.Cardtype = (factory.CardtypeRepository as XMLCardtypeRepository).GetCardtype(Convert.ToInt32(_cardtype.Value)); }
            //Add to dictionaries and parent
            cardsByID.Add(_card.ID, _card);
            cardsByComposite.Add(BuildComposite(_card.Game, _card.Code), _card);
            _game.Cards.Add(_card);
            return _card;
        }
        internal XElement FindElementByID(long id)
        {
            return (from XElement in factory.Document.Descendants("Card")
                    where XElement.Attribute("ID").Value.Equals(Convert.ToString(id))
                    select XElement).FirstOrDefault();
        }
        #endregion
        private string BuildComposite(Game game, string code)
        {
            return string.Format("{0}#{1}", game.Title, code);
        }

    }
}
