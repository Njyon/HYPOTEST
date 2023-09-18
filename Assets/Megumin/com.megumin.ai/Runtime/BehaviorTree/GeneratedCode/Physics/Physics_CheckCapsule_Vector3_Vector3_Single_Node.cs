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
    [DisplayName("Physics_CheckCapsule")]
    [Category("UnityEngine/Physics")]
    [AddComponentMenu("CheckCapsule(Vector3, Vector3, Single)")]
    public sealed class Physics_CheckCapsule_Vector3_Vector3_Single_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 start;
        public Megumin.Binding.RefVar_Vector3 end;
        public Megumin.Binding.RefVar_Float radius;

        [Space]
        public Megumin.Binding.RefVar_Bool SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Physics.CheckCapsule(start, end, radius);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




