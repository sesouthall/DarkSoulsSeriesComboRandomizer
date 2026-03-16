#ifndef _DS3REIMPLEMENTATIONS_H_
	#define _DS3REIMPLEMENTATIONS_H_

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
typedef void(EzStateTalkCommandFunction)(int64_t, EzStateExternalEventTemp*, int64_t, char*);
typedef int(GetQueryIdFunction)(EzStateEnvironmentQueryImpl*);
typedef int(GetArgCountFunction)(EzStateEnvironmentQueryImpl*);
typedef int(GetCommandIdFunction)(EzStateExternalEventTemp*);

typedef int(AtomicIncrementFunction)(int* param_1);
extern AtomicIncrementFunction *AtomicIncrement;

typedef void(Undefined8Undefined8FuncReturnsVoid)(undefined8 param_1, undefined8 param_2);
extern Undefined8Undefined8FuncReturnsVoid* FUN_140e60160;

typedef uint64_t(Undefined8Int64Int64Int64FuncReturnsUInt64)(undefined8 param_1, int64_t* param_2, int64_t param_3, int64_t param_4);
extern Undefined8Int64Int64Int64FuncReturnsUInt64* FUN_140e5f650;

#pragma pack(push, 1)
class ChrClassWarp
{
public:
	unsigned char padding[0xACC];
	int32_t LastBonfire;
};
#pragma pack(pop)

#endif // _DS3REIMPLEMENTATIONS_H_