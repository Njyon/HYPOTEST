using Megumin.Binding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RefVar_GameCharacter : RefVar<GameCharacter> { }

public class VariableCreator_GameCharacter : VariableCreator
{
	public override string Name { get; set; } = "GameCharacter";

	public override IRefable Create()
	{
		return new RefVar_GameCharacter() { RefName = "GameCharacter" };
	}
}

[Serializable]
public class RefVar_GameCharacter_List : RefVar<List<GameCharacter>> { }

public class VariableCreator_GameCharacter_List : VariableCreator
{
	public override string Name { get; set; } = "List/GameCharacter";

	public override IRefable Create()
	{
		return new RefVar_GameCharacter_List() { RefName = "List<GameCharacter>", value = new() };
	}
}

[Serializable]
public class RefVar_GameCharacter_Array : RefVar<GameCharacter[]> { }

public class VariableCreator_GameCharacter_Array : VariableCreator
{
	public override string Name { get; set; } = "Array/GameCharacter";

	public override IRefable Create()
	{
		return new RefVar_GameCharacter_Array() { RefName = "Array<GameCharacter>" };
	}
}

[Serializable]
public class RefVar_PlayerGameCharacter : RefVar<PlayerGameCharacter> { }

public class VariableCreator_PlayerGameCharacter : VariableCreator
{
	public override string Name { get; set; } = "PlayerGameCharacter";

	public override IRefable Create()
	{
		return new RefVar_PlayerGameCharacter() { RefName = "PlayerGameCharacter" };
	}
}

[Serializable]
public class RefVar_PlayerGameCharacter_List : RefVar<List<PlayerGameCharacter>> { }

public class VariableCreator_PlayerGameCharacter_List : VariableCreator
{
	public override string Name { get; set; } = "List/PlayerGameCharacter";

	public override IRefable Create()
	{
		return new RefVar_PlayerGameCharacter_List() { RefName = "List<PlayerGameCharacter>", value = new() };
	}
}

[Serializable]
public class RefVar_PlayerGameCharacter_Array : RefVar<GameCharacter[]> { }

public class VariableCreator_PlayerGameCharacter_Array : VariableCreator
{
	public override string Name { get; set; } = "Array/PlayerGameCharacter";

	public override IRefable Create()
	{
		return new RefVar_PlayerGameCharacter_Array() { RefName = "Array<PlayerGameCharacter>" };
	}
}
