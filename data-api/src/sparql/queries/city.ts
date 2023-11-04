import { getLastPathSegment } from "../../util";
import { wikidataUrl } from "../client";
import mapImage from "../mapImage";
import { paintingDataQuery } from "../query";

export const buildSubqueries = (cityId: string): string[] => [
  `
    SERVICE <${wikidataUrl}> {
      SELECT ?name ?description WHERE {
        BIND(wd:${cityId} as ?city) .

        ?city rdfs:label ?name .
        FILTER(LANG(?name) = "nl")
        ?city schema:description ?description .
        FILTER(LANG(?description) = "nl") .
      }
    }
  `,
  `
    ${paintingDataQuery}

    SERVICE <${wikidataUrl}> {
      SELECT * WHERE {
        BIND(wd:${cityId} as ?city) .

        ?artistwk wdt:P19 ?city .
        ?artistwk wdt:P650 ?rkdid .
      }
    }
  `,
  `
    SERVICE <${wikidataUrl}> {
      SELECT ?artistwk ?artistname WHERE {
        BIND(wd:${cityId} as ?city) .

        ?artistwk wdt:P19 ?city .
        ?artistwk rdfs:label ?artistname .
        ?artistwk wdt:P650 ?artistid .
        FILTER(LANG(?artistname) = "nl") .
      }
    }
  `
]

export const mapData = ([metadata, images, links]: any[][]) => {
  return {
    metadata: {
      name: metadata[0].name.value,
      description: metadata[0].description.value
    },
    images: mapImage(images),
    links: links.map((l) => ({
      label: l.artistname.value,
      type: 'artist',
      id: getLastPathSegment(l.artistwk.value)
    }))
  }
}

