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
    [DisplayName("Set_Physics_defaultContactOffset")]
    [Category("UnityEngine/Physics")]
    [AddComponentMenu("Set_defaultContactOffset")]
    public sealed class Physics_defaultContactOffset_Set_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Float Value;

        protected override Status OnTick(BTNode from, object options = null)
        {
            UnityEngine.Physics.defaultContactOffset = Value;
            return Status.Succeeded;
        }

    }
}




