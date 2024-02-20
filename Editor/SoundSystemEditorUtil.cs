using Simple.SoundSystem.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Simple.SoundSystem.Editor
{
    public static class SoundSystemEditorUtil
    {
        private static List<SoundLibrary> cachedLibs;
        public static List<SoundLibrary> Libs
        {
            get
            {
                if (cachedLibs != null && (cachedLibs.Contains(null) || cachedLibs.Count != AssetDatabase.FindAssets("t:SoundLibrary").Length))
                    cachedLibs = null;              

                if (cachedLibs == null)
                    cachedLibs = LoadAllLibs();

                if (cachedLibs == null)
                    return new List<SoundLibrary>();

                return cachedLibs;
            }
        }
        internal static List<SoundLibrary> LoadAllLibs()
        {
            List<SoundLibrary> libs = new List<SoundLibrary>();
            var guids = AssetDatabase.FindAssets("t:SoundLibrary");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var lib = AssetDatabase.LoadAssetAtPath<SoundLibrary>(path);

                if (lib != null)
                    libs.Add(lib);

            }
            return libs;
        }

        public static bool ClipIsPartOfAnyLibrary(AudioClip clip)
        {
            foreach (var lib in LoadAllLibs())
            {
                foreach (var sound in lib.Sounds)
                {
                    if (sound != null && sound.ContainsClip(clip))
                        return true;
                }
            }

            return false;
        }
    }
}
