import { genericClient, artworksV2Url, genericSparqlUrl, wikidataUrl } from './sparql/client'
import readableToObjectList from './util/readableToObjectList';

const PREFIX_WD = "http://www.wikidata.org/entity/"

// wdt:P650 = RKD artist ID
const query = (artistId: string, prefix = PREFIX_WD, limit = 100) => `
  PREFIX schema: <http://schema.org/>
  PREFIX schemas: <https://schema.org/>
  PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
  PREFIX wd: <http://www.wikidata.org/entity/>
  PREFIX wdt: <http://www.wikidata.org/prop/direct/>

  SELECT ?name ?description ?contentUrl WHERE {
    SERVICE <${wikidataUrl}> {
      SELECT * WHERE {
        <${prefix}${artistId}> wdt:P650 ?id .
      }
    }

    SERVICE <${artworksV2Url}> {
      SELECT ?name ?description ?contentUrl WHERE {
        ?painting schemas:creator ?id .
        ?painting schemas:image [schemas:contentUrl ?contentUrl] .
        OPTIONAL { ?painting schemas:name ?name . }
        OPTIONAL { ?painting schemas:description ?description }
      }
    }
  } limit ${limit}
`

/**
 * Get a list of paintings given a painterId.
 * @param painterId An RKD artist id, a number as string, no prefix.
 * @returns List of paintings with some metadata
 */
export const getArtworksByPainter = async (painterId: string) => {
  const data = await genericClient.query.select(query(painterId)).then(readableToObjectList)
  const paintings = data.map((d: any) => ({
    name: d.name.value,
    description: d.description.value,
    contentUrl: d.contentUrl.value
  }))
  return paintings
}
