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
    [Icon("d_Camera Icon")]
    [DisplayName("Camera_VerticalToHorizontalFieldOfView")]
    [Category("UnityEngine/Camera")]
    [AddComponentMenu("VerticalToHorizontalFieldOfView(Single, Single)")]
    public sealed class Camera_VerticalToHorizontalFieldOfView_Single_Single_Node : BTActionNode<UnityEngine.Camera>
    {
        [Space]
        public Megumin.Binding.RefVar_Float verticalFieldOfView;
        public Megumin.Binding.RefVar_Float aspectRatio;

        [Space]
        public Megumin.Binding.RefVar_Float SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Camera.VerticalToHorizontalFieldOfView(verticalFieldOfView, aspectRatio);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




