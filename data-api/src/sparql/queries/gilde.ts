import SparqlClient from "sparql-http-client";

import { getLastPathSegment } from "../../util";
import { wikidataClient } from "../client";

export const buildSubqueries = (gildeId: string): { query: string; client: SparqlClient }[] => [
  {
    query: `
      BIND(wd:${gildeId} as ?group) .
      OPTIONAL {
        ?group rdfs:label ?glabel .
        ?o wdt:P463 ?group .
        ?o rdfs:label ?olabel .
        OPTIONAL {
          ?o wdt:P18 $image .
        }
        FILTER(lang($glabel) = 'nl') .
        FILTER(lang($olabel) = 'nl') .
      }
    `,
    client: wikidataClient
  }
]

export const mapData = ([gilde]: any[][]) => {
  const [row] = gilde;

  return {
    metadata: {
      name: row.glabel?.value,
    },
    images: gilde.filter((g) => g.image?.value).map((g) => ({
      label: g.olabel?.value,
      url: g.image?.value,
    })),
    links: gilde.filter((g) => g.o?.value).map((g) => ({
      label: g.olabel?.value,
      type: "artist",
      id: getLastPathSegment(g.o?.value)
    }))
  }
}
