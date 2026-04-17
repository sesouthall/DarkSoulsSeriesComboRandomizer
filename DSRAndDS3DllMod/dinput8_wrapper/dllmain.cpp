#include <windows.h>
#include <thread>
#include <iostream>
#include <functional>
#include <format>
#include <map>
#include "dearxan/include/dearxan.h"
#include "SP/memory/injection/asm/x64.h"
#include "SP/memory/aob_scan.h"
#include "sp/memory/pointer.h"
#include "GameData.h"
//#include "DS2Reimplementations.h"
#ifdef DSR
#include "DSRReimplementations.h"
#endif // DSR
#ifdef DS3
#include "DS3Reimplementations.h"
#endif // DS3

#include "BonfireTableParser.h"
#include "HookManager.inl"
#include <chrono>
#include <format>
#include <thread>
using namespace std;
using namespace std::chrono_literals;

HINSTANCE hinst_dll = 0;
std::thread begin_thread;

extern "C" UINT_PTR directinput_create_proc = 0;
extern "C" __declspec(dllexport) HRESULT __cdecl DirectInput8Create(HINSTANCE hinst, DWORD dwVersion, REFIID riidltf, LPVOID * ppvOut, LPUNKNOWN punkOuter);
extern "C" void custom_texture_load();
extern uint64_t ContinueTextureLoadingAtAddress;
extern uint64_t AtomicIncrementAddress;
extern uint64_t FUN_140e60160Address;
extern uint64_t FUN_140e5f650Address;
extern uint64_t DAT_144799990Address;
static decltype(&DirectInput8Create) original_dinput8_create;

#ifdef DSR
const int EzStateTalkEventQueryFunctionOffset = 0x4d8ab0;
const int EzStateTalkEventCommandFunctionOffset = 0x4db340;
#endif
#ifdef DS3
const int EzStateTalkEventQueryFunctionOffset = 0xeff690;
const int EzStateTalkEventCommandFunctionOffset = 0xf035c0;
#endif // DS3

BonfireTable* parsedMappings;
HANDLE hPipe = INVALID_HANDLE_VALUE;
ChrClassWarp** warpInfo;

EzStateTalkQueryFunction* OriginalEzStateTalkEventQueryFunction;
EzStateTalkQueryFunction* OriginalEzStateTalkEventQueryFunctionTrampoline;

EzStateTalkCommandFunction* OriginalEzStateTalkEventCommandFunction;
EzStateTalkCommandFunction* OriginalEzStateTalkEventCommandFunctionTrampoline;

void InsertJMP(BYTE* address, uint64_t jumpTo, DWORD length)
{
    DWORD oldProtect, backup, relativeAddress;

    VirtualProtect(address, length, PAGE_EXECUTE_READWRITE, &oldProtect);

    // Store jumpTo in RAX
    *address = 0x48;
    *(address + 0x1) = 0xB8;
    *((uint64_t*)(address + 0x2)) = jumpTo;

    // Call RAX
    *(address + 0xA) = 0xFF;
    *(address + 0xB) = 0xD0;

    for (DWORD x = 0xC; x < length; x++)
    {
        *(address + x) = 0x90;
    }

    VirtualProtect(address, length, oldProtect, &backup);
}

enum QueryId
{
    ShouldHaveOtherGameWarps = 0x80,
    GetDS1BonfireId = 0x81,
    GetDS2BonfireId = 0x82,
    GetDS3BonfireId = 0x83
};

enum CommandId
{
    WarpToDS1 = 0x80,
    WarpToDS2 = 0x81,
    WarpToDS3 = 0x82
};

