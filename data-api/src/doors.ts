import client from "./sparql/client";

export default async function findDoors(origin: string) {
  const query = `
    PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
    PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
    PREFIX rkdo: <http://data.rkd.nl/def#>
    PREFIX rkda: <https://data.rkd.nl/artists/>
    PREFIX schema: <http://schema.org/>

    SELECT ?name {
      rkda:80476 schema:name ?name
    }
    ORDER BY ?name
    LIMIT 10
`;

  const stream = await client.query.select(query);

  stream.on("data", (row) => {
    Object.entries(row).forEach(([key, value]: [string, any]) => {
      console.log(`${key}: ${value.value} (${value.termType})`);
    });
  });

  return Promise.resolve();
}
