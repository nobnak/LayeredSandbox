using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MovableLandmarkPacket {
    public const string PATH = "/object";

    public int id;
    public int type; // 0:hill, 1:lake, 2:block
    public Rect rect;

    public static bool TryParse(Osc.OscPort.Capsule c, out MovableLandmarkPacket p) {
        var m = c.message;
        try {
            if (m.path == PATH) {
                p = Parse(m.data);
                return true;
            }
        } catch (System.Exception){
        }

        p = default(MovableLandmarkPacket);
        return false;
    }
    public static MovableLandmarkPacket Parse(object[] data) {
        return new MovableLandmarkPacket () {
            id = (int)data [0],
            type = (int)data [1],
            rect = new Rect((float)data [2], (float)data [3], (float)data [4], (float)data [5])
        };        
    }

    public override string ToString () {
        return string.Format ("[MovableLandmarkPacket]{{id={0},type={1},rect={2}}}", id, type, rect);
    }
}