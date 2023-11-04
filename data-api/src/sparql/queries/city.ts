import { wikidataUrl } from "../client";
import mapImage from "../mapImage";
import { paintingDataQuery } from "../query";

export const buildSubquery = (cityId: string): string => `
  ${paintingDataQuery}

  SERVICE <${wikidataUrl}> {
    SELECT * WHERE {
      ?person wdt:P19 wd:${cityId} .
      ?person wdt:P650 ?id
    }
  }
`

// export const buildMetadataQuery = (cityId: string): string => `
//   PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
//   PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
//   PREFIX wd: <http://www.wikidata.org/entity/>
//   PREFIX wdt: <http://www.wikidata.org/prop/direct/>
//   PREFIX schema: <http://schema.org/>
//   SELECT DISTINCT ?name ?description ?artist ?artist_label WHERE {
//     BIND(wd:${cityId} as ?city) .

//     ?city rdfs:label ?name .
//     FILTER(LANG(?name) = "nl") .
//     ?city schema:description ?description .
//     FILTER(LANG(?description) = "nl") .
//     ?artist wdt:P19 ?city .

//     ?artist wdt:P650 ?rkdid .
//     ?artist rdfs:label ?artist_label .
//     FILTER(LANG(?artist_label) = "nl")

//   } LIMIT 100
// `

export const mapMetaData = (data: any[]) => {
  return mapImage(data)
}

