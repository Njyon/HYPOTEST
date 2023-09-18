﻿///********************************************************************************************************************************
///本页代码由代码生成器生成，请勿手动修改。The code on this page is generated by the code generator, do not manually modify.
///生成器类型：Megumin.GameFramework.AI.BehaviorTree.Editor.NodeGenerator
///配置源文件：$(CodeGenericSourceFilePath)
///********************************************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Icon("d_Rigidbody Icon")]
    [DisplayName("Rigidbody_AddExplosionForce")]
    [Category("UnityEngine/Rigidbody")]
    [AddComponentMenu("AddExplosionForce(Single, Vector3, Single, Single, ForceMode)")]
    public sealed class Rigidbody_AddExplosionForce_Single_Vector3_Single_Single_ForceMode_Node : BTActionNode<UnityEngine.Rigidbody>
    {
        [Space]
        public Megumin.Binding.RefVar_Float explosionForce;
        public Megumin.Binding.RefVar_Vector3 explosionPosition;
        public Megumin.Binding.RefVar_Float explosionRadius;
        public Megumin.Binding.RefVar_Float upwardsModifier;
        public Megumin.Binding.RefVar<UnityEngine.ForceMode> mode;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Rigidbody)MyAgent).AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, mode);
            return Status.Succeeded;
        }
    }
}




