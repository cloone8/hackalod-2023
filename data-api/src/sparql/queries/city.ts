import { wikidataUrl } from "../client";
import { paintingDataQuery } from "../query";

export const buildSubquery = (cityId: string): string => `
  ${paintingDataQuery}

  SERVICE <${wikidataUrl}> {
    SELECT * WHERE {
      ?person wdt:P19 wd:${cityId} .
      ?person wdt:P650 ?id
    }
  }
`
