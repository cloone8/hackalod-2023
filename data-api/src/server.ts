import express, { Request, Response } from 'express';
import config from './config';
import query from './sparql';

import 'express-async-errors';

const entityTypes = ['artist', 'city'];

const app = express();

app.use(express.json());

app.get('/status', (req: Request, res: Response) => {
  return res.status(200).json({ message: 'I\'m alive!'})
})

app.get('/:entityType/:entityId', async (req: Request, res: Response) => {
  const { entityType, entityId } = req.params;

  if (!entityTypes.includes(entityType)) {
    return res.status(400).json({ error: `Invalid entity type ${entityType}. Expected one of [${entityTypes.join(', ')}]`})
  }

  const result = await query(entityType, entityId);

  return res.status(200).json(result);
})

app.listen(config.port, () => console.log('server started'));
