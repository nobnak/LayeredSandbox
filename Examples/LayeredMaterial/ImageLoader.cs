using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace LayeredSandbox {
    public abstract class ImageLoader : System.IDisposable {
        public Material filterMat;

        public Texture2D ImageTex { get; private set; }

        readonly float updateInterval = 0.5f;

        string lastPath;
        System.DateTime lastUpdateTime;

        public ImageLoader(float updateInterval = 0.5f) {
            this.updateInterval = updateInterval;
            lastUpdateTime = System.DateTime.MinValue;
        }

        #region IDisposable implementation
        public void Dispose () {
            ReleaseImageTex();
        }
        #endregion

        public void Update(System.Environment.SpecialFolder folder, string filename) {
            var now = System.DateTime.Now;
            var d = now - lastUpdateTime;
            if (d.TotalSeconds > updateInterval) {
                lastUpdateTime = now;
                UpdateImageTex (folder, filename);
            }
        }

        string GetPath(System.Environment.SpecialFolder folder, string filename) {
            return Path.Combine (System.Environment.GetFolderPath (folder), filename);
        }
        protected virtual void UpdateImageTex(System.Environment.SpecialFolder folder, string filename) {
            try {
                var path = GetPath ();
                if (!File.Exists (path)) {
                    ReleaseImageTex();
                    return;
                }

                var writeTime = File.GetLastWriteTime (path);
                if (writeTime != lastUpdateTime || path != lastPath) {
                    lastUpdateTime = writeTime;
                    lastPath = path;

                    if (ImageTex == null) {
                        ImageTex = new Texture2D(2, 2, TextureFormat.ARGB32, false, true);
                        ImageTex.filterMode = FilterMode.Bilinear;
                        ImageTex.wrapMode = TextureWrapMode.Clamp;
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