using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simple.SoundSystem.Core
{
    public class SoundParameters
    {
        public bool Loop = false;
        public float FadeDuration = 0.2f;
        public float CustomVolumeMultiplier = 1f;
        public Vector3? CustomSpacialPosition;
        public Transform CustomSpacialTransformTarget;
        public float CustomSpacialRange = 7f;
        public bool IsSpacialSound => CustomSpacialTransformTarget != null || CustomSpacialPosition != null;
    }
}
