#include "BonfireTableParser.h"

#include <fstream>
#include <map>
#include <string>
#include <vector>

std::map<std::string, int> DSRBonfireIds{
	{"Depths", 1002960},
	{"Sunlight Altar", 1012961},
	{"Undead Burg", 1012962},
	{"Undead Parish", 1012964},
	{"Firelink Shrine (DS1)", 1022960},
	{"Painted World of Ariamis", 1102960},
	{"Darkroot Garden", 1202961},
	{"Oolacile Sanctuary", 1212961},
	{"Oolacile Township", 1212962},
	{"Sanctuary Garden", 1212963},
	{"Oolacile Township Dungeon", 1212964},
	{"Chasm of the Abyss", 1212950},
	{"Upper Catacombs", 1302960},
	{"Inner Catacombs", 1302961},
	{"Vamos", 1302962},
	{"Lower Tomb of Giants", 1312960},
	{"Upper Tomb of Giants", 1312961},
	{"Great Hollow", 1320980},
	{"Stone Dragon", 1322960},
	{"Ash Lake", 1322961},
	{"Daughter of Chaos", 1402960},
	{"Lower Blighttown", 1402961},
	{"Upper Blighttown", 1402962},
	{"Lost Izalith", 1412960},
	{"Upper Demon Ruins", 1412961},
	{"Lower Demon Ruins", 1412962},
	{"Demon Ruins Catacombs", 1412963},
	{"Lost Izalith Lava Pits", 1412964},
	{"Sen's Fortress", 1502961},
	{"Chamber of the Princess", 1512950},
	{"Anor Londo (DS1)", 1512960},
	{"Inner Anor Londo", 1512961},
	{"Darkmoon Tomb", 1512962},
	{"Darkroot Basin", 1602961},
	{"Duke's Archives Balcony", 1702960},
	{"Prison Tower (DS1)", 1702961},
	{"Duke's Archives Entrance", 1702962},
	{"Firelink Altar", 1802960},
	{"Undead Asylum Courtyard", 1812960},
	{"Undead Asylum Sewer", 1812961}
};

std::map<std::string, int> DS2BonfireIds{
	{"Fire Keepers' Dwelling", 2650},
	{"The Far Fire", 4650},
	{"Cardinal Tower", 10655},
	{"Soldiers' Rest", 10660},
	{"The Crestfallen's Retreat", 10670},
	{"The Place Unbeknownst", 10675},
	{"Tower of Prayer (Amana)", 11650},
	{"Crumbled Ruins", 11655},
	{"Rhoy's Resting Place", 11660},
	{"Rise of the Dead", 11670},
	{"Lower Brightstone Cove", 14650},
	{"Royal Army Campsite", 14655},
	{"Chapel Threshold", 14660},
	{"Foregarden", 15650},
	{"Ritual Site", 15655},
	{"Straid's Cell", 16650},
	{"Exile Holding Cells", 16655},
	{"The Tower Apart", 16660},
	{"Upper Ramparts", 16665},
	{"McDuff's Workshop", 16670},
	{"Servants' Quarters", 16675},
	{"The Saltfort", 16685},
	{"The Mines", 17650},
	{"Lower Earthen Peak", 17655},
	{"Poison Pool", 17665},
	{"Central Earthen Peak", 17670},
	{"Upper Earthen Peak", 17675},
	{"Unseen Path to Heide", 18650},
	{"Ironhearth Hall", 19650},
	{"Threshold Bridge", 19655},
	{"Eygil's Idol", 19660},
	{"Belfry Sol Approach", 19665},
	{"King's Gate", 21650},
	{"Central Castle Drangleic", 21655},
	{"Forgotten Chamber", 21660},
	{"Under Castle Drangleic", 21665},
	{"Undead Refuge", 23650},
	{"Bridge Approach", 23655},
	{"Undead Lockaway", 23660},
	{"Undead Purgatory", 23665},
	{"Undead Ditch", 24650},
	{"Undead Crypt Entrance", 24655},
	{"Black Gulch Mouth", 25650},
	{"Central Gutter", 25655},
	{"Hidden Chamber", 25660},
	{"Upper Gutter", 25665},
	{"Dragon Aerie", 27650},
	{"Shrine Entrance", 27655},
	{"Old Akelarre", 29650},
	{"Tower of Flame", 31650},
	{"Heide's Ruin", 31655},
	{"The Blue Cathedral", 31660},
	{"Ruined Fork Road", 32655},
	{"Shaded Ruins", 32660},
	{"Gyrm's Respite", 33655},
	{"Ordeal's End", 33660},
	{"Grave Entrance", 34650},
	{"Harval's Resting Place", 34655},
	{"Sanctum Walk", 35650},
	{"Priestess' Chamber", 35655},
	{"Sanctum Nadir", 35665},
	{"Hidden Sanctum Chamber", 35670},
	{"Lair of the Imperfect", 35675},
	{"Sanctum Interior", 35680},
	{"Tower of Prayer (Shulva)", 35685},
	{"Throne Floor", 36650},
	{"Foyer", 36655},
	{"Upper Floor", 36660},
	{"Iron Hallway Entrance", 36665},
	{"Lowermost Floor", 36670},
	{"The Smelter Throne", 36675},
	{"Outer Wall", 37650},
	{"Abandoned Dwelling", 37660},
	{"Lower Garrison", 37665},
	{"Grand Cathedral", 37670},
	{"Expulsion Chamber", 37675},
	{"Inner Wall", 37685}
};

