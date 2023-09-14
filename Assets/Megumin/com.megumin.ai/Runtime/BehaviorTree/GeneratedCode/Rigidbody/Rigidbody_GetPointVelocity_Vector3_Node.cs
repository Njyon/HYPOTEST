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
    [DisplayName("Rigidbody_GetPointVelocity")]
    [Category("UnityEngine/Rigidbody")]
    [AddComponentMenu("GetPointVelocity(Vector3)")]
    public sealed class Rigidbody_GetPointVelocity_Vector3_Node : BTActionNode<UnityEngine.Rigidbody>
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 worldPoint;

        [Space]
        public Megumin.Binding.RefVar_Vector3 SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.Rigidbody)MyAgent).GetPointVelocity(worldPoint);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




