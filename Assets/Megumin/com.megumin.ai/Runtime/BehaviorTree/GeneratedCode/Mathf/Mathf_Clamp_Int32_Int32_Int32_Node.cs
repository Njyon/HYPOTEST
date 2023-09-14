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
    [Icon("d_collabcreate icon")]
    [DisplayName("Mathf_Clamp")]
    [Category("UnityEngine/Mathf")]
    [AddComponentMenu("Clamp(Int32, Int32, Int32)")]
    public sealed class Mathf_Clamp_Int32_Int32_Int32_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Int value;
        public Megumin.Binding.RefVar_Int min;
        public Megumin.Binding.RefVar_Int max;

        [Space]
        public Megumin.Binding.RefVar_Int SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Mathf.Clamp(value, min, max);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




