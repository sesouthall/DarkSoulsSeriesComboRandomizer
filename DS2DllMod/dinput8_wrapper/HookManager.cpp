#include "HookManager.inl"
#include "MinHook.h"
#include <xstring>
using namespace std;

namespace Hook
{
	HookManager* HookManager::s_instance = nullptr;

	HookManager::~HookManager()
	{
		this->Finalize();
	}

	HookManager* HookManager::GetInstance()
	{
		if (s_instance == nullptr)
			s_instance = new HookManager();

		return s_instance;
	}

	void HookManager::DestroyInstance()
	{
		if (s_instance)
		{
			delete s_instance;
			s_instance = nullptr;
		}
	}

	bool HookManager::Initialize()
	{
		if (m_bInitialized)
			return false;

		MH_STATUS status = MH_Initialize();
		if (status != MH_OK)
		{
			std::string sStatus = MH_StatusToString(status);
			printf("Minhook init failed (%s).\n", sStatus);
			return false;
		}
		else
			printf("MinHook init succeeded.\n");

		m_bInitialized = true;

		return true;
	}

	bool HookManager::Finalize()
	{
		if (!m_bInitialized)
			return false;

		RemoveAllHooks();

		MH_STATUS status = MH_Uninitialize();
		if (status != MH_OK)
		{
			std::string sStatus = MH_StatusToString(status);
			printf("Minhook uninit failed (%s).\n", sStatus);
			return false;
		}
		else
			printf("MinHook init succeeded.\n");

		m_bInitialized = false;

		return true;
	}

	HookInfo* HookManager::GetHookByName(const char* name)
	{
		for (size_t i = 0; i < m_hooks.size(); i++)
		{
			if (strcmp(m_hooks[i]->name, name) == 0)
				return m_hooks[i];
		}

		return nullptr;
	}

	bool HookManager::RemoveHook(const char* name)
	{
		HookInfo* pHook = GetHookByName(name);

		if (pHook == nullptr)
			return false;

		if (pHook->hookType == HookInfo::kHookTypeMH)
		{
			if (MH_DisableHook(reinterpret_cast<void*>(*pHook->pTargetFn)) != MH_OK) {
				printf("[MH] Hook %s disabling failed\n", name);
				return false;
			}
			else {
				printf("[MH] Hook %s disabled\n", name);
			}

			if (MH_RemoveHook(reinterpret_cast<void*>(*pHook->pTargetFn)) != MH_OK) {
				printf("[MH] Hook %s removal failed\n", name);
				return false;
			}
			else {
				printf("[MH] Hook %s removed\n", name);
			}
		}
		else if (pHook->hookType == HookInfo::kHookTypeHLS)
		{
			DWORD dOldProtect;

			if (!VirtualProtect(pHook->pTargetFn, 8, PAGE_READWRITE, &dOldProtect))
			{
				printf("[HLS] Unhook %s failed.\n", name);
				return false;
			}

			*(pHook->pTargetFn) = *(pHook->pOriginalFn);
			printf("[HLS] Unhook %s succeeded.\n", name);

			if (!VirtualProtect(pHook->pTargetFn, 8, dOldProtect, &dOldProtect))
			{
				printf("[HLS] Unhook %s failed.\n", name);
				return false;
			}
		}

		auto it = std::find(m_hooks.begin(), m_hooks.end(), pHook);

		if (it != m_hooks.end())
			m_hooks.erase(it);

		delete pHook;

		return true;
	}

	void HookManager::RemoveAllHooks()
	{
		while (m_hooks.size() > 0)
			RemoveHook(m_hooks[0]->name);
	}
}
