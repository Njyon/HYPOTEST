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
    [Icon("d_Animator Icon")]
    [DisplayName("Animator_SetIKPosition")]
    [Category("UnityEngine/Animator")]
    [AddComponentMenu("SetIKPosition(AvatarIKGoal, Vector3)")]
    public sealed class Animator_SetIKPosition_AvatarIKGoal_Vector3_Node : BTActionNode<UnityEngine.Animator>
    {
        [Space]
        public Megumin.Binding.RefVar<UnityEngine.AvatarIKGoal> goal;
        public Megumin.Binding.RefVar_Vector3 goalPosition;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Animator)MyAgent).SetIKPosition(goal, goalPosition);
            return Status.Succeeded;
        }
    }
}




