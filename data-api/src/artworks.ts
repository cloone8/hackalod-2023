import { genericClient, artworksV2Url, genericSparqlUrl, wikidataUrl } from './sparql/client'
import readableToObjectList from './util/readableToObjectList';

const PREFIX_WD = "http://www.wikidata.org/entity/"

const paintingDataQuery = `
  SERVICE <${artworksV2Url}> {
    SELECT ?name ?description ?contentUrl WHERE {
      ?painting schemas:creator ?id .
      ?painting schemas:image [schemas:contentUrl ?contentUrl] .
      OPTIONAL { ?painting schemas:name ?name . }
      OPTIONAL { ?painting schemas:description ?description }
    }
  }
`

// wdt:P650 = RKD artist ID
const queryByArtist = (artistId: string, limit = 100) => `
  PREFIX schema: <http://schema.org/>
  PREFIX schemas: <https://schema.org/>
  PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
  PREFIX wd: <http://www.wikidata.org/entity/>
  PREFIX wdt: <http://www.wikidata.org/prop/direct/>

  SELECT ?name ?description ?contentUrl WHERE {
    SERVICE <${wikidataUrl}> {
      SELECT * WHERE {
        <${PREFIX_WD}${artistId}> wdt:P650 ?id .
      }
    }

    ${paintingDataQuery}
  } limit ${limit}
`

const queryByCity = (cityId: string, limit = 100) => `
  PREFIX schema: <http://schema.org/>
  PREFIX schemas: <https://schema.org/>
  PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
  PREFIX wd: <http://www.wikidata.org/entity/>
  PREFIX wdt: <http://www.wikidata.org/prop/direct/>

  SELECT * WHERE {
    SERVICE <${wikidataUrl}> {
      SELECT * WHERE {
        ?person wdt:P19 <${PREFIX_WD}${cityId}> .
        ?person wdt:P650 ?id
      }
    }

    ${paintingDataQuery}
  } LIMIT ${limit}
`

const mapPaintingDataQuery = (data: any[]) => data.map((d: any) => ({
  name: d.name.value,
  description: d.description.value,
  contentUrl: d.contentUrl.value
}))

/**
 * Get a list of paintings given a painterId.
 * @param painterId An RKD artist id, a number as string, no prefix.
 * @returns List of paintings with some metadata
 */
export const getArtworksByPainter = async (painterId: string) => {
  return genericClient.query.select(queryByArtist(painterId))
    .then(readableToObjectList)
    .then(mapPaintingDataQuery)
}

export const getArtworksByCity = async (cityId: string) => {
  return genericClient.query.select(queryByCity(cityId))
    .then(readableToObjectList)
    .then(mapPaintingDataQuery)
}
