using System.Collections.Generic;
using Godot;

namespace HookyandSmoochus.game;

public enum PlayerID
{
	ONE, TWO,
}

public class GameManager
{
	public static GameManager I = new();

	public Dictionary<CharacterType, PlayerID> ActiveCharacters = new();
	
	public PlayerID GetPlayerIDByCharacterType(CharacterType type)
	{
		return ActiveCharacters[type];
	}
}
