using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gist;

namespace LayeredSandbox {
    public class LayeredMaterialSettings : Settings<LayeredMaterialSettings.Data> {
        public const int NUMBER_OF_LAYERS = 3;
        public static readonly string[] NATURE_TEX_NAMES = new string[]{ "_Tex1", "_Tex2", "_Tex3" };
        public const string NATURE_POWER = "_Pows";

        public System.Environment.SpecialFolder folder = System.Environment.SpecialFolder.MyDocuments;
        public Material layeredMaterial;

        Texture[] defaultNatureTexs;
        ImageLoader[] loaders;

        protected override void Awake () {
            base.Awake ();
            defaultNatureTexs = new Texture[NUMBER_OF_LAYERS];
            loaders = new ImageLoader[defaultNatureTexs.Length];
            for (var i = 0; i < defaultNatureTexs.Length; i++) {
                defaultNatureTexs [i] = layeredMaterial.GetTexture (NATURE_TEX_NAMES [i]);
                loaders [i] = new ImageLoader ();
            }
        }
        protected void OnDestroy() {
            for (var i = 0; i < loaders.Length; i++) {
                layeredMaterial.SetTexture (NATURE_TEX_NAMES [i], defaultNatureTexs [i]);
                loaders [i].Dispose ();
            }
        }

        protected override void OnDataChange () {
            base.OnDataChange ();

            loaders [0].Update (folder, data.natureImage1);
            loaders [1].Update (folder, data.natureImage2);
            loaders [2].Update (folder, data.natureImage3);

            for (var i = 0; i < loaders.Length; i++) {
                var tex = loaders [i].ImageTex;
                layeredMaterial.SetTexture (NATURE_TEX_NAMES [i], 
                    (tex == null ? defaultNatureTexs [i] : tex));
            }

            layeredMaterial.SetVector (NATURE_POWER, data.powers);
        }

        [System.Serializable]
        public class Data {
            public string natureImage1;
            public string natureImage2;
            public string natureImage3;
            public Vector4 powers;
        }
    }
}
