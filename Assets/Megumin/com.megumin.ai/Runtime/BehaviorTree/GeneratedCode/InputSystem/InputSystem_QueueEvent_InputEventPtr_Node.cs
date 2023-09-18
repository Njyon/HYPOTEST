﻿///********************************************************************************************************************************
///本页代码由代码生成器生成，请勿手动修改。The code on this page is generated by the code generator, do not manually modify.
///生成器类型：Megumin.GameFramework.AI.BehaviorTree.Editor.NodeGenerator
///配置源文件：$(CodeGenericSourceFilePath)
///********************************************************************************************************************************

#if ENABLE_INPUT_SYSTEM

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [DisplayName("InputSystem_QueueEvent")]
    [Category("UnityEngine/InputSystem")]
    [AddComponentMenu("QueueEvent(InputEventPtr)")]
    public sealed class InputSystem_QueueEvent_InputEventPtr_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar<UnityEngine.InputSystem.LowLevel.InputEventPtr> eventPtr;

        protected override Status OnTick(BTNode from, object options = null)
        {
            UnityEngine.InputSystem.InputSystem.QueueEvent(eventPtr);
            return Status.Succeeded;
        }
    }
}

#endif




