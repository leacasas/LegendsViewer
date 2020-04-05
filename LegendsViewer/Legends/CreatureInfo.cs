using LegendsViewer.Legends.Parser;
using System.Collections.Generic;

namespace LegendsViewer.Legends
{
    public class CreatureInfo
    {
        public static CreatureInfo Unknown { get; set; }
        public string Id { get; set; }
        public string NameSingular { get; set; }
        public string NamePlural { get; set; }

        public CreatureInfo(List<Property> properties)
        {
            foreach (Property property in properties)
            {
                switch (property.Name)
                {
                    case "creature_id": 
                        Id = string.Intern(property.Value);
                        break;
                    case "name_singular":
                        NameSingular = string.Intern(Formatting.FormatRace(property.Value));
                        break;
                    case "name_plural":
                        NamePlural = string.Intern(Formatting.FormatRace(property.Value));
                        break;
                    default:
                        property.Known = true;
                        break;
                }
            }
        }

        public CreatureInfo(string identifier)
        {
            Id = identifier;
            NameSingular = string.Intern(Formatting.FormatRace(identifier));
            NamePlural = string.Intern(Formatting.MakePopulationPlural(NameSingular));
        }

        public override string ToString()
        {
            return NamePlural;
        }
    }
}