import client from './sparql/client'
import readableToObjectList from './util/readableToObjectList';

const PREFIX_RKDA = "https://data.rkd.nl/artists/";

const query = (artistId: string, prefix = PREFIX_RKDA, limit = 100) => `
  PREFIX schema: <http://schema.org/>
  PREFIX schemas: <https://schema.org/>
  PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>

  select ?name ?description ?contentUrl where {
    ?painting schemas:creator <${prefix}${artistId}> .
    ?painting schemas:image [schemas:contentUrl ?contentUrl] .
    OPTIONAL { ?painting schemas:name ?name . }
    OPTIONAL { ?painting schemas:description ?description }
  } limit ${limit}
`

/**
 * Get a list of paintings given a painterId.
 * @param painterId An RKD artist id, a number as string, no prefix.
 * @returns List of paintings with some metadata
 */
export const getArtworksByPainter = async (painterId: string) => {
  const data = await client.query.select(query(painterId)).then(readableToObjectList)
  const paintings = data.map((d: any) => ({
    name: d.name.value,
    description: d.description.value,
    contentUrl: d.contentUrl.value
  }))
  return paintings
}
