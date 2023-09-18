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
    [DisplayName("Vector3Int_Min")]
    [Category("UnityEngine/Vector3Int")]
    [AddComponentMenu("Min(Vector3Int, Vector3Int)")]
    public sealed class Vector3Int_Min_Vector3Int_Vector3Int_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3Int lhs;
        public Megumin.Binding.RefVar_Vector3Int rhs;

        [Space]
        public Megumin.Binding.RefVar_Vector3Int SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Vector3Int.Min(lhs, rhs);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




