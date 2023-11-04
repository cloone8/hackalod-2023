using System;

[Serializable]
public struct Room {
    public string scene;
    public string painter;

    public string prompt;

    public Room(string scene, string painter, string prompt) {
        this.scene = scene;
        this.painter = painter;
        this.prompt = prompt;
    }
}
