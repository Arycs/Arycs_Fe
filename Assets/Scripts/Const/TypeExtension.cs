using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe
{
    //类型扩展方法
    public static class TypeExtension
    {
        /// <summary>
        /// Enum To Int32
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInteger(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// 在Animator 中获取动画Clip
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="clipName"></param>
        /// <returns></returns>
        public static AnimationClip FindClip(this Animator animator, string clipName)
        {
            if (animator == null || string.IsNullOrEmpty(clipName))
            {
                return null;
            }

            if (animator.runtimeAnimatorController == null)
            {
                return null;
            }

            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            if (clips == null || clips.Length == 0)
            {
                return null;
            }

            return Array.Find<AnimationClip>(clips, clip => clip != null && clip.name == clipName);
        }

        public static float GetClipLength(this Animator animator, string clipName)
        {
            AnimationClip clip = FindClip(animator, clipName);
            if (clip == null)
            {
                return 0;
            }

            return clip.length;
        }
    }
}