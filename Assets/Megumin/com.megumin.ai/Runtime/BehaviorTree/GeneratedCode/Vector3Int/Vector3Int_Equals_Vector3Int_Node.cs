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
    [DisplayName("Vector3Int_Equals")]
    [Category("UnityEngine/Vector3Int")]
    [AddComponentMenu("Equals(Vector3Int)")]
    public sealed class Vector3Int_Equals_Vector3Int_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3Int MyAgent;

        [Space]
        public Megumin.Binding.RefVar_Vector3Int other;

        [Space]
        public Megumin.Binding.RefVar_Bool SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.Vector3Int)MyAgent).Equals(other);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




