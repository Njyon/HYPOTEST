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
    [DisplayName("Vector3_ToString")]
    [Category("UnityEngine/Vector3")]
    [AddComponentMenu("ToString")]
    public sealed class Vector3_ToString_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 MyAgent;

        [Space]
        public Megumin.Binding.RefVar_String SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.Vector3)MyAgent).ToString();

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




