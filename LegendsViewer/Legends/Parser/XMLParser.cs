using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using LegendsViewer.Controls;
using LegendsViewer.Legends.Enums;
using LegendsViewer.Legends.EventCollections;
using LegendsViewer.Legends.Events;
using LegendsViewer.Legends.WorldObjects;

namespace LegendsViewer.Legends.Parser
{
    public class XmlParser : IDisposable
    {
        private readonly BackgroundWorker _worker;
        private string _currentItemName = "";
        private readonly XmlPlusParser _xmlPlusParser;

        protected readonly XmlTextReader XmlReader;
        protected World World;
        protected Section CurrentSection = Section.Unknown;
        
        protected XmlParser(string xmlFile)
        {
            var reader = new StreamReader(xmlFile, Encoding.GetEncoding("windows-1252"));

            XmlReader = new XmlTextReader(reader)
            {
                WhitespaceHandling = WhitespaceHandling.Significant
            };
        }

        public XmlParser(BackgroundWorker worker, World world, string xmlFile, string xmlPlusFile) : this(xmlFile)
        {
            _worker = worker;
            
            World = world;
            
            _worker.ReportProgress(0, "Parsing XML Sections...");

            if (xmlPlusFile != xmlFile && File.Exists(xmlPlusFile))
            {
                _xmlPlusParser = new XmlPlusParser(worker, world, xmlPlusFile);
                World.Log.AppendLine("Found LEGENDS_PLUS.XML!");
                World.Log.AppendLine("Parsed additional data...\n");
            }
            else
            {
                World.Log.AppendLine("Missing LEGENDS_PLUS.XML!");
                World.Log.AppendLine("Use DFHacks' \"exportlegends info\" if available...\n");
            }
        }

        public virtual void Parse()
        {
            while (!XmlReader.EOF)
            {
                CurrentSection = GetSectionType(XmlReader.Name);
                
                if (CurrentSection == Section.Junk)
                {
                    XmlReader.Read();
                }
                else if (CurrentSection == Section.Unknown || CurrentSection == Section.Landmasses || CurrentSection == Section.MountainPeaks)
                {
                    SkipSection();
                }
                else
                {
                    ParseSection();
                }
            }

            XmlReader.Close();
        }

        protected Section GetSectionType(string sectionName)
        {
            switch (sectionName)
            {
                case "artifacts":
                    return Section.Artifacts;
                case "entities":
                    return Section.Entities;
                case "entity_populations":
                    return Section.EntityPopulations;
                case "historical_eras":
                    return Section.Eras;
                case "historical_event_collections":
                    return Section.EventCollections;
                case "historical_events":
                    return Section.Events;
                case "historical_figures":
                    return Section.HistoricalFigures;
                case "regions":
                    return Section.Regions;
                case "sites":
                    return Section.Sites;
                case "underground_regions":
                    return Section.UndergroundRegions;
                case "world_constructions":
                    return Section.WorldConstructions;
                case "poetic_forms":
                    return Section.PoeticForms;
                case "musical_forms":
                    return Section.MusicalForms;
                case "dance_forms":
                    return Section.DanceForms;
                case "written_contents":
                    return Section.WrittenContent;
                case "landmasses":
                    return Section.Landmasses;
                case "mountain_peaks":
                    return Section.MountainPeaks;
                case "creature_raw":
                    return Section.CreatureRaw;
                case "identities":
                    return Section.Identities;
                case "rivers":
                    return Section.Rivers;
                case "name":
                case "altname":
                case "xml":
                case "":
                case "df_world":
                    return Section.Junk;
                default:
                    World.ParsingErrors.Report("Unknown XML Section: " + sectionName);
                    return Section.Unknown;
            }
        }

        protected virtual void ParseSection()
        {
            XmlReader.ReadStartElement();
            
            _worker.ReportProgress(0, "... " + CurrentSection.GetDescription());

            while (XmlReader.NodeType != XmlNodeType.EndElement)
            {
                List<Property> item = ParseItem();

                _xmlPlusParser?.AddNewProperties(item, CurrentSection);

                AddItemToWorld(item);
            }

            ProcessXmlSection(CurrentSection); //Done with section, do post processing

            XmlReader.ReadEndElement();
        }

        protected void SkipSection()
        {
            string currentSectionName = XmlReader.Name;
            XmlReader.ReadStartElement();
            while (!(XmlReader.NodeType == XmlNodeType.EndElement && XmlReader.Name == currentSectionName))
            {
                XmlReader.Read();
            }
            XmlReader.ReadEndElement();
        }

        protected List<Property> ParseItem()
        {
            if (XmlReader.NodeType == XmlNodeType.EndElement)
                return null;
            
            _currentItemName = XmlReader.Name;

            XmlReader.ReadStartElement();

            var properties = new List<Property>();

            while (XmlReader.NodeType != XmlNodeType.EndElement && XmlReader.Name != _currentItemName)
            {
                properties.Add(ParseProperty());
            }

            XmlReader.ReadEndElement();

            return properties;
        }

