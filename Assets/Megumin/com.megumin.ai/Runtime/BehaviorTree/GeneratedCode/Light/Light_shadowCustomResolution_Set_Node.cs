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
    [Icon("d_Light Icon")]
    [DisplayName("Set_Light_shadowCustomResolution")]
    [Category("UnityEngine/Light")]
    [AddComponentMenu("Set_shadowCustomResolution")]
    public sealed class Light_shadowCustomResolution_Set_Node : BTActionNode<UnityEngine.Light>
    {
        [Space]
        public Megumin.Binding.RefVar_Int Value;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Light)MyAgent).shadowCustomResolution = Value;
            return Status.Succeeded;
        }

    }
}




