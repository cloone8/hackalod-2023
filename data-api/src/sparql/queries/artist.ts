import { wikidataUrl } from "../client";
import { paintingDataQuery } from "../query";

export const buildSubquery = (artistId: string): string => `
  ${paintingDataQuery}

  SERVICE <${wikidataUrl}> {
    SELECT * WHERE {
      BIND(wd:${artistId} as ?wid) .
      ?wid wdt:P650 ?rkdid .
      ?wid rdfs:label ?name .
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
  return data
}
