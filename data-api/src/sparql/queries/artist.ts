import { getLastPathSegment, parseDate } from "../../util";
import { wikidataUrl } from "../client";
import { paintingDataQuery } from "../query";

export const buildSubquery = (artistId: string): string => `
  ${paintingDataQuery}

  SERVICE <${wikidataUrl}> {
    SELECT * WHERE {
      BIND(wd:${artistId} as ?wid) .
      ?wid wdt:P650 ?rkdid .
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
  }
`;

export const mapData = (data: any[]) => {
  const [first] = data;

  return {
    metadata: {
      name: first.name.value,
      pob: first.poblabel.value,
      dob: parseDate(first.dob.value),
      pod: first.podlabel.value,
      dod: parseDate(first.dod.value),
      spouse: first.spouselabel.value,
    },
    images: [
      {
        label: first.name.value,
        url: first.image.value,
      },
      ...data.map((row) => ({
        label: row.paintingname.value,
        desc: row.paintingdesc.value,
        url: row.paintingurl.value,
      }))
    ],
    links: [
      {
        label: `Geboortestad: ${first.poblabel.value}`,
        type: "city",
        id: getLastPathSegment(first.pob.value),
      },
      {
        label: `Sterfplaats: ${first.podlabel.value}`,
        type: "city",
        id: getLastPathSegment(first.pod.value),
      }
    ]
  }
}
