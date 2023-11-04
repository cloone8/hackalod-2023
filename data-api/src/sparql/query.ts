export const paintingDataQuery = `
  ?artist schema:identifier ?rkdid .
  ?painting schemas:creator ?artist .
  ?painting schemas:image [schemas:contentUrl ?paintingurl] .
  OPTIONAL { ?painting schemas:name ?paintingname . }
  OPTIONAL { ?painting schemas:description ?paintingdesc }
`

export const buildQuery = (query: string, limit = 100) => `
  PREFIX schema: <http://schema.org/>
  PREFIX schemas: <https://schema.org/>
  PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
  PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
  PREFIX wd: <http://www.wikidata.org/entity/>
  PREFIX wdt: <http://www.wikidata.org/prop/direct/>

  SELECT * WHERE {
    ${query}
  } LIMIT ${limit}
`
