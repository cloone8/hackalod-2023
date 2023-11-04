import { wikidataUrl } from "../client";
import { paintingDataQuery } from "../query";

export const buildSubquery = (artistId: string): string => `
  ${paintingDataQuery}

  SERVICE <${wikidataUrl}> {
    SELECT * WHERE {
      wd:${artistId} wdt:P650 ?id .
    }
  }
`;
