#ifndef _DS2REIMPLEMENTATIONS_H_
    #define _DS2REIMPLEMENTATIONS_H_

#include <windows.h>
#include <stdio.h>
#include "GameData.h"

typedef signed long long undefined8;
typedef signed int undefined4;

typedef undefined8(BonfireMenuOptionFunction)(undefined8, undefined8);

#pragma pack(push, 1)
class LocalizedString
{
public:
    int32_t LocTable;
    int32_t StringId;
};

class MapObjBonfireComponent
{
};

class DemoCharacterCtrl
{
public:
    undefined8 field_0x0;
    byte padding1[0x8];
    undefined8 field_0x10;
    undefined8 field_0x18;
    undefined8* field_0x20;
    byte field_0x28;
    byte padding2[0x7];
    undefined8 field_0x30;
    undefined8* field_0x38;
    byte padding3[0x8];
    undefined4 field_0x48;
    undefined4 field_0x4C;
    undefined8 field_0x50;
    byte padding4[0x900];
};

class UnknownClassProperty
{
public:
    byte padding[0x30];
    int64_t field_0x30;
};

class FeOperatorTestBonfirePropertyOfProperty
{
public:
    byte padding1[0x3a];
    byte field_0x3a;
    byte padding2[0xcd];
    UnknownClassProperty* field_0x108;
    byte padding3[0x1b0];
    undefined8 field_0x2c0;
    byte padding4[0x2c98];
    undefined4 field_0x2f60;
    undefined4 field_0x2f64;
};

class EventFlagManager
{
};

class EventBonfireManager
{
public:
    void* mapObjectBonfireComponentVtable;
    MapObjBonfireComponent* MapObjBonfireComponent;
    BonfireMenuOptionFunction* field_0x10;
    int64_t field_0x18;
    FeOperatorTestBonfirePropertyOfProperty* field_0x20;
};

class EventManager
{
public:
    byte padding1[0x20];
    EventFlagManager* event_flag_manager;
    byte padding2[0x30];
    EventBonfireManager* EventBonfireManager;
    byte padding3[0x10c];
    int32_t last_rested_bonfire;
};

class PlayerData
{
public:
    byte padding[0x68];
    int32_t field_0x68;
};

class GameDataManager
{
public:
    byte padding1[0x68];
    int64_t field_0x68;
    byte padding[0x50];
    PlayerData* player_data;
};

class GameManagerImp
{
public:
    byte padding1[0x70];
    EventManager* EventManager;
    byte padding2[0x30];
    GameDataManager* GameDataManager;
    byte padding3[0x21e0];
    int64_t* field1124_0x22f0;
};
#pragma pack(pop)

extern GameManagerImp* gameManagerImp;

extern undefined8 AddNewMenuOptions(undefined8 menuObject, FeOperatorTestBonfirePropertyOfProperty* param_1, undefined4 param_3);

typedef GameDataManager* (voidFuncReturnsGameDataManager)(void);
extern voidFuncReturnsGameDataManager* FUN_140040420;

typedef void(gameDataManagerUndefined8FuncReturnsVoid)(GameDataManager*, undefined8);
extern gameDataManagerUndefined8FuncReturnsVoid* FUN_1401abee0;

typedef undefined8(eventBonfireManagerIntFuncReturnsUndefined8)(EventBonfireManager*, int);
extern eventBonfireManagerIntFuncReturnsUndefined8* FUN_14017e370;

typedef undefined8* (undefined8FuncReturnsUndefined8)(undefined8*);
extern undefined8FuncReturnsUndefined8* FUN_14024f090;

typedef bool (undefined8Undefined4FuncReturnsBool)(undefined8*, undefined4);
extern undefined8Undefined4FuncReturnsBool* FUN_14024f2a0;

typedef bool (feOperatorTestBonfirePropertyOfPropertyFuncReturnsBool)(FeOperatorTestBonfirePropertyOfProperty*);
extern feOperatorTestBonfirePropertyOfPropertyFuncReturnsBool* FUN_140513600;

typedef byte(eventBonfireManagerUndefined4FuncReturnsByte)(EventBonfireManager*, undefined4);
extern eventBonfireManagerUndefined4FuncReturnsByte* FUN_14017e830;

typedef int64_t* (voidFuncReturnsInt64)(void);
extern voidFuncReturnsInt64* FUN_140046bd0;

typedef void* (intIntVtablePointerFuncReturnsPointer)(int, int, int64_t*);
extern intIntVtablePointerFuncReturnsPointer* heapAllocator;

