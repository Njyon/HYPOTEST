using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistWeapon : WeaponBase
{
    public FistWeapon() { }
	public FistWeapon(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{ }

    public override void EquipWeapon()
    {
        base.EquipWeapon();
    }

    public override void UnEquipWeapon()
    {
        base.UnEquipWeapon();
	}

    public override void UpdateWeapon(float deltaTime)
    {
    
    }

    public override void GroundAttack()   
    {
    
    }
    public override void GroundUpAttack()    
    {
    
    }
    public override void GroundDownAttack()  
    {
    
    }
    public override void GroundDirectionAttack()   
    {
    
    }

    public override void AirAttack()  
    {
    
    }
    public override void AirUpAttack()
    {
    
    }
    public override void AirDownAttack()  
    {
    
    }
    public override void AirDirectionAttack() 
    {
    
    }
}