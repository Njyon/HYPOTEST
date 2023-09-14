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
    [Icon("d_GameObject Icon")]
    [DisplayName("Get_GameObject_tag")]
    [Category("UnityEngine/GameObject")]
    [AddComponentMenu("Get_tag")]
    public sealed class GameObject_tag_Get_Node : BTActionNode<UnityEngine.GameObject>
    {
        [Space]
        public Megumin.Binding.RefVar_String SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.GameObject)MyAgent).tag;

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }

    }
}




