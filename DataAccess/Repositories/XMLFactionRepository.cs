using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using DataAccess.Types;

namespace DataAccess.Repositories
{
    sealed class XMLFactionRepository : AbstractFactionRepository
    {
        #region Attributes and Constructor
        private XMLRepositoryFactory factory;
        private Dictionary<string, Faction> factionsByComposite;
        private Dictionary<long, Faction> factionsByID;
        private int nextID;
        internal XMLFactionRepository(XMLRepositoryFactory _factory, int _nextID)
        {
            factory = _factory;
            nextID = _nextID;
            factionsByComposite = new Dictionary<string, Faction>();
            factionsByID = new Dictionary<long, Faction>();
        }
        #endregion
        #region Interface Overrides
        public override Faction GetFaction(int id)
        {
            Faction _faction;
            //Check if we already have the faction stored
            if (factionsByID.TryGetValue(id, out _faction)) { return _faction; }
            //If not, find the correct element in the file
            XElement element = FindElementByID(id);
            //Send the element to parsing, which will return null if it's blank.
            return ParseFaction(element);
        }
        public override Faction GetFaction(Game game, string title)
        {
            Faction _faction;
            //Check if we already have the faction stored
            if (factionsByComposite.TryGetValue(BuildComposite(game, title), out _faction)) { return _faction; }
            //All factions would have been loaded with the game, so no need to search XML
            return null;
        }
        public override Faction CreateOrGetFaction(Game game, string title, bool persist = true)
        {
            //See if we already have the faction
            Faction _faction = GetFaction(game, title);
            if (_faction == null)
            {
                //Create and store a new faction
                _faction = new Faction(nextID, game);
                _faction.Title = title;
                //Put faction in dictionaries
                factionsByComposite.Add(BuildComposite(game, _faction.Title), _faction);
                factionsByID.Add(_faction.ID, _faction);
                //Put the faction into the XML document
                XElement element = new XElement("Faction",
                    new XAttribute("Title", _faction.Title),
                    new XAttribute("ID", _faction.ID));
                (factory.GameRepository as XMLGameRepository).FindElementByID(game.ID).Add(element);
                //Update the NextFactionID, both here and in configuration.
                //This will also persist the element above if called for.
                nextID++;
                factory.ConfigurationRepository.SetValue("NextFactionID", Convert.ToString(nextID), persist);
            }
            return _faction;
        }
        public override void UpdateFaction(Faction updated, bool persist = true)
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
            //Attribute - ColourBackground
            XAttribute _colourBackground = element.Attribute("ColourBackground");
            if (updated.ColourBackground != null && !updated.ColourBackground.Equals(""))
            {
                if (_colourBackground != null) { _colourBackground.Value = updated.ColourBackground; }
                else { element.Add(new XAttribute("ColourBackground", updated.ColourBackground)); }
            }
            else
            {
                if (_colourBackground != null) { _colourBackground.Remove(); }
            }
            //Attribute - ColourText
            XAttribute _colourText = element.Attribute("ColourText");
            if (updated.ColourText != null && !updated.ColourText.Equals(""))
            {
                if (_colourText != null) { _colourText.Value = updated.ColourText; }
                else { element.Add(new XAttribute("ColourText", updated.ColourText)); }
            }
            else
            {
                if (_colourText != null) { _colourText.Remove(); }
            }
            //Ignored: ID, Game, child elements
            //Flush the file
            if (persist) { factory.Save(); }
        }
        public override void DeleteFaction(Faction deleted, bool cascade = false)
        {
            //Updated references and cache
            factionsByComposite.Remove(BuildComposite(deleted.Game, deleted.Title));
            factionsByID.Remove(deleted.ID);
            //Remove from tree & persist
            FindElementByID(deleted.ID).Remove();
            factory.Save();
        }
        #endregion
        #region XML Handling
        internal Faction ParseFaction(XElement element)
        {
            if (element == null) { return null; }
            //Parent - Game
            Game _game = (factory.GameRepository as XMLGameRepository).ParseGame(element.Parent);
            Faction _faction;
            //ID attribute
            XAttribute _id = element.Attribute("ID");
            if (_id == null)
            {
                //This really shouldn't be possible, but we can always fix it by setting a new ID.
                _faction = new Faction(nextID, _game);
                element.Add(new XAttribute("ID", _faction.ID));
                nextID++;
                factory.ConfigurationRepository.SetValue("NextFactionID", Convert.ToString(nextID));
            }
            else
            {
                //Check if the ID is stored first
                if (factionsByID.TryGetValue(Convert.ToInt32(_id.Value), out _faction)) { return _faction; }
                //It's not stored, so create a new and parse it
                _faction = new Faction(Convert.ToInt32(_id.Value), _game);
            }
            //Attribute - title
            XAttribute _title = element.Attribute("Title");
            if (_title != null) { _faction.Title = _title.Value; }
            //Attribute - iconPath
            XAttribute _iconPath = element.Attribute("IconPath");
            if (_iconPath != null) { _faction.IconPath = _iconPath.Value; }
            //Attribute - ColourText
            XAttribute _colourText = element.Attribute("ColourText");
            if (_colourText != null) { _faction.ColourText = _colourText.Value; }
            //Attribute - ColourBackground
            XAttribute _colourBackground = element.Attribute("ColourBackground");
            if (_colourBackground != null) { _faction.ColourBackground = _colourBackground.Value; }
            //Add to dictionaries and parent
            factionsByID.Add(_faction.ID, _faction);
            factionsByComposite.Add(BuildComposite(_faction.Game, _faction.Title), _faction);
            _game.Factions.Add(_faction);
            return _faction;
        }
        internal XElement FindElementByID(long id)
        {
            return (from XElement in factory.Document.Descendants("Faction")
                    where XElement.Attribute("ID").Value.Equals(Convert.ToString(id))
                    select XElement).FirstOrDefault();
        }
        #endregion
        private string BuildComposite(Game game, string faction)
        {
            return string.Format("{0}#S{1}", game.Title, faction);
        }

    }
}
