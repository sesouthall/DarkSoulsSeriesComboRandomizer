#include "MinHook.h"
#include <vector>

namespace Hook
{
	struct HookInfo
	{
		enum HookType
		{
			kHookTypeMH,
			kHookTypeHLS,
			kHookTypeNum
		};

		const char* name;
		void** pTargetFn;
		void* pHookFn;
		void** pOriginalFn;
		HookType hookType;

		HookInfo(const char* name, void** pTargetFn, void* pHookFn, void** pOriginalFn, HookType type)
			: name(name),
			pTargetFn(pTargetFn),
			pHookFn(pHookFn),
			pOriginalFn(pOriginalFn),
			hookType(type)
		{
		}
	};

	class HookManager
	{
		static HookManager* s_instance;

		bool m_bInitialized = false;
		std::vector<HookInfo*> m_hooks;

		HookManager() = default;
		~HookManager();
	public:
		static HookManager* GetInstance();
		static void DestroyInstance();

		bool Initialize();
		bool Finalize();

		HookInfo* GetHookByName(const char* name);
		bool RemoveHook(const char* name);
		void RemoveAllHooks();

		template <typename T>
		bool Hookless(void** vfptr, size_t index, T pHookFn, T* pOriginalFn, const char* name)
		{
			void** pTarget = &vfptr[index];

			if (!m_bInitialized)
			{
				printf("[HLS] Hook %s failed. HookManager not initialized.\n", name);
				return false;
			}

			DWORD dOldProtect;

			if (!VirtualProtect(pTarget, 8, PAGE_READWRITE, &dOldProtect))
			{
				printf("[HLS] Hook %s failed.\n", name);
				return false;
			}

			*pOriginalFn = static_cast<T>(*pTarget);
			*pTarget = pHookFn;

			printf("[HLS] Hook %s succeeded.\n", name);

			if (VirtualProtect(pTarget, 8, dOldProtect, &dOldProtect))
			{
				m_hooks.push_back(new HookInfo(name, pTarget, reinterpret_cast<void*>(pHookFn), reinterpret_cast<void**>(pOriginalFn), HookInfo::kHookTypeHLS));
				return true;
			}

			printf("[HLS] Hook %s failed.\n", name);
			return false;
		}

		template <typename T>
		bool CreateHook(uint64_t targetAddress, T* pTargetFn, T pHookFn, T* pOriginalFn, const char* hookName)
		{
			if (!m_bInitialized)
			{
				printf("[MH] Hook %s creation failed. HookManager not initialized.\n", hookName);
				return false;
			}

			*pTargetFn = reinterpret_cast<T>(targetAddress);

			if (MH_CreateHook((*pTargetFn), pHookFn, (LPVOID*)(pOriginalFn)) != MH_OK) {
				printf("[MH] Hook %s creation failed.\n", hookName);
				return false;
			}
			else {
				printf("[MH] Hook %s creation succeeded\n", hookName);
			}

			if (MH_EnableHook(reinterpret_cast<void*>(*pTargetFn)) != MH_OK) {
				printf("[MH] Hook %s enabling failed\n", hookName);
				return false;
			}
			else {
				printf("[MH] Hook %s enabled\n", hookName);
				m_hooks.push_back(new HookInfo(hookName, reinterpret_cast<void**>(pTargetFn), reinterpret_cast<void*>(pHookFn), reinterpret_cast<void**>(pOriginalFn), HookInfo::kHookTypeMH));
			}

			return true;
		}
	};
}