std::map<std::string, int> DS3BonfireIds{
	{"Firelink Shrine (DS3)", 4002950},
	{"Cemetery of Ash", 4002951},
	{"Iudex Gundyr", 4002952},
	{"Untended Graves", 4002953},
	{"Champion Gundyr", 4002954},
	{"High Wall of Lothric", 3002950},
	{"Tower on the Wall", 3002955},
	{"Vordt of the Boreal Valley", 3002952},
	{"Dancer of the Boreal Valley", 3002954},
	{"Oceiros, the Consumed King", 3002951},
	{"Foot of the High Wall", 3102954},
	{"Undead Settlement", 3102950},
	{"Cliff Underside", 3102952},
	{"Dilipidated Bridge", 3102953},
	{"Pit of Hollows", 3102951},
	{"Road of Sacrifices", 3302956},
	{"Halfway Fortress", 3302950},
	{"Crucifixion Woods", 3302957},
	{"Crystal Sage", 3302952},
	{"Farron Keep", 3302953},
	{"Keep Ruins", 3302954},
	{"Farron Keep Perimeter", 3302958},
	{"Old Wolf of Farron", 3302955},
	{"Abyss Watchers", 3302951},
	{"Cathedral of the Deep", 3502953},
	{"Cleansing Chapel", 3502950},
	{"Deacons of the Deep", 3502951},
	{"Rosaria's Bed Chamber", 3502952},
	{"Catacombs of Carthus", 3802956},
	{"High Lord Wolnir", 3802950},
	{"Abandoned Tomb", 3802951},
	{"Old King's Antechamber", 3802952},
	{"Demon Ruins", 3802953},
	{"Old Demon King", 3802954},
	{"Irithyll of the Boreal valley", 3702957},
	{"Central Irithyll", 3702954},
	{"Church of Yorshka", 3702950},
	{"Distant Manor", 3702955},
	{"Pontiff Sulyvahn", 3702951},
	{"Water Reserve", 3702956},
	{"Anor Londo (DS3)", 3702953},
	{"Prison Tower (DS3)", 3702958},
	{"Aldrich, Devourer of Gods", 3702952},
	{"Irithyll Dungeon", 3902950},
	{"Profaned Capital", 3902952},
	{"Yhorm The Giant", 3902951},
	{"Lothric Castle", 3012950},
	{"Dragon Barracks", 3012952},
	{"Dragonslayer Armour", 3012951},
	{"Grand Archives", 3412951},
	{"Twin Princes", 3412950},
	{"Archdragon Peak", 3202950},
	{"Dragon-Kin Mausoleum", 3202953},
	{"Great Belfry", 3202952},
	{"Nameless King", 3202951},
	{"Flameless Shrine", 4102950},
	{"Kiln of the First Flame", 4102951},
	{"The First Flame", 4102952},
	{"Snowfield", 4502951},
	{"Rope Bridge Cave", 4502952},
	{"Corvian Settlement", 4502953},
	{"Snowy Mountain Pass", 4502954},
	{"Ariandel Chapel", 4502955},
	{"Sister Friede", 4502950},
	{"Depths of the Painting", 4502957},
	{"Champion's Gravetender", 4502956},
	{"The Dreg Heap", 5002951},
	{"Earthen Peak Ruins", 5002952},
	{"Within the Earthen Peak Ruins", 5002953},
	{"The Demon Prince", 5002950},
	{"The Ringed City", 5102110},
	{"Mausoleum Lookout", 5102952},
	{"Ringed Inner Wall", 5102953},
	{"Ringed City Streets", 5102954},
	{"Shared Grave", 5102955},
	{"Church of Filianore", 5102950},
	{"Filianore's rest", 5112951},
	{"Slave Knight Gael", 5112950},
	{"Darkeater Midir", 5102951}
};

BonfireTriple::BonfireTriple(int rowIndex, int ds1BonfireId, int ds2BonfireId, int ds3BonfireId, std::string ds1BonfireName, std::string ds2BonfireName, std::string ds3BonfireName)
	: RowIndex(rowIndex),
	DS1BonfireId(ds1BonfireId),
	DS2BonfireId(ds2BonfireId),
	DS3BonfireId(ds3BonfireId),
	DS1BonfireName(ds1BonfireName),
	DS2BonfireName(ds2BonfireName),
	DS3BonfireName(ds3BonfireName)
{

}

