import { getLastPathSegment, parseDate } from "../../util";
import { wikidataUrl } from "../client";
import mapImage from "../mapImage";
import { paintingDataQuery } from "../query";

export const buildSubqueries = (artistId: string): string[] => {
  return [
    `
    SERVICE <${wikidataUrl}> {
      SELECT * WHERE {
        BIND(wd:${artistId} as ?wid) .
        ?wid rdfs:label ?name .
        ?wid wdt:P18 $image .
        ?wid wdt:P19 ?pob .
        ?pob rdfs:label ?poblabel .
        ?wid wdt:P569 ?dob .
        ?wid wdt:P20 ?pod .
        ?pod rdfs:label ?podlabel .
        ?wid wdt:P570 ?dod .
        ?wid wdt:P26 ?spouse .
        ?spouse rdfs:label ?spouselabel .
        FILTER (lang(?name) = 'nl') .
        FILTER (lang(?poblabel) = 'nl') .
        FILTER (lang(?podlabel) = 'nl') .
        FILTER (lang(?spouselabel) = 'nl') .
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
  ];
}

export const mapData = ([metadata, images]: any[][]) => {
  const [metafirst] = metadata;

  return {
    metadata: {
      name: metafirst.name.value,
      pob: metafirst.poblabel.value,
      dob: parseDate(metafirst.dob.value),
      pod: metafirst.podlabel.value,
      dod: parseDate(metafirst.dod.value),
      spouse: metafirst.spouselabel.value,
    },
    images: [
      {
        label: metafirst.name.value,
        url: metafirst.image.value,
      },
      ...mapImage(images)
    ],
    links: [
      {
        label: `Geboortestad: ${metafirst.poblabel.value}`,
        type: "city",
        id: getLastPathSegment(metafirst.pob.value),
      },
      {
        label: `Sterfplaats: ${metafirst.podlabel.value}`,
        type: "city",
        id: getLastPathSegment(metafirst.pod.value),
      }
    ]
  }
}
