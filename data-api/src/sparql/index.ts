import { readableToObjectList } from "../util";
import { artworksClient } from "./client";
import { buildQuery } from "./query";

export default async (entityType: string, entityId: string) => {
  const { buildSubquery, mapMetaData } = require(`./queries/${entityType}`);

  const q = buildQuery(buildSubquery(entityId));

  return artworksClient.query.select(q)
    .then(readableToObjectList)
    .then(mapMetaData);
}