BonfireTriple* BonfireTriple::nullMapping = new BonfireTriple(-1, -1, -1, -1, "", "", "");

BonfireTable::BonfireTable(std::map<int, int> DS1ToDS3Associations, std::map<int, int> DS2ToDS3Associations, std::map<int, BonfireTriple*> DS3ToBonfireTripleAssociations)
	: DS1ToDS3Associations{DS1ToDS3Associations},
	  DS2ToDS3Associations{DS2ToDS3Associations},
	  DS3ToBonfireTripleAssociations{DS3ToBonfireTripleAssociations}
{
}

BonfireTriple* BonfireTable::GetByDS1BonfireId(int bonfireId)
{
	return DS3ToBonfireTripleAssociations[DS1ToDS3Associations[bonfireId]];
}

BonfireTriple* BonfireTable::GetByDS2BonfireId(int bonfireId)
{
	return DS3ToBonfireTripleAssociations[DS2ToDS3Associations[bonfireId]];
}

BonfireTriple* BonfireTable::GetByDS3BonfireId(int bonfireId)
{
	return DS3ToBonfireTripleAssociations[bonfireId];
}

bool BonfireTable::ContainsDS1BonfireId(int bonfireId)
{
	return DS1ToDS3Associations.find(bonfireId) != DS1ToDS3Associations.end();
}

bool BonfireTable::ContainsDS2BonfireId(int bonfireId)
{
	return DS2ToDS3Associations.find(bonfireId) != DS2ToDS3Associations.end();
}

bool BonfireTable::ContainsDS3BonfireId(int bonfireId)
{
	return DS3ToBonfireTripleAssociations.find(bonfireId) != DS3ToBonfireTripleAssociations.end();
}

BonfireTable* ParseBonfireTable(const char* bonfireTableFile)
{
	std::ifstream file(bonfireTableFile);
	std::string line;
	std::map<int, int> DS1ToDS3Associations;
	std::map<int, int> DS2ToDS3Associations;
	std::map<int, BonfireTriple*> DS3ToBonfireTripleAssociations;
	int rowIndex = 0;
	while (std::getline(file, line))
	{
		int firstComma = line.find(',');
		if (firstComma == std::string::npos)
		{
			// Don't crash the game, just skip unparsable lines
			continue;
		}
		int secondComma = line.find(',', firstComma + 1);
		if (secondComma == std::string::npos)
		{
			// Don't crash the game, just skip unparsable lines
			continue;
		}
		int endOfLine = std::min(line.find('\n'), line.find('\r'));
		std::string ds1BonfireName = line.substr(0, firstComma);
		std::string ds2BonfireName = line.substr(firstComma + 1, secondComma - firstComma - 1);
		std::string ds3BonfireName = endOfLine == std::string::npos ? line.substr(secondComma + 1) : line.substr(secondComma + 1, secondComma - endOfLine - 1);
		printf_s("Line %d contains the following bonfires: ", rowIndex);
		printf_s(ds1BonfireName.c_str());
		printf_s(", ");
		printf_s(ds2BonfireName.c_str());
		printf_s(", ");
		printf_s(ds3BonfireName.c_str());
		printf_s("\n");
		if (DSRBonfireIds.find(ds1BonfireName) == DSRBonfireIds.end() ||
			DS2BonfireIds.find(ds2BonfireName) == DS2BonfireIds.end() ||
			DS3BonfireIds.find(ds3BonfireName) == DS3BonfireIds.end())
		{
			printf_s("Could not parse line: ");
			printf_s(line.c_str());
			printf_s("\n");
			// Don't crash the game, just skip unparsable lines
			continue;
		}

		printf_s("Parsed line: ");
		printf_s(line.c_str());
		printf_s("\n");

		int ds1BonfireId = DSRBonfireIds[ds1BonfireName];
		int ds2BonfireId = DS2BonfireIds[ds2BonfireName];
		int ds3BonfireId = DS3BonfireIds[ds3BonfireName];
		BonfireTriple *parsedLine = new BonfireTriple(rowIndex, ds1BonfireId, ds2BonfireId, ds3BonfireId, ds1BonfireName, ds2BonfireName, ds3BonfireName);
		
		DS1ToDS3Associations[ds1BonfireId] = ds3BonfireId;
		DS2ToDS3Associations[ds2BonfireId] = ds3BonfireId;
		DS3ToBonfireTripleAssociations[ds3BonfireId] = parsedLine;

		rowIndex++;
	}
	return new BonfireTable(DS1ToDS3Associations, DS2ToDS3Associations, DS3ToBonfireTripleAssociations);
}