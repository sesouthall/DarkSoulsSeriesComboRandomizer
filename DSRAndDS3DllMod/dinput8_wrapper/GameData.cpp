#include "GameData.h"
#include "sp/memory/injection/asm/x64.h"

uint64_t Game::base_address = NULL;

void Game::init()
{
	Game::base_address = (uint64_t)sp::mem::get_process_base();
}