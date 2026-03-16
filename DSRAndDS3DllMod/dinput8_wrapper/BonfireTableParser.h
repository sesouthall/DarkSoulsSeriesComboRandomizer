#ifndef _BONFIRE_TABLE_PARSER_H_
	#define _BONFIRE_TABLE_PARSER_H_

#include <string>
#include <map>

class BonfireTriple
{
public:
	static BonfireTriple* nullMapping;
	BonfireTriple(int rowIndex, int ds1BonfireId, int ds2BonfireId, int ds3BonfireId, std::string ds1BonfireName, std::string ds2BonfireName, std::string ds3BonfireName);
	int RowIndex;
	int DS1BonfireId;
	int DS2BonfireId;
	int DS3BonfireId;
	std::string DS1BonfireName;
	std::string DS2BonfireName;
	std::string DS3BonfireName;
};

class BonfireTable
{
private:
	std::map<int, int> DS1ToDS3Associations;
	std::map<int, int> DS2ToDS3Associations;
	std::map<int, BonfireTriple*> DS3ToBonfireTripleAssociations;
public:
	BonfireTable(std::map<int, int> DS1ToDS3Associations, std::map<int, int> DS2ToDS3Associations, std::map<int, BonfireTriple*> DS3ToBonfireTripleAssociations);
	BonfireTriple* GetByDS1BonfireId(int bonfireId);
	BonfireTriple* GetByDS2BonfireId(int bonfireId);
	BonfireTriple* GetByDS3BonfireId(int bonfireId);
	bool ContainsDS1BonfireId(int bonfireId);
	bool ContainsDS2BonfireId(int bonfireId);
	bool ContainsDS3BonfireId(int bonfireId);
};

BonfireTable* ParseBonfireTable(const char* bonfireTableFile);

#endif //!_BONFIRE_TABLE_PARSER_H_