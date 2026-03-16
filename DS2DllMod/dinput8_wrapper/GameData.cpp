#include "GameData.h"
#include "sp/memory/injection/asm/x64.h"

uint64_t Game::ds2_base = NULL;

void Game::init()
{
	Game::ds2_base = (uint64_t)sp::mem::get_process_base();
}