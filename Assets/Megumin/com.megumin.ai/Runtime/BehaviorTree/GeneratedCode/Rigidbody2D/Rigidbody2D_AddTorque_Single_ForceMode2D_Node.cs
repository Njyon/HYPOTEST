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
    [Icon("d_Rigidbody2D Icon")]
    [DisplayName("Rigidbody2D_AddTorque")]
    [Category("UnityEngine/Rigidbody2D")]
    [AddComponentMenu("AddTorque(Single, ForceMode2D)")]
    public sealed class Rigidbody2D_AddTorque_Single_ForceMode2D_Node : BTActionNode<UnityEngine.Rigidbody2D>
    {
        [Space]
        public Megumin.Binding.RefVar_Float torque;
        public Megumin.Binding.RefVar<UnityEngine.ForceMode2D> mode;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Rigidbody2D)MyAgent).AddTorque(torque, mode);
            return Status.Succeeded;
        }
    }
}




