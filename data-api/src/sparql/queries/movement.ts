import SparqlClient from "sparql-http-client";

import { getLastPathSegment } from "../../util";
import { artworksClient, wikidataUrl } from "../client";
import mapImage from "../mapImage";
import { paintingDataQuery } from "../query";

export const buildSubqueries = (movementId: string): { query: string; client: SparqlClient }[] => [
  {
    query: `
    SERVICE <${wikidataUrl}> {
      SELECT ?name ?description WHERE {
        BIND(wd:${movementId} as ?movement) .

        ?movement rdfs:label ?name .
        FILTER(LANG(?name) = "nl")
      }
    }
  `,
    client: artworksClient,
  },
  {
    query: `
    ${paintingDataQuery}

    SERVICE <${wikidataUrl}> {
      SELECT * WHERE {
        BIND(wd:${movementId} as ?movement) .

        ?artistwk wdt:P135 ?movement .
        ?artistwk wdt:P650 ?rkdid .
      }
    }
  `,
    client: artworksClient,
  },
  {
    query: `
    SERVICE <${wikidataUrl}> {
      SELECT ?artistwk ?artistname WHERE {
        BIND(wd:${movementId} as ?movement) .

        ?artistwk wdt:P135 ?movement .
        ?artistwk rdfs:label ?artistname .
        ?artistwk wdt:P650 ?rkdid .
        FILTER(LANG(?artistname) = "nl") .
      }
    }
  `,
    client: artworksClient,
  },
];

export const mapData = ([metadata, images, links]: any[][]) => {
  console.log(links);
  return {
    metadata: {
      name: metadata[0].name.value,
    },
    images: mapImage(images),
    links: links
      .filter((l) => l.artistwk?.value && l.artistname?.value)
      .map((l) => ({
        label: l.artistname.value,
        type: "artist",
        id: getLastPathSegment(l.artistwk.value),
      })),
  };
};

// MovementID is from rkd-artist, not wikidata
// export const buildSubqueries = (movementId: string): string[] => [
//   `
//     SERVICE <http://vocab.getty.edu/sparql> {
//       SELECT ?name WHERE {
//         BIND(aat:${movementId} as ?movement) .
//         ?movement rdfs:label ?name .
//         FILTER(LANG(?name) = "nl")
//       }
//     }
//   `,
//   `
//     ?painting schemas:temporalCoverage aat:${movementId} .
//     ?painting schemas:image [schemas:contentUrl ?paintingurl] .
//     OPTIONAL { ?painting schemas:name ?paintingname . } .
//     OPTIONAL { ?painting schemas:description ?paintingdesc }
//   `,
//   `
//     ?painting schemas:temporalCoverage aat:${movementId} .
//     ?painting schemas:creator ?artist .
//     ?artist schema:identifier ?rkdid

//     SERVICE <${wikidataUrl}> {
//       SELECT * WHERE {
//           ?wid wdt:P650 ?rkdid .
//           ?wid rdfs:label ?artistname .
//         FILTER(LANG(?artistname) = "nl")
//       } LIMIT 10
//     }
//   `
// ]

// export const mapData = ([metadata, images, links]: any[][]) => {
//   console.log(metadata)
//   return {
//     metadata: {
//       name: metadata[0].name.value,
//     },
//     images: mapImage(images),
//     links: links.map((l) => ({
//       label: l.artistname.value,
//       type: 'artist',
//       id: getLastPathSegment(l.wid.value)
//     }))
//   }
// }