        private Property ParseProperty()
        {
            if (string.IsNullOrEmpty(XmlReader.Name))
                return null;

            Property property = new Property();

            if (XmlReader.IsEmptyElement)
            {
                property.Name = XmlReader.Name;

                XmlReader.ReadStartElement();

                return property;
            }

            property.Name = XmlReader.Name;

            XmlReader.ReadStartElement();

            if (XmlReader.NodeType == XmlNodeType.Text)
            {
                property.Value = XmlReader.Value;

                XmlReader.Read();
            }
            else if (XmlReader.NodeType == XmlNodeType.Element)
            {
                property.SubProperties = new List<Property>();

                while (XmlReader.NodeType != XmlNodeType.EndElement)
                {
                    property.SubProperties.Add(ParseProperty());
                }
            }

            XmlReader.ReadEndElement();

            return property;
        }

        protected void AddItemToWorld(List<Property> properties)
        {
            string eventType = "";

            if (CurrentSection == Section.Events || CurrentSection == Section.EventCollections)
            {
                eventType = properties.Find(property => property.Name == "type")?.Value ?? "undefined";
            }

            if (CurrentSection != Section.Events && CurrentSection != Section.EventCollections)
            {
                AddFromXmlSection(CurrentSection, properties);
            }
            else if (CurrentSection == Section.EventCollections)
            {
                WorldEventFactory.AddEventCollectionToWorld(eventType, World, properties);
            }
            else if (CurrentSection == Section.Events)
            {
                WorldEventFactory.AddEventToWorld(eventType, World, properties);
            }

            string path = CurrentSection.ToString();

            if (CurrentSection == Section.Events || CurrentSection == Section.EventCollections)
            {
                path += " '" + eventType + "'/";
            }

            CheckKnownStateOfProperties(path, properties);
        }

        public void CheckKnownStateOfProperties(string path, List<Property> properties)
        {
            foreach (Property property in properties)
            {
                if (!property.Known)
                {
                    World.ParsingErrors.Report("|==> " + path + " --- Unknown Property: " + property.Name, property.Value);
                }
                if (property.SubProperties != null)
                {
                    CheckKnownStateOfProperties(path + "/" + property.Name, property.SubProperties);
                }
            }
        }

        public void AddFromXmlSection(Section section, List<Property> properties)
        {
            switch (section)
            {
                case Section.Regions:
                    World.Regions.Add(new WorldRegion(properties, World));
                    break;
                case Section.UndergroundRegions:
                    World.UndergroundRegions.Add(new UndergroundRegion(properties, World));
                    break;
                case Section.Sites:
                    World.Sites.Add(new Site(properties, World));
                    break;
                case Section.HistoricalFigures:
                    World.HistoricalFigures.Add(new HistoricalFigure(properties, World));
                    break;
                case Section.EntityPopulations:
                    World.EntityPopulations.Add(new EntityPopulation(properties, World));
                    break;
                case Section.Entities:
                    World.Entities.Add(new Entity(properties, World));
                    break;
                case Section.Eras:
                    World.Eras.Add(new Era(properties, World));
                    break;
                case Section.Artifacts:
                    World.Artifacts.Add(new Artifact(properties, World));
                    break;
                case Section.WorldConstructions:
                    World.WorldConstructions.Add(new WorldConstruction(properties, World));
                    break;
                case Section.PoeticForms:
                    World.PoeticForms.Add(new PoeticForm(properties, World));
                    break;
                case Section.MusicalForms:
                    World.MusicalForms.Add(new MusicalForm(properties, World));
                    break;
                case Section.DanceForms:
                    World.DanceForms.Add(new DanceForm(properties, World));
                    break;
                case Section.WrittenContent:
                    World.WrittenContents.Add(new WrittenContent(properties, World));
                    break;
                case Section.Landmasses:
                    World.Landmasses.Add(new Landmass(properties, World));
                    break;
                case Section.MountainPeaks:
                    World.MountainPeaks.Add(new MountainPeak(properties, World));
                    break;
                case Section.CreatureRaw:
                    World.AddCreatureInfo(new CreatureInfo(properties, World));
                    break;
                case Section.Identities:
                    World.Identities.Add(new Identity(properties, World));
                    break;
                case Section.Rivers:
                    World.Rivers.Add(new River(properties, World));
                    break;
                default:
                    World.ParsingErrors.Report("Unknown XML Section: " + section);
                    break;
            }
        }