void AdditionalEzStateTalkEventsQueryFunctions(int64_t param_1, float* param_2, EzStateEnvironmentQueryImpl* param_3)
{
    int queryId = (*(GetQueryIdFunction**)(*(int64_t*)param_3 + 0x8))(param_3);
    //std::this_thread::sleep_for(30s);

    if (queryId >= QueryId::ShouldHaveOtherGameWarps && queryId <= QueryId::GetDS3BonfireId)
    {
        printf_s("QueryId is: %d\n", queryId);
        bool currentBonfireIsInMappings = false;
        BonfireTriple* mapping = BonfireTriple::nullMapping;
#ifdef DSR
        if (warpInfo != NULL && *warpInfo != NULL && (*warpInfo)->LastBonfire != NULL && parsedMappings->ContainsDS1BonfireId((*warpInfo)->LastBonfire))
        {
            currentBonfireIsInMappings = true;
            mapping = parsedMappings->GetByDS1BonfireId((*warpInfo)->LastBonfire);
        }
#endif // DSR
#ifdef DS3
        if (warpInfo != NULL && *warpInfo != NULL && (*warpInfo)->LastBonfire != NULL && parsedMappings->ContainsDS3BonfireId((*warpInfo)->LastBonfire))
        {
            currentBonfireIsInMappings = true;
            mapping = parsedMappings->GetByDS3BonfireId((*warpInfo)->LastBonfire);
        }
#endif // DS3

        switch (queryId)
        {
        case QueryId::ShouldHaveOtherGameWarps:
            *param_2 = currentBonfireIsInMappings;
            *(int64_t*)((int64_t)param_2 + 0x8) = 2;
            break;
        case QueryId::GetDS1BonfireId:
            printf_s("Returning %d\n", mapping->DS1BonfireId);
            *param_2 = mapping->DS1BonfireId;
            *(int64_t*)((int64_t)param_2 + 0x8) = 1;
            break;
        case QueryId::GetDS2BonfireId:
            printf_s("Returning %d\n", mapping->DS2BonfireId);
            *param_2 = mapping->DS2BonfireId;
            *(int64_t*)((int64_t)param_2 + 0x8) = 1;
            break;
        case QueryId::GetDS3BonfireId:
            printf_s("Returning %d\n", mapping->DS3BonfireId);
            *param_2 = mapping->DS3BonfireId;
            *(int64_t*)((int64_t)param_2 + 0x8) = 1;
            break;
        }
    }
    else
    {
        OriginalEzStateTalkEventQueryFunctionTrampoline(param_1, param_2, param_3);
    }
}

#ifdef DSR
void AdditionalEzStateTalkEventsCommandFunctions(int64_t param_1, EzStateExternalEventTemp* param_2)
#endif // DSR
#ifdef DS3
void AdditionalEzStateTalkEventsCommandFunctions(int64_t param_1, EzStateExternalEventTemp* param_2, int64_t param_3, char* param_4)
#endif // DS3
{
#ifdef DSR
    int getCommandIdFunctionOffset = 0x8;
#endif // DSR
#ifdef DS3
    int getCommandIdFunctionOffset = 0x10;
#endif // DS3
    int commandId = (*(GetCommandIdFunction**)(*(int64_t*)param_2 + getCommandIdFunctionOffset))(param_2);

    printf_s("CommandId is: %d\n", commandId);
    if (commandId >= CommandId::WarpToDS1 && commandId <= CommandId::WarpToDS3)
    {
        BonfireTriple* mapping = BonfireTriple::nullMapping;
#ifdef DSR
        if (warpInfo != NULL && *warpInfo != NULL && (*warpInfo)->LastBonfire != NULL && parsedMappings->ContainsDS1BonfireId((*warpInfo)->LastBonfire))
        {
            mapping = parsedMappings->GetByDS1BonfireId((*warpInfo)->LastBonfire);
        }
#endif // DSR
#ifdef DS3
        if (warpInfo != NULL && *warpInfo != NULL && (*warpInfo)->LastBonfire != NULL && parsedMappings->ContainsDS3BonfireId((*warpInfo)->LastBonfire))
        {
            mapping = parsedMappings->GetByDS3BonfireId((*warpInfo)->LastBonfire);
        }
#endif // DS3

        if (mapping == NULL)
        {
            return;
        }

        std::string message;
        
        switch (commandId)
        {
        case CommandId::WarpToDS1:
            message = std::format("{}\n", mapping->DS1BonfireId);
            break;
        case CommandId::WarpToDS2:
            message = std::format("{}\n", mapping->DS2BonfireId);
            break;
        case CommandId::WarpToDS3:
            message = std::format("{}\n", mapping->DS3BonfireId);
            break;
        }

        DWORD length, written;
        length = (message.length() + 1) * sizeof(TCHAR);
        WriteFile(hPipe, message.c_str(), length, &written, NULL);
        FlushFileBuffers(hPipe);
        printf_s("Wrote %d/%d characters from '%s'", written, length, message.c_str());
    }
    else
    {
#ifdef DSR
        OriginalEzStateTalkEventCommandFunctionTrampoline(param_1, param_2);
#endif // DSR
#ifdef DS3
        OriginalEzStateTalkEventCommandFunctionTrampoline(param_1, param_2, param_3, param_4);
#endif // DS3
    }
}

