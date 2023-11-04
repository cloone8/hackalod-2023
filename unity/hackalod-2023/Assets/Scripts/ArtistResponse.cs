using System;
using System.Collections.Generic;

[Serializable]
public class ArtistResponse
{
    public MetaData metadata;
    public List<Artwork> images;
    public List<Link> links;
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
    public string movements;
}

[Serializable]
public class Link
{
    public string label;
    public string type;
    public string id;
}