        private void ProcessXmlSection(Section section)
        {
            if (section == Section.Events)
            {
                //Calculate Historical Figure Ages.
                int lastYear = World.Events.Last().Year;
                foreach (HistoricalFigure hf in World.HistoricalFigures)
                {
                    if (hf.DeathYear > 0)
                    {
                        hf.Age = hf.DeathYear - hf.BirthYear;
                    }
                    else
                    {
                        hf.Age = lastYear - hf.BirthYear;
                    }
                }
            }

            if (section == Section.EventCollections)
            {
                ProcessCollections();
            }

            //Create sorted Historical Figures so they can be binary searched by name, needed for parsing History file
            if (section == Section.HistoricalFigures)
            {
                World.ProcessHFtoHfLinks();
            }

            //Create sorted entities so they can be binary searched by name, needed for History/sites files
            if (section == Section.Entities)
            {
                World.ProcessReputations();
                World.ProcessHFtoSiteLinks();
                World.ProcessEntityEntityLinks();
            }

            //Calculate end years for eras and add list of wars during era.
            if (section == Section.Eras)
            {
                World.Eras.Last().EndYear = World.Events.Last().Year;
                for (int i = 0; i < World.Eras.Count - 1; i++)
                {
                    World.Eras[i].EndYear = World.Eras[i + 1].StartYear - 1;
                }

                foreach (Era era in World.Eras)
                {
                    era.Events =
                        World.Events.Where(events => events.Year >= era.StartYear && events.Year <= era.EndYear)
                            .OrderBy(events => events.Year)
                            .ToList();
                    era.Wars =
                        World.EventCollections.OfType<War>()
                            .Where(
                                war =>
                                    war.StartYear >= era.StartYear && war.EndYear <= era.EndYear && war.EndYear != -1
                                    //entire war between
                                    || war.StartYear >= era.StartYear && war.StartYear <= era.EndYear
                                    //war started before & ended
                                    || war.EndYear >= era.StartYear && war.EndYear <= era.EndYear && war.EndYear != -1
                                    //war started during
                                    || war.StartYear <= era.StartYear && war.EndYear >= era.EndYear
                                    //war started before & ended after
                                    || war.StartYear <= era.StartYear && war.EndYear == -1).ToList();
                }

            }
        }