typedef void (mapObjBonfireComponentFuncReturnsVoid)(MapObjBonfireComponent**);
extern mapObjBonfireComponentFuncReturnsVoid* FUN_140833650;

typedef LocalizedString* (LocalizedStringInitializer)(LocalizedString*, int32_t, int32_t);
extern LocalizedStringInitializer* FUN_14003d870;

typedef DemoCharacterCtrl* (localizedStringDemoCharacterCtrlUndefined4FuncReturnsDemoCharacterCtrl)(LocalizedString*, DemoCharacterCtrl*, int32_t);
extern localizedStringDemoCharacterCtrlUndefined4FuncReturnsDemoCharacterCtrl* FUN_14003d880;

typedef undefined8* (DemoCharacterCtrlDemoCharacterCtrlUInt64UInt64FuncReturnsUndefined8)(DemoCharacterCtrl*, DemoCharacterCtrl*, uint64_t, uint64_t);
extern DemoCharacterCtrlDemoCharacterCtrlUInt64UInt64FuncReturnsUndefined8* FUN_14001dce0;

typedef byte(FeOperatorTestBonfirePropertyOfPropertyFuncReturnsByte)(FeOperatorTestBonfirePropertyOfProperty*);
extern FeOperatorTestBonfirePropertyOfPropertyFuncReturnsByte* FUN_1400d81e0;

typedef char* (RetrieveLocalizedString)(int32_t, int32_t);
extern RetrieveLocalizedString* FUN_140503620;

typedef undefined8(undefined8Undefined8Undefined8FuncReturnsUndefined8)(undefined8**, int64_t*, undefined4*);
extern undefined8Undefined8Undefined8FuncReturnsUndefined8* FUN_14002aad0;

typedef undefined8(undefined8StringFuncReturnsUndefined8)(undefined8, char*);
extern undefined8StringFuncReturnsUndefined8* FUN_14002c580;

typedef undefined8(BonfireMenuFuncReturnsBonfireMenu)(undefined8);
extern BonfireMenuFuncReturnsBonfireMenu* FUN_14002b170;

typedef undefined8(addOptionToBonfireMenu)(undefined8, char*, EventBonfireManager**);
extern addOptionToBonfireMenu* FUN_14002b240;

typedef undefined8(undefined8ByteFuncReturnsUndefined8)(undefined8, char);
extern undefined8ByteFuncReturnsUndefined8* FUN_14002c680;

typedef undefined8(undefined8DemoCharacterCtrlEventBonfireManagerFuncReturnsUndefined8)(undefined8, DemoCharacterCtrl*, EventBonfireManager**);
extern undefined8DemoCharacterCtrlEventBonfireManagerFuncReturnsUndefined8* FUN_14002b3e0;

typedef undefined8(undefined8Undefined8FuncReturnsUndefined8)(undefined8, undefined8*);
extern undefined8Undefined8FuncReturnsUndefined8* FUN_14002b670;

typedef undefined8(undefined8CharFuncReturnsUndefined8)(undefined8*, char*);
extern undefined8CharFuncReturnsUndefined8* FUN_1400268c0;

typedef void(undefined8Undefined8Undefined8FuncReturnsVoid)(undefined8, undefined8, undefined8);
extern undefined8Undefined8Undefined8FuncReturnsVoid* FUN_140028bb0;

typedef void(undefined4FuncReturnsVoid)(undefined4*);
extern undefined4FuncReturnsVoid* FUN_14002aed0;

typedef void(undefined8Undefined8FuncReturnsVoid)(undefined8*, undefined8);

typedef void(undefined8FuncReturnsVoid)(undefined8);
extern undefined8FuncReturnsVoid* thunk_FUN_141b6b19f;

typedef undefined8(FeOperatorTestBonfirePropertyOfPropertyUndefined8Undefined4FuncReturnsVoid)(FeOperatorTestBonfirePropertyOfProperty*, undefined8, undefined4);
extern FeOperatorTestBonfirePropertyOfPropertyUndefined8Undefined4FuncReturnsVoid* OriginalCreateBonfireMenu;

typedef bool (WriteEventFlagFunction)(EventFlagManager*, uint32_t, bool);
extern WriteEventFlagFunction* writeEventFlag;

