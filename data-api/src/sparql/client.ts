import SparqlClient from 'sparql-http-client'
// Creates a SPARQL client for the artworks-v2 dataset

const artworksV2Url = 'https://api.data.netwerkdigitaalerfgoed.nl/datasets/heritageflix/artworks-v2/sparql'
const genericSparqlUrl = 'http://sparql.org/sparql'

export const artworksClient = new SparqlClient({ endpointUrl: artworksV2Url })
export const genericClient = new SparqlClient({ endpointUrl: genericSparqlUrl })
