using System;
using System.Collections.Generic;

[Serializable]
public class ArtistResponse
{
    public MetaData metadata;
    public List<Artwork> images;
    public List<Door> doors;
}

[Serializable]
public class Artwork
{
    public string label;
    public string desc;
    public string url;
}

[Serializable]
public class MetaData
{
    public string name;
    public string pob;
    public string dob;
    public string pod;
    public string dod;
    public string spouse;
}

[Serializable]
public class Door
{
    public string label;
    public string type;
    public string id;
}
