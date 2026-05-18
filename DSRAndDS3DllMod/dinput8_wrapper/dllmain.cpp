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
//extern "C" void custom_texture_load();
//extern uint64_t ContinueTextureLoadingAtAddress;
//extern uint64_t AtomicIncrementAddress;
//extern uint64_t FUN_140e60160Address;
//extern uint64_t FUN_140e5f650Address;
//extern uint64_t DAT_144799990Address;
static decltype(&DirectInput8Create) original_dinput8_create;

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
    GetDS3BonfireId = 0x83,
    ShouldHaveDS1Warp = 0x84,
    ShouldHaveDS2Warp = 0x85,
    ShouldHaveDS3Warp = 0x86
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
        bool currentBonfireIsInMappings = false;
        BonfireTriple* mapping = BonfireTriple::nullMapping;
#ifdef DSR
        if (warpInfo == NULL || *warpInfo == NULL)
        {
            printf_s("malformed warp info\n");
        }
        else if ((*warpInfo)->LastBonfire == NULL)
        {
            printf_s("no last bonfire\n");
        }
        else if (!parsedMappings->ContainsDS1BonfireId((*warpInfo)->LastBonfire))
        {
            printf_s("%d isn't in warp list\n", (*warpInfo)->LastBonfire);
        }
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
            printf_s("ShouldHaveOtherGameWarps returning %d\n", currentBonfireIsInMappings);
            *param_2 = currentBonfireIsInMappings;
            *(int64_t*)((int64_t)param_2 + 0x8) = 2;
            break;
        case QueryId::GetDS1BonfireId:
            printf_s("Get DS1BonfireId returning %d\n", mapping->DS1BonfireId);
            *param_2 = mapping->DS1BonfireId;
            *(int64_t*)((int64_t)param_2 + 0x8) = 1;
            break;
        case QueryId::GetDS2BonfireId:
            printf_s("GetDS2BonfireId returning %d\n", mapping->DS2BonfireId);
            *param_2 = mapping->DS2BonfireId;
            *(int64_t*)((int64_t)param_2 + 0x8) = 1;
            break;
        case QueryId::GetDS3BonfireId:
            printf_s("GetDS3BonfireId returning %d\n", mapping->DS3BonfireId);
            *param_2 = mapping->DS3BonfireId;
            *(int64_t*)((int64_t)param_2 + 0x8) = 1;
            break;
        case QueryId::ShouldHaveDS1Warp:
            printf_s("ShouldHaveOtherGameWarps returning %d\n", currentBonfireIsInMappings && mapping->DS1BonfireId != -1);
            *param_2 = currentBonfireIsInMappings && mapping->DS1BonfireId != -1;
            *(int64_t*)((int64_t)param_2 + 0x8) = 2;
            break;
        case QueryId::ShouldHaveDS2Warp:
            printf_s("ShouldHaveOtherGameWarps returning %d\n", currentBonfireIsInMappings && mapping->DS2BonfireId != -1);
            *param_2 = currentBonfireIsInMappings && mapping->DS2BonfireId != -1;
            *(int64_t*)((int64_t)param_2 + 0x8) = 2;
            break;
        case QueryId::ShouldHaveDS3Warp:
            printf_s("ShouldHaveOtherGameWarps returning %d\n", currentBonfireIsInMappings && mapping->DS3BonfireId != -1);
            *param_2 = currentBonfireIsInMappings && mapping->DS3BonfireId != -1;
            *(int64_t*)((int64_t)param_2 + 0x8) = 2;
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
        else if (warpInfo == NULL)
        {
            printf_s("Could not find pointer to warp info\n");
        }
        else if (*warpInfo == NULL)
        {
            printf_s("Warp info object is not initialized\n");
        }
        else
        {
            printf_s("Last bonfire (%d) is not found in bonfire mappings\n", (*warpInfo)->LastBonfire);
        }

        if (mapping == NULL)
        {
            return;
        }

        std::string message;
        
        switch (commandId)
        {
        case CommandId::WarpToDS1:
            printf_s("Warping to %s in DS1 with id %d\n", mapping->DS1BonfireName.c_str(), mapping->DS1BonfireId);
            message = std::format("{}\n", mapping->DS1BonfireId);
            break;
        case CommandId::WarpToDS2:
            printf_s("Warping to %s in DS2 with id %d\n", mapping->DS2BonfireName.c_str(), mapping->DS2BonfireId);
            message = std::format("{}\n", mapping->DS2BonfireId);
            break;
        case CommandId::WarpToDS3:
            printf_s("Warping to %s in DS3 with id %d\n", mapping->DS3BonfireName.c_str(), mapping->DS3BonfireId);
            message = std::format("{}\n", mapping->DS3BonfireId);
            break;
        }

        DWORD length, written;
        length = (message.length() + 1) * sizeof(TCHAR);
        printf_s("Pipe handle is %p\n", hPipe);
        WriteFile(hPipe, message.c_str(), length, &written, NULL);
        FlushFileBuffers(hPipe);
        printf_s("Wrote %d/%d characters from '%s'\n", written, length, message.c_str());
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
        MessageBoxA(NULL, "Failed to load original DLL\n", "Error", MB_ICONERROR);
        return false;
    };

    original_dinput8_create = (decltype(&DirectInput8Create))GetProcAddress(hinst_dll, "DirectInput8Create");
    if (!original_dinput8_create) {
        MessageBoxA(NULL, "Failed to load original DLL\n", "Error", MB_ICONERROR);
        return false;
    }

    printf_s("Working fine so far\n");

    Game::init();

