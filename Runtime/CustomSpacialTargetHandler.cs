using System.Collections.Generic;
using UnityEngine;

namespace Simple.SoundSystem.Core
{
    internal class CustomSpacialTargetHandler
    {
        List<CustomSpacialTarget> targets = new List<CustomSpacialTarget>();
        internal CustomSpacialTarget GetCustomTarget(SoundParameters parameters, PlayingSound playing)
        {
            var transform = parameters.CustomSpacialTransformTarget;
            bool hasCustomTarget = transform != null;
            if (hasCustomTarget && Has(transform))
            {
                return GetTarget(playing, transform);
            }
            else
            {
                if (hasCustomTarget)
                {
                    return CreateNewTarget(playing, transform: transform);
                }
                else if (parameters.CustomSpacialPosition != null)
                {
                    return CreateNewTarget(playing, position: parameters.CustomSpacialPosition);
                }
            }
            return null;
        }
        internal void RemoveFromCustomTarget(PlayingSound playingSound)
        {
            var target = playingSound.CustomTarget;
            target.ActiveSounds.Remove(playingSound);
            if (target.ActiveSounds.Count == 0)
            {
                targets.Remove(target);
                GameObject.Destroy(target.Object);
            }
        }
        private CustomSpacialTarget CreateNewTarget(PlayingSound playing, Transform transform = null, Vector3? position = null)
        {
            CustomSpacialTarget newTarget = CreateNewTempObjectAndTarget(transform: transform, position: position);
            newTarget.ActiveSounds.Add(playing);
            targets.Add(newTarget);
            playing.CustomTarget = newTarget;
            return newTarget;
        }
        private CustomSpacialTarget GetTarget(PlayingSound playing, Transform customTransform)
        {
            var target = Match(customTransform);
            target.ActiveSounds.Add(playing);
            playing.CustomTarget = target;
            return target;
        }
        private bool Has(Transform customTransform)
        {
            return Match(customTransform) != null;
        }
        private CustomSpacialTarget Match(Transform customTransform)
        {
            foreach (var target in targets)
            {
                if (target.TransRef == customTransform)
                    return target;
            }
            return null;
        }
        private static CustomSpacialTarget CreateNewTempObjectAndTarget(Transform transform = null, Vector3? position = null)
        {
            if (transform != null)
            {
                GameObject obj = CreateGameObject();
                obj.transform.parent = transform;
                obj.transform.localPosition = Vector3.zero;
                return new CustomSpacialTarget() { Object = obj, TransRef = transform };
            }
            else if (position != null)
            {
                GameObject obj = CreateGameObject();
                obj.transform.parent = SoundManager.Instance.transform;
                obj.transform.position = position.Value;
                return new CustomSpacialTarget() { Object = obj, PosRef = position };
            }
            Debug.LogWarning($"Sound System tried creating new temp 3D sound target: {(transform == null ? "transform was null; " : "")}{(position == null ? "position was null;" : "")}");
            return null;

            static GameObject CreateGameObject()
            {
                return new GameObject("3D-SOUND(TEMP)");
            }
        }
    }
    internal class CustomSpacialTarget
    {
        public Vector3? PosRef;
        public Transform TransRef;
        public GameObject Object;
        public List<PlayingSound> ActiveSounds = new List<PlayingSound>();
        public bool IsTrans => TransRef != null;
    }
}
