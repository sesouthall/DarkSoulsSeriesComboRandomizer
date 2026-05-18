#include <windows.h>
#include <thread>
#include <iostream>
#include <functional>
#include <format>
#include <map>
#include "SP/memory/injection/asm/x64.h"
#include "SP/memory/aob_scan.h"
#include "sp/memory/pointer.h"
#include "GameData.h"
#include "DS2Reimplementations.h"
#include "BonfireTableParser.h"
#include "HookManager.inl"
using namespace std;

HINSTANCE hinst_dll = 0;
std::thread begin_thread;

extern "C" UINT_PTR directinput_create_proc = 0;
extern "C" __declspec(dllexport) HRESULT __cdecl DirectInput8Create(HINSTANCE hinst, DWORD dwVersion, REFIID riidltf, LPVOID * ppvOut, LPUNKNOWN punkOuter);
static decltype(&DirectInput8Create) original_dinput8_create;

GameManagerImp* gameManagerImp;
undefined8 AddNewMenuOptions(undefined8 menuObject, FeOperatorTestBonfirePropertyOfProperty* param_1, undefined4 param_3);
voidFuncReturnsGameDataManager* FUN_140040420;
gameDataManagerUndefined8FuncReturnsVoid* FUN_1401abee0;
eventBonfireManagerIntFuncReturnsUndefined8* FUN_14017e370;
undefined8FuncReturnsUndefined8* FUN_14024f090;
undefined8Undefined4FuncReturnsBool* FUN_14024f2a0;
feOperatorTestBonfirePropertyOfPropertyFuncReturnsBool* FUN_140513600;
eventBonfireManagerUndefined4FuncReturnsByte* FUN_14017e830;
voidFuncReturnsInt64* FUN_140046bd0;
intIntVtablePointerFuncReturnsPointer* heapAllocator;
mapObjBonfireComponentFuncReturnsVoid* FUN_140833650;
LocalizedStringInitializer* FUN_14003d870;
localizedStringDemoCharacterCtrlUndefined4FuncReturnsDemoCharacterCtrl* FUN_14003d880;
DemoCharacterCtrlDemoCharacterCtrlUInt64UInt64FuncReturnsUndefined8* FUN_14001dce0;
FeOperatorTestBonfirePropertyOfPropertyFuncReturnsByte* FUN_1400d81e0;
RetrieveLocalizedString* FUN_140503620;
undefined8Undefined8Undefined8FuncReturnsUndefined8* FUN_14002aad0;
undefined8StringFuncReturnsUndefined8* FUN_14002c580;
BonfireMenuFuncReturnsBonfireMenu* FUN_14002b170;
addOptionToBonfireMenu* FUN_14002b240;
undefined8ByteFuncReturnsUndefined8* FUN_14002c680;
undefined8DemoCharacterCtrlEventBonfireManagerFuncReturnsUndefined8* FUN_14002b3e0;
undefined8Undefined8FuncReturnsUndefined8* FUN_14002b670;
undefined8CharFuncReturnsUndefined8* FUN_1400268c0;
undefined8Undefined8Undefined8FuncReturnsVoid* FUN_140028bb0;
undefined4FuncReturnsVoid* FUN_14002aed0;
undefined8FuncReturnsVoid* thunk_FUN_141b6b19f;
FeOperatorTestBonfirePropertyOfPropertyUndefined8Undefined4FuncReturnsVoid* OriginalCreateBonfireMenu;
WriteEventFlagFunction* writeEventFlag;

BonfireTable* parsedMappings;
HANDLE hPipe;

static undefined8 DS1WarpEvent(undefined8 param_1, undefined8 param_2)
{
    printf_s("Writing to pipe for DS1 warp\n");
    int currentBonfireId = gameManagerImp->EventManager->last_rested_bonfire;
    std::string message = std::format("{}\n", parsedMappings->GetByDS2BonfireId(currentBonfireId)->DS1BonfireId);
    DWORD length = (message.length() + 1) * sizeof(TCHAR);
    DWORD written;
    if (hPipe == INVALID_HANDLE_VALUE)
    {
        printf_s("hPipe has gone bad\n");
        return param_2;
    }
    printf_s("hPipe is %llx\n", hPipe);
    if (WriteFile(hPipe, message.c_str(), length, &written, NULL))
    {
        FlushFileBuffers(hPipe);
        printf_s("Wrote %d/%d characters of %s\n", written, length, message.c_str());
    }
    else
    {
        int error = GetLastError();
        printf_s("Failed to write to pipe with error: %x\n", error);
    }
    return param_2;
}

