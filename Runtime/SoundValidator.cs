using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Simple.SoundSystem.Core
{
    public abstract class SoundValidator : MonoBehaviour
    {
        public abstract bool ValidateSound(Sound sound);
        public abstract bool ValidateSound3D(Sound sound, Vector3 position);
    }
}
