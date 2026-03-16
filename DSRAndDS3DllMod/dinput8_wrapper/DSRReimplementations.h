#ifndef _DSRREIMPLEMENTATIONS_H_
	#define _DSRREIMPLEMENTATIONS_H_

#include <stdint.h>
#include <windows.h>

typedef signed long long undefined8;
typedef signed int undefined4;

class EzStateEnvironmentQueryImpl
{

};

class EzStateExternalEventTemp
{

};

typedef void(EzStateTalkQueryFunction)(int64_t, float*, EzStateEnvironmentQueryImpl*);
typedef void(EzStateTalkCommandFunction)(int64_t, EzStateExternalEventTemp*);
typedef int(GetQueryIdFunction)(EzStateEnvironmentQueryImpl*);
typedef int(GetArgCountFunction)(EzStateEnvironmentQueryImpl*);
typedef int(GetCommandIdFunction)(EzStateExternalEventTemp*);

#pragma pack(push, 1)
class ChrClassWarp
{
public:
	byte padding[0xB34];
	int32_t LastBonfire;
};
#pragma pack(pop)

#endif // !_DSRREIMPLEMENTATIONS_H_