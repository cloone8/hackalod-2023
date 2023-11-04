using System;

[Serializable]
public struct Room {
    public string scene;
    public string painter;

    public Room(string scene, string painter) {
        this.scene = scene;
        this.painter = painter;
    }
}
