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
    [DisplayName("Quaternion_Angle")]
    [Category("UnityEngine/Quaternion")]
    [AddComponentMenu("Angle(Quaternion, Quaternion)")]
    public sealed class Quaternion_Angle_Quaternion_Quaternion_Method_Decorator : CompareDecorator<float>
    {
        [Space]
        public Megumin.Binding.RefVar<UnityEngine.Quaternion> a;
        public Megumin.Binding.RefVar<UnityEngine.Quaternion> b;

        [Space]
        public Megumin.Binding.RefVar_Float CompareTo;

        [Space]
        public Megumin.Binding.RefVar_Float SaveValueTo;

        public override float GetResult()
        {
            var result = UnityEngine.Quaternion.Angle(a, b);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return result;
        }

        public override float GetCompareTo()
        {
            return CompareTo;
        }

    }
}