        private void ProcessCollections()
        {
            World.Wars = World.EventCollections.OfType<War>().ToList();
            World.Battles = World.EventCollections.OfType<Battle>().ToList();
            World.BeastAttacks = World.EventCollections.OfType<BeastAttack>().ToList();

            foreach (EventCollection eventCollection in World.EventCollections)
            {
                //Sub Event Collections aren't created until after the main collection
                //So only IDs are stored in the main collection until here now that all collections have been created
                //and can now be added to their Parent collection
                foreach (int collectionId in eventCollection.CollectionIDs)
                {
                    eventCollection.Collections.Add(World.GetEventCollection(collectionId));
                }
            }

            //Attempt at calculating beast historical figure for beast attacks.
            //Find beast by looking at eventsList and fill in some event properties from the beast attacks's properties
            //Calculated here so it can look in Duel collections contained in beast attacks
            foreach (BeastAttack beastAttack in World.EventCollections.OfType<BeastAttack>())
            {
                if (beastAttack.Beast == null && beastAttack.GetSubEvents().OfType<HfAttackedSite>().Any())
                {
                    beastAttack.Beast = beastAttack.GetSubEvents().OfType<HfAttackedSite>().First().Attacker;
                }
                if (beastAttack.Beast == null && beastAttack.GetSubEvents().OfType<HfDestroyedSite>().Any())
                {
                    beastAttack.Beast = beastAttack.GetSubEvents().OfType<HfDestroyedSite>().First().Attacker;
                }

                //Find Beast by looking at fights, Beast always engages the first fight in a Beast Attack?
                if (beastAttack.Beast == null && beastAttack.GetSubEvents().OfType<HfSimpleBattleEvent>().Any())
                {
                    var hfSimpleBattleEvent = beastAttack.GetSubEvents().OfType<HfSimpleBattleEvent>().First();
                    beastAttack.Beast = hfSimpleBattleEvent.HistoricalFigure1;
                }
                if (beastAttack.Beast == null && beastAttack.GetSubEvents().OfType<AddHfEntityLink>().Any())
                {
                    var addHfEntityLink = beastAttack.GetSubEvents().OfType<AddHfEntityLink>().FirstOrDefault(link => link.LinkType == HfEntityLinkType.Enemy);
                    beastAttack.Beast = addHfEntityLink?.HistoricalFigure;
                }
                if (beastAttack.Beast == null && beastAttack.GetSubEvents().OfType<HfDied>().Any())
                {
                    var hfDied = beastAttack.GetSubEvents().OfType<HfDied>().FirstOrDefault();
                    if (hfDied?.HistoricalFigure != null && hfDied.HistoricalFigure.RelatedSites.Any(siteLink => siteLink.Type == SiteLinkType.Lair))
                    {
                        beastAttack.Beast = hfDied.HistoricalFigure;
                    }
                    else if (hfDied?.Slayer != null && hfDied.Slayer.RelatedSites.Any(siteLink => siteLink.Type == SiteLinkType.Lair))
                    {
                        beastAttack.Beast = hfDied.Slayer;
                    }
                    else
                    {
                        var slayers =
                            beastAttack.GetSubEvents()
                                .OfType<HfDied>()
                                .GroupBy(death => death.Slayer)
                                .Select(hf => new { HF = hf.Key, Count = hf.Count() });
                        if (slayers.Count(slayer => slayer.Count > 1) == 1)
                        {
                            HistoricalFigure beast = slayers.Single(slayer => slayer.Count > 1).HF;
                            beastAttack.Beast = beast;
                        }
                    }
                }


                //Fill in some various event info from collections.

                int insertIndex;

                foreach (ItemStolen theft in beastAttack.Collection.OfType<ItemStolen>())
                {
                    if (theft.Site == null)
                    {
                        theft.Site = beastAttack.Site;
                    }
                    else
                    {
                        beastAttack.Site = theft.Site;
                    }
                    if (theft.Thief == null)
                    {
                        theft.Thief = beastAttack.Beast;
                    }
                    else if (beastAttack.Beast == null)
                    {
                        beastAttack.Beast = theft.Thief;
                    }

                    if (beastAttack.Site != null)
                    {
                        insertIndex = beastAttack.Site.Events.BinarySearch(theft);
                        if (insertIndex < 0)
                        {
                            beastAttack.Site.Events.Add(theft);
                        }
                    }
                    if (beastAttack.Beast != null)
                    {
                        insertIndex = beastAttack.Beast.Events.BinarySearch(theft);
                        if (insertIndex < 0)
                        {
                            beastAttack.Beast.Events.Add(theft);
                        }
                    }
                }

                foreach (CreatureDevoured devoured in beastAttack.Collection.OfType<CreatureDevoured>())
                {
                    if (devoured.Eater == null)
                    {
                        devoured.Eater = beastAttack.Beast;
                    }
                    else if (beastAttack.Beast == null)
                    {
                        beastAttack.Beast = devoured.Eater;
                    }
                    if (beastAttack.Beast != null)
                    {
                        insertIndex = beastAttack.Beast.Events.BinarySearch(devoured);
                        if (insertIndex < 0)
                        {
                            beastAttack.Beast.Events.Add(devoured);
                        }
                    }
                }

                foreach (HfAbducted abducted in beastAttack.Collection.OfType<HfAbducted>())
                {
                    if (abducted.Snatcher == null)
                    {
                        abducted.Snatcher = beastAttack.Beast;
                    }
                    else if (beastAttack.Beast == null)
                    {
                        beastAttack.Beast = abducted.Snatcher;
                    }
                    if (beastAttack.Beast != null)
                    {
                        insertIndex = beastAttack.Beast.Events.BinarySearch(abducted);
                        if (insertIndex < 0)
                        {
                            beastAttack.Beast.Events.Add(abducted);
                        }
                    }
                }

                if (beastAttack.Beast != null)
                {
                    if (beastAttack.Beast.BeastAttacks == null)
                    {
                        beastAttack.Beast.BeastAttacks = new List<BeastAttack>();
                    }
                    beastAttack.Beast.BeastAttacks.Add(beastAttack);
                }
            }

            AssignBattleToSiteConquer();

            AssignSiteToItemStolen();
        }

        private void AssignSiteToItemStolen()
        {
            foreach (var raid in World.EventCollections.OfType<Raid>().Where(r => r.Site != null))
            {
                foreach (var itemStolenEvent in raid.GetSubEvents().OfType<ItemStolen>().Where(i => i.Site == null))
                {
                    itemStolenEvent.Site = raid.Site;
                }
            }
        }

        private void AssignBattleToSiteConquer()
        {
            //Assign a Conquering Event its corresponding battle
            //Battle = first Battle prior to the conquering?
            foreach (SiteConquered conquer in World.EventCollections.OfType<SiteConquered>())
            {
                for (int i = conquer.Id - 1; i >= 0; i--)
                {
                    EventCollection collection = World.GetEventCollection(i);
                    if (collection == null)
                    {
                        continue;
                    }

                    if (collection.GetType() == typeof(Battle))
                    {
                        conquer.Battle = collection as Battle;
                        if (conquer.Battle != null)
                        {
                            conquer.Battle.Conquering = conquer;
                            if (conquer.Battle.Defender == null && conquer.Defender != null)
                            {
                                conquer.Battle.Defender = conquer.Defender;
                            }
                        }

                        break;
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                XmlReader.Close();
                _xmlPlusParser.Dispose();
            }
        }
    }
}