static undefined8 DS3WarpEvent(undefined8 param_1, undefined8 param_2)
{
    printf_s("Writing to pipe for DS3 warp\n");
    int currentBonfireId = gameManagerImp->EventManager->last_rested_bonfire;
    std::string message = std::format("{}\n", parsedMappings->GetByDS2BonfireId(currentBonfireId)->DS3BonfireId);
    DWORD length = (message.length() + 1) * sizeof(TCHAR);
    DWORD written;
    if (hPipe == INVALID_HANDLE_VALUE)
    {
        printf_s("hPipe has gone bad\n");
        return param_2;
    }
    printf_s("hPipe is %llx\n", hPipe);
    if (WriteFile(hPipe, message.c_str(), length, &written, NULL))
    {
        FlushFileBuffers(hPipe);
        printf_s("Wrote %d/%d characters of %s\n", written, length, message.c_str());
    }
    else
    {
        int error = GetLastError();
        printf_s("Failed to write to pipe with error: %x\n", error);
    }
    return param_2;
}

static undefined8 AddSpecificMenuOption(undefined8 menuOject, LocalizedString menuOptionText, BonfireMenuOptionFunction* menuOptionCallback, FeOperatorTestBonfirePropertyOfProperty* param_1)
{
    EventBonfireManager* newMenuOption = ConstructEventBonfireManager(param_1, menuOptionCallback);
    char* newMenuOptionString = FUN_140503620(menuOptionText.LocTable, menuOptionText.StringId);
    menuOject = FUN_14002b240(menuOject, newMenuOptionString, &newMenuOption);
    return menuOject;
}

static undefined8 AddNewMenuOptions(undefined8 menuObject, FeOperatorTestBonfirePropertyOfProperty* param_1, undefined4 param_3)
{
    printf_s("Considering bonfire %d\n", param_3);
    if (parsedMappings->ContainsDS2BonfireId(param_3))
    {
        LocalizedString ds1MenuText, ds3MenuText;
        ds1MenuText.LocTable = 0xb;
        ds1MenuText.StringId = parsedMappings->GetByDS2BonfireId(param_3)->DS1BonfireId;
        if (ds1MenuText.StringId != -1)
        {
            menuObject = AddSpecificMenuOption(menuObject, ds1MenuText, DS1WarpEvent, param_1);
        }
        ds3MenuText.LocTable = 0xb;
        ds3MenuText.StringId = parsedMappings->GetByDS2BonfireId(param_3)->DS3BonfireId;
        if (ds3MenuText.StringId != -1)
        {
            menuObject = AddSpecificMenuOption(menuObject, ds3MenuText, DS3WarpEvent, param_1);
        }
    }
    else
    {
        printf_s("Not adding extra menu options to %d\n", param_3);
    }
    return menuObject;
}

