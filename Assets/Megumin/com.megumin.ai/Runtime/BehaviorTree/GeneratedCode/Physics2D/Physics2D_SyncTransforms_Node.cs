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
    [DisplayName("Physics2D_SyncTransforms")]
    [Category("UnityEngine/Physics2D")]
    [AddComponentMenu("SyncTransforms")]
    public sealed class Physics2D_SyncTransforms_Node : BTActionNode
    {
        protected override Status OnTick(BTNode from, object options = null)
        {
            UnityEngine.Physics2D.SyncTransforms();
            return Status.Succeeded;
        }
    }
}




