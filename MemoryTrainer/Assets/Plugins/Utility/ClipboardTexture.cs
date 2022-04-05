using System;

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.IO.MemoryMappedFiles;
#endif

namespace Utility
{
    public static class ClipboardTexture
    {
#if UNITY_EDITOR
        [MenuItem("Tools/PasteImageFromClipboard")]
        private static void Paste()
        {
            var selection = Selection.activeGameObject;
            if (selection != null)
            {
                foreach (var handler in _handlers)
                    if (handler.Invoke(selection))
                        return;
            }
        }

        private static List<Func<GameObject, bool>> _handlers;

        private static bool GetAndRun<T>(GameObject gameObject, Action<T> action) where T : Component
        {
            if (gameObject.TryGetComponent(out T component))
            {
                void Continue()
                {
                    Undo.RecordObject(component, "Set texture");
                    action?.Invoke(component);
                }

                GrabTexture(Continue);
                return true;
            }
            return false;
        }

        [InitializeOnLoadMethod]
        private static void Init()
        {
            AssemblyReloadEvents.beforeAssemblyReload += Unload;

            _textures = new List<Texture2D>();
            _sprites = new List<Sprite>();

            _handlers = new List<Func<GameObject, bool>>();
            _handlers.Add(g => GetAndRun<Image>(g, c => c.sprite = _sprite));
            _handlers.Add(g => GetAndRun<SpriteRenderer>(g, c => c.sprite = _sprite));
            _handlers.Add(g => GetAndRun<RawImage>(g, c => c.texture = _texture));
            _handlers.Add(g => GetAndRun<MeshRenderer>(g, c => c.material.mainTexture = _texture));
        }

        private const string _CLIPBOARD_READER_PATH = "Plugins/Utility/Bin/ClippboardReader.exe";
        private const string _MAPPED_FILE_PATH = "ClippboardReader.tmpClippboardImage.png";

        private static List<Texture2D> _textures;
        private static List<Sprite> _sprites;

        private static Texture2D _texture => _textures.Last();
        private static Sprite _sprite => _sprites.Last();

        private static void GrabTexture(Action continueWith)
        {
            var copyBuffer = GUIUtility.systemCopyBuffer;
            if (string.IsNullOrEmpty(copyBuffer))
            {
                 GrabAsImage();
                continueWith?.Invoke();
            }
            else if (Uri.TryCreate(copyBuffer, UriKind.Absolute, out var uri))
            {
                GrabAsUri(uri, continueWith);
            }
            else
            {
                copyBuffer = copyBuffer.Trim('\"');
                if (File.Exists(copyBuffer))
                {
                    GrabAsFile(copyBuffer);
                    continueWith?.Invoke();
                }
            }
        }

        private static void GrabAsImage()
        {
            var clippboardReader = new System.Diagnostics.Process();
            clippboardReader.StartInfo.UseShellExecute = false;
            clippboardReader.StartInfo.CreateNoWindow = true;
            clippboardReader.StartInfo.RedirectStandardOutput = true;
            clippboardReader.StartInfo.RedirectStandardInput = true;
            clippboardReader.StartInfo.FileName = Path.Combine(Application.dataPath, _CLIPBOARD_READER_PATH);
            clippboardReader.Start();

            clippboardReader.StandardInput.WriteLine("Start");

            //INITAL RESULT
            var resultString = clippboardReader.StandardOutput.ReadLine();
            if (int.TryParse(resultString, out var result) && result == 0)
            {
                var size = long.Parse(clippboardReader.StandardOutput.ReadLine());
                using (var mappedFile = MemoryMappedFile.CreateNew(_MAPPED_FILE_PATH, size, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.DelayAllocatePages, HandleInheritability.Inheritable))
                {
                    clippboardReader.StandardInput.WriteLine("Continue");
                    //WAIT WRITE TO FILE
                    clippboardReader.StandardOutput.ReadLine();
                    try
                    {
                        using (var stream = mappedFile.CreateViewStream())
                        {
                            var array = new byte[stream.Length];
                            stream.Read(array, 0, array.Length);
                            LoadImage(array);
                        }

                        Debug.Log($"Texture Grabbed from Clippboard");
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

                }
            }
            else
            {
                Debug.Log($"No texture. Result is {resultString}");
            }
        }

        private static void GrabAsFile(string path)
        {
            if (File.Exists(path))
            {
                var array = File.ReadAllBytes(path);
                LoadImage(array);
                Debug.Log($"Texture Grabbed from file");
            }
        }

        private static void GrabAsUri(Uri uri, Action onComplete)
        {
            var request = UnityWebRequestTexture.GetTexture(uri, true);
            var asyncOperation = request.SendWebRequest();
            asyncOperation.completed += Completed;

            void Completed(AsyncOperation _)
            {
                if (request.isHttpError || request.isNetworkError)
                {
                    Debug.Log($"Failded grabbing texture from Uri: {request.error}");
                    return;
                }
                LoadImage(DownloadHandlerTexture.GetContent(request));
                Debug.Log($"Texture Grabbed from Uri");
                onComplete?.Invoke();
            }
        }

        private static void LoadImage(Texture2D texture)
        {
            _textures.Add(texture);
            _texture.alphaIsTransparency = true;
            _texture.hideFlags = HideFlags.HideAndDontSave;
            _sprites.Add(Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect));
        }

        private static void LoadImage(byte[] data)
        {
            _textures.Add(new Texture2D(4, 4, TextureFormat.ARGB32, false));
            _texture.alphaIsTransparency = true;
            _texture.hideFlags = HideFlags.HideAndDontSave;
            _texture.LoadImage(data, true);
            _sprites.Add(Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect));
        }

        private static void Unload()
        {
            DestroyTextures();
        }

        private static void DestroyTextures()
        {
            foreach (var texure in _textures)
                if (texure != null)
                    GameObject.DestroyImmediate(texure);
            _textures.Clear();

            foreach (var sprite in _sprites)
                if (sprite != null)
                    GameObject.DestroyImmediate(_sprite);
            _sprites.Clear();
        }
#endif
    }
}
