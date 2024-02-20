using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Simple.SoundSystem.Editor;
using System;
using System.Collections.Generic;

namespace Simple.SoundSystem.Core
{

    public class CreateSoundFromSoundFile
    {
        [MenuItem("Assets/Create Sound From Clip", true)]
        static bool ValidateLogSelectedTransformName()
        {
            if (Selection.count == 0)
                return false;
            else if (Selection.count > 1)
            {
                foreach (var obj in Selection.objects)
                {
                    if (obj is AudioClip clip)
                    {
                        if (SoundSystemEditorUtil.ClipIsPartOfAnyLibrary(clip))
                            return false;
                    }
                    else
                        return false;

                }
                return true;
            }
            else
            {
                return (Selection.activeObject is AudioClip clip && !SoundSystemEditorUtil.ClipIsPartOfAnyLibrary(clip));
            }
        }

        [MenuItem("Assets/Create Sound From Clip", false, 10)]
        private static void EmptyParent(MenuCommand menuCommand)
        {
            ShowPopupExample window = ScriptableObject.CreateInstance<ShowPopupExample>();

            if (!GetSelectedAudioClips(out var clips))
                return;

            window.Callback = (l) =>
            {
                foreach (var clip in clips)
                {
                    var entry = l.AddNewSoundEntry(clip);
                    if (entry != null)
                        Selection.activeObject = entry;
                }
                EditorUtility.SetDirty(l);
            };

            var width = 250;
            var height = EditorGUIUtility.singleLineHeight * (3 + SoundSystemEditorUtil.Libs.Count);
            var pos = EditorWindow.focusedWindow.position.center - new Vector2(width / 2f, height / 2f);

            window.position = new Rect(pos.x, pos.y, width, height);
            window.ShowPopup();
        }

        private static bool GetSelectedAudioClips(out AudioClip[] clips)
        {
            if (Selection.count == 0)
            {
                clips = new AudioClip[0];
                return false;
            }
            else if (Selection.count > 1)
            {
                List<AudioClip> toReturn = new List<AudioClip>();
                foreach (var obj in Selection.objects)
                    if (obj is AudioClip clip)
                        toReturn.Add(clip);

                clips = toReturn.ToArray();
                return clips.Length > 0;
            }

            if (Selection.activeObject is AudioClip c)
            {
                clips = new AudioClip[] { c };
                return true;
            }
            else
            {
                clips = new AudioClip[0];
                return false;
            }
        }
    }

    public class ShowPopupExample : EditorWindow
    {
        public Action<SoundLibrary> Callback;
        void CreateGUI()
        {
            var label = new Label("Choose Library to add Sound to:");
            rootVisualElement.Add(label);

            foreach (var library in SoundSystemEditorUtil.Libs)
            {
                var button = new Button();
                button.text = library.name;
                button.clicked += () =>
                {
                    Callback?.Invoke(library);
                    this.Close();
                };
                rootVisualElement.Add(button);
            }

            var abortButton = new Button();
            abortButton.text = "<Abort>";
            abortButton.clicked += () => this.Close();
            rootVisualElement.Add(abortButton);
        }
    }
}