bool Begin(uint64_t qModuleHandle) 
{
    char dllpath[MAX_PATH];
    GetSystemDirectoryA(dllpath, MAX_PATH);
    strcat_s(dllpath, "\\dinput8.dll");
    hinst_dll = LoadLibraryA(dllpath);

    if (!hinst_dll) {
        MessageBoxA(NULL, "Failed to load original DLL", "Error", MB_ICONERROR);
        return false;
    };

    original_dinput8_create = (decltype(&DirectInput8Create))GetProcAddress(hinst_dll, "DirectInput8Create");
    if (!original_dinput8_create) {
        MessageBoxA(NULL, "Failed to load original DLL", "Error", MB_ICONERROR);
        return false;
    }

    // Uncomment these lines to create a console and allow console output for debugging
    AllocConsole();
    freopen_s((FILE**)stdout, "CONOUT$", "w", stdout);
    printf_s("Working fine so far\n");

    Game::init();

#ifdef DSR
    void* warpInfo_sp = sp::mem::aob_scan("48 8B 05 xx xx xx xx 0F 28 01 66 0F 7F 80 xx xx 00 00 C6 80");
    warpInfo = sp::mem::pointer<ChrClassWarp*>((void*)((uint64_t)warpInfo_sp + *(uint32_t*)((uint64_t)warpInfo_sp + 3) + 7)).resolve();
#endif // DSR
#ifdef DS3
    void* warpInfo_sp = sp::mem::aob_scan("48 8B xx xx xx xx 04 89 48 28 C3");
    warpInfo = sp::mem::pointer<ChrClassWarp*>((void*)((uint64_t)warpInfo_sp + *(uint32_t*)((uint64_t)warpInfo_sp + 3) + 7)).resolve();
    ContinueTextureLoadingAtAddress = Game::base_address + 0xe5e480;
    AtomicIncrementAddress = Game::base_address + 0x17a35d0;
    FUN_140e60160Address = Game::base_address + 0xe60160;
    FUN_140e5f650Address = Game::base_address + 0xe5f650;
    DAT_144799990Address = Game::base_address + 0x4799990;
    //InsertJMP((BYTE*)(Game::base_address + 0xe5e47b), (int64_t)custom_texture_load, 13);
#endif // DS3

    char mappingFile[MAX_PATH] = "%LOCALAPPDATA%\\DarkSoulsSeriesComboRandomizer\\BonfireMappings.txt";
    DoEnvironmentSubstA(mappingFile, MAX_PATH);
    parsedMappings = ParseBonfireTable(mappingFile);

#ifdef DSR
    LPCWSTR pipeName = TEXT("\\\\.\\pipe\\DarkSoulsSeriesComboRandomizerDS1");
#endif // DSR
#ifdef DS3
    LPCWSTR pipeName = TEXT("\\\\.\\pipe\\DarkSoulsSeriesComboRandomizerDS3");
#endif // DS3

    hPipe = CreateFile(
        pipeName,
        GENERIC_WRITE,
        0,
        NULL,
        OPEN_EXISTING,
        0,
        NULL);

    Hook::HookManager* pHookManager = Hook::HookManager::GetInstance();
    pHookManager->Initialize();
    pHookManager->CreateHook<EzStateTalkQueryFunction*>(Game::base_address + EzStateTalkEventQueryFunctionOffset, &OriginalEzStateTalkEventQueryFunction, &AdditionalEzStateTalkEventsQueryFunctions, &OriginalEzStateTalkEventQueryFunctionTrampoline, "EzStateTalkEventQueryFunction");
    pHookManager->CreateHook<EzStateTalkCommandFunction*>(Game::base_address + EzStateTalkEventCommandFunctionOffset, &OriginalEzStateTalkEventCommandFunction, &AdditionalEzStateTalkEventsCommandFunctions, &OriginalEzStateTalkEventCommandFunctionTrampoline, "EzStateTalkEventCommandFunction");

    return true;
};

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved) {

    switch (fdwReason) {
        case (DLL_PROCESS_ATTACH): {
#ifdef DSR
            dearxan::neuter_arxan([](const dearxan::DearxanResult& result)
                {
                    if (result.status() == dearxan::DearxanStatus::DearxanSuccess)
                    {
                        printf_s("Arxan disabled\n");
                    }
                    else
                    {
                        printf_s("Failed to disable Arxan! Error: %s\n", result.error_msg().c_str());
                    }
                });
#endif // DSR
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