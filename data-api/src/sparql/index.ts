import SparqlClient from "sparql-http-client";

import { readableToObjectList } from "../util";
import { buildQuery } from "./query";

export const entityTypes = ["artist", "city", "movement", "gilde"];

export default async (entityType: string, entityId: string) => {
  const { buildSubqueries, mapData } = require(`./queries/${entityType}`);

  const qs = buildSubqueries(entityId).map(({ query, client }: { query: string; client: SparqlClient }) => ({ query: buildQuery(query), client }));

  console.log(qs[0].query);

  const responses = await Promise.all(qs.map(({ query, client }: { query: string; client: SparqlClient }) => client.query.select(query).then(readableToObjectList)));

  return mapData(responses);
};
