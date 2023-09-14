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
    [DisplayName("Get_Vector2Int_sqrMagnitude")]
    [Category("UnityEngine/Vector2Int")]
    [AddComponentMenu("Get_sqrMagnitude")]
    public sealed class Vector2Int_sqrMagnitude_Get_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Vector2Int MyAgent;

        [Space]
        public Megumin.Binding.RefVar_Int SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.Vector2Int)MyAgent).sqrMagnitude;

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }

    }
}