#ifdef DSR
    void* warpInfo_sp = sp::mem::aob_scan("48 8B 05 xx xx xx xx 0F 28 01 66 0F 7F 80 xx xx 00 00 C6 80");
    warpInfo = sp::mem::pointer<ChrClassWarp*>((void*)((uint64_t)warpInfo_sp + *(uint32_t*)((uint64_t)warpInfo_sp + 3) + 7)).resolve();
#endif // DSR
#ifdef DS3
    void* warpInfo_sp = sp::mem::aob_scan("48 8B xx xx xx xx 04 89 48 28 C3");
    warpInfo = sp::mem::pointer<ChrClassWarp*>((void*)((uint64_t)warpInfo_sp + *(uint32_t*)((uint64_t)warpInfo_sp + 3) + 7)).resolve();
    void* talkEventQueryFunction = sp::mem::aob_scan("48 8B C4 55 56 57 41 54 41 55 41 56 41 57 48 8D A8 68 FC FF FF");
    int EzStateTalkEventQueryFunctionOffset = (uint64_t)talkEventQueryFunction - Game::base_address;
    void* talkEventCommandFunction = sp::mem::aob_scan("48 8B C4 55 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24 90 48 81 EC 70 01 00 00 48 C7 45 88 FE FF FF FF 48 89 58 18");
    int EzStateTalkEventCommandFunctionOffset = (uint64_t)talkEventCommandFunction - Game::base_address;
    /*ContinueTextureLoadingAtAddress = Game::base_address + 0xe5e480;
    AtomicIncrementAddress = Game::base_address + 0x17a35d0;
    FUN_140e60160Address = Game::base_address + 0xe60160;
    FUN_140e5f650Address = Game::base_address + 0xe5f650;
    DAT_144799990Address = Game::base_address + 0x4799990;*/
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

#ifdef DS3
    Hook::HookManager* pHookManager = Hook::HookManager::GetInstance();
    pHookManager->Initialize();
    pHookManager->CreateHook<EzStateTalkQueryFunction*>(Game::base_address + EzStateTalkEventQueryFunctionOffset, &OriginalEzStateTalkEventQueryFunction, &AdditionalEzStateTalkEventsQueryFunctions, &OriginalEzStateTalkEventQueryFunctionTrampoline, "EzStateTalkEventQueryFunction");
    pHookManager->CreateHook<EzStateTalkCommandFunction*>(Game::base_address + EzStateTalkEventCommandFunctionOffset, &OriginalEzStateTalkEventCommandFunction, &AdditionalEzStateTalkEventsCommandFunctions, &OriginalEzStateTalkEventCommandFunctionTrampoline, "EzStateTalkEventCommandFunction");
#endif

    return true;
};

// Network block code copied from Katalash's ModEngine
typedef int(__stdcall* WSASTARTUP)(WORD, void*);

WSASTARTUP fpWsaStartup = NULL;

// block windows sockets from ever being initialized
INT __stdcall tWSAStartup(WORD wVersionRequested, void* lpWSAData)
{
    return 10091L; // WSASYSNOTREADY
}

bool BlockNetworkConnection()
{
    if (MH_CreateHookApi(L"ws2_32", "WSAStartup", &tWSAStartup, reinterpret_cast<LPVOID*>(&fpWsaStartup)) != MH_OK)
    {
        printf_s("Failed to create network block hook\n");
        return false;
    }

    if (MH_EnableHook((LPVOID)GetProcAddress(GetModuleHandleW(L"ws2_32"), "WSAStartup")) != MH_OK)
    {
        printf_s("Failed to enable network block hook\n");
        return false;
    }

    printf_s("WSAStartup blocked\n");
    return true;
}

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved) {

    switch (fdwReason) {
        case (DLL_PROCESS_ATTACH): {
            // Uncomment the two lines below to enable debug logging
            AllocConsole();
            freopen_s((FILE**)stdout, "CONOUT$", "w", stdout);
#ifdef DSR
            dearxan::neuter_arxan([](const dearxan::DearxanResult& result)
                {
                    if (result.status() == dearxan::DearxanStatus::DearxanSuccess)
                    {
                        printf_s("Arxan disabled\n");
                        int EzStateTalkEventQueryFunctionOffset = 0x4d8ab0;
                        int EzStateTalkEventCommandFunctionOffset = 0x4db340;
                        Hook::HookManager* pHookManager = Hook::HookManager::GetInstance();
                        pHookManager->Initialize();
                        pHookManager->CreateHook<EzStateTalkQueryFunction*>(Game::base_address + EzStateTalkEventQueryFunctionOffset, &OriginalEzStateTalkEventQueryFunction, &AdditionalEzStateTalkEventsQueryFunctions, &OriginalEzStateTalkEventQueryFunctionTrampoline, "EzStateTalkEventQueryFunction");
                        pHookManager->CreateHook<EzStateTalkCommandFunction*>(Game::base_address + EzStateTalkEventCommandFunctionOffset, &OriginalEzStateTalkEventCommandFunction, &AdditionalEzStateTalkEventsCommandFunctions, &OriginalEzStateTalkEventCommandFunctionTrampoline, "EzStateTalkEventCommandFunction");
                    }
                    else
                    {
                        printf_s("Failed to disable Arxan! Error: %s\n", result.error_msg().c_str());
                    }
                });

            Hook::HookManager::GetInstance()->Initialize();

            printf_s("Attempting to block network\n");
            if (!BlockNetworkConnection())
            {
                printf_s("Failed to block network\n");
            }
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