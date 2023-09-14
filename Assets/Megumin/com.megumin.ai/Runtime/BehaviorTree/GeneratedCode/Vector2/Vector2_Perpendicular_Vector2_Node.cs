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
    [DisplayName("Vector2_Perpendicular")]
    [Category("UnityEngine/Vector2")]
    [AddComponentMenu("Perpendicular(Vector2)")]
    public sealed class Vector2_Perpendicular_Vector2_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Vector2 inDirection;

        [Space]
        public Megumin.Binding.RefVar_Vector2 SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Vector2.Perpendicular(inDirection);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




