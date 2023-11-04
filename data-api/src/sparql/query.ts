import { artworksV2Url } from "./client"

export const paintingDataQuery = `
  SERVICE <${artworksV2Url}> {
    SELECT ?name ?description ?contentUrl WHERE {
      ?painting schemas:creator ?id .
      ?painting schemas:image [schemas:contentUrl ?contentUrl] .
      OPTIONAL { ?painting schemas:name ?name . }
      OPTIONAL { ?painting schemas:description ?description }
    }
  }
`

export const buildQuery = (query: string, limit = 100) => `
  PREFIX schema: <http://schema.org/>
  PREFIX schemas: <https://schema.org/>
  PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
  PREFIX wd: <http://www.wikidata.org/entity/>
  PREFIX wdt: <http://www.wikidata.org/prop/direct/>

  SELECT * WHERE {
    ${query}
  } LIMIT ${limit}
`
