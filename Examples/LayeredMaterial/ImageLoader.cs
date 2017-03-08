using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace LayeredSandbox {
    public class ImageLoader : System.IDisposable {
        public const float DEFAULT_INTERVAL = 1f;

        public Texture2D ImageTex { get; private set; }

        readonly float updateInterval;

        string lastPath;
        float lastUpdateTime;
        System.DateTime lastFiletime;

        public ImageLoader(float updateInterval = DEFAULT_INTERVAL) {
            this.updateInterval = updateInterval;
            lastUpdateTime = float.MinValue;
            lastFiletime = System.DateTime.MinValue;
        }

        #region IDisposable implementation
        public void Dispose () {
            ReleaseImageTex();
        }
        #endregion

        public void Update(System.Environment.SpecialFolder folder, string filename) {
            var tnow = Time.timeSinceLevelLoad;
            var dt = tnow - lastUpdateTime;
            if (dt > updateInterval) {
                lastUpdateTime = tnow;
                UpdateImageTex (folder, filename);
            }
        }

        string GetPath(System.Environment.SpecialFolder folder, string filename) {
            return Path.Combine (System.Environment.GetFolderPath (folder), filename);
        }
        protected virtual void UpdateImageTex(System.Environment.SpecialFolder folder, string filename) {
            try {
                var path = GetPath (folder, filename);
                if (!File.Exists (path)) {
                    ReleaseImageTex();
                    return;
                }

                var writeTime = File.GetLastWriteTime (path);
                if (writeTime != lastFiletime || path != lastPath) {
                    lastFiletime = writeTime;
                    lastPath = path;

                    if (ImageTex == null) {
                        ImageTex = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                        ImageTex.filterMode = FilterMode.Bilinear;
                        ImageTex.wrapMode = TextureWrapMode.Repeat;
                        ImageTex.anisoLevel = 0;
                    }
                    ImageTex.LoadImage(File.ReadAllBytes(path));
                }
            } catch (System.Exception e) {
                Debug.LogError (e);
            }
        }
        void ReleaseImageTex () {
            if (Application.isPlaying)
                Object.Destroy (ImageTex);
            else
                Object.DestroyImmediate (ImageTex);
        }
    }
}