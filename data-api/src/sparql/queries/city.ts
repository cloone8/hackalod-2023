import { wikidataUrl } from "../client";
import mapImage from "../mapImage";
import { paintingDataQuery } from "../query";

export const buildSubquery = (cityId: string): string => `
  ${paintingDataQuery}

  SERVICE <${wikidataUrl}> {
    SELECT * WHERE {
      BIND(wd:${cityId} as ?city) .

      OPTIONAL {
        ?city rdfs:label ?name .
        FILTER(LANG(?name) = "nl")
      }
      OPTIONAL {
        ?city schema:description ?description .
        FILTER(LANG(?description) = "nl") .
      }

      ?artistwk wdt:P19 ?city .
      ?artistwk wdt:P650 ?id .
      ?artistwk rdfs:label ?artistname
      BIND(REPLACE(STR(?artistwk), "http://www.wikidata.org/entity/", "") as ?artistwkid)
    }
  }
`

export const mapMetaData = (data: any[]) => {
  console.log(data)
  const [first] = data

  const links = data.reduce((total, cur) => {
    const id = cur.artistwkid.value
    if (!total.some((d: any) => d.id == id)) {
      total.push({
        label: cur.artistname.value,
        type: 'artist',
        id
      })
    }
    return total
  }, [] as any[])

  return {
    metadata: {
      name: first.name.value,
      description: first.description.value
    },
    images: [
      ...mapImage(data)
    ],
    links
  }
}

