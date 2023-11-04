import { getLastPathSegment, parseDate } from "../../util";
import { wikidataUrl } from "../client";
import mapImage from "../mapImage";
import { paintingDataQuery } from "../query";

export const buildSubqueries = (artistId: string): string[] => {
  return [
    `
    ${paintingDataQuery}

    SERVICE <${wikidataUrl}> {
      SELECT * WHERE {
        BIND(wd:${artistId} as ?wid) .
        ?wid wdt:P650 ?rkdid .
        ?wid rdfs:label ?name .
        OPTIONAL {
          ?wid wdt:P18 $image .
          ?wid wdt:P19 ?pob .
          ?pob rdfs:label ?poblabel .
          ?wid wdt:P569 ?dob .
          ?wid wdt:P20 ?pod .
          ?pod rdfs:label ?podlabel .
          ?wid wdt:P570 ?dod .
        	?wid wdt:P26 ?spouse .
        	?spouse rdfs:label ?spouselabel .
        	FILTER (lang(?spouselabel) = 'nl') .
          FILTER (lang(?poblabel) = 'nl') .
          FILTER (lang(?podlabel) = 'nl') .
      	} .
        FILTER (lang(?name) = 'nl') .
      }
    }`,
    `
    ${paintingDataQuery}

    SERVICE <${wikidataUrl}> {
      SELECT * WHERE {
        BIND(wd:${artistId} as ?wid) .
        ?wid wdt:P650 ?rkdid .
      }
    }`,
    `
    SERVICE <https://query.wikidata.org/sparql> {
      SELECT * WHERE {
        BIND(wd:${artistId} as ?wid) .
        OPTIONAL {
        	?wid wdt:P800 $painting .
        	?painting rdfs:label ?paintingname .
          ?painting wdt:P1476 ?paintingdesc .
          $painting wdt:P18 $paintingurl .
        	FILTER(lang($paintingdesc) = 'nl') .
      		FILTER(lang($paintingname) = 'nl') .
      	}
      }
    }
    `,
    `
    SERVICE <${wikidataUrl}> {
      SELECT * WHERE {
        BIND(wd:${artistId} as ?wid) .
        ?wid wdt:P135 ?movement .
        ?movement rdfs:label ?movementlabel .
        FILTER (lang(?movementlabel) = 'nl') .
      }
    }`,
  ];
}

export const mapData = ([metadata, images, images2, movements]: any[][]) => {
  const [metafirst] = metadata;

  return {
    metadata: {
      name: metafirst.name?.value,
      pob: metafirst.poblabel?.value,
      dob: parseDate(metafirst.dob?.value),
      pod: metafirst.podlabel?.value,
      dod: parseDate(metafirst.dod?.value),
      spouse: metafirst.spouselabel?.value,
      movements: movements.map(m => m.movementlabel?.value).join(', '),
    },
    images: [
      ...(metafirst.image ? [{
        label: metafirst.name?.value,
        url: metafirst.image?.value,
      }] : []),
      ...mapImage(images),
      ...mapImage(images2)
    ],
    links: [
      ...(metafirst.pob && metafirst.poblabel ? [{
        label: `Geboortestad: ${metafirst.poblabel.value}`,
        type: "city",
        id: getLastPathSegment(metafirst.pob.value),
      }] : []),
      ...(metafirst.pod && metafirst.podlabel ? [{
        label: `Sterfplaats: ${metafirst.podlabel.value}`,
        type: "city",
        id: getLastPathSegment(metafirst.pod.value),
      }] : []),
      // ...movements.map(m => ({
      //   label: `Beweging: ${m.movementlabel.value}`,
      //   type: "movement",
      //   id: getLastPathSegment(m.movement.value),
      // }))
    ]
  }
}
