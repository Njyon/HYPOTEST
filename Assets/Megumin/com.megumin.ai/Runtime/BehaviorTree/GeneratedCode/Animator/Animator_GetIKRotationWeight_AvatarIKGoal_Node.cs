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
    [DisplayName("Animator_GetIKRotationWeight")]
    [Category("UnityEngine/Animator")]
    [AddComponentMenu("GetIKRotationWeight(AvatarIKGoal)")]
    public sealed class Animator_GetIKRotationWeight_AvatarIKGoal_Node : BTActionNode<UnityEngine.Animator>
    {
        [Space]
        public Megumin.Binding.RefVar<UnityEngine.AvatarIKGoal> goal;

        [Space]
        public Megumin.Binding.RefVar_Float SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.Animator)MyAgent).GetIKRotationWeight(goal);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




