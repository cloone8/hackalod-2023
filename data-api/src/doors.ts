import readableToObjectList from './util/readableToObjectList';
import { genericClient } from './sparql/client';

export type EntityType = 'artist'

export default async function findDoors(entity: EntityType, wdentity: string) {
  let query;
  switch (entity) {
    case 'artist':
      query = await artistDoorsQuery(wdentity);
  }

  const stream = await genericClient.query.select(query);

  const result = await readableToObjectList(stream);

  return result.map((d: any) => (
    Object.fromEntries(Object.entries(d).map(([k, v]: [string, any]) => [k, {
      id: v.value,
      name:
    }]))
  ));
}

async function artistDoorsQuery(wdentity: string): Promise<string> {
  return `
    PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
    PREFIX wd: <http://www.wikidata.org/entity/>
    PREFIX wdt: <http://www.wikidata.org/prop/direct/>

    SELECT *
    WHERE {
      SERVICE <https://query.wikidata.org/sparql> {
        SELECT * WHERE {
          BIND (wd:${wdentity} AS ?artist) .
          ?artist wdt:P135 ?movement .
          ?artist wdt:P19 ?pob .
          ?artist wdt:P463 ?memberof
        }
      }
    } LIMIT 100
  `;
}
