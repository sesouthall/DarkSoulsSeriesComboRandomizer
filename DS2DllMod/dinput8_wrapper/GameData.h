#ifndef _DS2S_BONFIRE_MENU_INJECTION_GAME_DATA_H_
	#define _DS2S_BONFIRE_MENU_INJECTION_GAME_DATA_H_

#include <stdint.h>

class Game
{
public:
	static uint64_t ds2_base;

	static void init();
};
#endif // !_DS2S_BONFIRE_MENU_INJECTION_GAME_DATA_H_

