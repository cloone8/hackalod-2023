import SparqlClient from "sparql-http-client";
// Creates a SPARQL client for the artworks-v2 dataset

export const artworksV2Url = "https://api.data.netwerkdigitaalerfgoed.nl/datasets/heritageflix/artworks-v2/sparql";
export const wikidataUrl = "https://query.wikidata.org/sparql";

export const artworksClient = new SparqlClient({ endpointUrl: artworksV2Url });
export const wikidataClient = new SparqlClient({ endpointUrl: wikidataUrl });