bool Begin(uint64_t qModuleHandle)
{
    char dllpath[MAX_PATH];
    GetSystemDirectoryA(dllpath, MAX_PATH);
    strcat_s(dllpath, "\\dinput8.dll");
    hinst_dll = LoadLibraryA(dllpath);

    if (!hinst_dll) {
        MessageBoxA(NULL, "Failed to load original DLL\n", "Error", MB_ICONERROR);
        return false;
    };

    original_dinput8_create = (decltype(&DirectInput8Create))GetProcAddress(hinst_dll, "DirectInput8Create");
    if (!original_dinput8_create) {
        MessageBoxA(NULL, "Failed to load original DLL\n", "Error", MB_ICONERROR);
        return false;
    }

    // Uncomment these lines to create a console and allow console output for debugging
    AllocConsole();
    freopen_s((FILE**)stdout, "CONOUT$", "w", stdout);
    printf_s("Working fine so far\n");

    Game::init();

    void* gameManagerImp_sp = sp::mem::aob_scan("48 8B 05 xx xx xx xx 48 8B 58 38 48 85 DB 74 xx F6");
    std::chrono::milliseconds gameManagerRefreshInterval(500);
    while (gameManagerImp == 0)
    {
        gameManagerImp = (GameManagerImp*)sp::mem::pointer<uint8_t>((void*)((uint64_t)gameManagerImp_sp + *(uint32_t*)((uint64_t)gameManagerImp_sp + 3) + 7), { 0x0 }).resolve();
        
        std::this_thread::sleep_for(gameManagerRefreshInterval);
    }

    char mappingFile[MAX_PATH] = "%LOCALAPPDATA%\\DarkSoulsSeriesComboRandomizer\\BonfireMappings.txt";
    DoEnvironmentSubstA(mappingFile, MAX_PATH);
    parsedMappings = ParseBonfireTable(mappingFile);

    hPipe = CreateFile(
        TEXT("\\\\.\\pipe\\DarkSoulsSeriesComboRandomizerDS2"),
        GENERIC_WRITE,
        0,
        NULL,
        OPEN_EXISTING,
        0,
        NULL);

    if (hPipe == INVALID_HANDLE_VALUE)
    {
        int error = GetLastError();
        printf_s("Failed to create pipe with error %d\n", error);
    }

    printf_s("hPipe is %llx\n", hPipe);

    Hook::HookManager* pHookManager = Hook::HookManager::GetInstance();
    pHookManager->Initialize();

    pHookManager->CreateHook<FeOperatorTestBonfirePropertyOfPropertyUndefined8Undefined4FuncReturnsVoid*>(Game::ds2_base + 0xd6dc0, &OriginalCreateBonfireMenu, &Override_CreateBonfireMenu, NULL, "CreateBonfireMenu");

    FUN_140040420 = (voidFuncReturnsGameDataManager*)(Game::ds2_base + 0x40420);
    FUN_1401abee0 = (gameDataManagerUndefined8FuncReturnsVoid*)(Game::ds2_base + 0x1abee0);
    FUN_14017e370 = (eventBonfireManagerIntFuncReturnsUndefined8*)(Game::ds2_base + 0x17e370);
    FUN_14024f090 = (undefined8FuncReturnsUndefined8*)(Game::ds2_base + 0x24f090);
    FUN_14024f2a0 = (undefined8Undefined4FuncReturnsBool*)(Game::ds2_base + 0x24f2a0);
    FUN_140513600 = (feOperatorTestBonfirePropertyOfPropertyFuncReturnsBool*)(Game::ds2_base + 0x513600);
    FUN_14017e830 = (eventBonfireManagerUndefined4FuncReturnsByte*)(Game::ds2_base + 0x14017e830);
    heapAllocator = (intIntVtablePointerFuncReturnsPointer*)(Game::ds2_base + 0x833320);
    FUN_140833650 = (mapObjBonfireComponentFuncReturnsVoid*)(Game::ds2_base + 0x833650);
    FUN_14003d870 = (LocalizedStringInitializer*)(Game::ds2_base + 0x3d870);
    FUN_14003d880 = (localizedStringDemoCharacterCtrlUndefined4FuncReturnsDemoCharacterCtrl*)(Game::ds2_base + 0x3d880);
    FUN_14001dce0 = (DemoCharacterCtrlDemoCharacterCtrlUInt64UInt64FuncReturnsUndefined8*)(Game::ds2_base + 0x1dce0);
    FUN_140046bd0 = (voidFuncReturnsInt64*)(Game::ds2_base + 0x46bd0);
    FUN_1400d81e0 = (FeOperatorTestBonfirePropertyOfPropertyFuncReturnsByte*)(Game::ds2_base + 0xd81e0);
    FUN_140503620 = (RetrieveLocalizedString*)(Game::ds2_base + 0x503620);
    FUN_14002aad0 = (undefined8Undefined8Undefined8FuncReturnsUndefined8*)(Game::ds2_base + 0x2aad0);
    FUN_14002c580 = (undefined8StringFuncReturnsUndefined8*)(Game::ds2_base + 0x2c580);
    FUN_14002b170 = (BonfireMenuFuncReturnsBonfireMenu*)(Game::ds2_base + 0x2b170);
    FUN_14002b240 = (addOptionToBonfireMenu*)(Game::ds2_base + 0x2b240);
    FUN_14002c680 = (undefined8ByteFuncReturnsUndefined8*)(Game::ds2_base + 0x2c680);
    FUN_14002b3e0 = (undefined8DemoCharacterCtrlEventBonfireManagerFuncReturnsUndefined8*)(Game::ds2_base + 0x2b3e0);
    FUN_14002b670 = (undefined8Undefined8FuncReturnsUndefined8*)(Game::ds2_base + 0x2b670);
    FUN_1400268c0 = (undefined8CharFuncReturnsUndefined8*)(Game::ds2_base + 0x268c0);
    FUN_140028bb0 = (undefined8Undefined8Undefined8FuncReturnsVoid*)(Game::ds2_base + 0x28bb0);
    FUN_14002aed0 = (undefined4FuncReturnsVoid*)(Game::ds2_base + 0x2aed0);
    thunk_FUN_141b6b19f = (undefined8FuncReturnsVoid*)(Game::ds2_base + 0xc2c9e0);

    writeEventFlag = (WriteEventFlagFunction*)(Game::ds2_base + 0x4750b0);

    return true;
};

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved) {

    switch (fdwReason) {
        case (DLL_PROCESS_ATTACH): {
            DisableThreadLibraryCalls(hinstDLL);
            begin_thread = std::thread(Begin, (uint64_t)hinstDLL);
            break;
        };
        case (DLL_PROCESS_DETACH): {
            begin_thread.detach();
            FreeLibrary(hinst_dll);
            break;
        };
    }
    return TRUE;
}

// Define original dll export and call the original function
extern "C" __declspec(dllexport) HRESULT __cdecl DirectInput8Create(
    HINSTANCE hinst,
    DWORD dwVersion,
    REFIID riidltf,
    LPVOID * ppvOut,
    LPUNKNOWN punkOuter)
{
    return original_dinput8_create(hinst, dwVersion, riidltf, ppvOut, punkOuter);
}