static EventBonfireManager* ConstructEventBonfireManager(FeOperatorTestBonfirePropertyOfProperty* param_1, BonfireMenuOptionFunction* callback)
{
    int64_t* plVar6 = FUN_140046bd0();
    EventBonfireManager* pEVar7 = (EventBonfireManager*)heapAllocator(0x28, 8, plVar6);
    if (pEVar7 != (EventBonfireManager*)0x0) {
        pEVar7->field_0x10 = (BonfireMenuOptionFunction*)(callback);
        pEVar7->field_0x18 = 0;
        pEVar7->field_0x20 = param_1;
        pEVar7->mapObjectBonfireComponentVtable = (void*)(Game::base_address + 0x10bad98);
        pEVar7->MapObjBonfireComponent = 0;
        FUN_140833650(&pEVar7->MapObjBonfireComponent);
        return pEVar7;
    }

    return 0;
}

static undefined8 Override_CreateBonfireMenu(FeOperatorTestBonfirePropertyOfProperty* param_1, undefined8 param_2, undefined4 param_3)
{
    bool uVar2;
    int iVar3;
    GameDataManager* lVar4;
    int64_t lVar5;
    DemoCharacterCtrl* pDVar8;
    char* itemBoxString;
    char* nullifyHumanEffigysEffectString;
    char* burnString;
    char* attuneSpellsString;
    char* travelString;
    char* bonfireNameString;
    undefined8 uVar16;
    undefined8 uVar17;
    int nextNGPlusCount;
    char canHumanEffigyBeNullified;
    undefined4 local_a70;
    undefined4 local_a6c;
    uint32_t local_a68;
    EventBonfireManager* itemBoxCallback;
    LocalizedString nullifyHumanEffigysEffectLocId;
    EventBonfireManager* nullifyHumanEffigyEffectCallback;
    LocalizedString travelLocId;
    LocalizedString beginJourneyNToDrangleicLocId;
    undefined8 local_a28;
    EventBonfireManager* travelFunctionCallback;
    EventBonfireManager* goToNGPlusCallback;
    EventBonfireManager* burnCallback;
    LocalizedString itemBoxLocId;
    EventBonfireManager* attuneSpellsCallback;
    LocalizedString burnLocId;
    LocalizedString bonfireNameLocId;
    LocalizedString attuneSpellsLocId;
    DemoCharacterCtrl auStack_9e0;
    char local_958[16];
    DemoCharacterCtrl local_940;

    printf_s("Started menu creation\n");
    param_1->field_0x2f60 = param_3;
    lVar4 = FUN_140040420();
    if (lVar4) {
        FUN_1401abee0(lVar4, 0x304b480);
        FUN_1401abee0(lVar4, 0x304db90);
    }
    nextNGPlusCount = 0;
    iVar3 = 0;
    if (lVar5 = FUN_14017e370(gameManagerImp->EventManager->EventBonfireManager, param_3), lVar5 != 0)
    {
        iVar3 = *(int*)(lVar5 + 0x1c);
    }
    param_1->field_0x2f64 = iVar3;
    FUN_14024f090(&local_a28);
    if (FUN_14024f2a0(&local_a28, param_1->field_0x2f64) && gameManagerImp->field1124_0x22f0 &&
        FUN_140513600(param_1) && FUN_14017e830(gameManagerImp->EventManager->EventBonfireManager, param_1->field_0x2f60)) {
        canHumanEffigyBeNullified = 1;
    }
    else
    {
        canHumanEffigyBeNullified = 0;
    }
    local_a68 = 0;
    auStack_9e0.field_0x48 = 0;

    printf_s("Ready to construct 1st EventBonfireManager\n");
    goToNGPlusCallback = ConstructEventBonfireManager(param_1, (BonfireMenuOptionFunction*)(Game::base_address + 0xd9a10));

    if (gameManagerImp->GameDataManager && gameManagerImp->GameDataManager->player_data) {
        nextNGPlusCount = (gameManagerImp->GameDataManager->player_data->field_0x68) + 1;
    }
    FUN_14003d870(&beginJourneyNToDrangleicLocId, 0xb, 0x277d); //Begin journey %d to Drangleic
    pDVar8 = FUN_14003d880(&beginJourneyNToDrangleicLocId, &local_940, nextNGPlusCount);
    auStack_9e0.field_0x20 = pDVar8->field_0x20;
    auStack_9e0.field_0x18 = 7;
    auStack_9e0.field_0x10 = 0;
    auStack_9e0.field_0x0 = auStack_9e0.field_0x0 & 0xffffff00;
    auStack_9e0.field_0x38 = auStack_9e0.field_0x20;
    FUN_14001dce0(&auStack_9e0, pDVar8, 0, 0xffffffffffffffff);
    auStack_9e0.field_0x28 = pDVar8->field_0x28;
    auStack_9e0.field_0x30 = 0x5f5c5b200000067;

    printf_s("Ready to construct 2nd EventBonfireManager\n");
    itemBoxCallback = ConstructEventBonfireManager(param_1, (BonfireMenuOptionFunction*)(Game::base_address + 0xd90b0));

    printf_s("Ready to construct 3rd EventBonfireManager\n");
    nullifyHumanEffigyEffectCallback = ConstructEventBonfireManager(param_1, (BonfireMenuOptionFunction*)(Game::base_address + 0xd93b0));

    printf_s("Ready to construct 4th EventBonfireManager\n");
    burnCallback = ConstructEventBonfireManager(param_1, (BonfireMenuOptionFunction*)(Game::base_address + 0xd8870));

    printf_s("Ready to construct 5th EventBonfireManager\n");
    attuneSpellsCallback = ConstructEventBonfireManager(param_1, (BonfireMenuOptionFunction*)(Game::base_address + 0xd9c90));

    printf_s("Ready to construct 6th EventBonfireManager\n");
    travelFunctionCallback = ConstructEventBonfireManager(param_1, (BonfireMenuOptionFunction*)(Game::base_address + 0xd9dd0));

    printf_s("Ready to write bonfire menu options\n");

    FUN_14003d870(&attuneSpellsLocId, 0xb, 0x2775); //Attune spells
    FUN_14003d870(&travelLocId, 0xb, 0x2776); //Travel
    FUN_14003d870(&itemBoxLocId, 0xb, 0x2777); //Item box
    FUN_14003d870(&burnLocId, 0xb, 0x2778); //Burn
    FUN_14003d870(&nullifyHumanEffigysEffectLocId, 0xb, 0x277e); //Nullify Human Effigy's effect
    FUN_14003d870(&bonfireNameLocId, 0x12, param_3); // Bonfire name (e.g. The Far Fire)
    local_a70 = 0;
    local_a6c = 0x14;
    local_a68 = local_a68 & 0xffffff00;
    uVar2 = FUN_1400d81e0(param_1);
    printf_s("Getting localized strings\n");
    itemBoxString = FUN_140503620(itemBoxLocId.LocTable, itemBoxLocId.StringId);
    nullifyHumanEffigysEffectString = FUN_140503620(nullifyHumanEffigysEffectLocId.LocTable, nullifyHumanEffigysEffectLocId.StringId);
    burnString = FUN_140503620(burnLocId.LocTable, burnLocId.StringId);
    attuneSpellsString = FUN_140503620(attuneSpellsLocId.LocTable, attuneSpellsLocId.StringId);
    travelString = FUN_140503620(travelLocId.LocTable, travelLocId.StringId);
    bonfireNameString = FUN_140503620(bonfireNameLocId.LocTable, bonfireNameLocId.StringId);
    uVar16 = FUN_14002aad0(&local_940.field_0x38, &param_1->field_0x108->field_0x30, &local_a70);
    printf_s("Writing into menu object\n");
    uVar16 = FUN_14002c580(uVar16, bonfireNameString);
    uVar16 = FUN_14002b170(uVar16);
    uVar16 = FUN_14002b240(uVar16, travelString, &travelFunctionCallback);
    uVar16 = FUN_14002b240(uVar16, attuneSpellsString, &attuneSpellsCallback);
    uVar16 = FUN_14002b240(uVar16, burnString, &burnCallback);
    uVar16 = FUN_14002b240(uVar16, nullifyHumanEffigysEffectString, &nullifyHumanEffigyEffectCallback);
    uVar16 = FUN_14002c680(uVar16, canHumanEffigyBeNullified); // Hide previous menu option if false?
    uVar16 = FUN_14002b240(uVar16, itemBoxString, &itemBoxCallback);
    uVar16 = AddNewMenuOptions(uVar16, param_1, param_3);
    uVar16 = FUN_14002b3e0(uVar16, &auStack_9e0, &goToNGPlusCallback); //go to NG+ menu option?
    uVar16 = FUN_14002c680(uVar16, uVar2); // Hide previous menu option if the game isn't complete?
    uVar16 = FUN_14002b670(uVar16, &auStack_9e0.field_0x50);
    printf_s("Finishing stuff?\n");
    uVar17 = FUN_1400268c0(&param_1->field_0x2c0, local_958);
    FUN_140028bb0(param_2, uVar17, uVar16);
    FUN_14002aed0(&local_940.field_0x48);
    if (7 < (uint64_t)local_940.field_0x18) {
        (*(undefined8Undefined8FuncReturnsVoid**)((*local_940.field_0x20) + 0x68))
            (local_940.field_0x20, local_940.field_0x0);
    }

    printf_s("Done\n");

    return param_2;
}
#endif // !_DS2REIMPLEMENTATIONS_H_